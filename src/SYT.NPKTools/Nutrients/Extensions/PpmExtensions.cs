using SYT.NPKTools.Fertilizers;
using SYT.NPKTools.Internal;

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
    /// Converts the profile to millimoles per litre, the unit the horticultural literature uses.
    /// </summary>
    /// <param name="ppm">The measured concentrations.</param>
    /// <returns>The same profile in mM.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="ppm"/> is null.</exception>
    /// <remarks>
    /// Ppm is a mass unit, so it flatters the light elements: 40 ppm of calcium and 40 ppm of magnesium
    /// are 1.0 mM and 1.65 mM. Every published formulation — Steiner, Hoagland, the Dutch advisory tables
    /// — is stated in mM for that reason.
    /// </remarks>
    public static MolarProfile AsMillimolar(this Ppm ppm)
    {
        ArgumentNullException.ThrowIfNull(ppm);

        return new MolarProfile(
            nitrate: ppm.Nitrogen.Nitrate / AtomicMasses.N,
            ammonium: ppm.Nitrogen.Ammonium / AtomicMasses.N,
            amine: ppm.Nitrogen.Amine / AtomicMasses.N,
            phosphorus: ppm.Phosphorus.Value / AtomicMasses.P,
            potassium: ppm.Potassium.Value / AtomicMasses.K,
            calcium: ppm.Calcium.Value / AtomicMasses.Ca,
            magnesium: ppm.Magnesium.Value / AtomicMasses.Mg,
            sulfur: ppm.Sulfur.Value / AtomicMasses.S,
            iron: ppm.Iron.Value / AtomicMasses.Fe,
            copper: ppm.Copper.Value / AtomicMasses.Cu,
            manganese: ppm.Manganese.Value / AtomicMasses.Mn,
            zinc: ppm.Zinc.Value / AtomicMasses.Zn,
            boron: ppm.Boron.Value / AtomicMasses.B,
            molybdenum: ppm.Molybdenum.Value / AtomicMasses.Mo,
            chlorine: ppm.Chlorine.Value / AtomicMasses.Cl,
            silicon: ppm.Silicon.Value / AtomicMasses.Si,
            selenium: ppm.Selenium.Value / AtomicMasses.Se,
            sodium: ppm.Sodium.Value / AtomicMasses.Na);
    }

    /// <summary>
    /// Expresses the profile as charge, in milliequivalents per litre, and reports whether it balances.
    /// </summary>
    /// <param name="ppm">The measured concentrations.</param>
    /// <returns>The cations, the anions and the difference between them.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="ppm"/> is null.</exception>
    /// <remarks>
    /// A recipe made only of salts balances exactly, because every salt is electrically neutral. Where it
    /// does not, the gap is the acid or base the recipe itself contributes — phosphoric acid releases a
    /// proton per phosphorus, dipotassium phosphate consumes one — so
    /// <see cref="Nutrients.IonBalance.AcidEquivalents"/> measures how much acidity the recipe adds to the
    /// water before any pH adjustment. See <see cref="IonBalance"/> for what is counted and what is
    /// deliberately left out.
    /// </remarks>
    public static IonBalance IonBalance(this Ppm ppm)
    {
        ArgumentNullException.ThrowIfNull(ppm);

        MolarProfile mM = ppm.AsMillimolar();

        return new IonBalance(
            potassium: mM.Potassium * Charges.Potassium,
            calcium: mM.Calcium * Charges.Calcium,
            magnesium: mM.Magnesium * Charges.Magnesium,
            ammonium: mM.Ammonium * Charges.Ammonium,
            sodium: mM.Sodium * Charges.Sodium,
            nitrate: mM.Nitrate * Charges.Nitrate,
            phosphate: mM.Phosphorus * Charges.DihydrogenPhosphate,
            sulfate: mM.Sulfur * Charges.Sulfate,
            chloride: mM.Chlorine * Charges.Chloride);
    }

    /// <summary>
    /// Estimates the electrical conductivity a meter would read for this solution.
    /// </summary>
    /// <param name="ppm">The measured concentrations.</param>
    /// <returns>The estimate, its ideal upper bound, and each ion's share.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="ppm"/> is null.</exception>
    /// <remarks>
    /// Computed from each ion's own molar conductivity rather than by scaling total dissolved solids, because
    /// a mole of sulfate conducts more than twice what a mole of dihydrogen phosphate does — two solutions of
    /// identical ppm can read differently, and a single factor cannot tell which it was given. See
    /// <see cref="ConductivityEstimate"/> for the accuracy and what is excluded.
    /// </remarks>
    public static ConductivityEstimate EstimateConductivity(this Ppm ppm)
    {
        ArgumentNullException.ThrowIfNull(ppm);

        MolarProfile mM = ppm.AsMillimolar();

        // Ionic strength is half the sum of cz², in moles per litre. The mM figures are divided by 1000 to
        // get there, and the divalent ions contribute four times their concentration.
        double ionicStrength = (
            mM.Potassium + mM.Ammonium + mM.Sodium + mM.Nitrate + mM.Phosphorus + mM.Chlorine
            + 4 * (mM.Calcium + mM.Magnesium + mM.Sulfur)) / 2000;

        double correction = Math.Max(
            0,
            1 - MolarConductivities.InteractionCoefficient * Math.Sqrt(ionicStrength));

        return new ConductivityEstimate(
            potassium: mM.Potassium * MolarConductivities.Potassium,
            calcium: mM.Calcium * MolarConductivities.Calcium,
            magnesium: mM.Magnesium * MolarConductivities.Magnesium,
            ammonium: mM.Ammonium * MolarConductivities.Ammonium,
            sodium: mM.Sodium * MolarConductivities.Sodium,
            nitrate: mM.Nitrate * MolarConductivities.Nitrate,
            phosphate: mM.Phosphorus * MolarConductivities.DihydrogenPhosphate,
            sulfate: mM.Sulfur * MolarConductivities.Sulfate,
            chloride: mM.Chlorine * MolarConductivities.Chloride,
            ionicStrength: ionicStrength,
            correction: correction);
    }

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
