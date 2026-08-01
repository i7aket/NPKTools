namespace SYT.NPKTools.Nutrients;

/// <summary>
/// The ratios between nutrients in a solution, which is how a nutrient profile is normally judged.
/// </summary>
/// <remarks>
/// <para>
/// Absolute ppm figures say how strong a solution is; the ratios say what it will actually do. Two
/// solutions with identical total strength behave completely differently if one has four times the
/// potassium relative to calcium.
/// </para>
/// <para>
/// Every ratio is expressed as "how many parts of the first element per one part of the second", so
/// <see cref="PotassiumToCalcium"/> of 2 means twice as much potassium as calcium by weight. A ratio is
/// null when its denominator is zero, because "unbounded" is not a number and silently returning zero
/// or infinity would be worse than saying nothing.
/// </para>
/// <para>
/// Obtain one with <see cref="PpmExtensions.Ratios"/>.
/// </para>
/// </remarks>
public sealed record NutrientRatios
{
    /// <summary>
    /// Gets the ratio of nitrate to ammonium nitrogen.
    /// </summary>
    /// <remarks>
    /// The one ratio worth watching most closely, because it drives pH movement at the root rather than
    /// plant nutrition: taking up ammonium acidifies the root zone, taking up nitrate alkalizes it. A
    /// solution that is mostly nitrate drifts pH up over the days after mixing, one with a large
    /// ammonium share drifts down. Null when there is no ammonium, which is the common case for a
    /// nitrate-only mix.
    /// </remarks>
    public double? NitrateToAmmonium { get; }

    /// <summary>
    /// Gets the ratio of total nitrogen to potassium.
    /// </summary>
    /// <remarks>
    /// Broadly tracks vegetative versus generative balance: relatively more nitrogen favours leaf and
    /// shoot growth, relatively more potassium favours fruiting and ripening.
    /// </remarks>
    public double? NitrogenToPotassium { get; }

    /// <summary>
    /// Gets the ratio of potassium to calcium.
    /// </summary>
    /// <remarks>
    /// The two compete for uptake, so a high value can starve a plant of calcium even when the absolute
    /// calcium figure looks adequate.
    /// </remarks>
    public double? PotassiumToCalcium { get; }

    /// <summary>
    /// Gets the ratio of calcium to magnesium.
    /// </summary>
    /// <remarks>
    /// These also compete. Both directions cause visible trouble, which is why the ratio is watched
    /// rather than the two figures separately.
    /// </remarks>
    public double? CalciumToMagnesium { get; }

    /// <summary>
    /// Gets the ratio of potassium to magnesium.
    /// </summary>
    public double? PotassiumToMagnesium { get; }

    /// <summary>
    /// Gets the ratio of nitrogen to sulfur.
    /// </summary>
    public double? NitrogenToSulfur { get; }

    /// <summary>
    /// Gets the ratio of nitrogen to phosphorus.
    /// </summary>
    public double? NitrogenToPhosphorus { get; }

    /// <summary>
    /// Gets the total of every nutrient concentration, in ppm.
    /// </summary>
    /// <remarks>
    /// Useful as a strength figure, but not the same thing as a TDS meter reading: a meter infers
    /// dissolved solids from electrical conductivity, which depends on which ions are present and not
    /// merely on how much of everything there is.
    /// </remarks>
    public double TotalPpm { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NutrientRatios"/> record from a measured solution.
    /// </summary>
    /// <param name="ppm">The nutrient concentrations to describe.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="ppm"/> is null.</exception>
    public NutrientRatios(Ppm ppm)
    {
        ArgumentNullException.ThrowIfNull(ppm);

        TotalPpm = ppm.Value;

        NitrateToAmmonium = Divide(ppm.Nitrogen.Nitrate, ppm.Nitrogen.Ammonium);
        NitrogenToPotassium = Divide(ppm.Nitrogen.Value, ppm.Potassium.Value);
        PotassiumToCalcium = Divide(ppm.Potassium.Value, ppm.Calcium.Value);
        CalciumToMagnesium = Divide(ppm.Calcium.Value, ppm.Magnesium.Value);
        PotassiumToMagnesium = Divide(ppm.Potassium.Value, ppm.Magnesium.Value);
        NitrogenToSulfur = Divide(ppm.Nitrogen.Value, ppm.Sulfur.Value);
        NitrogenToPhosphorus = Divide(ppm.Nitrogen.Value, ppm.Phosphorus.Value);
    }

    /// <summary>
    /// Divides, returning null rather than infinity or NaN when the denominator is absent.
    /// </summary>
    private static double? Divide(double numerator, double denominator) =>
        denominator > 0 ? numerator / denominator : null;
}
