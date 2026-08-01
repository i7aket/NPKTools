namespace SYT.NPKTools;

/// <summary>
/// The result of a combined macronutrient and micronutrient search.
/// </summary>
/// <param name="Macro">
/// Mixes satisfying the macronutrient part of the target, or <see cref="Solutions.Empty"/> if none do.
/// </param>
/// <param name="Micro">
/// Mixes satisfying the micronutrient part of the target, or <see cref="Solutions.Empty"/> if none do.
/// </param>
/// <remarks>
/// The two sets are independent: a target naming only macronutrients legitimately yields an empty
/// <paramref name="Micro"/>, and vice versa. Macro and micro fertilizers are blended into separate
/// concentrate tanks, so there is no single combined mix to return.
/// </remarks>
public sealed record FertilizerSolutions(Solutions Macro, Solutions Micro)
{
    /// <summary>
    /// A result carrying no solutions at all.
    /// </summary>
    public static readonly FertilizerSolutions Empty = new(Solutions.Empty, Solutions.Empty);

    /// <summary>
    /// Gets a value indicating whether both sets are empty.
    /// </summary>
    public bool IsEmpty => Macro.Count == 0 && Micro.Count == 0;
}
