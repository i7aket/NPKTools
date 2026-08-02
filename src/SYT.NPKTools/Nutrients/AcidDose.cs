namespace SYT.NPKTools.Nutrients;

/// <summary>
/// The acid needed to bring source water to a working pH.
/// </summary>
/// <remarks>
/// <para>
/// Worked from the carbonate equilibrium rather than from a rule of thumb. Alkalinity at ordinary
/// source-water pH is very nearly all bicarbonate, so the total carbonate follows from the alkalinity
/// and the water's own pH; adding strong acid converts bicarbonate to carbonic acid without changing
/// that total, and the fraction left at the target pH is what does not need neutralising.
/// </para>
/// <para>
/// The usual rule — neutralise the whole alkalinity — overstates the dose by about a quarter at
/// pH 5.8, because a fifth of the carbonate is still bicarbonate there.
/// </para>
/// <para>
/// <b>The model is for a closed vessel.</b> An open, aerated reservoir loses carbon dioxide, the
/// equilibrium shifts back, and pH climbs again over hours; the dose a grower ends up adding
/// approaches the full alkalinity. That is chemistry, not an error here, and it is worth stating
/// wherever this figure is shown.
/// </para>
/// </remarks>
public static class AcidDose
{
    /// <summary>The first dissociation of carbonic acid, at 25 °C.</summary>
    private const double FirstPka = 6.35;

    /// <summary>The second dissociation, bicarbonate to carbonate.</summary>
    private const double SecondPka = 10.33;

    /// <summary>
    /// The share of a water's total carbonate that is bicarbonate at a given pH.
    /// </summary>
    /// <param name="ph">The pH.</param>
    /// <returns>A fraction between zero and one.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the pH is outside 0–14.</exception>
    public static double BicarbonateFraction(double ph)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(ph);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(ph, 14);

        return 1 / (1 + Math.Pow(10, FirstPka - ph) + Math.Pow(10, ph - SecondPka));
    }

    /// <summary>
    /// Works out the acid a tank needs, and what that acid contributes.
    /// </summary>
    /// <param name="alkalinityMilliequivalentsPerLitre">The water's alkalinity, in meq/L.</param>
    /// <param name="waterPh">The pH of the untreated water.</param>
    /// <param name="targetPh">The pH to reach.</param>
    /// <param name="acid">The acid to use.</param>
    /// <param name="litres">The volume of the tank.</param>
    /// <returns>The plan. Zero throughout when nothing needs neutralising.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="acid"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the alkalinity is negative, a pH is outside 0–14, or the volume is not positive.
    /// </exception>
    public static AcidPlan Calculate(
        double alkalinityMilliequivalentsPerLitre,
        double waterPh,
        double targetPh,
        Acid acid,
        double litres)
    {
        ArgumentNullException.ThrowIfNull(acid);
        ArgumentOutOfRangeException.ThrowIfNegative(alkalinityMilliequivalentsPerLitre);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(litres);

        double residualFraction = BicarbonateFraction(targetPh);
        double waterFraction = BicarbonateFraction(waterPh);

        if (alkalinityMilliequivalentsPerLitre <= 0 || targetPh >= waterPh)
        {
            return new AcidPlan(0, 0, acid.NutrientSymbol, 0);
        }

        double totalCarbonate = alkalinityMilliequivalentsPerLitre / waterFraction;
        double freeProtons = Math.Pow(10, -targetPh) * 1000;
        double residual = (totalCarbonate * residualFraction) - freeProtons;
        double needed = Math.Max(0, alkalinityMilliequivalentsPerLitre - residual);

        return new AcidPlan(
            needed,
            needed * litres / acid.EquivalentsPerLitre,
            acid.NutrientSymbol,
            needed * acid.MilligramsOfNutrientPerMilliequivalent);
    }
}
