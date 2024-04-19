using NPKOptimizer.Common;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.NPK;
using NPKOptimizer.Domain.SolutionsFinderSettings;

namespace NPKOptimizer.Components.OptimizationProblemSolvers.Contracts;

public interface IFertilizerOptimizer
{
    ActionResult<FertilizerCollection> Optimize(NpkTarget target, FertilizerCollection collection, SolutionFinderSettings settings);
}