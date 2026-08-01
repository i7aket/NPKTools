using SYT.NPKTools.Fertilizers;

namespace SYT.NPKTools.Concentrates;

/// <summary>
/// One stock tank of a concentrate: which salts go in it, and how concentrated each ends up.
/// </summary>
/// <param name="Tank">Which tank this is.</param>
/// <param name="Components">The salts to weigh into it, in the mix's own order.</param>
public sealed record ConcentrateTank(ConcentrateType Tank, IReadOnlyList<ConcentrateComponent> Components)
{
    /// <summary>
    /// Gets the total weight of salt going into this tank, in grams.
    /// </summary>
    public double TotalGrams => Components.Sum(c => c.Grams);

    /// <summary>
    /// Gets the total dissolved solids this tank will hold, in grams per liter.
    /// </summary>
    /// <remarks>
    /// On its own this says less than it appears to, because salts differ enormously in how much will
    /// dissolve: 600 g/L of calcium nitrate is comfortable and 600 g/L of monocalcium phosphate is thirty
    /// times impossible. <see cref="SaturationFraction"/> is the figure to judge a tank by.
    /// </remarks>
    public double TotalGramsPerLiter => Components.Sum(c => c.GramsPerLiter);

    /// <summary>
    /// Gets how saturated the tank is: each salt's share of its own solubility, added up. 1 is saturated.
    /// Null when any salt in the tank has no known limit.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Salts in one tank compete for the same water, so checking each against its own limit in isolation
    /// is too permissive — four salts each at 40% of their limit will not dissolve, though none of them
    /// individually exceeds anything. Adding the fractions is the standard first-order screen for that: it
    /// is exact for a single salt and approximate for a mixture, erring slightly optimistic because salts
    /// sharing an ion crowd each other out more than the sum suggests.
    /// </para>
    /// <para>
    /// Null rather than a partial sum when a limit is missing. A sum over the known salts only would read
    /// as a verdict on the tank while silently omitting a term, and the omitted salt could be the one that
    /// binds.
    /// </para>
    /// </remarks>
    public double? SaturationFraction =>
        Components.Any(c => c.SolubilityLimit is null)
            ? null
            : Components.Sum(c => c.SaturationFraction!.Value);

    /// <summary>
    /// Gets a value indicating whether the tank is beyond saturation and will not dissolve as specified.
    /// </summary>
    /// <remarks>
    /// False when <see cref="SaturationFraction"/> is unknown, so read
    /// <see cref="ConcentratePlan.UnknownSolubility"/> alongside it rather than treating false as safe.
    /// </remarks>
    public bool IsSaturated => SaturationFraction > 1;

    /// <summary>
    /// Gets a value indicating whether this tank has nothing in it.
    /// </summary>
    /// <remarks>
    /// Normal rather than a problem: a mix of only nitrates and phosphates has no calcium to keep apart,
    /// so a single tank is enough and the other stays empty.
    /// </remarks>
    public bool IsEmpty => Components.Count == 0;
}
