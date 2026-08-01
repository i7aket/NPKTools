using SYT.NPKTools.Fertilizers;

namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Analysis of a finished solution: the ratios that characterise it, and which fertilizer supplied what.
/// </summary>
public static class PpmExtensions
{
    /// <summary>
    /// Describes the solution by the ratios between its nutrients rather than their absolute amounts.
    /// </summary>
    /// <param name="ppm">The measured concentrations.</param>
    /// <returns>The ratios, and the total concentration.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="ppm"/> is null.</exception>
    public static NutrientRatios Ratios(this Ppm ppm) => new(ppm);

    /// <summary>
    /// Breaks a mix down into what each fertilizer in it contributes.
    /// </summary>
    /// <param name="solution">The mix, as returned by the optimizer.</param>
    /// <param name="calculator">The ppm calculator to measure each fertilizer with.</param>
    /// <returns>
    /// One entry per fertilizer, in the mix's own order. Summing the entries reproduces the mix's total
    /// concentrations, because each is measured against the same water volume.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="solution"/> or <paramref name="calculator"/> is null.
    /// </exception>
    /// <remarks>
    /// Measured through the same calculator as the whole mix rather than by separate arithmetic, so the
    /// parts cannot drift from the total.
    /// </remarks>
    public static IReadOnlyList<FertilizerContribution> Breakdown(
        this Solution solution,
        IPpmCalculationService calculator)
    {
        ArgumentNullException.ThrowIfNull(solution);
        ArgumentNullException.ThrowIfNull(calculator);

        List<FertilizerContribution> contributions = new(solution.Count);

        foreach (Fertilizer fertilizer in solution)
        {
            contributions.Add(new FertilizerContribution(
                fertilizer,
                calculator.CalculatePpm([fertilizer], solution.WaterLiters)));
        }

        return contributions;
    }
}
