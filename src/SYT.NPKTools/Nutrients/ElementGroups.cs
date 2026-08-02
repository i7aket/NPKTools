using SYT.NPKTools.Internal;

namespace SYT.NPKTools.Nutrients;

/// <summary>
/// The element symbols an input form is grouped by, in display order.
/// </summary>
/// <remarks>
/// <para>
/// Three groups rather than two, because the library already draws the finer distinction and an
/// interface that draws only the coarse one loses information. <see cref="Micro"/> is what
/// <see cref="FertilizerBundleGenerator"/> doses for. <see cref="CounterIons"/> is chlorine
/// and sodium: they arrive with other salts rather than being dosed for, which is why the generator
/// leaves them out of its own micro list — reporting them as uncovered would be noise.
/// </para>
/// <para>
/// They are still entered, though. A water analysis reports both, and sodium is the whole story in
/// water from an ion-exchange softener, so leaving them off a form would hide the case that most
/// needs seeing.
/// </para>
/// </remarks>
public static class ElementGroups
{
    /// <summary>The macronutrients, in the order a feed chart lists them.</summary>
    public static IReadOnlyList<string> Macro { get; } =
        [Names.N, Names.P, Names.K, Names.Ca, Names.Mg, Names.S];

    /// <summary>The micronutrients that are dosed for.</summary>
    public static IReadOnlyList<string> Micro { get; } =
        [Names.Fe, Names.Cu, Names.Mn, Names.Zn, Names.B, Names.Mo, Names.Si, Names.Se];

    /// <summary>Ions that arrive with other salts rather than being dosed for.</summary>
    public static IReadOnlyList<string> CounterIons { get; } = [Names.Cl, Names.Na];

    /// <summary>Every symbol an analysis or a target can carry, macro first.</summary>
    public static IReadOnlyList<string> All { get; } = [.. Macro, .. Micro, .. CounterIons];
}
