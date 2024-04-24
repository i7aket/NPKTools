namespace NPKOptimizer.Contracts;

public record OptimizationProblem
{
    public Dictionary<string, double> Variables = new(); 
    public IList<OptimizationConstraint> Constraints = new List<OptimizationConstraint>();
    public OptimizationObjective Objective = new (); 
    public record OptimizationObjective
    {
        public IDictionary<string, double> Coefficients = new Dictionary<string, double>(); 
        public bool IsMinimization = true; 
    }
    public record OptimizationConstraint
    {
        public required string Name; 
        public double LowerBound; 
        public double UpperBound; 
        public IDictionary<string, double> Coefficients = new Dictionary<string, double>(); 
    }
}