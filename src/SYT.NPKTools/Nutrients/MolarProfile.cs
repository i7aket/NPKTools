namespace SYT.NPKTools.Nutrients;

/// <summary>
/// A nutrient profile in millimoles per litre (mM).
/// </summary>
/// <remarks>
/// <para>
/// Ppm is a mass unit and plants take up ions, so ppm makes elements of different atomic weight look
/// comparable when they are not: 40 ppm of calcium and 40 ppm of magnesium are 1.0 mM and 1.65 mM — the
/// magnesium is two-thirds again as many ions. Every published formulation in the horticultural
/// literature — Steiner, Hoagland, the Dutch advisory tables — is stated in mM for that reason, so
/// working from a paper recipe means converting one way or the other.
/// </para>
/// <para>
/// The conversion is division by atomic weight and nothing else, which is why this covers all sixteen
/// elements while <see cref="IonBalance"/> deliberately does not.
/// </para>
/// </remarks>
public sealed record MolarProfile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MolarProfile"/> record.
    /// </summary>
    /// <remarks>
    /// Prefer <see cref="PpmExtensions.AsMillimolar"/> over constructing one directly; the
    /// constructor exists so that a profile read from a paper recipe can be expressed in its own units.
    /// </remarks>
    internal MolarProfile(
        double nitrate,
        double ammonium,
        double amine,
        double phosphorus,
        double potassium,
        double calcium,
        double magnesium,
        double sulfur,
        double iron,
        double copper,
        double manganese,
        double zinc,
        double boron,
        double molybdenum,
        double chlorine,
        double silicon,
        double selenium,
        double sodium)
    {
        Nitrate = nitrate;
        Ammonium = ammonium;
        Amine = amine;
        Phosphorus = phosphorus;
        Potassium = potassium;
        Calcium = calcium;
        Magnesium = magnesium;
        Sulfur = sulfur;
        Iron = iron;
        Copper = copper;
        Manganese = manganese;
        Zinc = zinc;
        Boron = boron;
        Molybdenum = molybdenum;
        Chlorine = chlorine;
        Silicon = silicon;
        Selenium = selenium;
        Sodium = sodium;
    }

    /// <summary>Gets nitrate nitrogen in mM.</summary>
    public double Nitrate { get; }

    /// <summary>Gets ammonium nitrogen in mM.</summary>
    public double Ammonium { get; }

    /// <summary>Gets amine nitrogen — urea — in mM.</summary>
    public double Amine { get; }

    /// <summary>Gets total nitrogen in mM, across all three forms.</summary>
    public double Nitrogen => Nitrate + Ammonium + Amine;

    /// <summary>Gets phosphorus in mM.</summary>
    public double Phosphorus { get; }

    /// <summary>Gets potassium in mM.</summary>
    public double Potassium { get; }

    /// <summary>Gets calcium in mM.</summary>
    public double Calcium { get; }

    /// <summary>Gets magnesium in mM.</summary>
    public double Magnesium { get; }

    /// <summary>Gets sulfur in mM.</summary>
    public double Sulfur { get; }

    /// <summary>Gets iron in mM.</summary>
    public double Iron { get; }

    /// <summary>Gets copper in mM.</summary>
    public double Copper { get; }

    /// <summary>Gets manganese in mM.</summary>
    public double Manganese { get; }

    /// <summary>Gets zinc in mM.</summary>
    public double Zinc { get; }

    /// <summary>Gets boron in mM.</summary>
    public double Boron { get; }

    /// <summary>Gets molybdenum in mM.</summary>
    public double Molybdenum { get; }

    /// <summary>Gets chlorine in mM.</summary>
    public double Chlorine { get; }

    /// <summary>Gets silicon in mM.</summary>
    public double Silicon { get; }

    /// <summary>Gets selenium in mM.</summary>
    public double Selenium { get; }

    /// <summary>Gets sodium in mM.</summary>
    public double Sodium { get; }
}
