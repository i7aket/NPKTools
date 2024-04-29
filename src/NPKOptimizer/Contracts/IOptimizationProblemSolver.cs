namespace NPKOptimizer.Contracts;

/// <summary>
/// Defines an interface for optimization problem solvers using different algorithms.
/// </summary>
public interface IOptimizationProblemSolver
{
    /// <summary>
    /// Solves an optimization problem and returns the optimized variable values.
    /// </summary>
    /// <param name="problem">The optimization problem to solve.</param>
    /// <returns>A dictionary containing the variable names and their optimized values.</returns>
    Dictionary<string, double>? Solve(OptimizationProblem problem);
}