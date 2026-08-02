using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers estimating a water analysis from the readings a grower can actually take.
/// </summary>
/// <remarks>
/// The estimator's contract is narrow and worth stating: it reproduces the conductivity it was given,
/// it honours a hardness reading exactly when one is supplied, and it says so rather than compromising
/// when the readings it was given cannot describe the same water.
/// </remarks>
public class WaterEstimatorTests
{
    /// <summary>
    /// The central claim: whatever the meter said, the estimate reads the same on the same model.
    /// </summary>
    /// <param name="microSiemens">The meter reading to reproduce.</param>
    [Theory]
    [InlineData(150)]
    [InlineData(300)]
    [InlineData(471)]
    [InlineData(900)]
    [Trait("Category", "Unit")]
    public void Estimate_FromConductivityAlone_ReproducesTheReading(double microSiemens)
    {
        WaterEstimate estimate = WaterEstimator.Estimate(
            WaterPreset.CalciumBicarbonateModerate,
            microSiemens);

        estimate.Feasible.Should().BeTrue();
        estimate.MicroSiemensPerCm.Should().BeApproximately(microSiemens, microSiemens * 0.005);
    }

    /// <summary>
    /// At its own nominal conductivity a preset comes back unscaled, so the estimator adds nothing
    /// of its own when it has nothing to solve for.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Estimate_AtThePresetsOwnConductivity_ReturnsThePresetUnchanged()
    {
        WaterPreset preset = WaterPreset.CalciumBicarbonateModerate;
        double nominal = preset.ToProfile().EstimateConductivity().MicroSiemensPerCm;

        WaterEstimate estimate = WaterEstimator.Estimate(preset, nominal);

        estimate.Profile.Calcium.Value.Should().BeApproximately(preset.Calcium, 0.5);
        estimate.Profile.Sodium.Value.Should().BeApproximately(preset.Sodium, 0.5);
    }

    /// <summary>
    /// A hardness reading is a measurement, not a hint: it comes back out of the estimate unchanged.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Estimate_WithHardnessReadings_HonoursThemExactly()
    {
        WaterEstimate estimate = WaterEstimator.Estimate(
            WaterPreset.CalciumBicarbonateModerate,
            microSiemensPerCm: 450,
            generalHardness: 9,
            carbonateHardness: 7);

        estimate.Feasible.Should().BeTrue();
        estimate.Profile.GeneralHardness().Should().BeApproximately(9, 0.05);
        estimate.Profile.CarbonateHardness().Should().BeApproximately(7, 0.05);
        estimate.MicroSiemensPerCm.Should().BeApproximately(450, 5);
    }

    /// <summary>
    /// Carbonate hardness above general hardness means a cation surplus that calcium cannot supply,
    /// which is what softened water is. The sodium shape resolves it; the estimate stays feasible and
    /// still reads back both drop tests.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Estimate_ForSoftenedWater_ResolvesTheSurplusWithSodium()
    {
        WaterEstimate estimate = WaterEstimator.Estimate(
            WaterPreset.SodiumExchangeSoftened,
            microSiemensPerCm: 560,
            generalHardness: 0.8,
            carbonateHardness: 10.2);

        estimate.Feasible.Should().BeTrue();
        estimate.Profile.CarbonateHardness().Should().BeApproximately(10.2, 0.05);
        estimate.Profile.Sodium.Value.Should().BeGreaterThan(50);
        estimate.Profile.Calcium.Value.Should().BeLessThan(10);
    }

    /// <summary>
    /// Drop tests that already imply more conductivity than the meter read are not reconcilable. The
    /// estimator reports that rather than quietly preferring one reading over the other.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Estimate_WhenHardnessExceedsTheReading_IsNotFeasible()
    {
        WaterEstimate estimate = WaterEstimator.Estimate(
            WaterPreset.CalciumBicarbonateHard,
            microSiemensPerCm: 100,
            generalHardness: 20,
            carbonateHardness: 14);

        estimate.Feasible.Should().BeFalse();
        estimate.MicroSiemensPerCm.Should().BeGreaterThan(100);
        estimate.RelativeError.Should().BeGreaterThan(0.2);
    }

    /// <summary>
    /// A meter reading of zero is reverse osmosis, and nothing is dissolved in it.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Estimate_AtZeroConductivity_IsPureWater()
    {
        WaterEstimate estimate = WaterEstimator.Estimate(WaterPreset.CalciumBicarbonateHard, 0);

        estimate.Feasible.Should().BeTrue();
        estimate.Profile.Calcium.Value.Should().BeApproximately(0, 1e-6);
        estimate.MicroSiemensPerCm.Should().BeApproximately(0, 1e-6);
    }

    /// <summary>
    /// Guard clauses: a null preset and a negative reading are caller errors.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Estimate_WithInvalidArguments_Throws()
    {
        Action nullPreset = () => WaterEstimator.Estimate(null!, 400);
        Action negative = () => WaterEstimator.Estimate(WaterPreset.SoftLowAlkalinity, -1);

        nullPreset.Should().Throw<ArgumentNullException>();
        negative.Should().Throw<ArgumentOutOfRangeException>();
    }
}
