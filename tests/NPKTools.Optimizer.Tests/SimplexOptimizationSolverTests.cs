using AwesomeAssertions;
using NPKTools.Optimizer.Components;
using NPKTools.Optimizer.Contracts;
using Xunit;

namespace NPKTools.Optimizer.Tests;

/// <summary>
/// Direct unit tests for the default solver, with optima computed by hand.
/// </summary>
/// <remarks>
/// The suite's other coverage of this class is differential against OR-Tools, which only runs where
/// the OR-Tools native binaries exist. These tests hold on every platform and pin the exact numbers,
/// so a sign error or a tolerance change cannot ship green.
/// </remarks>
public class SimplexOptimizationSolverTests
{
    private const double Precision = 1e-9;

    private readonly IOptimizationProblemSolver _solver = new SimplexOptimizationSolver();

    /// <summary>
    /// Builds a problem over variables named x0..xN from dense coefficient rows.
    /// </summary>
    private static OptimizationProblem Problem(
        bool isMinimization,
        double[] costs,
        params (double LowerBound, double[] Coefficients, double UpperBound)[] constraints)
    {
        Dictionary<string, double> variables = [];
        Dictionary<string, double> objective = [];
        for (int j = 0; j < costs.Length; j++)
        {
            variables[$"x{j}"] = 0;
            objective[$"x{j}"] = costs[j];
        }

        OptimizationProblem problem = new()
        {
            Variables = variables,
            Objective = new OptimizationProblem.OptimizationObjective
            {
                Coefficients = objective,
                IsMinimization = isMinimization
            }
        };

        for (int i = 0; i < constraints.Length; i++)
        {
            (double lowerBound, double[] coefficients, double upperBound) = constraints[i];
            Dictionary<string, double> row = [];
            for (int j = 0; j < coefficients.Length; j++)
            {
                if (coefficients[j] != 0)
                {
                    row[$"x{j}"] = coefficients[j];
                }
            }

            problem.Constraints.Add(new OptimizationProblem.OptimizationConstraint
            {
                Name = $"c{i}",
                LowerBound = lowerBound,
                UpperBound = upperBound,
                Coefficients = row
            });
        }

        return problem;
    }

    private static double Objective(OptimizationProblem problem, Dictionary<string, double> values)
    {
        double total = 0;
        foreach (KeyValuePair<string, double> term in problem.Objective.Coefficients)
        {
            total += term.Value * values[term.Key];
        }

        return total;
    }

    // ---------------------------------------------------------------- maximization

    /// <summary>
    /// The textbook LP: maximise 3x + 5y subject to x ≤ 4, 2y ≤ 12, 3x + 2y ≤ 18.
    /// The optimum is at (2, 6) with value 36. Nothing else in the suite covers
    /// <see cref="OptimizationProblem.OptimizationObjective.IsMinimization"/> being false for this
    /// solver, so a sign error in the objective negation would otherwise go unnoticed.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_Maximization_ReturnsHandComputedOptimum()
    {
        // Arrange
        OptimizationProblem problem = Problem(
            isMinimization: false,
            [3, 5],
            (double.NegativeInfinity, [1, 0], 4),
            (double.NegativeInfinity, [0, 2], 12),
            (double.NegativeInfinity, [3, 2], 18));

        // Act
        Dictionary<string, double>? solution = _solver.Solve(problem);

        // Assert
        solution.Should().NotBeNull();
        solution!["x0"].Should().BeApproximately(2, Precision);
        solution["x1"].Should().BeApproximately(6, Precision);
        Objective(problem, solution).Should().BeApproximately(36, Precision);
    }

    /// <summary>
    /// Maximising with the same data as a minimisation must give a different answer; this pins the
    /// direction rather than only the value, so negating twice would not pass.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_MinimizationAndMaximization_MoveInOppositeDirections()
    {
        // Arrange
        (double, double[], double) constraint = (2, [1, 1], 10);

        // Act
        Dictionary<string, double>? minimum = _solver.Solve(Problem(true, [1, 1], constraint));
        Dictionary<string, double>? maximum = _solver.Solve(Problem(false, [1, 1], constraint));

        // Assert
        minimum.Should().NotBeNull();
        maximum.Should().NotBeNull();
        (minimum!["x0"] + minimum["x1"]).Should().BeApproximately(2, Precision);
        (maximum!["x0"] + maximum["x1"]).Should().BeApproximately(10, Precision);
    }

    // ---------------------------------------------------------------- minimization

    /// <summary>
    /// Minimise 2x + 3y subject to x + y ≥ 10 and x ≤ 6. The cheaper variable is capped, so the
    /// optimum is x = 6, y = 4, value 24.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_Minimization_ReturnsHandComputedOptimum()
    {
        // Arrange
        OptimizationProblem problem = Problem(
            isMinimization: true,
            [2, 3],
            (10, [1, 1], double.PositiveInfinity),
            (double.NegativeInfinity, [1, 0], 6));

        // Act
        Dictionary<string, double>? solution = _solver.Solve(problem);

        // Assert
        solution.Should().NotBeNull();
        solution!["x0"].Should().BeApproximately(6, Precision);
        solution["x1"].Should().BeApproximately(4, Precision);
        Objective(problem, solution).Should().BeApproximately(24, Precision);
    }

    /// <summary>
    /// Two equality rows fixing a unique point: x + y = 10, x − y = 2 gives (6, 4) regardless of the
    /// objective. Equality rows are what the mapper produces when an element's precision is 1.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_EqualityConstraints_ReturnsTheUniqueFeasiblePoint()
    {
        // Arrange
        OptimizationProblem problem = Problem(
            isMinimization: true,
            [1, 1],
            (10, [1, 1], 10),
            (2, [1, -1], 2));

        // Act
        Dictionary<string, double>? solution = _solver.Solve(problem);

        // Assert
        solution.Should().NotBeNull();
        solution!["x0"].Should().BeApproximately(6, Precision);
        solution["x1"].Should().BeApproximately(4, Precision);
    }

    /// <summary>
    /// A single variable inside a range: minimise 5x subject to 3 ≤ x ≤ 7 gives x = 3.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_RangeConstraint_LandsOnTheCheaperBound()
    {
        // Arrange
        OptimizationProblem problem = Problem(isMinimization: true, [5], (3, [1], 7));

        // Act
        Dictionary<string, double>? solution = _solver.Solve(problem);

        // Assert
        solution.Should().NotBeNull();
        solution!["x0"].Should().BeApproximately(3, Precision);
    }

    // ---------------------------------------------------------------- one-sided bounds

    /// <summary>
    /// A non-finite bound is how <see cref="OptimizationProblem.OptimizationConstraint"/> expresses a
    /// one-sided constraint. Both shipped solvers must accept it: before 2.0.0 the infinity went
    /// straight into the right-hand side and every such problem came back infeasible.
    /// </summary>
    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(2.0, double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity, 2.0)]
    public void Solve_OneSidedConstraint_IsSolvedNotReportedInfeasible(double lowerBound, double upperBound)
    {
        // Arrange
        OptimizationProblem problem = Problem(
            isMinimization: true,
            [1, 1],
            (lowerBound, [1, 1], upperBound),
            (1, [1, 0], 1));

        // Act
        Dictionary<string, double>? solution = _solver.Solve(problem);

        // Assert
        solution.Should().NotBeNull();
        solution!["x0"].Should().BeApproximately(1, Precision);
    }

    /// <summary>
    /// A constraint unbounded on both sides restricts nothing and must not make the problem
    /// infeasible; only the real constraint applies.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_ConstraintUnboundedOnBothSides_IsIgnored()
    {
        // Arrange
        OptimizationProblem problem = Problem(
            isMinimization: true,
            [1],
            (double.NegativeInfinity, [1], double.PositiveInfinity),
            (4, [1], 4));

        // Act
        Dictionary<string, double>? solution = _solver.Solve(problem);

        // Assert
        solution.Should().NotBeNull();
        solution!["x0"].Should().BeApproximately(4, Precision);
    }

    // ---------------------------------------------------------------- null results

    /// <summary>
    /// Minimising a negative cost with only an upper-open constraint has no finite optimum. The
    /// contract is null, not an exception and not the iteration cap.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_UnboundedObjective_ReturnsNull()
    {
        // Arrange
        OptimizationProblem problem = Problem(
            isMinimization: true,
            [-1],
            (1, [1], double.PositiveInfinity));

        // Act
        Dictionary<string, double>? solution = _solver.Solve(problem);

        // Assert
        solution.Should().BeNull();
    }

    /// <summary>
    /// Contradictory rows: x ≥ 5 and x ≤ 3 cannot both hold.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_InfeasibleConstraints_ReturnsNull()
    {
        // Arrange
        OptimizationProblem problem = Problem(
            isMinimization: true,
            [1],
            (5, [1], double.PositiveInfinity),
            (double.NegativeInfinity, [1], 3));

        // Act
        Dictionary<string, double>? solution = _solver.Solve(problem);

        // Assert
        solution.Should().BeNull();
    }

    /// <summary>
    /// A single row whose own bounds are inverted is infeasible too.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_InvertedBounds_ReturnsNull()
    {
        // Arrange
        OptimizationProblem problem = Problem(isMinimization: true, [1], (5, [1], 3));

        // Act
        Dictionary<string, double>? solution = _solver.Solve(problem);

        // Assert
        solution.Should().BeNull();
    }

    /// <summary>
    /// A negative right-hand side is flipped during standard-form construction so that the artificial
    /// basis starts feasible.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_NegativeRightHandSide_IsHandled()
    {
        // Arrange: -x0 - x1 = -10, minimise x0 + 2*x1 -> x0 = 10.
        OptimizationProblem problem = Problem(
            isMinimization: true,
            [1, 2],
            (-10, [-1, -1], -10));

        // Act
        Dictionary<string, double>? solution = _solver.Solve(problem);

        // Assert
        solution.Should().NotBeNull();
        solution!["x0"].Should().BeApproximately(10, Precision);
        solution["x1"].Should().BeApproximately(0, Precision);
    }

    /// <summary>
    /// Every returned quantity must be non-negative; the mapper turns these straight into fertilizer
    /// weights, and a negative one used to be reachable on badly scaled input.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_AnySolution_ContainsNoNegativeValues()
    {
        // Arrange
        OptimizationProblem problem = Problem(
            isMinimization: true,
            [1, 1, 1],
            (10, [1, 2, 3], 12),
            (2, [0, 1, -1], 5),
            (1, [1, -1, 0], double.PositiveInfinity));

        // Act
        Dictionary<string, double>? solution = _solver.Solve(problem);

        // Assert
        solution.Should().NotBeNull();
        solution!.Values.Should().AllSatisfy(value => value.Should().BeGreaterThanOrEqualTo(0));
    }

    // ---------------------------------------------------------------- guard clauses

    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_NullProblem_ThrowsArgumentNullException()
    {
        Action act = () => _solver.Solve(null);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_EmptyVariables_ThrowsArgumentException()
    {
        // Arrange
        OptimizationProblem problem = Problem(isMinimization: true, [1], (1, [1], 2));
        problem.Variables.Clear();

        // Act
        Action act = () => _solver.Solve(problem);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_EmptyConstraints_ThrowsArgumentException()
    {
        // Arrange
        OptimizationProblem problem = Problem(isMinimization: true, [1], (1, [1], 2));
        problem.Constraints.Clear();

        // Act
        Action act = () => _solver.Solve(problem);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_EmptyObjectiveCoefficients_ThrowsArgumentException()
    {
        // Arrange
        OptimizationProblem problem = Problem(isMinimization: true, [1], (1, [1], 2));
        problem.Objective.Coefficients.Clear();

        // Act
        Action act = () => _solver.Solve(problem);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// A NaN bound used to produce a "solution" of NaN reported as success, which is the worst
    /// possible output. It is a caller error, so it is rejected outright.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_NaNBound_ThrowsArgumentException()
    {
        // Arrange
        OptimizationProblem problem = Problem(isMinimization: true, [1], (double.NaN, [1], 2));

        // Act
        Action act = () => _solver.Solve(problem);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*NaN*");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_NaNCoefficient_ThrowsArgumentException()
    {
        // Arrange
        OptimizationProblem problem = Problem(isMinimization: true, [1], (1, [double.NaN], 2));

        // Act
        Action act = () => _solver.Solve(problem);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*NaN*");
    }

    /// <summary>
    /// A coefficient naming a variable that was never declared is a typo, not a zero. It used to be
    /// silently dropped while the OR-Tools backend threw; both now throw.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_UndeclaredVariableInConstraint_ThrowsKeyNotFoundException()
    {
        // Arrange
        OptimizationProblem problem = Problem(isMinimization: true, [1], (1, [1], 2));
        problem.Constraints[0].Coefficients["ghost"] = 5;

        // Act
        Action act = () => _solver.Solve(problem);

        // Assert
        act.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_UndeclaredVariableInObjective_ThrowsKeyNotFoundException()
    {
        // Arrange
        OptimizationProblem problem = Problem(isMinimization: true, [1], (1, [1], 2));
        problem.Objective.Coefficients["ghost"] = 5;

        // Act
        Action act = () => _solver.Solve(problem);

        // Assert
        act.Should().Throw<KeyNotFoundException>();
    }

    // ---------------------------------------------------------------- verification

    /// <summary>
    /// The solver verifies its answer against the original constraints before returning it. On a
    /// problem whose coefficients span ten orders of magnitude the tableau's arithmetic drifts far
    /// enough to produce a point that violates x ≥ 0 by 0.88; the verification turns that into null,
    /// which the whole pipeline already handles. The exact doubles are the reproduction found by
    /// differential testing against GLOP.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_IllScaledProblem_ReturnsNullRatherThanAnInfeasiblePoint()
    {
        // Arrange
        OptimizationProblem problem = Problem(
            isMinimization: false,
            [-157.708018, -0.000118, -5.0000000000000002E-05, -38.337023000000002],
            (1.4175979999999999, [0, 0.26340000000000002, 0.026939000000000001, 0.64615999999999996],
                1.4175979999999999),
            (110416.021459, [0, 35716.587041999999, 82000.744892999995, 12166.639236999999],
                209911.534078),
            (2.764945, [0.958866, -0.011455999999999999, 0.119007, 0], 2.7651286499999999),
            (0.00032200000000000002, [4.8999999999999998E-05, 5.3000000000000001E-05,
                6.8999999999999997E-05, 7.2999999999999999E-05], 0.000379));

        // Act
        Dictionary<string, double>? solution = _solver.Solve(problem);

        // Assert
        solution.Should().BeNull();
    }
}
