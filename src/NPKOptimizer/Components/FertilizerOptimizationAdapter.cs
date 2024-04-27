using NPKOptimizer.Contracts;
using NPKOptimizer.Domain.Collections;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.PpmTarget;
using NPKOptimizer.Domain.SolutionsFinderSettings;

namespace NPKOptimizer.Components;

/// <summary>
/// Provides an implementation of <see cref="IFertilizerOptimizer"/> using a specific optimization problem solver and mapper.
/// </summary>
public class FertilizerOptimizationAdapter : IFertilizerOptimizer
{
    protected readonly IOptimizationProblemSolver OptimizationProblemSolver;
    protected readonly IOptimizationProblemMapper Mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="FertilizerOptimizationAdapter"/> class.
    /// </summary>
    /// <param name="optimizationProblemSolver">The solver used to find optimal solutions for the fertilizer optimization problem.</param>
    /// <param name="mapper">The mapper used to convert between the domain model and the optimization problem/solution format.</param>
    public FertilizerOptimizationAdapter(IOptimizationProblemSolver optimizationProblemSolver,
        IOptimizationProblemMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(optimizationProblemSolver);
        ArgumentNullException.ThrowIfNull(mapper);
        OptimizationProblemSolver = optimizationProblemSolver;
        Mapper = mapper;
    }

    /// <summary>
    /// Optimizes the amount of each fertilizer in a collection to meet specified nutrient targets based on the given settings.
    /// </summary>
    /// <param name="target">The target PPM values for each nutrient, including the volume of water.</param>
    /// <param name="sourceCollection">The collection of fertilizers available for use.</param>
    /// <param name="settings">The settings that influence how the optimization is performed.</param>
    /// <returns>A <see cref="Solution"/> that specifies the optimized amounts of each fertilizer to meet the nutrient targets.</returns>
    public Solution Optimize(PpmTarget target, IList<FertilizerOptimizationModel> sourceCollection,
        SolutionFinderSettings settings)
    {
        OptimizationProblem problem = Mapper.CreateOptimizationProblem(target, sourceCollection, settings);

        Dictionary<string, double> result = OptimizationProblemSolver.Solve(problem);

        return Mapper.CreateSolution(result, sourceCollection, target.Liters.Value);
    }
}