using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers estimating electrical conductivity from a solution's ions.
/// </summary>
/// <remarks>
/// EC is the one figure in this library that can be checked against an external authority, so the central
/// tests do exactly that: the certified KCl conductivity standards at 25 °C — 147, 1412 and 12,880 µS/cm
/// for 0.001, 0.01 and 0.1 M — are the same reference solutions conductivity meters are calibrated
/// against. Pinning the model's error against them is worth more than any internally consistent assertion,
/// and it is what stops a future change to the coefficients from passing unnoticed.
/// </remarks>
public class ConductivityTests
{
    /// <summary>
    /// Builds a potassium chloride solution at a given molarity, in ppm of each element.
    /// </summary>
    private static Ppm PotassiumChloride(double molarity) => new PpmBuilder()
        .AddK(molarity * 39098)      // mol/L × g/mol × 1000 mg/g
        .AddCl(molarity * 35450)
        .AddLiters(100)
        .Build();

    // ---------------------------------------------------------------- against the standards

    /// <summary>
    /// The two standards that bracket a real nutrient solution's ionic strength. Within half a percent is
    /// well inside a meter's own tolerance, and it is the claim the whole feature rests on.
    /// </summary>
    [Theory]
    [InlineData(0.001, 147.0)]
    [InlineData(0.01, 1412.0)]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_AtTheCertifiedKclStandards_IsWithinHalfAPercent(
        double molarity,
        double certifiedMicroSiemens)
    {
        // Act
        ConductivityEstimate result = PotassiumChloride(molarity).EstimateConductivity();

        // Assert
        double relativeError = result.MicroSiemensPerCm / certifiedMicroSiemens - 1;
        Math.Abs(relativeError).Should().BeLessThan(0.005);
    }

    /// <summary>
    /// The third standard is ten times stronger than any feed, and the model reads about 4% low there. It is
    /// asserted rather than ignored so the boundary of usefulness is a fact in the test suite instead of a
    /// claim in a comment.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_WellAboveNutrientStrength_ReadsAboutFourPercentLow()
    {
        // Act
        ConductivityEstimate result = PotassiumChloride(0.1).EstimateConductivity();

        // Assert
        (result.MicroSiemensPerCm / 12880).Should().BeInRange(0.94, 0.97);
    }

    /// <summary>
    /// The uncorrected sum has to stay available and has to be the larger figure, because it is the
    /// physically exact one at infinite dilution and the correction is the only modelled step. At 0.01 M the
    /// gap is the 6% by which limiting conductivities overstate a real solution.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_IdealSum_IsAnUpperBoundOnTheEstimate()
    {
        // Act
        ConductivityEstimate result = PotassiumChloride(0.01).EstimateConductivity();

        // Assert
        result.IdealMicroSiemensPerCm.Should().BeApproximately(1498, 2);
        result.IdealMicroSiemensPerCm.Should().BeGreaterThan(result.MicroSiemensPerCm);
        result.Correction.Should().BeInRange(0.94, 0.95);
    }

    // ---------------------------------------------------------------- ionic strength

    /// <summary>
    /// Ionic strength is ½Σcz², so a divalent salt has four times the strength per mole its concentration
    /// suggests. It drives the correction, so getting it wrong would bias every estimate.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_IonicStrength_CountsDivalentIonsFourTimes()
    {
        // Arrange — 1 mM of magnesium sulfate: Mg²⁺ and SO₄²⁻, so I = ½(1×4 + 1×4) = 4 mM
        Ppm magnesiumSulfate = new PpmBuilder()
            .AddMg(24.305).AddS(32.06)
            .AddLiters(100)
            .Build();

        // Act
        ConductivityEstimate result = magnesiumSulfate.EstimateConductivity();

        // Assert
        result.IonicStrength.Should().BeApproximately(0.004, 1e-5);
    }

    /// <summary>
    /// A working feed sits around 0.02–0.04 M, and the correction near 0.91 is the 9% by which the ideal sum
    /// overstates it. Both figures are pinned because the docs quote them.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_TypicalFeed_HasTheIonicStrengthAndCorrectionTheDocsQuote()
    {
        // Arrange — N150 P50 K210 Ca160 Mg50 S65, the recipe used throughout the documentation
        Ppm feed = new PpmBuilder()
            .AddNitrate(150).AddP(50).AddK(210).AddCa(160).AddMg(50).AddS(65)
            .AddLiters(100)
            .Build();

        // Act
        ConductivityEstimate result = feed.EstimateConductivity();

        // Assert
        result.IonicStrength.Should().BeApproximately(0.025, 0.001);
        result.Correction.Should().BeApproximately(0.913, 0.005);
        result.MilliSiemensPerCm.Should().BeApproximately(2.04, 0.05);
    }

    // ---------------------------------------------------------------- what does not conduct

    /// <summary>
    /// The case that shows why EC is a poor proxy for feed strength: a solution of urea carries a great deal
    /// of nitrogen and reads as pure water, because an uncharged molecule cannot carry current.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_NitrogenAsUrea_ReadsAsPureWater()
    {
        // Arrange
        Ppm urea = new PpmBuilder().AddAmine(150).AddLiters(100).Build();

        // Act
        ConductivityEstimate result = urea.EstimateConductivity();

        // Assert
        urea.AsMillimolar().Nitrogen.Should().BeApproximately(10.7, 0.1);
        result.MicroSiemensPerCm.Should().Be(0);
    }

    /// <summary>
    /// Micronutrients are excluded from the calculation, matching <see cref="IonBalance"/>. Asserted so the
    /// omission is deliberate and visible rather than an oversight someone later "fixes" without reading why.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_MicronutrientsAlone_ContributeNothing()
    {
        // Arrange
        Ppm micros = new PpmBuilder()
            .AddFe(3).AddCu(0.05).AddMn(0.5).AddZn(0.15).AddB(0.5).AddMo(0.05).AddSi(10).AddSe(0.01)
            .AddLiters(100)
            .Build();

        // Act
        ConductivityEstimate result = micros.EstimateConductivity();

        // Assert
        result.MicroSiemensPerCm.Should().Be(0);
        result.IonicStrength.Should().Be(0);
        result.Correction.Should().Be(1, "with no ions there is no interaction to correct for");
    }

    // ---------------------------------------------------------------- per-ion shares

    /// <summary>
    /// The shares must add up to the ideal sum, or the breakdown would not be a breakdown. It is what
    /// answers "what is driving my EC" — usually nitrate, at about a third.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_PerIonShares_SumToTheIdealTotal()
    {
        // Arrange
        Ppm feed = new PpmBuilder()
            .AddNitrate(150).AddP(50).AddK(210).AddCa(160).AddMg(50).AddS(65).AddNa(20).AddCl(25)
            .AddLiters(100)
            .Build();

        // Act
        ConductivityEstimate result = feed.EstimateConductivity();

        // Assert
        double sum = result.Potassium + result.Calcium + result.Magnesium + result.Ammonium
            + result.Sodium + result.Nitrate + result.Phosphate + result.Sulfate + result.Chloride;

        sum.Should().BeApproximately(result.IdealMicroSiemensPerCm, 1e-9);
        result.Nitrate.Should().BeGreaterThan(result.Phosphate,
            "a mole of nitrate conducts twice what a mole of dihydrogen phosphate does");
    }

    /// <summary>
    /// Identical ppm need not mean identical EC, which is the reason this is computed per ion rather than
    /// scaled from total dissolved solids. A single factor could not tell these two apart.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_SamePpmDifferentIons_ReadDifferently()
    {
        // Arrange — 100 ppm of sulfur against 100 ppm of phosphorus
        Ppm asSulfate = new PpmBuilder().AddS(100).AddLiters(100).Build();
        Ppm asPhosphate = new PpmBuilder().AddP(100).AddLiters(100).Build();

        // Act & Assert
        asSulfate.EstimateConductivity().IdealMicroSiemensPerCm
            .Should().BeGreaterThan(asPhosphate.EstimateConductivity().IdealMicroSiemensPerCm * 3);
    }

    // ---------------------------------------------------------------- TDS

    /// <summary>
    /// A TDS meter measures conductivity and multiplies by a convention, which is why two meters disagree by
    /// 40% on one solution while both being right. The scale is a parameter for that reason.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsTdsPpm_ScalesTheConductivityByTheMetersOwnConvention()
    {
        // Arrange
        ConductivityEstimate result = PotassiumChloride(0.01).EstimateConductivity();

        // Act & Assert
        result.AsTdsPpm().Should().BeApproximately(result.MilliSiemensPerCm * 500, 1e-9);
        result.AsTdsPpm(700).Should().BeApproximately(result.MilliSiemensPerCm * 700, 1e-9);
        result.AsTdsPpm(700).Should().BeApproximately(result.AsTdsPpm() * 1.4, 1e-6);
    }

    /// <summary>
    /// A scale of zero or less would silently produce a meaningless reading.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-500)]
    [Trait("Category", "Unit")]
    public void AsTdsPpm_NonPositiveScale_Throws(double scale)
    {
        // Arrange
        ConductivityEstimate result = PotassiumChloride(0.01).EstimateConductivity();

        // Act
        Action act = () => result.AsTdsPpm(scale);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName(nameof(scale));
    }

    /// <summary>
    /// Guards the entry point.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_NullProfile_Throws()
    {
        // Act & Assert
        ((Action)(() => PpmExtensions.EstimateConductivity(null!)))
            .Should().Throw<ArgumentNullException>();
    }
}
