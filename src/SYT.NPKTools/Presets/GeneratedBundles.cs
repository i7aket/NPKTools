using SYT.NPKTools.Fertilizers;

namespace SYT.NPKTools;

/// <summary>
/// Bundles generated from a list of salts, together with anything generation had to leave out.
/// </summary>
/// <param name="Bundles">
/// The bundles. The first holds every salt supplied; each of the rest leaves out exactly one, in name
/// order. A caller wanting to label a bundle can take the set difference against the first —
/// <c>Bundles[0].Except(bundle)</c> — which is what makes a recipe presentable as "without the MKP".
/// </param>
/// <param name="UncoveredElements">
/// Elements no supplied salt contains, as chemical symbols. A target asking for one of these cannot be
/// met by any bundle, and saying which salt is missing is more use than returning no solutions and
/// leaving the caller to guess why.
/// </param>
/// <param name="BundlesDropped">
/// How many bundles exceeded <see cref="BundleGenerationSettings.MaxBundles"/> and were discarded.
/// </param>
/// <remarks>
/// The two reporting fields are the point of this type. A generator that quietly ignored a missing
/// magnesium source, or returned eight bundles from a shelf that warranted eighty, would read as "this
/// is everything" when it was not.
/// </remarks>
public sealed record GeneratedBundles(
    IReadOnlyList<IReadOnlyList<Fertilizer>> Bundles,
    IReadOnlyList<string> UncoveredElements,
    int BundlesDropped)
{
    /// <summary>
    /// Gets an empty result, for a shelf holding nothing this tier can use.
    /// </summary>
    public static GeneratedBundles Empty { get; } = new([], [], 0);

    /// <summary>
    /// Gets a value indicating whether every element is reachable and no bundle was discarded.
    /// </summary>
    public bool IsComplete => UncoveredElements.Count == 0 && BundlesDropped == 0;
}
