using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers the ratio analysis and the per-fertilizer breakdown.
/// </summary>
/// <remarks>
/// These close the standing complaint about comparable tools — that they compute a recipe but say
/// nothing about whether it is a sensible one.
/// </remarks>
public class NutrientRatiosTests
{
    private const double Precision = 1e-9;

    // ---------------------------------------------------------------- ratios

    [Fact]
    [Trait("Category", "Unit")]
    public void Ratios_AreComputedAsPartsOfTheFirstPerOneOfTheSecond()
    {
        // Arrange: N 150 (all nitrate), P 50, K 200, Ca 100, Mg 50, S 60.
        Ppm ppm = new PpmBuilder()
            .AddNitrate(150).AddP(50).AddK(200).AddCa(100).AddMg(50).AddS(60).AddLiters(100)
            .Build();

        // Act
        NutrientRatios ratios = ppm.Ratios();

        // Assert
        ratios.NitrogenToPotassium.Should().BeApproximately(150.0 / 200, Precision);
        ratios.PotassiumToCalcium.Should().BeApproximately(2, Precision);
        ratios.CalciumToMagnesium.Should().BeApproximately(2, Precision);
        ratios.PotassiumToMagnesium.Should().BeApproximately(4, Precision);
        ratios.NitrogenToSulfur.Should().BeApproximately(2.5, Precision);
        ratios.NitrogenToPhosphorus.Should().BeApproximately(3, Precision);
    }

    /// <summary>
    /// The nitrate-to-ammonium ratio is the one that predicts pH movement rather than nutrition, so it
    /// is asserted separately including its total-nitrogen interaction.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Ratios_NitrateToAmmonium_UsesTheNitrogenForms()
    {
        // Arrange: 120 nitrate + 30 ammonium = 150 total nitrogen.
        Ppm ppm = new PpmBuilder()
            .AddNitrate(120).AddAmmonium(30).AddK(200).AddLiters(100)
            .Build();

        // Act
        NutrientRatios ratios = ppm.Ratios();

        // Assert
        ratios.NitrateToAmmonium.Should().BeApproximately(4, Precision);
        ratios.NitrogenToPotassium.Should().BeApproximately(150.0 / 200, Precision);
    }

    /// <summary>
    /// A ratio with nothing in its denominator is null, not zero and not infinity. A nitrate-only mix is
    /// the everyday case, and reporting "0" or "∞" for it would be actively misleading.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Ratios_MissingDenominator_IsNullRatherThanZeroOrInfinity()
    {
        // Arrange: no ammonium, no magnesium, no sulfur.
        Ppm ppm = new PpmBuilder().AddNitrate(150).AddK(200).AddLiters(100).Build();

        // Act
        NutrientRatios ratios = ppm.Ratios();

        // Assert
        ratios.NitrateToAmmonium.Should().BeNull();
        ratios.CalciumToMagnesium.Should().BeNull();
        ratios.PotassiumToMagnesium.Should().BeNull();
        ratios.NitrogenToSulfur.Should().BeNull();
        ratios.NitrogenToPhosphorus.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Ratios_TotalPpm_MatchesTheSolutionTotal()
    {
        Ppm ppm = new PpmBuilder()
            .AddNitrate(150).AddP(50).AddK(200).AddLiters(100)
            .Build();

        ppm.Ratios().TotalPpm.Should().BeApproximately(ppm.Value, Precision);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Ratios_NullPpm_ThrowsArgumentNullException()
    {
        Action act = () => ((Ppm)null!).Ratios();

        act.Should().Throw<ArgumentNullException>();
    }

    // ---------------------------------------------------------------- breakdown

    /// <summary>
    /// The parts must sum to the whole. Measured through the same calculator as the total, so a change
    /// to the ppm arithmetic cannot make them disagree.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void Breakdown_ContributionsSumToTheWholeMix()
    {
        // Arrange
        IFertilizerOptimizationService service = NpkTools.CreateOptimizationService();
        IPpmCalculationService calculator = NpkTools.CreatePpmCalculator();
        PpmTarget target = new PpmTargetBuilder()
            .AddN(150).AddP(50).AddK(200).AddCa(100).AddMg(50).AddLiters(100)
            .Build();

        Solution mix = service.FindMacroSolutions(target)[0];
        Ppm whole = calculator.CalculatePpm(mix, mix.WaterLiters);

        // Act
        IReadOnlyList<FertilizerContribution> parts = mix.Breakdown(calculator);

        // Assert
        parts.Should().HaveCount(mix.Count);
        parts.Sum(p => p.Contribution.Nitrogen.Value).Should().BeApproximately(whole.Nitrogen.Value, 1e-6);
        parts.Sum(p => p.Contribution.Potassium.Value).Should().BeApproximately(whole.Potassium.Value, 1e-6);
        parts.Sum(p => p.Contribution.Calcium.Value).Should().BeApproximately(whole.Calcium.Value, 1e-6);
        parts.Sum(p => p.Contribution.Sulfur.Value).Should().BeApproximately(whole.Sulfur.Value, 1e-6);
    }

    /// <summary>
    /// The point of the breakdown: attributing an element nobody asked for to the salt that carried it.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void Breakdown_AttributesAnUnrequestedElementToItsSource()
    {
        // Arrange: the target says nothing about sulfur, but magnesium sulfate brings it along.
        IFertilizerOptimizationService service = NpkTools.CreateOptimizationService();
        IPpmCalculationService calculator = NpkTools.CreatePpmCalculator();
        PpmTarget target = new PpmTargetBuilder()
            .AddN(150).AddP(50).AddK(200).AddCa(100).AddMg(50).AddLiters(100)
            .Build();

        Solution mix = service.FindMacroSolutions(target)[0];

        // Act
        IReadOnlyList<FertilizerContribution> parts = mix.Breakdown(calculator);
        FertilizerContribution[] sulfurSources =
            [.. parts.Where(p => p.Contribution.Sulfur.Value > 0)];

        // Assert: sulfur arrived, and it is traceable to specific salts rather than being anonymous.
        calculator.CalculatePpm(mix, mix.WaterLiters).Sulfur.Value.Should().BeGreaterThan(0);
        sulfurSources.Should().NotBeEmpty();
        sulfurSources.Should().AllSatisfy(p => p.Fertilizer.Name.Value.Should().NotBeEmpty());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Breakdown_NullCalculator_ThrowsArgumentNullException()
    {
        Solution mix = new([], 100);

        Action act = () => mix.Breakdown(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Breakdown_NullSolution_ThrowsArgumentNullException()
    {
        Action act = () => ((Solution)null!).Breakdown(NpkTools.CreatePpmCalculator());

        act.Should().Throw<ArgumentNullException>();
    }
}
