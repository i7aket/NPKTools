using NPKTools.Core.Domain.Fertilizers;

namespace NPKTools.Optimizer.Preset;

/// <summary>
/// Defines a contract for repositories managing collections of fertilizer optimization models.
/// This interface allows access to specific bundles of fertilizers categorized into macro and micro nutrients.
/// </summary>
public interface IFertilizerBundleRepository
{
    /// <summary>
    /// Retrieves collections of fertilizer models optimized for macro nutrient needs.
    /// </summary>
    /// <returns>A list of lists, each representing a group of fertilizers for macro nutrient optimization.</returns>
    IList<IList<Fertilizer>> Marco();

    /// <summary>
    /// Retrieves collections of fertilizer models optimized for micro nutrient needs.
    /// </summary>
    /// <returns>A list of lists, each representing a group of fertilizers for micro nutrient optimization.</returns>
    IList<IList<Fertilizer>> Micro();
}