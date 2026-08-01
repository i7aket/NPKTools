namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Reports an element the source water already supplies in excess of the target.
/// </summary>
/// <param name="Element">
/// The element symbol, using the same names <see cref="IPpmTargetParser"/> accepts — <c>N</c>,
/// <c>P</c>, <c>K</c>, <c>Ca</c> and so on — so it can be fed straight back into a target string.
/// </param>
/// <param name="InWater">What the water supplies, in ppm.</param>
/// <param name="Target">What was asked for, in ppm.</param>
/// <remarks>
/// This is not an error, it is a fact about the water. Fertilizer can only add, so an element already
/// above target cannot be brought down by mixing: the options are to raise the target, or to dilute the
/// source water with something purer. Hard water hitting a low calcium target is the everyday case, and
/// it is exactly why growers buy reverse osmosis equipment.
/// </remarks>
public sealed record NutrientExcess(string Element, double InWater, double Target)
{
    /// <summary>
    /// Gets how much the water overshoots the target by, in ppm.
    /// </summary>
    public double Overshoot => InWater - Target;
}
