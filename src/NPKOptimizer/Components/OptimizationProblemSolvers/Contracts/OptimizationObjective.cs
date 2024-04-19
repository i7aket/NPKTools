namespace NPKOptimizer.Components.OptimizationProblemSolvers.Contracts;

public record OptimizationObjective
{
    public readonly Dictionary<string, double> Coefficients = new(); 
    public readonly bool IsMinimization = true; 
}