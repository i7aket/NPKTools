using AwesomeAssertions;
using SYT.NPKTools.Fertilizers;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers a salt the grower described, in both the ways they can describe one.
/// </summary>
public class CustomSaltTests
{
    /// <summary>
    /// Described by formula, the percentages are worked out rather than entered.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryMaterialise_FromAFormula_DerivesThePercentages()
    {
        CustomSalt salt = new() { Name = "Shop KNO3", Formula = "KNO3", Tank = ConcentrateType.A };

        salt.TryMaterialise(out Fertilizer? built, out SaltProblem? problem).Should().BeTrue(because: problem?.ToString());

        built!.Potassium.Value.Should().BeApproximately(38.672, 0.02);
    }

    /// <summary>
    /// Described by percentages — the path for blends and for chelates, which a formula cannot
    /// usefully express.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryMaterialise_FromPercentages_UsesThemAsGiven()
    {
        CustomSalt salt = new()
        {
            Name = "Fe chelate 13%",
            Tank = ConcentrateType.B,
            Percentages = { ["FeEdta"] = 13 },
        };

        salt.TryMaterialise(out Fertilizer? built, out SaltProblem? problem).Should().BeTrue(because: problem?.ToString());

        built!.Iron.Value.Should().BeApproximately(13, 0.01);
        FertilizerBundleGenerator.IsMicro(built).Should().BeTrue();
    }

    /// <summary>
    /// A salt that carries nothing helps no target, and saying so at entry beats a silent absence
    /// from every recipe.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryMaterialise_WithNothingInIt_Fails()
    {
        CustomSalt salt = new() { Name = "Empty", Tank = ConcentrateType.A };

        salt.TryMaterialise(out Fertilizer? built, out SaltProblem? problem).Should().BeFalse();

        built.Should().BeNull();
        problem.Should().NotBeNull();
    }

    /// <summary>
    /// Percentages cannot add up to more than the whole.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryMaterialise_WhenPercentagesExceedTheWhole_Fails()
    {
        CustomSalt salt = new()
        {
            Name = "Impossible",
            Tank = ConcentrateType.A,
            Percentages = { ["K"] = 70, ["Ca"] = 45 },
        };

        salt.TryMaterialise(out _, out SaltProblem? problem).Should().BeFalse();
        problem!.Key.Should().Be("salt.error.percentagesOver100");
        problem.Values.Should().Equal("115.0");
    }

    /// <summary>
    /// The formula wins when both are present, because it is the more precise description and the
    /// one the form fills in automatically.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryMaterialise_WithBoth_PrefersTheFormula()
    {
        CustomSalt salt = new()
        {
            Name = "Both",
            Formula = "KNO3",
            Tank = ConcentrateType.A,
            Percentages = { ["K"] = 1 },
        };

        salt.TryMaterialise(out Fertilizer? built, out _).Should().BeTrue();

        built!.Potassium.Value.Should().BeApproximately(38.672, 0.02);
    }

    /// <summary>
    /// A form the calculator does not know is refused by name rather than silently dropped.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryMaterialise_WithAnUnknownForm_Fails()
    {
        CustomSalt salt = new() { Name = "Odd", Percentages = { ["Unobtainium"] = 5 } };

        salt.TryMaterialise(out _, out SaltProblem? problem).Should().BeFalse();
        problem!.Key.Should().Be("salt.error.unknownForm");
        problem.Values.Should().Equal("Unobtainium");
    }
}
