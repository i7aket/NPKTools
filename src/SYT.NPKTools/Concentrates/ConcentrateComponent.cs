using SYT.NPKTools.Fertilizers;

namespace SYT.NPKTools.Concentrates;

/// <summary>
/// One fertilizer weighed into a concentrate tank.
/// </summary>
/// <param name="Fertilizer">
/// The fertilizer, carrying the same weight the working solution called for. Concentrating changes how
/// much water the salt goes into, never how much salt is needed.
/// </param>
/// <param name="GramsPerLiter">
/// How concentrated this salt ends up in the tank. This is the figure to check against the salt's
/// solubility: a recipe that dissolves happily at working strength can be impossible at 100×.
/// </param>
/// <param name="SolubilityLimit">
/// How much of this salt a litre of water holds at 20 °C, or null when no figure is known for it. Null is
/// the honest answer for a custom salt and for the catalogue entries whose published figures disagree; see
/// <see cref="SolubilityTable"/>.
/// </param>
public sealed record ConcentrateComponent(
    Fertilizer Fertilizer,
    double GramsPerLiter,
    double? SolubilityLimit)
{
    /// <summary>
    /// Gets the weight to measure out, in grams.
    /// </summary>
    public double Grams => Fertilizer.Weight.Value;

    /// <summary>
    /// Gets how much of this salt's solubility the tank uses, where 1 is saturated — or null when the
    /// limit is unknown.
    /// </summary>
    /// <remarks>
    /// Above 1 the salt cannot dissolve at this strength. Summed across a tank it becomes a screen for the
    /// mixture as a whole; see <see cref="ConcentrateTank.SaturationFraction"/>.
    /// </remarks>
    public double? SaturationFraction =>
        SolubilityLimit is null ? null : GramsPerLiter / SolubilityLimit.Value;

    /// <summary>
    /// Gets a value indicating whether this salt alone is asked to dissolve past its limit.
    /// </summary>
    /// <remarks>
    /// The most certain signal in a concentrate plan: arithmetic against a published figure, rather than a
    /// prediction. False when the limit is unknown, which is why
    /// <see cref="ConcentratePlan.UnknownSolubility"/> has to be read alongside it.
    /// </remarks>
    public bool ExceedsSolubility => SaturationFraction > 1;
}
