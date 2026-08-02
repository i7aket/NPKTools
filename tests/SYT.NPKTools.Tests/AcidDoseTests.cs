using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers the acid needed to bring source water down to a working pH.
/// </summary>
/// <remarks>
/// The worked case throughout is the moderately hard preset — 166 ppm of bicarbonate, 2.72 meq/L of
/// alkalinity — taken from pH 7.6 to 5.8, which is the ordinary situation this feature exists for.
/// </remarks>
public class AcidDoseTests
{
    private const double ModerateAlkalinity = 2.721;

    /// <summary>
    /// At the first pKa the carbonate is half bicarbonate by definition, which is the one point on
    /// the curve that can be checked without trusting the implementation.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void BicarbonateFraction_AtTheFirstPka_IsAHalf()
    {
        AcidDose.BicarbonateFraction(6.35).Should().BeApproximately(0.5, 0.001);
    }

    /// <summary>
    /// Below the pKa the carbonate is mostly carbonic acid; above it, mostly bicarbonate.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void BicarbonateFraction_RisesWithPh()
    {
        AcidDose.BicarbonateFraction(5.8).Should().BeApproximately(0.2199, 0.001);
        AcidDose.BicarbonateFraction(7.6).Should().BeApproximately(0.9451, 0.001);
    }

    /// <summary>
    /// The headline figure: neutralising to pH 5.8 takes about three quarters of the alkalinity, not
    /// all of it. Most guides quote the full figure, and the difference is a quarter of the acid.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Calculate_ForModeratelyHardWater_TakesAboutThreeQuartersOfTheAlkalinity()
    {
        AcidPlan plan = AcidDose.Calculate(ModerateAlkalinity, 7.6, 5.8, Acid.Nitric60, 100);

        plan.MilliequivalentsPerLitre.Should().BeApproximately(2.089, 0.005);
        (plan.MilliequivalentsPerLitre / ModerateAlkalinity).Should().BeApproximately(0.768, 0.005);
    }

    /// <summary>
    /// The volume a grower actually measures out, and the nitrogen it brings with it.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Calculate_WithNitricAcid_GivesTheVolumeAndTheNitrogen()
    {
        AcidPlan plan = AcidDose.Calculate(ModerateAlkalinity, 7.6, 5.8, Acid.Nitric60, 100);

        plan.Millilitres.Should().BeApproximately(16.05, 0.1);
        plan.NutrientSymbol.Should().Be("N");
        plan.NutrientPpm.Should().BeApproximately(29.3, 0.1);
    }

    /// <summary>
    /// Phosphoric acid is the case that justifies reporting the nutrient at all: neutralising this
    /// water with it delivers 65 ppm of phosphorus, past a typical 50 ppm target before any salt is
    /// weighed, and no recipe can take it back out.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Calculate_WithPhosphoricAcid_OvershootsATypicalPhosphorusTarget()
    {
        AcidPlan plan = AcidDose.Calculate(ModerateAlkalinity, 7.6, 5.8, Acid.Phosphoric85, 100);

        plan.NutrientSymbol.Should().Be("P");
        plan.NutrientPpm.Should().BeApproximately(64.7, 0.5);
        plan.Millilitres.Should().BeApproximately(14.3, 0.1);
    }

    /// <summary>
    /// Sulfuric acid gives up two protons, so it takes least volume and brings sulfur rather than a
    /// nutrient that is usually already at target.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Calculate_WithSulfuricAcid_UsesTwoProtonsPerMolecule()
    {
        AcidPlan plan = AcidDose.Calculate(ModerateAlkalinity, 7.6, 5.8, Acid.Sulfuric98, 100);

        plan.NutrientSymbol.Should().Be("S");
        plan.Millilitres.Should().BeApproximately(5.69, 0.1);
        plan.NutrientPpm.Should().BeApproximately(33.5, 0.5);
    }

    /// <summary>
    /// Every acid in the table: strength follows from its own percentage, density and the protons it
    /// can give up at working pH, so a typo in the table cannot pass unnoticed.
    /// </summary>
    /// <param name="id">The acid identifier.</param>
    /// <param name="expected">The equivalents per litre it should have.</param>
    [Theory]
    [InlineData("Nitric60", 13.02)]
    [InlineData("Nitric38", 7.44)]
    [InlineData("Phosphoric85", 14.62)]
    [InlineData("Phosphoric75", 12.09)]
    [InlineData("Sulfuric98", 36.69)]
    [InlineData("Sulfuric37", 9.63)]
    [Trait("Category", "Unit")]
    public void EquivalentsPerLitre_MatchTheTable(string id, double expected)
    {
        Acid acid = Acid.All.Single(a => a.Id == id);

        acid.EquivalentsPerLitre.Should().BeApproximately(expected, 0.02);
    }

    /// <summary>
    /// Reverse osmosis needs no acid, and neither does water already at the target pH.
    /// </summary>
    /// <param name="alkalinity">The water's alkalinity, in meq/L.</param>
    /// <param name="waterPh">The pH of the untreated water.</param>
    /// <param name="targetPh">The pH to reach.</param>
    [Theory]
    [InlineData(0, 7.6, 5.8)]
    [InlineData(2.721, 5.8, 5.8)]
    [InlineData(2.721, 5.5, 5.8)]
    [Trait("Category", "Unit")]
    public void Calculate_WhenNothingNeedsNeutralising_IsZero(
        double alkalinity,
        double waterPh,
        double targetPh)
    {
        AcidPlan plan = AcidDose.Calculate(alkalinity, waterPh, targetPh, Acid.Nitric60, 100);

        plan.MilliequivalentsPerLitre.Should().Be(0);
        plan.Millilitres.Should().Be(0);
        plan.NutrientPpm.Should().Be(0);
    }

    /// <summary>
    /// Volume scales with the tank, and concentration does not.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Calculate_ScalesVolumeWithTheTankButNotConcentration()
    {
        AcidPlan small = AcidDose.Calculate(ModerateAlkalinity, 7.6, 5.8, Acid.Nitric60, 10);
        AcidPlan large = AcidDose.Calculate(ModerateAlkalinity, 7.6, 5.8, Acid.Nitric60, 1000);

        large.Millilitres.Should().BeApproximately(small.Millilitres * 100, 0.01);
        large.NutrientPpm.Should().BeApproximately(small.NutrientPpm, 1e-9);
    }

    /// <summary>
    /// A custom acid is described the same way the built-in ones are, and behaves the same.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Custom_MatchesABuiltInAcidGivenTheSameFigures()
    {
        Acid custom = new(AcidKind.Nitric, 60, 1.367);

        custom.EquivalentsPerLitre.Should().BeApproximately(Acid.Nitric60.EquivalentsPerLitre, 1e-9);
    }
}
