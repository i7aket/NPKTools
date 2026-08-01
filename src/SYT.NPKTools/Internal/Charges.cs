namespace SYT.NPKTools.Internal;

/// <summary>
/// Charge per ion, for converting millimoles to milliequivalents.
/// </summary>
/// <remarks>
/// These are the species actually present in a nutrient solution run between pH 5.5 and 6.5, not the
/// element's possible oxidation states. Phosphorus is dihydrogen phosphate at one charge rather than the
/// two its formula might suggest, because H₂PO₄⁻ dominates in that range; nitrogen appears twice with
/// opposite signs because nitrate and ammonium are different ions; and urea has no entry because it is a
/// neutral molecule.
/// </remarks>
internal static class Charges
{
    // Cations.
    public const double Potassium = 1;
    public const double Calcium = 2;
    public const double Magnesium = 2;
    public const double Ammonium = 1;
    public const double Sodium = 1;

    // Anions. The sign is carried by which total the value is added to, so these are magnitudes.
    public const double Nitrate = 1;
    public const double DihydrogenPhosphate = 1;
    public const double Sulfate = 2;
    public const double Chloride = 1;
}
