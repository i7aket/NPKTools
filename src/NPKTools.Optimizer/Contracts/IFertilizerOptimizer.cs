using NPKTools.Core.Domain.Collections;
using NPKTools.Core.Domain.Fertilizers;
using NPKTools.Core.Domain.PpmTarget;
using NPKTools.Core.Domain.SolutionsFinderSettings;

namespace NPKTools.Optimizer.Contracts;

/// <summary>
/// Defines a contract for optimizing fertilizer usage based on target PPM (parts per million) values.
/// </summary>
public interface IFertilizerOptimizer
{
    /// <summary>
    /// Optimizes the amount of each fertilizer in a collection to meet specified nutrient targets based on the given settings.
    /// </summary>
    /// <param name="target">The target PPM values for each nutrient, including volume of water.</param>
    /// <param name="sourceCollection">The collection of fertilizers available for optimization.</param>
    /// <param name="settings">Settings to guide the optimization process.</param>
    /// <returns>
    /// A solution detailing the optimized quantities of each fertilizer to meet the nutrient targets.
    /// Returns null if an optimal solution cannot be found.
    /// </returns>
    Solution? Optimize(PpmTarget target, IList<Fertilizer> sourceCollection, SolutionFinderSettings settings);
}