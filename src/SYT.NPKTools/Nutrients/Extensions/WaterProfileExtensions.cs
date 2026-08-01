namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Reading a source-water analysis with the same tools as a finished solution.
/// </summary>
public static class WaterProfileExtensions
{
    /// <summary>
    /// Expresses a water analysis as a <see cref="Ppm"/>, so the solution analyses apply to it.
    /// </summary>
    /// <param name="water">The source water's analysis.</param>
    /// <param name="liters">
    /// A nominal volume. A water analysis is a concentration and holds regardless of how much is used, so
    /// this changes nothing the analyses read — it exists only because <see cref="Ppm"/> carries a volume.
    /// </param>
    /// <returns>The same concentrations as a ppm profile.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="water"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="liters"/> is not positive.</exception>
    /// <remarks>
    /// <para>
    /// Worth doing for two readings in particular. <see cref="PpmExtensions.EstimateConductivity"/> gives the
    /// EC of the water alone, which is directly comparable with the meter reading a grower already has and is
    /// the quickest check that an analysis was entered correctly.
    /// </para>
    /// <para>
    /// <see cref="PpmExtensions.IonBalance"/> measures something a finished recipe's balance does not. Real
    /// water is electrically neutral, but the ion that usually balances it — bicarbonate — is not one of the
    /// sixteen nutrients and so has nowhere to be entered. Whatever cation surplus an analysis shows is
    /// therefore its alkalinity, in meq/L, which is also the acid needed per litre to neutralise it.
    /// </para>
    /// </remarks>
    public static Ppm AsPpm(this WaterProfile water, double liters = 1)
    {
        ArgumentNullException.ThrowIfNull(water);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(liters);

        return new PpmBuilder()
            .AddNitrate(water.Nitrogen.Nitrate)
            .AddAmmonium(water.Nitrogen.Ammonium)
            .AddAmine(water.Nitrogen.Amine)
            .AddP(water.Phosphorus.Value)
            .AddK(water.Potassium.Value)
            .AddCa(water.Calcium.Value)
            .AddMg(water.Magnesium.Value)
            .AddS(water.Sulfur.Value)
            .AddFe(water.Iron.Value)
            .AddCu(water.Copper.Value)
            .AddMn(water.Manganese.Value)
            .AddZn(water.Zinc.Value)
            .AddB(water.Boron.Value)
            .AddMo(water.Molybdenum.Value)
            .AddCl(water.Chlorine.Value)
            .AddSi(water.Silicon.Value)
            .AddSe(water.Selenium.Value)
            .AddNa(water.Sodium.Value)
            .AddLiters(liters)
            .Build();
    }

    /// <summary>
    /// Estimates the water's alkalinity from its cation-anion gap, in meq/L.
    /// </summary>
    /// <param name="water">The source water's analysis.</param>
    /// <returns>
    /// The bicarbonate implied by the analysis, in meq/L, or zero when the analysis shows no cation surplus.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="water"/> is null.</exception>
    /// <remarks>
    /// <para>
    /// Water is electrically neutral, so the cations in an analysis must be matched by anions. Bicarbonate is
    /// usually what matches them, and it is not a plant nutrient, so it has nowhere to be entered — which
    /// means the gap left over is a measurement of it rather than an error in the analysis. That gap is also
    /// the acid required per litre to neutralise the water, which is what makes it worth surfacing: hard water
    /// pushes the root-zone pH up all season if it is not accounted for.
    /// </para>
    /// <para>
    /// An estimate from what the analysis contains, not a substitute for a measured alkalinity figure. It
    /// assumes the analysis is complete for the ions it does cover, and it cannot distinguish bicarbonate from
    /// carbonate or from an ion nobody entered. Multiply by 61 for ppm of HCO₃⁻, or by 50 for ppm as CaCO₃,
    /// the form most water reports use.
    /// </para>
    /// </remarks>
    public static double EstimatedAlkalinity(this WaterProfile water)
    {
        ArgumentNullException.ThrowIfNull(water);

        IonBalance balance = water.AsPpm().IonBalance();

        return Math.Max(0, -balance.AcidEquivalents);
    }
}
