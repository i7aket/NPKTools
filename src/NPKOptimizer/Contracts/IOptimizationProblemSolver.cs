namespace NPKOptimizer.Contracts;

public interface IOptimizationProblemSolver
{
    Dictionary<string, double> Solve(OptimizationProblem problem);
}