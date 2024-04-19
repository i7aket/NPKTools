namespace NPKOptimizer.Components.OptimizationProblemSolvers.Contracts;

public record OptimizationProblem
{
    public readonly Dictionary<string, double> Variables = new(); 
    public readonly List<OptimizationConstraint> Constraints = new();
    public readonly OptimizationObjective Objective = new (); 
}