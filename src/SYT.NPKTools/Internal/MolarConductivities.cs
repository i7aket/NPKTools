namespace SYT.NPKTools.Internal;

/// <summary>
/// Limiting molar conductivities at 25 °C, in S·cm² per mole of ion.
/// </summary>
/// <remarks>
/// <para>
/// These are per mole of the ion rather than per equivalent, so the divalent values already carry their
/// two charges: sulfate's 160.0 is 2 × 80.0. Multiplying a concentration in mM by one of these gives
/// µS/cm directly, which is what makes the summation in
/// <see cref="Nutrients.ConductivityEstimate"/> come out in a meter's own unit without a scale factor.
/// </para>
/// <para>
/// 25 °C is not an arbitrary choice: conductivity meters apply automatic temperature compensation and
/// report as if at 25 °C, so a figure computed at 25 °C is directly comparable with a reading.
/// </para>
/// </remarks>
internal static class MolarConductivities
{
    // Cations.
    public const double Potassium = 73.5;
    public const double Calcium = 119.0;
    public const double Magnesium = 106.0;
    public const double Ammonium = 73.5;
    public const double Sodium = 50.1;

    // Anions.
    public const double Nitrate = 71.4;
    public const double DihydrogenPhosphate = 36.0;
    public const double Sulfate = 160.0;
    public const double Chloride = 76.3;

    /// <summary>
    /// The coefficient in the Kohlrausch-form correction for ion-ion interaction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A sum of limiting conductivities describes infinitely dilute water. In a real solution the ions
    /// interfere with each other's movement, so the true conductivity is lower, and increasingly so with
    /// ionic strength: the sum runs 1.9% high at 0.001 M, 6.1% at 0.01 M and 16.3% at 0.1 M. Kohlrausch's
    /// square-root form captures that shape, and applying it as
    /// <c>EC = ideal × (1 − k√I)</c> needs one coefficient.
    /// </para>
    /// <para>
    /// This one comes from the certified KCl conductivity standards — 147, 1412 and 12,880 µS/cm at 25 °C
    /// for 0.001, 0.01 and 0.1 M — which are the same reference solutions conductivity meters are
    /// calibrated against. At 0.55 the model lands within 0.3% of the first two, which bracket the ionic
    /// strength a nutrient solution actually has (0.01–0.04 M), and about 4% low at 0.1 M, an order of
    /// magnitude stronger than any feed. It is a reference-anchored constant rather than a coefficient
    /// fitted to fertilizer recipes.
    /// </para>
    /// </remarks>
    public const double InteractionCoefficient = 0.55;
}
