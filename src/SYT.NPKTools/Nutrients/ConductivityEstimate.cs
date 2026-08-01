namespace SYT.NPKTools.Nutrients;

/// <summary>
/// An estimate of a solution's electrical conductivity, computed from its ions.
/// </summary>
/// <remarks>
/// <para>
/// EC is what a grower actually measures, so being able to predict it is what connects a calculated recipe
/// to the meter in the reservoir. It is computed here the way conductivity works — each ion's own molar
/// conductivity times how much of it there is — rather than by scaling total dissolved solids by a fitted
/// factor. That matters because the two are not interchangeable: a mole of sulfate conducts more than twice
/// as much as a mole of dihydrogen phosphate, so two solutions of identical ppm can read differently, and a
/// single factor cannot know which one it has been given.
/// </para>
/// <para>
/// <b>How accurate.</b> <see cref="IdealMicroSiemensPerCm"/> is the sum of limiting molar conductivities,
/// exact for infinitely dilute water and increasingly high above it, because ions interfere with each
/// other's movement as they crowd. <see cref="MicroSiemensPerCm"/> applies a Kohlrausch-form correction for
/// that, with its one coefficient taken from the certified KCl conductivity standards — the same solutions
/// meters are calibrated against. Against those standards the corrected figure is within 0.3% at 0.001 M
/// and 0.01 M, which bracket the ionic strength of a real feed.
/// </para>
/// <para>
/// Expect to be within a few percent of a meter, not to replace it. The remaining error is not the model's
/// alone: the source water's bicarbonate conducts and is not modelled here, meter calibration drifts, and
/// temperature compensation is itself approximate.
/// </para>
/// <para>
/// <b>What is left out.</b> The same ions <see cref="IonBalance"/> excludes, for the same reasons.
/// Micronutrients are ambiguous in speciation and, at under 5 ppm in total, would add roughly 3 µS/cm to a
/// figure around 2,000 — under 0.2%, and less than the meter's own tolerance. Urea and boric acid conduct
/// nothing at all, being uncharged; a solution of urea alone reads as pure water on an EC meter while
/// carrying a great deal of nitrogen, which is exactly why EC is a poor proxy for feed strength when urea
/// is involved.
/// </para>
/// </remarks>
public sealed record ConductivityEstimate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConductivityEstimate"/> record.
    /// </summary>
    internal ConductivityEstimate(
        double potassium,
        double calcium,
        double magnesium,
        double ammonium,
        double sodium,
        double nitrate,
        double phosphate,
        double sulfate,
        double chloride,
        double bicarbonate,
        double ionicStrength,
        double correction)
    {
        Bicarbonate = bicarbonate;
        Potassium = potassium;
        Calcium = calcium;
        Magnesium = magnesium;
        Ammonium = ammonium;
        Sodium = sodium;
        Nitrate = nitrate;
        Phosphate = phosphate;
        Sulfate = sulfate;
        Chloride = chloride;
        IonicStrength = ionicStrength;
        Correction = correction;
    }

    /// <summary>Gets K⁺'s share of the ideal conductivity, in µS/cm.</summary>
    public double Potassium { get; }

    /// <summary>Gets Ca²⁺'s share of the ideal conductivity, in µS/cm.</summary>
    public double Calcium { get; }

    /// <summary>Gets Mg²⁺'s share of the ideal conductivity, in µS/cm.</summary>
    public double Magnesium { get; }

    /// <summary>Gets NH₄⁺'s share of the ideal conductivity, in µS/cm.</summary>
    public double Ammonium { get; }

    /// <summary>Gets Na⁺'s share of the ideal conductivity, in µS/cm.</summary>
    public double Sodium { get; }

    /// <summary>Gets NO₃⁻'s share of the ideal conductivity, in µS/cm.</summary>
    public double Nitrate { get; }

    /// <summary>Gets H₂PO₄⁻'s share of the ideal conductivity, in µS/cm.</summary>
    public double Phosphate { get; }

    /// <summary>Gets SO₄²⁻'s share of the ideal conductivity, in µS/cm.</summary>
    public double Sulfate { get; }

    /// <summary>Gets Cl⁻'s share of the ideal conductivity, in µS/cm.</summary>
    public double Chloride { get; }

    /// <summary>
    /// Gets HCO₃⁻'s share of the ideal conductivity, in µS/cm. Zero unless bicarbonate was supplied.
    /// </summary>
    /// <remarks>
    /// Bicarbonate is not a plant nutrient and so cannot live in a <see cref="Ppm"/> profile, but it conducts
    /// and in ordinary tap water it carries most of the negative charge. Omitting it understates a moderately
    /// hard supply by around a quarter.
    /// </remarks>
    public double Bicarbonate { get; }

    /// <summary>
    /// Gets the solution's ionic strength in moles per litre, ½Σcz².
    /// </summary>
    /// <remarks>
    /// Exactly computable, and the quantity that decides how far the ideal sum overshoots. A working feed
    /// sits around 0.02–0.04; above about 0.05 the correction here is outside the range its coefficient was
    /// anchored over.
    /// </remarks>
    public double IonicStrength { get; }

    /// <summary>
    /// Gets the factor applied to the ideal sum to account for ion-ion interaction, between 0 and 1.
    /// </summary>
    /// <remarks>
    /// Exposed rather than hidden, because it is the one modelled quantity in the estimate. Around 0.91 for
    /// a typical feed — that is, the ideal sum runs about 9% high before correction. It stops changing above
    /// <see cref="ValidatedIonicStrength"/>; see <see cref="IsWithinValidatedRange"/>.
    /// </remarks>
    public double Correction { get; }

    /// <summary>
    /// The ionic strength, in mol/L, up to which this estimate is anchored on certified reference solutions.
    /// </summary>
    /// <remarks>
    /// The highest of the three KCl conductivity standards the correction is fitted against. A working
    /// nutrient solution sits around 0.02–0.04, comfortably inside it.
    /// </remarks>
    public const double ValidatedIonicStrength = 0.1;

    /// <summary>
    /// Gets a value indicating whether the solution is dilute enough for this estimate to mean anything.
    /// </summary>
    /// <remarks>
    /// <para>
    /// False above <see cref="ValidatedIonicStrength"/>, where no simple model works and this one has no
    /// reference solution to have been checked against. The figure is still monotonic there — more salt
    /// always reads as more conductivity — but it runs increasingly high and should be treated as an
    /// ordering, not a measurement.
    /// </para>
    /// <para>
    /// The case that makes this worth checking is a concentrate. Reading the EC of a tank rather than of the
    /// finished feed puts the ionic strength an order of magnitude past anything the model was anchored on: a
    /// working solution at 1:100 lands near I = 2.5. There is no useful estimate to give there, so the honest
    /// signal is this flag.
    /// </para>
    /// </remarks>
    public bool IsWithinValidatedRange => IonicStrength <= ValidatedIonicStrength;

    /// <summary>
    /// Gets the conductivity ignoring ion-ion interaction, in µS/cm. An upper bound.
    /// </summary>
    public double IdealMicroSiemensPerCm =>
        Potassium + Calcium + Magnesium + Ammonium + Sodium
        + Nitrate + Phosphate + Sulfate + Chloride + Bicarbonate;

    /// <summary>
    /// Gets the estimated conductivity in µS/cm — the figure to compare with a meter.
    /// </summary>
    public double MicroSiemensPerCm => IdealMicroSiemensPerCm * Correction;

    /// <summary>
    /// Gets the estimated conductivity in mS/cm, the unit most meters display.
    /// </summary>
    public double MilliSiemensPerCm => MicroSiemensPerCm / 1000;

    /// <summary>
    /// Converts the estimate to the "ppm" figure a TDS meter shows.
    /// </summary>
    /// <param name="scale">
    /// The conversion factor the meter uses. 500 is the NaCl scale used by Truncheon and Eutech meters; 700
    /// is the KCl scale used by Hanna; 640 also appears. Defaults to 500.
    /// </param>
    /// <returns>The displayed TDS figure.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="scale"/> is not positive.</exception>
    /// <remarks>
    /// A TDS meter measures conductivity and multiplies. The scale is a convention rather than chemistry,
    /// which is why it is a parameter and why two meters disagree by 40% on the same solution while both
    /// being correct. This is also why the nutrient ppm elsewhere in this library — actual milligrams of an
    /// element per litre — is a different quantity from a TDS meter's "ppm", and the two should never be
    /// compared.
    /// </remarks>
    public double AsTdsPpm(double scale = 500)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(scale);

        return MilliSiemensPerCm * scale;
    }
}
