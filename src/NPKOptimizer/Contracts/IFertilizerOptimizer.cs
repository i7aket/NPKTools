using NPKOptimizer.Domain.Collections;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.PpmTarget;
using NPKOptimizer.Domain.SolutionsFinderSettings;

namespace NPKOptimizer.Contracts;

/// <summary>
/// Defines a contract for optimizing fertilizer usage based on target PPM (parts per million) values.
/// </summary>
public interface IFertilizerOptimizer
{
    /// <summary>
    /// Optimizes the amount of each fertilizer in a collection to meet specified nutrient targets.
    /// </summary>
    /// <param name="target">The target PPM values for each nutrient.</param>
    /// <param name="sourceCollection">The collection of fertilizers available for optimization.</param>
    /// <param name="settings">Settings to guide the optimization process.</param>
    /// <returns>A solution detailing the optimized quantities of each fertilizer.</returns>
    Solution? Optimize(PpmTarget target, IList<FertilizerOptimizationModel> sourceCollection, SolutionFinderSettings settings);
}