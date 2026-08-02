using SYT.NPKTools.Internal;

namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Infers a water analysis from the readings a grower can take themselves.
/// </summary>
/// <remarks>
/// <para>
/// A conductivity meter measures how much is dissolved, never what. A <see cref="WaterPreset"/>
/// supplies the proportions, and this scales them until the computed conductivity matches the meter.
/// Because the scaling runs through this library's own ion-by-ion conductivity model, the result is
/// consistent with the reading rather than merely plausible.
/// </para>
/// <para>
/// A drop test, when there is one, is treated as a measurement and pinned: general hardness fixes
/// calcium and magnesium, carbonate hardness fixes the bicarbonate, and only what is left over is
/// scaled. Pinning the bicarbonate means the charge balance no longer closes by itself, so the
/// shortfall is taken up by sodium or chloride — the two ions this library knows least about, and the
/// two a laboratory closes a real analysis on for the same reason.
/// </para>
/// <para>
/// Three readings can describe no water at all. Drop tests implying more conductivity than the meter
/// showed leave nothing to scale, and the estimate reports
/// <see cref="WaterEstimate.Feasible"/> as false rather than silently preferring one reading.
/// </para>
/// </remarks>
public static class WaterEstimator
{
    /// <summary>The widest scale the solver will consider — far beyond any drinkable water.</summary>
    private const double MaximumScale = 100;

    /// <summary>Enough halvings to pin the scale to the last bit of a double.</summary>
    private const int Iterations = 60;

    /// <summary>
    /// Infers an analysis from a conductivity reading and, when available, hardness drop tests.
    /// </summary>
    /// <param name="preset">The shape of the water.</param>
    /// <param name="microSiemensPerCm">The meter reading, in µS/cm.</param>
    /// <param name="generalHardness">Calcium and magnesium, in °dH, or null when not measured.</param>
    /// <param name="carbonateHardness">Alkalinity, in °dKH, or null when not measured.</param>
    /// <returns>The estimate, with the evidence for judging it.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="preset"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any reading is negative.</exception>
    public static WaterEstimate Estimate(
        WaterPreset preset,
        double microSiemensPerCm,
        double? generalHardness = null,
        double? carbonateHardness = null)
    {
        ArgumentNullException.ThrowIfNull(preset);
        ArgumentOutOfRangeException.ThrowIfNegative(microSiemensPerCm);

        if (generalHardness is { } gh)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(gh, nameof(generalHardness));
        }

        if (carbonateHardness is { } kh)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(kh, nameof(carbonateHardness));
        }

        WaterProfile atFloor = Build(preset, 0, generalHardness, carbonateHardness);
        double floor = Conductivity(atFloor);
        if (floor > microSiemensPerCm)
        {
            return new WaterEstimate(atFloor, floor, microSiemensPerCm, feasible: false);
        }

        WaterProfile atCeiling = Build(preset, MaximumScale, generalHardness, carbonateHardness);
        double ceiling = Conductivity(atCeiling);
        if (ceiling < microSiemensPerCm)
        {
            return new WaterEstimate(atCeiling, ceiling, microSiemensPerCm, feasible: false);
        }

        double low = 0;
        double high = MaximumScale;
        for (int i = 0; i < Iterations; i++)
        {
            double middle = (low + high) / 2;
            if (Conductivity(Build(preset, middle, generalHardness, carbonateHardness)) < microSiemensPerCm)
            {
                low = middle;
            }
            else
            {
                high = middle;
            }
        }

        WaterProfile solved = Build(preset, (low + high) / 2, generalHardness, carbonateHardness);
        return new WaterEstimate(solved, Conductivity(solved), microSiemensPerCm, feasible: true);
    }

    private static double Conductivity(WaterProfile water) =>
        water.EstimateConductivity().MicroSiemensPerCm;

    /// <summary>
    /// Builds the candidate water at a scale, pinning whatever was measured.
    /// </summary>
    /// <remarks>
    /// Monotone in <paramref name="scale"/>, which is what makes bisection valid: every unpinned ion
    /// rises with it, and so does whichever ion closes the charge balance.
    /// </remarks>
    private static WaterProfile Build(
        WaterPreset preset,
        double scale,
        double? generalHardness,
        double? carbonateHardness)
    {
        double calcium = preset.Calcium * scale;
        double magnesium = preset.Magnesium * scale;

        if (generalHardness is { } gh)
        {
            // Split the measured hardness the way the preset splits its own, per equivalent.
            double presetCalciumMeq = preset.Calcium / AtomicMasses.Ca * Charges.Calcium;
            double presetMagnesiumMeq = preset.Magnesium / AtomicMasses.Mg * Charges.Magnesium;
            double presetTotalMeq = presetCalciumMeq + presetMagnesiumMeq;

            double measuredMeq = gh * WaterProfileExtensions.MilliequivalentsPerGermanDegree;
            double calciumShare = presetTotalMeq > 0 ? presetCalciumMeq / presetTotalMeq : 1;

            calcium = measuredMeq * calciumShare / Charges.Calcium * AtomicMasses.Ca;
            magnesium = measuredMeq * (1 - calciumShare) / Charges.Magnesium * AtomicMasses.Mg;
        }

        double sodium = preset.Sodium * scale;
        double chlorine = preset.Chlorine * scale;
        double sulfur = preset.Sulfur * scale;
        double nitrogen = preset.Nitrogen * scale;

        if (carbonateHardness is { } kh)
        {
            WaterProfile unclosed = Compose(calcium, magnesium, sodium, sulfur, chlorine, nitrogen);
            double surplus = -unclosed.AsPpm().IonBalance().AcidEquivalents;
            double wanted = kh * WaterProfileExtensions.MilliequivalentsPerGermanDegree;
            double delta = wanted - surplus;

            if (delta > 0)
            {
                sodium += delta * AtomicMasses.Na / Charges.Sodium;
            }
            else
            {
                chlorine += -delta * AtomicMasses.Cl / Charges.Chloride;
            }
        }

        return Compose(calcium, magnesium, sodium, sulfur, chlorine, nitrogen);
    }

    private static WaterProfile Compose(
        double calcium,
        double magnesium,
        double sodium,
        double sulfur,
        double chlorine,
        double nitrogen) =>
        new WaterProfileBuilder()
            .AddCa(calcium)
            .AddMg(magnesium)
            .AddNa(sodium)
            .AddS(sulfur)
            .AddCl(chlorine)
            .AddNitrate(nitrogen)
            .Build();
}
