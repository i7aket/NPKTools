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
public sealed record ConcentrateComponent(Fertilizer Fertilizer, double GramsPerLiter)
{
    /// <summary>
    /// Gets the weight to measure out, in grams.
    /// </summary>
    public double Grams => Fertilizer.Weight.Value;
}
