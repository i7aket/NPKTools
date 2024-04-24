using NPKOptimizer.Contracts;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.PpmTarget;
using NPKOptimizer.Domain.SolutionsFinderSettings;

namespace NPKOptimizer.Components;

public class FertilizerOptimizationAdapter : IFertilizerOptimizer
{
    protected readonly IOptimizationProblemSolver OptimizationProblemSolver;
    protected readonly IOptimizationProblemMapper Mapper;

    public FertilizerOptimizationAdapter(IOptimizationProblemSolver optimizationProblemSolver, IOptimizationProblemMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(optimizationProblemSolver);
        OptimizationProblemSolver = optimizationProblemSolver;
        Mapper = mapper;
    }

    public IList<Fertilizer> Optimize(PpmTarget target, IList<FertilizerOptimizationModel> sourceCollection,
        SolutionFinderSettings settings)
    {
        OptimizationProblem problem = Mapper.CreateOptimizationProblem(target, sourceCollection, settings);

        Dictionary<string, double> result = OptimizationProblemSolver.Solve(problem);
        
        return Mapper.MapSolution(result, sourceCollection);
    }
}