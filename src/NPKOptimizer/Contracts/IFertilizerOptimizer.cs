using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.PpmTarget;
using NPKOptimizer.Domain.SolutionsFinderSettings;

namespace NPKOptimizer.Contracts;

public interface IFertilizerOptimizer
{
    IList<Fertilizer> Optimize (PpmTarget target, IList<FertilizerOptimizationModel> sourceCollection, SolutionFinderSettings settings);
}