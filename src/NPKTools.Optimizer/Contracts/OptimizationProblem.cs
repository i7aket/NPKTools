namespace NPKTools.Optimizer.Contracts;

/// <summary>
/// Represents an optimization problem including variables, constraints, and the objective.
/// </summary>
public record OptimizationProblem
{
    /// <summary>
    /// Gets or sets the variables for the optimization problem.
    /// </summary>
    public Dictionary<string, double> Variables { get; init; } = new();

    /// <summary>
    /// Gets or sets the list of constraints that the solution must satisfy.
    /// </summary>
    public IList<OptimizationConstraint> Constraints { get; init; } = new List<OptimizationConstraint>();

    /// <summary>
    /// Gets or sets the objective of the optimization problem.
    /// </summary>
    public OptimizationObjective Objective { get; init; } = new();

    /// <summary>
    /// Represents the objective of an optimization problem, specifying coefficients and whether it's a minimization or maximization problem.
    /// </summary>
    public record OptimizationObjective
    {
        /// <summary>
        /// Gets or sets the coefficients for the objective function.
        /// </summary>
        public IDictionary<string, double> Coefficients { get; init; } = new Dictionary<string, double>();

        /// <summary>
        /// Gets or sets a value indicating whether this objective is a minimization problem.
        /// </summary>
        public bool IsMinimization { get; init; } = true;
    }

    /// <summary>
    /// Represents a constraint in an optimization problem, containing coefficient mappings and bounds.
    /// </summary>
    public record OptimizationConstraint
    {
        /// <summary>
        /// Gets the name of the constraint.
        /// </summary>
        public required string Name;

        /// <summary>
        /// Gets the lower bound for the constraint.
        /// </summary>
        public double LowerBound { get; init; }

        /// <summary>
        /// Gets the upper bound for the constraint.
        /// </summary>
        public double UpperBound { get; init; }

        /// <summary>
        /// Gets or sets the coefficients for the constraint variables.
        /// </summary>
        public IDictionary<string, double> Coefficients { get; init; } = new Dictionary<string, double>();
    }
}
