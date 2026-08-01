using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers reading a source-water analysis with the solution analyses.
/// </summary>
/// <remarks>
/// The alkalinity estimate is the interesting one and the reasoning is worth restating, because it looks
/// like a bug at first glance. A finished recipe's cations equal its anions; a water analysis's do not, and
/// that is not a mistake in the analysis. Bicarbonate is what balances real water and it is not a plant
/// nutrient, so it has nowhere to be entered — leaving the gap as a measurement of it.
/// </remarks>
public class WaterProfileAnalysisTests
{
    private static WaterProfile ModeratelyHardTapWater() => new WaterProfileBuilder()
        .AddCa(45).AddMg(12).AddS(18).AddNa(20).AddCl(25)
        .Build();

    /// <summary>
    /// The worked example the documentation quotes, pinned so the two cannot drift apart. 2.27 meq/L is about
    /// 139 ppm of bicarbonate or 114 as calcium carbonate — an ordinary municipal supply.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimatedAlkalinity_ModeratelyHardWater_IsTheCationSurplus()
    {
        // Arrange
        WaterProfile tap = ModeratelyHardTapWater();

        // Act
        double alkalinity = tap.EstimatedAlkalinity();
        IonBalance balance = tap.AsPpm().IonBalance();

        // Assert
        alkalinity.Should().BeApproximately(2.27, 0.02);
        alkalinity.Should().BeApproximately(-balance.AcidEquivalents, 1e-9);
        (alkalinity * 61).Should().BeApproximately(139, 2, "ppm of HCO3-");
        (alkalinity * 50).Should().BeApproximately(114, 2, "ppm as CaCO3");
    }

    /// <summary>
    /// Reverse osmosis water has nothing in it, so it must report no alkalinity and no conductivity rather
    /// than a small artefact of the arithmetic.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimatedAlkalinity_PureWater_IsZero()
    {
        // Act & Assert
        WaterProfile.Pure.EstimatedAlkalinity().Should().Be(0);
        WaterProfile.Pure.AsPpm().EstimateConductivity().MicroSiemensPerCm.Should().Be(0);
    }

    /// <summary>
    /// An analysis with more anions than cations cannot have negative alkalinity — there is no such thing —
    /// so the estimate clamps at zero rather than reporting a number that reads as acidity.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimatedAlkalinity_AnionHeavyAnalysis_IsZeroRatherThanNegative()
    {
        // Arrange — a sulfate-dominated or acidified supply
        WaterProfile water = new WaterProfileBuilder().AddCa(10).AddS(60).AddCl(40).Build();

        // Act & Assert
        water.EstimatedAlkalinity().Should().Be(0);
    }

    /// <summary>
    /// A water analysis is a concentration, so the nominal volume must not change anything read from it. If
    /// it did, the parameter would be a trap.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsPpm_NominalVolume_DoesNotAffectAnyReading()
    {
        // Arrange
        WaterProfile tap = ModeratelyHardTapWater();

        // Act
        ConductivityEstimate atOneLiter = tap.AsPpm(1).EstimateConductivity();
        ConductivityEstimate atAThousand = tap.AsPpm(1000).EstimateConductivity();

        // Assert
        atAThousand.MicroSiemensPerCm.Should().BeApproximately(atOneLiter.MicroSiemensPerCm, 1e-9);
    }

    /// <summary>
    /// The conversion has to carry every element across, including the nitrogen forms separately — an
    /// analysis reporting nitrate must not have it silently become ammonium, which would move both the
    /// charge balance and the ratios.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsPpm_CarriesEveryElementAcross()
    {
        // Arrange
        WaterProfile water = new WaterProfileBuilder()
            .AddNitrate(6).AddAmmonium(2).AddP(1).AddK(3).AddCa(45).AddMg(12).AddS(18)
            .AddFe(0.1).AddCu(0.02).AddMn(0.05).AddZn(0.03).AddB(0.04).AddMo(0.01)
            .AddCl(25).AddSi(8).AddSe(0.005).AddNa(20)
            .Build();

        // Act
        Ppm ppm = water.AsPpm();

        // Assert
        ppm.Nitrogen.Nitrate.Should().Be(6);
        ppm.Nitrogen.Ammonium.Should().Be(2);
        ppm.Phosphorus.Value.Should().Be(1);
        ppm.Potassium.Value.Should().Be(3);
        ppm.Calcium.Value.Should().Be(45);
        ppm.Magnesium.Value.Should().Be(12);
        ppm.Sulfur.Value.Should().Be(18);
        ppm.Iron.Value.Should().Be(0.1);
        ppm.Copper.Value.Should().Be(0.02);
        ppm.Manganese.Value.Should().Be(0.05);
        ppm.Zinc.Value.Should().Be(0.03);
        ppm.Boron.Value.Should().Be(0.04);
        ppm.Molybdenum.Value.Should().Be(0.01);
        ppm.Chlorine.Value.Should().Be(25);
        ppm.Silicon.Value.Should().Be(8);
        ppm.Selenium.Value.Should().Be(0.005);
        ppm.Sodium.Value.Should().Be(20);
    }

    /// <summary>
    /// The water's own EC is the quickest check that an analysis was entered correctly, so it has to land in
    /// the range a meter would show for water of that hardness — a few hundred µS/cm.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_ModeratelyHardWater_ReadsInTheRangeAMeterWould()
    {
        // Act
        double microSiemens = ModeratelyHardTapWater().AsPpm().EstimateConductivity().MicroSiemensPerCm;

        // Assert
        microSiemens.Should().BeInRange(250, 500);
    }

    /// <summary>
    /// Guards both entry points.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Extensions_InvalidArguments_Throw()
    {
        // Act & Assert
        ((Action)(() => WaterProfileExtensions.AsPpm(null!))).Should().Throw<ArgumentNullException>();
        ((Action)(() => WaterProfileExtensions.EstimatedAlkalinity(null!)))
            .Should().Throw<ArgumentNullException>();
        ((Action)(() => WaterProfile.Pure.AsPpm(0))).Should().Throw<ArgumentOutOfRangeException>();
        ((Action)(() => WaterProfile.Pure.AsPpm(-1))).Should().Throw<ArgumentOutOfRangeException>();
    }
}
