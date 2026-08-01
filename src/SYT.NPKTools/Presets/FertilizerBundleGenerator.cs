using SYT.NPKTools.Fertilizers;
using SYT.NPKTools.Internal;

namespace SYT.NPKTools;

/// <summary>
/// Builds optimization bundles from whatever salts a person actually has.
/// </summary>
/// <remarks>
/// <para>
/// The built-in catalogue offers eighteen macro bundles because they were written out by hand. Someone
/// working from their own shelf had no equivalent: one list of salts is one bundle, so they saw a single
/// recipe where the presets give a dozen to compare. This closes that gap — the same salts, the same
/// solver, the same number of alternatives.
/// </para>
/// <para>
/// <b>How.</b> The first bundle holds everything on the shelf; each of the others leaves out exactly one
/// salt. Handing the optimizer every salt at once yields the single best mix it can find, and every
/// superset of that yields the same mix again — so alternatives can only come from taking something
/// away. Removing one salt forces the linear program to route around it, which is both a genuinely
/// different recipe and the question a grower actually asks: <em>what does this look like without the
/// MKP?</em>
/// </para>
/// <para>
/// <b>Why not combinations.</b> Two other strategies were measured against the hand-written catalogue on
/// three macro targets, counting distinct recipes returned. Picking one source per element and taking the
/// cross product scored 5; holding out pairs and triples as well as singles scored the same 21 as
/// holding out singles alone, because a smaller subset either reproduces a recipe already found or
/// solves nothing. The hand-written catalogue scored 19. Depth beyond one buys nothing, and building
/// bundles up from per-element choices produces bundles too small to satisfy six simultaneous
/// constraints at non-negative weights.
/// </para>
/// </remarks>
public static class FertilizerBundleGenerator
{
    /// <summary>
    /// The elements a macro target can ask for.
    /// </summary>
    private static readonly string[] MacroElements =
        [Names.N, Names.P, Names.K, Names.Ca, Names.Mg, Names.S];

    /// <summary>
    /// The elements a micro target can ask for. Chlorine and sodium are absent deliberately: they arrive
    /// as counter-ions rather than being dosed for, so reporting them as uncovered would be noise.
    /// </summary>
    private static readonly string[] MicroElements =
        [Names.Fe, Names.Cu, Names.Mn, Names.Zn, Names.B, Names.Mo, Names.Si, Names.Se];

    /// <summary>
    /// Generates macro bundles from the salts that carry macro nutrients.
    /// </summary>
    /// <param name="available">The salts on hand. Micro salts are ignored; see <see cref="IsMicro"/>.</param>
    /// <param name="settings">Bounds on the output. Defaults to <see cref="BundleGenerationSettings.Default"/>.</param>
    /// <returns>The bundles, and what generation left out.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="available"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <see cref="BundleGenerationSettings.MaxBundles"/> is not positive.</exception>
    public static GeneratedBundles GenerateMacro(
        IEnumerable<Fertilizer> available,
        BundleGenerationSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(available);

        // Usable as well as non-micro: a salt of nothing but sodium and chloride is neither tier's, and
        // admitting it here would put a salt in every macro bundle that no target can ever call for.
        return Generate(available.Where(f => IsUsable(f) && !IsMicro(f)), MacroElements, settings);
    }

    /// <summary>
    /// Generates micro bundles from the salts that carry micronutrients.
    /// </summary>
    /// <param name="available">The salts on hand. Macro-only salts are ignored.</param>
    /// <param name="settings">Bounds on the output. Defaults to <see cref="BundleGenerationSettings.Default"/>.</param>
    /// <returns>The bundles, and what generation left out.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="available"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <see cref="BundleGenerationSettings.MaxBundles"/> is not positive.</exception>
    public static GeneratedBundles GenerateMicro(
        IEnumerable<Fertilizer> available,
        BundleGenerationSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(available);

        return Generate(available.Where(IsMicro), MicroElements, settings);
    }

    /// <summary>
    /// Reports whether a salt belongs to the micro tier.
    /// </summary>
    /// <remarks>
    /// Carrying any micronutrient is what decides it, even when the salt also carries a macro element.
    /// Iron sulfate is a micro salt whose sulfur is incidental: dosing it to meet a sulfur target would
    /// mean iron at a hundred times the intended rate. This mirrors how the built-in catalogue is split.
    /// </remarks>
    /// <param name="fertilizer">The salt to classify.</param>
    /// <returns><see langword="true"/> when the salt carries at least one micronutrient.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="fertilizer"/> is null.</exception>
    public static bool IsMicro(Fertilizer fertilizer)
    {
        ArgumentNullException.ThrowIfNull(fertilizer);

        return MicroElements.Any(element => Content(fertilizer, element) > 0);
    }

    /// <summary>
    /// Reports whether a salt carries any element a target can ask for.
    /// </summary>
    /// <remarks>
    /// A salt of nothing but sodium and chloride is real — table salt — but there is no target it helps
    /// meet, so it belongs in no bundle. Callers surface these rather than let them look included.
    /// </remarks>
    /// <param name="fertilizer">The salt to check.</param>
    /// <returns><see langword="true"/> when the salt carries at least one macro or micro nutrient.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="fertilizer"/> is null.</exception>
    public static bool IsUsable(Fertilizer fertilizer)
    {
        ArgumentNullException.ThrowIfNull(fertilizer);

        return MacroElements.Concat(MicroElements).Any(element => Content(fertilizer, element) > 0);
    }

    private static GeneratedBundles Generate(
        IEnumerable<Fertilizer> available,
        string[] elements,
        BundleGenerationSettings? settings)
    {
        settings ??= BundleGenerationSettings.Default;
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(
            settings.MaxBundles,
            nameof(BundleGenerationSettings.MaxBundles));

        // The same salt listed twice is one salt. Left in, it would become two identical columns in the
        // linear program — harmless to the answer, but the recipe would list the salt twice. Sorting by
        // name makes the output stable regardless of the order the caller supplied.
        List<Fertilizer> salts = [.. available
            .DistinctBy(f => f.Name.Value, StringComparer.OrdinalIgnoreCase)
            .OrderBy(f => f.Name.Value, StringComparer.Ordinal)];

        if (salts.Count == 0)
        {
            return GeneratedBundles.Empty;
        }

        List<string> uncovered =
            [.. elements.Where(element => !salts.Any(f => Content(f, element) > 0))];

        // Everything on the shelf, then the shelf minus each salt in turn. A bundle of one salt cannot
        // be reduced further, so a single-salt shelf yields exactly that one bundle.
        List<IReadOnlyList<Fertilizer>> bundles = [salts];

        if (salts.Count > 1)
        {
            for (int held = 0; held < salts.Count && bundles.Count < settings.MaxBundles; held++)
            {
                bundles.Add([.. salts.Where((_, index) => index != held)]);
            }
        }

        int candidates = salts.Count > 1 ? salts.Count + 1 : 1;
        int dropped = Math.Max(0, candidates - settings.MaxBundles);

        return new GeneratedBundles(bundles, uncovered, dropped);
    }

    /// <summary>
    /// Gets how much of an element a salt contains, as a percentage by weight.
    /// </summary>
    private static double Content(Fertilizer fertilizer, string element) => element switch
    {
        Names.N => fertilizer.Nitrogen.Value,
        Names.P => fertilizer.Phosphorus.Value,
        Names.K => fertilizer.Potassium.Value,
        Names.Ca => fertilizer.Calcium.Value,
        Names.Mg => fertilizer.Magnesium.Value,
        Names.S => fertilizer.Sulfur.Value,
        Names.Fe => fertilizer.Iron.Value,
        Names.Cu => fertilizer.Copper.Value,
        Names.Mn => fertilizer.Manganese.Value,
        Names.Zn => fertilizer.Zinc.Value,
        Names.B => fertilizer.Boron.Value,
        Names.Mo => fertilizer.Molybdenum.Value,
        Names.Si => fertilizer.Silicon.Value,
        Names.Se => fertilizer.Selenium.Value,
        _ => 0
    };
}
