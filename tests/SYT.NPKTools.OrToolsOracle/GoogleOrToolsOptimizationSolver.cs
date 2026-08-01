using Google.OrTools.LinearSolver;
using SYT.NPKTools.Optimization;

namespace SYT.NPKTools.OrToolsOracle;

/// <summary>
/// An <see cref="IOptimizationProblemSolver"/> backed by Google OR-Tools' GLOP linear solver,
/// used as the independent oracle the shipped managed simplex is validated against.
/// </summary>
/// <remarks>
/// <para>
/// This is deliberately not part of the published package. OR-Tools distributes native binaries only
/// for linux-x64/arm64, osx-x64/arm64 and win-x64 — there is no <c>browser-wasm</c> build — so
/// depending on it would make NPKTools server-only. Keeping it here means the package stays fully
/// managed while its solver is still checked against a mature production implementation.
/// </para>
/// <para>
/// It also serves as the comparison baseline in the benchmarks, which is why it lives in its own
/// project rather than inside a test assembly.
/// </para>
/// <para>
/// A consumer who wants GLOP at runtime can implement <see cref="IOptimizationProblemSolver"/> the
/// same way this class does and register it before <c>AddNpkTools()</c>.
/// </para>
/// </remarks>
public sealed class GoogleOrToolsOptimizationSolver : IOptimizationProblemSolver
{
    /// <summary>
    /// Solves the given optimization problem using the linear solver from Google OR-Tools.
    /// Returns a dictionary where keys are variable names and values are their optimized numerical values,
    /// or returns null if no optimal solution can be found.
    /// </summary>
    /// <param name="problem">The optimization problem to solve, containing variables, constraints, and an objective.</param>
    /// <returns>
    /// A dictionary where keys are variable names and values are their optimized numerical values. 
    /// Returns null if the solver does not find an optimal solution.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when any critical component of the problem (such as the problem itself, its variables, constraints, or objective) is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the variables, constraints or objective coefficients are empty.</exception>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when a constraint or the objective references a variable that is not declared in
    /// <see cref="OptimizationProblem.Variables"/>.
    /// </exception>
    public Dictionary<string, double>? Solve(OptimizationProblem problem)
    {
        ArgumentNullException.ThrowIfNull(problem);
        ArgumentNullException.ThrowIfNull(problem.Objective);

        // Guards written out rather than reusing the package's internal ThrowIf helper: the oracle
        // must reject the same inputs without reaching into the assembly it validates.
        ArgumentNullException.ThrowIfNull(problem.Objective.Coefficients);
        ArgumentNullException.ThrowIfNull(problem.Variables);
        ArgumentNullException.ThrowIfNull(problem.Constraints);

        if (problem.Objective.Coefficients.Count == 0 ||
            problem.Variables.Count == 0 ||
            problem.Constraints.Count == 0)
        {
            throw new ArgumentException(
                "The problem must declare variables, constraints and objective coefficients.",
                nameof(problem));
        }

        Solver solver = Solver.CreateSolver("GLOP");

        Dictionary<string, Variable> variables = problem.Variables.ToDictionary(
            name => name.Key,
            name => solver.MakeNumVar(0, double.PositiveInfinity, name.Key)
        );

        Objective costObjective = solver.Objective();
        foreach (KeyValuePair<string, double> coefficientPair in problem.Objective.Coefficients)
        {
            costObjective.SetCoefficient(variables[coefficientPair.Key], coefficientPair.Value);
        }

        if (problem.Objective.IsMinimization)
        {
            costObjective.SetMinimization();
        }
        else
        {
            costObjective.SetMaximization();
        }

        foreach (OptimizationProblem.OptimizationConstraint constraint in problem.Constraints)
        {
            Constraint solverConstraint = solver.MakeConstraint(constraint.LowerBound, constraint.UpperBound, constraint.Name);
            foreach (KeyValuePair<string, double> coefficient in constraint.Coefficients)
            {
                solverConstraint.SetCoefficient(variables[coefficient.Key], coefficient.Value);
            }
        }

        Solver.ResultStatus resultStatus = solver.Solve();

        return resultStatus != Solver.ResultStatus.OPTIMAL
            ? default
            : variables.ToDictionary(variable => variable.Key, variable => variable.Value.SolutionValue());
    }
}
