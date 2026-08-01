namespace SYT.NPKTools.Nutrients;

/// <summary>
/// The charge a solution carries, in milliequivalents per litre (meq/L), split into cations and anions.
/// </summary>
/// <remarks>
/// <para>
/// Milliequivalents are millimoles times charge, and they are how a solution's ionic strength and balance
/// are read: 1 mM of calcium is 2 meq because Ca²⁺ carries two charges, while 1 mM of potassium is 1 meq.
/// Formulation tables in the literature quote both.
/// </para>
/// <para>
/// <b>What the difference between the two sides means.</b> Not an error. Every salt is electrically
/// neutral, so a recipe of nothing but salts comes out exactly balanced — and where it does not, the gap
/// is the acid or base the recipe itself contributes. Phosphoric acid supplies H₂PO₄⁻ with a proton
/// rather than a metal, so it shows an anion surplus of one per phosphorus: that is the H⁺ it releases.
/// Dipotassium phosphate is the mirror image, supplying two K⁺ against an HPO₄²⁻ that takes up a proton
/// on its way to H₂PO₄⁻ at working pH, so it shows a cation surplus of one per phosphorus: that is the H⁺
/// it consumes. <see cref="AcidEquivalents"/> is therefore a real measurement — how much acidity the
/// recipe adds to or removes from the water, before any pH adjustment.
/// </para>
/// <para>
/// Measured across the built-in catalogue, fourteen of the seventeen macro salts balance to zero. The
/// three that do not are exactly the three with acid-base character: phosphoric acid and urea phosphate
/// on the acid side, dipotassium phosphate on the basic side.
/// </para>
/// <para>
/// <b>What is deliberately left out.</b> Micronutrients are not counted. Their speciation is genuinely
/// ambiguous — iron may be Fe²⁺ or Fe³⁺, and in chelated form the complex is an anion rather than a
/// cation — and at under 5 ppm in total they would move the balance by well under 0.1 meq/L against a
/// typical 25–30. Guessing a charge for them would add uncertainty to a figure whose worth is that it is
/// exact. Boron and silicon are excluded for a firmer reason: as boric and silicic acid they are
/// undissociated at nutrient-solution pH and carry no charge at all. Urea is likewise absent — it is a
/// neutral molecule, which is also why it contributes nothing to a plant's cation-anion uptake balance
/// even though it contributes nitrogen.
/// </para>
/// <para>
/// Phosphorus is counted as H₂PO₄⁻ at one charge, the dominant species between pH 5.5 and 6.5 where these
/// solutions are run. Above pH 7.2 half of it is HPO₄²⁻ and the phosphate figure would be nearer 1.5
/// charges. This library does not model pH, so the assumption is stated rather than computed — and it is
/// the assumption that makes the acid-base reading above come out in whole protons.
/// </para>
/// </remarks>
public sealed record IonBalance
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IonBalance"/> record.
    /// </summary>
    internal IonBalance(
        double potassium,
        double calcium,
        double magnesium,
        double ammonium,
        double sodium,
        double nitrate,
        double phosphate,
        double sulfate,
        double chloride)
    {
        Potassium = potassium;
        Calcium = calcium;
        Magnesium = magnesium;
        Ammonium = ammonium;
        Sodium = sodium;
        Nitrate = nitrate;
        Phosphate = phosphate;
        Sulfate = sulfate;
        Chloride = chloride;
    }

    /// <summary>Gets K⁺ in meq/L.</summary>
    public double Potassium { get; }

    /// <summary>Gets Ca²⁺ in meq/L.</summary>
    public double Calcium { get; }

    /// <summary>Gets Mg²⁺ in meq/L.</summary>
    public double Magnesium { get; }

    /// <summary>Gets NH₄⁺ in meq/L.</summary>
    public double Ammonium { get; }

    /// <summary>Gets Na⁺ in meq/L.</summary>
    public double Sodium { get; }

    /// <summary>Gets NO₃⁻ in meq/L.</summary>
    public double Nitrate { get; }

    /// <summary>Gets H₂PO₄⁻ in meq/L.</summary>
    public double Phosphate { get; }

    /// <summary>Gets SO₄²⁻ in meq/L.</summary>
    public double Sulfate { get; }

    /// <summary>Gets Cl⁻ in meq/L.</summary>
    public double Chloride { get; }

    /// <summary>
    /// Gets the total positive charge in meq/L.
    /// </summary>
    public double Cations => Potassium + Calcium + Magnesium + Ammonium + Sodium;

    /// <summary>
    /// Gets the total negative charge in meq/L.
    /// </summary>
    public double Anions => Nitrate + Phosphate + Sulfate + Chloride;

    /// <summary>
    /// Gets the total charge either way, in meq/L — a reasonable proxy for ionic strength, and the
    /// quantity electrical conductivity tracks.
    /// </summary>
    public double Total => Cations + Anions;

    /// <summary>
    /// Gets the acidity the recipe itself contributes, in meq/L of H⁺.
    /// </summary>
    /// <remarks>
    /// Positive means the recipe releases protons and pulls pH down, so less pH-down is needed than the
    /// water's alkalinity alone would suggest. Negative means it consumes them and pushes pH up. Zero —
    /// the usual case — means the recipe is made of neutral salts and does not move pH by itself.
    /// </remarks>
    public double AcidEquivalents => Anions - Cations;

    /// <summary>
    /// Gets <see cref="AcidEquivalents"/> as a fraction of the larger side, or zero for an empty solution.
    /// </summary>
    /// <remarks>
    /// Relative rather than absolute, so the same figure means the same thing for a seedling feed and a
    /// full-strength tomato recipe.
    /// </remarks>
    public double RelativeDifference
    {
        get
        {
            double larger = Math.Max(Cations, Anions);
            return larger > 0 ? AcidEquivalents / larger : 0;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the charges balance to within 2%.
    /// </summary>
    /// <remarks>
    /// True for a recipe of neutral salts, which balances exactly; the tolerance is for rounding, not for
    /// chemistry. False means the recipe includes an acid or a basic salt, which is a fact about it rather
    /// than a fault — read <see cref="AcidEquivalents"/> for the size and direction.
    /// </remarks>
    public bool IsChargeNeutral => Math.Abs(RelativeDifference) <= 0.02;
}
