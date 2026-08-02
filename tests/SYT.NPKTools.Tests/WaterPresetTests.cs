using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers the built-in shapes of ordinary source water.
/// </summary>
/// <remarks>
/// Each preset is asserted against the readings a grower can take themselves — conductivity,
/// general hardness, carbonate hardness — rather than against its own ppm figures. That is what
/// makes the table in the design document a claim about real water rather than a restatement of the
/// constants, and it is what would fail if the conductivity model were changed underneath.
/// </remarks>
public class WaterPresetTests
{
    /// <summary>
    /// The documented readings for every preset. Ranges are the textbook classification each preset
    /// is named for, so a preset drifting out of its own class fails here.
    /// </summary>
    /// <param name="id">The preset identifier.</param>
    /// <param name="microSiemens">The conductivity it should have, in µS/cm.</param>
    /// <param name="bicarbonatePpm">The bicarbonate it should imply, in ppm.</param>
    /// <param name="generalHardness">The general hardness it should have, in °dH.</param>
    /// <param name="carbonateHardness">The carbonate hardness it should have, in °dKH.</param>
    [Theory]
    [InlineData("SoftLowAlkalinity", 173, 61, 3.4, 2.8)]
    [InlineData("CalciumBicarbonateModerate", 471, 166, 10.2, 7.6)]
    [InlineData("CalciumBicarbonateHard", 892, 295, 19.8, 13.6)]
    [InlineData("SodiumExchangeSoftened", 559, 222, 0.8, 10.2)]
    [Trait("Category", "Unit")]
    public void ToProfile_MatchesTheDocumentedReadings(
        string id,
        double microSiemens,
        double bicarbonatePpm,
        double generalHardness,
        double carbonateHardness)
    {
        WaterPreset preset = WaterPreset.All.Single(p => p.Id == id);

        WaterProfile water = preset.ToProfile();

        water.EstimateConductivity().MicroSiemensPerCm.Should().BeApproximately(microSiemens, 1.0);
        (water.EstimatedAlkalinity() * 61.016).Should().BeApproximately(bicarbonatePpm, 1.0);
        water.GeneralHardness().Should().BeApproximately(generalHardness, 0.1);
        water.CarbonateHardness().Should().BeApproximately(carbonateHardness, 0.1);
    }

    /// <summary>
    /// Conductivity rises with scale, which is the property the estimator's bisection depends on.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ToProfile_Conductivity_RisesWithScale()
    {
        WaterPreset preset = WaterPreset.CalciumBicarbonateModerate;

        double half = preset.ToProfile(0.5).EstimateConductivity().MicroSiemensPerCm;
        double whole = preset.ToProfile(1.0).EstimateConductivity().MicroSiemensPerCm;
        double doubled = preset.ToProfile(2.0).EstimateConductivity().MicroSiemensPerCm;

        half.Should().BeLessThan(whole);
        whole.Should().BeLessThan(doubled);
    }

    /// <summary>
    /// A scale of zero is pure water, so an estimator driven to its floor produces something the rest
    /// of the library already handles rather than a special case.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ToProfile_AtZeroScale_IsPureWater()
    {
        WaterProfile water = WaterPreset.CalciumBicarbonateHard.ToProfile(0);

        water.EstimateConductivity().MicroSiemensPerCm.Should().Be(0);
        water.EstimatedAlkalinity().Should().Be(0);
    }

    /// <summary>
    /// Softened water is the preset that earns its place: high conductivity, no hardness. Estimated
    /// as one of the calcium presets it would promise calcium that is not there.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void SodiumExchangeSoftened_IsConductiveButNotHard()
    {
        WaterProfile softened = WaterPreset.SodiumExchangeSoftened.ToProfile();
        WaterProfile moderate = WaterPreset.CalciumBicarbonateModerate.ToProfile();

        softened.EstimateConductivity().MicroSiemensPerCm
            .Should().BeGreaterThan(moderate.EstimateConductivity().MicroSiemensPerCm);
        softened.GeneralHardness().Should().BeLessThan(1);
    }

    /// <summary>
    /// A negative scale is a caller error, not a water.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ToProfile_WithNegativeScale_Throws()
    {
        Action act = () => WaterPreset.SoftLowAlkalinity.ToProfile(-1);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
