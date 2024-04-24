using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.PpmTarget;
using NPKOptimizer.Domain.SolutionsFinderSettings;

namespace NPKOptimizer.Contracts;

public interface IOptimizationProblemMapper
{
    OptimizationProblem CreateOptimizationProblem (PpmTarget target,
        IList<FertilizerOptimizationModel> sourceCollection, SolutionFinderSettings settings);

    IList<Fertilizer> MapSolution(Dictionary<string, double> solutionValues,
        IList<FertilizerOptimizationModel> originalSourceCollection);
}