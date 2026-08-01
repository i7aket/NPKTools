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
    /// A rough feasibility signal for the tank as a whole. Individual salts have their own solubility
    /// limits and one of them will usually bind before this total does, so treat a comfortable figure
    /// here as necessary rather than sufficient.
    /// </remarks>
    public double TotalGramsPerLiter => Components.Sum(c => c.GramsPerLiter);

    /// <summary>
    /// Gets a value indicating whether this tank has nothing in it.
    /// </summary>
    /// <remarks>
    /// Normal rather than a problem: a mix of only nitrates and phosphates has no calcium to keep apart,
    /// so a single tank is enough and the other stays empty.
    /// </remarks>
    public bool IsEmpty => Components.Count == 0;
}
