using SYT.NPKTools.Fertilizers;

namespace SYT.NPKTools;

/// <summary>
/// A bundle repository built from a person's own salts instead of the preset catalogue.
/// </summary>
/// <remarks>
/// <para>
/// This is the whole answer to "I want to use what I have". It is an
/// <see cref="IFertilizerBundleRepository"/>, so everything downstream — the optimization service, the
/// solver, the ppm calculator, the concentrate split — works on custom salts exactly as it does on the
/// presets. Nothing else in the library needs to know the difference.
/// </para>
/// <para>
/// Bundles are generated once and cached, the same as the preset repository, because generation walks a
/// cross product and a caller may hit <see cref="Macro"/> repeatedly.
/// </para>
/// </remarks>
public sealed class CustomFertilizerBundleRepository : IFertilizerBundleRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomFertilizerBundleRepository"/> class.
    /// </summary>
    /// <param name="salts">
    /// The salts on hand, in any order and any mix of macro and micro. Weights are irrelevant here —
    /// the optimizer computes them — so a salt need only carry its composition.
    /// </param>
    /// <param name="settings">
    /// Bounds on how many bundles to generate. Defaults to <see cref="BundleGenerationSettings.Default"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="salts"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when either limit in <paramref name="settings"/> is not positive.</exception>
    public CustomFertilizerBundleRepository(
        IEnumerable<Fertilizer> salts,
        BundleGenerationSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(salts);

        Fertilizer[] all = [.. salts];

        MacroGeneration = FertilizerBundleGenerator.GenerateMacro(all, settings);
        MicroGeneration = FertilizerBundleGenerator.GenerateMicro(all, settings);
        UnusableSalts =
        [
            .. all.Where(f => !FertilizerBundleGenerator.IsUsable(f))
                  .Select(f => f.Name.Value)
                  .Distinct(StringComparer.OrdinalIgnoreCase)
                  .OrderBy(name => name, StringComparer.Ordinal)
        ];
    }

    /// <summary>
    /// Gets the macro bundles together with what generation left out.
    /// </summary>
    public GeneratedBundles MacroGeneration { get; }

    /// <summary>
    /// Gets the micro bundles together with what generation left out.
    /// </summary>
    public GeneratedBundles MicroGeneration { get; }

    /// <summary>
    /// Gets the names of supplied salts that carry no element any target can ask for.
    /// </summary>
    /// <remarks>
    /// Reported rather than ignored: a salt listed here is in no bundle, and a caller who thinks it is
    /// being used would misread every recipe that comes back.
    /// </remarks>
    public IReadOnlyList<string> UnusableSalts { get; }

    /// <inheritdoc />
    public IReadOnlyList<IReadOnlyList<Fertilizer>> Macro() => MacroGeneration.Bundles;

    /// <inheritdoc />
    public IReadOnlyList<IReadOnlyList<Fertilizer>> Micro() => MicroGeneration.Bundles;
}
