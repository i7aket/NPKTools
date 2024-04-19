using NPKOptimizer.Common;

namespace NPKOptimizer.Components.OptimizationProblemSolvers.Contracts;

public interface IOptimizationProblemSolver
{
    ActionResult<Dictionary<string, double>> Solve(OptimizationProblem problem);
}