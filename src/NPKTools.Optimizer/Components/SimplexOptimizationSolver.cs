using NPKTools.Core.Common;
using NPKTools.Optimizer.Contracts;

namespace NPKTools.Optimizer.Components;

/// <summary>
/// A fully managed <see cref="IOptimizationProblemSolver"/>: a two-phase primal simplex with no
/// native dependencies, so it runs anywhere .NET runs — including WebAssembly, where the
/// OR-Tools native binaries are unavailable.
/// </summary>
/// <remarks>
/// <para>
/// Fertilizer optimization problems are small. A macronutrient bundle produces at most 16 variables
/// (one per candidate fertilizer) and 7 range constraints (one per constrained element), and a full
/// preset search solves 40 such problems. A dense tableau is the right data structure at that size;
/// the sparse, scaled machinery of a production solver buys nothing here.
/// </para>
/// <para>
/// The problem solved is: minimise <c>cᵀx</c> subject to <c>L ≤ Ax ≤ U</c> and <c>x ≥ 0</c>.
/// Range constraints whose bounds coincide — which is what the mapper produces when an element's
/// precision is 1 — become a single equality row instead of two inequality rows. A non-finite bound
/// expresses a one-sided constraint and simply contributes no row on that side.
/// </para>
/// <para>
/// Entering and leaving variables are chosen by Bland's rule, which guarantees termination on
/// degenerate problems at the cost of more iterations than Dantzig's rule. At this problem size that
/// trade is free, and it removes cycling as a failure mode.
/// </para>
/// <para>
/// <b>Numerical envelope.</b> This is a dense tableau with a fixed tolerance and no scaling,
/// equilibration or iterative refinement. It is exact enough for the problems this library
/// generates — coefficients are nutrient percentages (roughly 1–50) and right-hand sides are ppm/10
/// (roughly 0–100), a spread of about three orders of magnitude — and it is differentially tested
/// against GLOP over the whole preset catalogue. On a badly scaled problem, one mixing coefficient
/// magnitudes across ten or more orders of magnitude, accumulated round-off can make it stop at a
/// suboptimal vertex or report no solution where one exists. Every answer it returns is verified
/// feasible before being handed back, so the failure mode is a missed or suboptimal solution, never
/// a violated constraint. If you need a solver robust across arbitrary scaling, use
/// <c>NPKTools.Optimizer.OrTools</c>.
/// </para>
/// </remarks>
public sealed class SimplexOptimizationSolver : IOptimizationProblemSolver
{
    /// <summary>
    /// Values within this distance of zero are treated as zero. Constraint coefficients here are
    /// nutrient percentages (roughly 1–50) and right-hand sides are ppm/10 (roughly 0–100), so the
    /// problem is well scaled and a tight tolerance is safe.
    /// </summary>
    private const double Tolerance = 1e-9;

    /// <summary>
    /// Backstop against a pivoting bug looping forever. Real problems here finish in tens of
    /// iterations; this bound is orders of magnitude above that.
    /// </summary>
    private const int MaxIterations = 10_000;

    /// <summary>
    /// Relative slack allowed when verifying the answer against the original constraints. It is
    /// scaled by the magnitude of each row, so it means "correct to about six significant digits"
    /// rather than a fixed absolute distance. In-regime problems land within 1e-14 of their bounds,
    /// which is many orders of magnitude inside this limit.
    /// </summary>
    private const double VerificationTolerance = 1e-6;

    /// <summary>
    /// Solves the given optimization problem.
    /// </summary>
    /// <param name="problem">The optimization problem to solve.</param>
    /// <returns>
    /// A dictionary mapping every variable name to its optimal value, or null when the problem is
    /// infeasible or unbounded — matching the contract of the OR-Tools implementation. Null is also
    /// returned when the computed answer fails verification against the original constraints, which
    /// is how numerical trouble on a badly scaled problem surfaces. See the class remarks.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the problem or its objective is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the variables, constraints or objective coefficients are empty, or when a bound or
    /// coefficient is <see cref="double.NaN"/>.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when a constraint or the objective references a variable that is not declared in
    /// <see cref="OptimizationProblem.Variables"/>.
    /// </exception>
    public Dictionary<string, double>? Solve(OptimizationProblem problem)
    {
        ArgumentNullException.ThrowIfNull(problem);
        ArgumentNullException.ThrowIfNull(problem.Objective);
        ThrowIf.NullOrEmpty(problem.Objective.Coefficients);
        ThrowIf.NullOrEmpty(problem.Variables);
        ThrowIf.NullOrEmpty(problem.Constraints);

        string[] variableNames = [.. problem.Variables.Keys];
        Dictionary<string, int> columnOf = new(variableNames.Length, StringComparer.Ordinal);
        for (int j = 0; j < variableNames.Length; j++)
        {
            columnOf[variableNames[j]] = j;
        }

        Tableau tableau = Build(problem, columnOf, variableNames.Length);

        double[]? solution = tableau.Solve(problem.Objective.IsMinimization);
        if (solution is null)
        {
            return null;
        }

        Dictionary<string, double> result = new(variableNames.Length, StringComparer.Ordinal);
        for (int j = 0; j < variableNames.Length; j++)
        {
            // Clamp the rounding dust a pivot can leave on a variable that is really zero.
            double value = solution[j];
            result[variableNames[j]] = Math.Abs(value) < Tolerance ? 0 : value;
        }

        // The tableau reports optimality with respect to its own accumulated arithmetic. Check the
        // answer against the problem as it was given before claiming success: a solution that
        // violates the constraints is worse than no solution, because callers act on it.
        return IsVerified(problem, result) ? result : null;
    }

    /// <summary>
    /// Re-evaluates the original constraints at the computed point, in one O(mn) pass. Rejects
    /// negative quantities and any row outside its bounds by more than a scaled tolerance.
    /// </summary>
    private static bool IsVerified(OptimizationProblem problem, Dictionary<string, double> values)
    {
        foreach (double value in values.Values)
        {
            if (double.IsNaN(value) || value < -Tolerance)
            {
                return false;
            }
        }

        foreach (OptimizationProblem.OptimizationConstraint constraint in problem.Constraints)
        {
            double leftHandSide = 0;
            double magnitude = 0;

            foreach (KeyValuePair<string, double> coefficient in constraint.Coefficients)
            {
                double term = coefficient.Value * values[coefficient.Key];
                leftHandSide += term;
                magnitude += Math.Abs(term);
            }

            // Cancellation between large terms costs absolute precision, so the allowance grows with
            // the size of the numbers that produced the row, not with the size of the bound alone.
            double allowance = VerificationTolerance * Math.Max(1, magnitude);

            if (double.IsFinite(constraint.UpperBound) && leftHandSide > constraint.UpperBound + allowance)
            {
                return false;
            }

            if (double.IsFinite(constraint.LowerBound) && leftHandSide < constraint.LowerBound - allowance)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Translates the problem into standard form: one row per constraint (two for a genuine range,
    /// one for a one-sided or equality constraint), a slack or surplus column per inequality, and a
    /// right-hand side normalised to be non-negative.
    /// </summary>
    private static Tableau Build(OptimizationProblem problem, Dictionary<string, int> columnOf, int variableCount)
    {
        List<double[]> coefficientRows = [];
        List<double> rightHandSides = [];
        List<int> slackSigns = [];

        foreach (OptimizationProblem.OptimizationConstraint constraint in problem.Constraints)
        {
            if (double.IsNaN(constraint.LowerBound) || double.IsNaN(constraint.UpperBound))
            {
                throw new ArgumentException(
                    $"Constraint '{constraint.Name}' has a NaN bound.",
                    nameof(problem));
            }

            double[] row = new double[variableCount];
            foreach (KeyValuePair<string, double> coefficient in constraint.Coefficients)
            {
                if (double.IsNaN(coefficient.Value))
                {
                    throw new ArgumentException(
                        $"Constraint '{constraint.Name}' has a NaN coefficient for variable '{coefficient.Key}'.",
                        nameof(problem));
                }

                // An undeclared variable is a caller error, not a zero coefficient. The OR-Tools
                // backend throws here too, so both solvers reject the same problems.
                row[columnOf[coefficient.Key]] = coefficient.Value;
            }

            // A non-finite bound is not a constraint. It is how OptimizationConstraint expresses a
            // one-sided restriction, so it contributes no row on that side; putting an infinity in
            // the right-hand side would make every such problem look infeasible.
            bool hasUpperBound = double.IsFinite(constraint.UpperBound);
            bool hasLowerBound = double.IsFinite(constraint.LowerBound);

            if (!hasUpperBound && !hasLowerBound)
            {
                continue;
            }

            if (hasUpperBound && hasLowerBound &&
                Math.Abs(constraint.UpperBound - constraint.LowerBound) <= Tolerance)
            {
                // Equality: Ax = b, no slack needed.
                coefficientRows.Add(row);
                rightHandSides.Add(constraint.UpperBound);
                slackSigns.Add(0);
                continue;
            }

            if (hasUpperBound)
            {
                // Ax + s = U, s >= 0
                coefficientRows.Add(row);
                rightHandSides.Add(constraint.UpperBound);
                slackSigns.Add(1);
            }

            if (hasLowerBound)
            {
                // Ax - t = L, t >= 0
                coefficientRows.Add(row);
                rightHandSides.Add(constraint.LowerBound);
                slackSigns.Add(-1);
            }
        }

        int rowCount = coefficientRows.Count;
        int slackCount = slackSigns.Count(sign => sign != 0);
        int totalColumns = variableCount + slackCount + rowCount;

        double[,] matrix = new double[rowCount, totalColumns];
        double[] rhs = new double[rowCount];
        double[] costs = new double[totalColumns];

        foreach (KeyValuePair<string, double> objectiveTerm in problem.Objective.Coefficients)
        {
            if (double.IsNaN(objectiveTerm.Value))
            {
                throw new ArgumentException(
                    $"The objective has a NaN coefficient for variable '{objectiveTerm.Key}'.",
                    nameof(problem));
            }

            costs[columnOf[objectiveTerm.Key]] = objectiveTerm.Value;
        }

        int nextSlack = variableCount;
        int[] basis = new int[rowCount];

        for (int i = 0; i < rowCount; i++)
        {
            // A negative right-hand side is flipped so that the artificial basis starts feasible.
            double sign = rightHandSides[i] < 0 ? -1 : 1;

            for (int j = 0; j < variableCount; j++)
            {
                matrix[i, j] = sign * coefficientRows[i][j];
            }

            if (slackSigns[i] != 0)
            {
                matrix[i, nextSlack] = sign * slackSigns[i];
                nextSlack++;
            }

            rhs[i] = sign * rightHandSides[i];

            int artificial = variableCount + slackCount + i;
            matrix[i, artificial] = 1;
            basis[i] = artificial;
        }

        return new Tableau(matrix, rhs, costs, basis, variableCount, variableCount + slackCount);
    }

    /// <summary>
    /// A dense simplex tableau in canonical form with respect to its basis.
    /// </summary>
    private sealed class Tableau(
        double[,] matrix,
        double[] rhs,
        double[] costs,
        int[] basis,
        int structuralColumnCount,
        int firstArtificialColumn)
    {
        private readonly double[,] _matrix = matrix;
        private readonly double[] _rhs = rhs;
        private readonly double[] _costs = costs;
        private readonly int[] _basis = basis;
        private readonly int _rowCount = rhs.Length;
        private readonly int _columnCount = costs.Length;

        /// <summary>
        /// Runs phase 1 to find a feasible basis, then phase 2 to optimise the real objective.
        /// </summary>
        /// <param name="isMinimization">
        /// When false the objective is negated, so a maximisation is solved by the same minimising core.
        /// </param>
        /// <returns>The optimal values of the structural variables, or null if infeasible or unbounded.</returns>
        public double[]? Solve(bool isMinimization)
        {
            double[] phaseOneCosts = new double[_columnCount];
            for (int j = firstArtificialColumn; j < _columnCount; j++)
            {
                phaseOneCosts[j] = 1;
            }

            if (!Optimize(phaseOneCosts, allowArtificials: true))
            {
                return null;
            }

            // Any residual artificial value means the constraints cannot all be satisfied.
            double infeasibility = 0;
            for (int i = 0; i < _rowCount; i++)
            {
                if (_basis[i] >= firstArtificialColumn)
                {
                    infeasibility += Math.Abs(_rhs[i]);
                }
            }

            if (infeasibility > 1e-7)
            {
                return null;
            }

            DriveArtificialsOutOfBasis();

            double[] phaseTwoCosts = new double[_columnCount];
            for (int j = 0; j < structuralColumnCount; j++)
            {
                phaseTwoCosts[j] = isMinimization ? _costs[j] : -_costs[j];
            }

            if (!Optimize(phaseTwoCosts, allowArtificials: false))
            {
                return null;
            }

            double[] solution = new double[structuralColumnCount];
            for (int i = 0; i < _rowCount; i++)
            {
                if (_basis[i] < structuralColumnCount)
                {
                    solution[_basis[i]] = _rhs[i];
                }
            }

            return solution;
        }

        /// <summary>
        /// Pivots until no reduced cost is negative. Returns false if the problem is unbounded or the
        /// iteration cap is hit.
        /// </summary>
        private bool Optimize(double[] costs, bool allowArtificials)
        {
            double[] reducedCosts = ComputeReducedCosts(costs);
            int lastColumn = allowArtificials ? _columnCount : firstArtificialColumn;

            for (int iteration = 0; iteration < MaxIterations; iteration++)
            {
                // Bland's rule: the lowest-indexed column with a negative reduced cost.
                int entering = -1;
                for (int j = 0; j < lastColumn; j++)
                {
                    if (reducedCosts[j] < -Tolerance)
                    {
                        entering = j;
                        break;
                    }
                }

                if (entering < 0)
                {
                    return true;
                }

                int leaving = ChooseLeavingRow(entering);
                if (leaving < 0)
                {
                    return false;
                }

                Pivot(leaving, entering);
                _basis[leaving] = entering;
                reducedCosts = ComputeReducedCosts(costs);
            }

            return false;
        }

        /// <summary>
        /// Reduced costs relative to the current basis: <c>c_j - c_Bᵀ B⁻¹ A_j</c>. The tableau is kept
        /// in canonical form, so <c>B⁻¹A</c> is just the stored matrix.
        /// </summary>
        private double[] ComputeReducedCosts(double[] costs)
        {
            double[] reduced = (double[])costs.Clone();

            for (int i = 0; i < _rowCount; i++)
            {
                double basicCost = costs[_basis[i]];
                if (basicCost == 0)
                {
                    continue;
                }

                for (int j = 0; j < _columnCount; j++)
                {
                    reduced[j] -= basicCost * _matrix[i, j];
                }
            }

            return reduced;
        }

        /// <summary>
        /// Minimum-ratio test, breaking ties by the lowest basic variable index (Bland's rule).
        /// </summary>
        private int ChooseLeavingRow(int entering)
        {
            int leaving = -1;
            double bestRatio = double.PositiveInfinity;

            for (int i = 0; i < _rowCount; i++)
            {
                double pivot = _matrix[i, entering];
                if (pivot <= Tolerance)
                {
                    continue;
                }

                double ratio = _rhs[i] / pivot;
                if (ratio < bestRatio - Tolerance ||
                    (ratio < bestRatio + Tolerance && (leaving < 0 || _basis[i] < _basis[leaving])))
                {
                    bestRatio = ratio;
                    leaving = i;
                }
            }

            return leaving;
        }

        /// <summary>
        /// Restores canonical form by normalising the pivot row and eliminating the entering column
        /// from every other row.
        /// </summary>
        private void Pivot(int pivotRow, int pivotColumn)
        {
            double pivotValue = _matrix[pivotRow, pivotColumn];

            for (int j = 0; j < _columnCount; j++)
            {
                _matrix[pivotRow, j] /= pivotValue;
            }

            _rhs[pivotRow] /= pivotValue;

            for (int i = 0; i < _rowCount; i++)
            {
                if (i == pivotRow)
                {
                    continue;
                }

                double factor = _matrix[i, pivotColumn];
                if (factor == 0)
                {
                    continue;
                }

                for (int j = 0; j < _columnCount; j++)
                {
                    _matrix[i, j] -= factor * _matrix[pivotRow, j];
                }

                _rhs[i] -= factor * _rhs[pivotRow];
            }
        }

        /// <summary>
        /// Replaces artificial variables that remain basic at value zero. A row where no real column
        /// has a non-zero coefficient is a redundant constraint and is zeroed out so phase 2 ignores it.
        /// </summary>
        private void DriveArtificialsOutOfBasis()
        {
            for (int i = 0; i < _rowCount; i++)
            {
                if (_basis[i] < firstArtificialColumn)
                {
                    continue;
                }

                int replacement = -1;
                for (int j = 0; j < firstArtificialColumn; j++)
                {
                    if (Math.Abs(_matrix[i, j]) > Tolerance)
                    {
                        replacement = j;
                        break;
                    }
                }

                if (replacement >= 0)
                {
                    Pivot(i, replacement);
                    _basis[i] = replacement;
                    continue;
                }

                for (int j = 0; j < _columnCount; j++)
                {
                    _matrix[i, j] = 0;
                }

                _rhs[i] = 0;
            }
        }
    }
}
