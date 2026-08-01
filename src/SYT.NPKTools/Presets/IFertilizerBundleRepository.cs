using SYT.NPKTools.Fertilizers;

namespace SYT.NPKTools;

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
    /// <remarks>Renamed from the misspelled <c>Marco()</c> of the NPKTools.* packages.</remarks>
    IReadOnlyList<IReadOnlyList<Fertilizer>> Macro();

    /// <summary>
    /// Retrieves collections of fertilizer models optimized for micro nutrient needs.
    /// </summary>
    /// <returns>A list of lists, each representing a group of fertilizers for micro nutrient optimization.</returns>
    IReadOnlyList<IReadOnlyList<Fertilizer>> Micro();
}
