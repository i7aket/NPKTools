namespace NPKOptimizer.Components.OptimizationProblemSolvers.Contracts;

public record OptimizationConstraint
{
    public required string Name; 
    public double LowerBound; 
    public double UpperBound; 
    public Dictionary<string, double> Coefficients = new(); 
}