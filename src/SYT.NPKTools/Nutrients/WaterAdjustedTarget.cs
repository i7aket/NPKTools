namespace SYT.NPKTools.Nutrients;

/// <summary>
/// A nutrient target with the source water's own contribution already deducted, plus a note of
/// anything the water oversupplies.
/// </summary>
/// <param name="Target">
/// What the fertilizers still have to provide. Feed this to the optimizer, not the original target.
/// </param>
/// <param name="Excesses">
/// Elements the water already supplies above the target, if any. Their entries in
/// <paramref name="Target"/> are zero, because no amount of fertilizer can reduce them.
/// </param>
/// <remarks>
/// Deducting the water is deliberately a separate step ahead of the optimizer rather than something
/// buried inside it: the result is an ordinary <see cref="PpmTarget"/>, so nothing downstream needs to
/// know that water was ever involved, and the arithmetic stays inspectable.
/// </remarks>
public sealed record WaterAdjustedTarget(PpmTarget Target, IReadOnlyList<NutrientExcess> Excesses)
{
    /// <summary>
    /// Gets a value indicating whether the water oversupplies at least one element, meaning the
    /// original target cannot be reached exactly by adding fertilizer.
    /// </summary>
    public bool HasExcesses => Excesses.Count > 0;
}
