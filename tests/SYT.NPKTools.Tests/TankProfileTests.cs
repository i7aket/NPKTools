using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers composing a mix with its source water, and the bicarbonate term in the conductivity estimate.
/// </summary>
/// <remarks>
/// Both of these close the same gap from opposite ends. Every analysis in the library describes the profile
/// it is handed, so handing it a mix's own ppm silently answers a question nobody asked — what the salts
/// would read in pure water. For a hard supply that is 22% below what the meter in the reservoir shows, and
/// two-thirds of the shortfall is the water's nutrients while the rest is its bicarbonate.
/// </remarks>
public class TankProfileTests
{
    private static WaterProfile TapWater() => new WaterProfileBuilder()
        .AddCa(45).AddMg(12).AddS(18).AddNa(20).AddCl(25)
        .Build();

    private static Ppm SaltsOnly() => new PpmBuilder()
        .AddNitrate(143).AddAmmonium(7).AddP(50).AddK(210).AddCa(115).AddMg(38).AddS(47)
        .AddLiters(100)
        .Build();

    // ---------------------------------------------------------------- composition

    /// <summary>
    /// The nitrogen forms have to be added form by form. Collapsing them to a total and re-splitting it is
    /// the mistake this method exists to prevent — it drops the ammonium, which moves the charge balance, the
    /// NO₃:NH₄ ratio and the conductivity all at once.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Plus_KeepsAllThreeNitrogenFormsSeparate()
    {
        // Arrange
        WaterProfile water = new WaterProfileBuilder().AddNitrate(6).AddAmmonium(2).AddAmine(1).Build();

        // Act
        Ppm tank = SaltsOnly().Plus(water);

        // Assert
        tank.Nitrogen.Nitrate.Should().Be(149);
        tank.Nitrogen.Ammonium.Should().Be(9);
        tank.Nitrogen.Amine.Should().Be(1);
        tank.Nitrogen.Value.Should().Be(159);
    }

    /// <summary>
    /// Every element must be carried, or an omitted one reads as absent from the reservoir when it is present.
    /// Sodium and chloride matter most here: they come almost entirely from the water and appear in no target.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Plus_AddsEveryElementFromBothSides()
    {
        // Act
        Ppm tank = SaltsOnly().Plus(TapWater());

        // Assert
        tank.Calcium.Value.Should().Be(115 + 45);
        tank.Magnesium.Value.Should().Be(38 + 12);
        tank.Sulfur.Value.Should().Be(47 + 18);
        tank.Sodium.Value.Should().Be(20);
        tank.Chlorine.Value.Should().Be(25);
        tank.Potassium.Value.Should().Be(210);
    }

    /// <summary>
    /// The reservoir volume belongs to the mix; the water analysis is a concentration and carries none. If the
    /// volume moved, the recipe would no longer describe the same batch.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Plus_KeepsTheMixesOwnWaterVolume()
    {
        // Act
        Ppm tank = SaltsOnly().Plus(TapWater());

        // Assert
        tank.Liters.Value.Should().Be(100);
    }

    /// <summary>
    /// Reverse osmosis water changes nothing, which is the regression guard for anyone who does not have a
    /// water analysis to supply.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Plus_PureWater_ChangesNothing()
    {
        // Arrange
        Ppm salts = SaltsOnly();

        // Act
        Ppm tank = salts.Plus(WaterProfile.Pure);

        // Assert
        tank.Calcium.Value.Should().Be(salts.Calcium.Value);
        tank.EstimateConductivity().MicroSiemensPerCm
            .Should().BeApproximately(salts.EstimateConductivity().MicroSiemensPerCm, 1e-9);
    }

    /// <summary>
    /// The composed profile lands on the original target, which is the whole point of deducting the water
    /// beforehand — and the check that the deduction and the composition agree with each other.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Plus_RecomposesTheTargetTheWaterWasDeductedFrom()
    {
        // Arrange
        PpmTarget original = new PpmTargetBuilder()
            .AddN(150).AddP(50).AddK(210).AddCa(160).AddMg(50).AddS(65).AddLiters(100)
            .Build();
        WaterProfile water = TapWater();
        PpmTarget adjusted = original.AdjustFor(water).Target;

        // Act — a mix meeting the adjusted target, put back into the water it was adjusted for
        Ppm salts = new PpmBuilder()
            .AddNitrate(adjusted.N.Value).AddP(adjusted.P.Value).AddK(adjusted.K.Value)
            .AddCa(adjusted.Ca.Value).AddMg(adjusted.Mg.Value).AddS(adjusted.S.Value)
            .AddLiters(100)
            .Build();
        Ppm tank = salts.Plus(water);

        // Assert
        tank.Calcium.Value.Should().BeApproximately(original.Ca.Value, 1e-9);
        tank.Magnesium.Value.Should().BeApproximately(original.Mg.Value, 1e-9);
        tank.Sulfur.Value.Should().BeApproximately(original.S.Value, 1e-9);
    }

    // ---------------------------------------------------------------- EC of the reservoir

    /// <summary>
    /// The number that makes the feature worth having: on a hard supply the reservoir reads well above what
    /// the salts alone would, so a caller measuring against a salts-only figure would think their feed was
    /// weak and push it further.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_ReservoirVersusSaltsAlone_DiffersByAFifth()
    {
        // Arrange
        WaterProfile water = TapWater();
        Ppm salts = SaltsOnly();

        // Act
        double saltsOnly = salts.EstimateConductivity().MilliSiemensPerCm;
        double reservoir = salts.Plus(water).EstimateConductivity(water.EstimatedAlkalinity())
            .MilliSiemensPerCm;

        // Assert
        (reservoir / saltsOnly).Should().BeInRange(1.15, 1.30);
    }

    // ---------------------------------------------------------------- bicarbonate

    /// <summary>
    /// Bicarbonate carries most of the negative charge in tap water. Leaving it out understates a moderately
    /// hard supply by around a quarter, which is the figure the documentation quotes.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_WaterWithAndWithoutBicarbonate_DiffersByAboutAQuarter()
    {
        // Arrange
        WaterProfile water = TapWater();

        // Act
        double without = water.AsPpm().EstimateConductivity().MicroSiemensPerCm;
        double with = water.EstimateConductivity().MicroSiemensPerCm;

        // Assert
        without.Should().BeApproximately(358, 5);
        with.Should().BeApproximately(454, 5);
        (with / without - 1).Should().BeInRange(0.20, 0.32);
    }

    /// <summary>
    /// The overload on <see cref="WaterProfile"/> is the same call with the alkalinity filled in, and it must
    /// stay that way — inferring bicarbonate is defensible for a water analysis and wrong for a recipe, where
    /// the same charge gap is the salts' acid-base character.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_OnWaterProfile_FillsInItsOwnAlkalinity()
    {
        // Arrange
        WaterProfile water = TapWater();

        // Act
        ConductivityEstimate implicitly = water.EstimateConductivity();
        ConductivityEstimate explicitly = water.AsPpm()
            .EstimateConductivity(water.EstimatedAlkalinity());

        // Assert
        implicitly.MicroSiemensPerCm.Should().BeApproximately(explicitly.MicroSiemensPerCm, 1e-9);
        implicitly.Bicarbonate.Should().BeGreaterThan(0);
    }

    /// <summary>
    /// Bicarbonate is monovalent, so its share is its meq/L times the ion's molar conductivity, and it must
    /// raise ionic strength as well — the correction has to know about the ion that was added.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_Bicarbonate_ContributesAndRaisesIonicStrength()
    {
        // Arrange
        Ppm profile = new PpmBuilder().AddK(39.098).AddLiters(100).Build();

        // Act
        ConductivityEstimate without = profile.EstimateConductivity();
        ConductivityEstimate with = profile.EstimateConductivity(bicarbonateMeqPerLitre: 1);

        // Assert
        with.Bicarbonate.Should().BeApproximately(44.5, 0.01);
        with.IonicStrength.Should().BeApproximately(without.IonicStrength + 0.0005, 1e-9);
        with.IdealMicroSiemensPerCm.Should()
            .BeApproximately(without.IdealMicroSiemensPerCm + 44.5, 0.01);
    }

    /// <summary>
    /// Reverse osmosis water has no alkalinity, so no bicarbonate term appears.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_PureWater_HasNoBicarbonate()
    {
        // Act
        ConductivityEstimate result = WaterProfile.Pure.EstimateConductivity();

        // Assert
        result.Bicarbonate.Should().Be(0);
        result.MicroSiemensPerCm.Should().Be(0);
    }

    // ---------------------------------------------------------------- guards

    /// <summary>
    /// Negative bicarbonate is not a quantity, and left unchecked it would quietly reduce a conductivity
    /// estimate.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_NegativeBicarbonate_Throws()
    {
        // Act
        Action act = () => SaltsOnly().EstimateConductivity(-1);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// Guards both sides of the composition.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Plus_NullArguments_Throw()
    {
        // Act & Assert
        ((Action)(() => PpmExtensions.Plus(null!, TapWater()))).Should().Throw<ArgumentNullException>();
        ((Action)(() => SaltsOnly().Plus(null!))).Should().Throw<ArgumentNullException>();
        ((Action)(() => WaterProfileExtensions.EstimateConductivity(null!)))
            .Should().Throw<ArgumentNullException>();
    }
}
