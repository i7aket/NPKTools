using AwesomeAssertions;
using SYT.NPKTools.Fertilizers;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers a grower's own salts taking part in the recipe search.
/// </summary>
public class CustomSaltShelfTests
{
    private static CustomSalt Kno3() =>
        new() { Name = "Shop KNO3", Formula = "KNO3", Tank = ConcentrateType.A };

    /// <summary>
    /// An added salt appears on the shelf, ticked, and the count goes up by one.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryAddCustomSalt_PutsItOnTheShelfTicked()
    {
        CalculatorModel model = new();
        int before = model.Catalogue.Count;

        model.TryAddCustomSalt(Kno3(), out string? error).Should().BeTrue(because: error);

        model.Catalogue.Should().HaveCount(before + 1);
        model.Selected.Should().Contain("Shop KNO3");
    }

    /// <summary>
    /// A name already taken is refused. The shelf is keyed by name, so two salts sharing one would
    /// be selected and deselected together and one would silently ride along in every recipe.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryAddCustomSalt_WithATakenName_IsRefused()
    {
        CalculatorModel model = new();
        string existing = model.Catalogue[0].Name.Value;

        model.TryAddCustomSalt(
            new CustomSalt { Name = existing, Formula = "KNO3" }, out string? error)
            .Should().BeFalse();

        error.Should().Contain(existing);
    }

    /// <summary>
    /// The same custom name twice is refused for the same reason.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryAddCustomSalt_Twice_IsRefused()
    {
        CalculatorModel model = new();
        model.TryAddCustomSalt(Kno3(), out _).Should().BeTrue();

        model.TryAddCustomSalt(Kno3(), out string? error).Should().BeFalse();
        error.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// A custom salt stands in for a built-in one. The macro bundle generator only forms a bundle
    /// when all six macronutrients are covered, so the honest test is substitution rather than a
    /// two-salt shelf: drop the catalogue's potassium nitrate, add the grower's own, and the
    /// recipes still come out.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_UsesACustomSaltInPlaceOfABuiltInOne()
    {
        CalculatorModel model = new();
        model.Recalculate();
        model.Recipes.Should().NotBeEmpty(because: "the untouched shelf solves the starter target");

        model.Selected.Remove("Potassium Nitrate");
        model.TryAddCustomSalt(Kno3(), out string? error).Should().BeTrue(because: error);
        model.Recalculate();

        model.Recipes.Should().NotBeEmpty(because: model.Error);
        model.Recipes.SelectMany(r => r.Mix).Select(f => f.Name.Value)
            .Should().Contain("Shop KNO3");
    }

    /// <summary>
    /// Removing one takes it off the shelf and out of the selection.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void RemoveCustomSalt_TakesItOff()
    {
        CalculatorModel model = new();
        model.TryAddCustomSalt(Kno3(), out _).Should().BeTrue();

        model.RemoveCustomSalt("Shop KNO3");

        model.CustomSalts.Should().BeEmpty();
        model.Catalogue.Should().NotContain(f => f.Name.Value == "Shop KNO3");
        model.Selected.Should().NotContain("Shop KNO3");
    }

    /// <summary>
    /// An unusable description never reaches the shelf.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryAddCustomSalt_WithAnUnreadableFormula_IsRefused()
    {
        CalculatorModel model = new();

        model.TryAddCustomSalt(
            new CustomSalt { Name = "Bad", Formula = "Zz9" }, out string? error).Should().BeFalse();

        error.Should().NotBeNullOrWhiteSpace();
        model.CustomSalts.Should().BeEmpty();
    }

    /// <summary>
    /// A custom micronutrient salt joins the micro group, decided by composition rather than a flag.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryAddCustomSalt_ForAMicroSalt_JoinsTheMicroGroup()
    {
        CalculatorModel model = new();

        model.TryAddCustomSalt(
            new CustomSalt { Name = "Shop zinc", Formula = "ZnSO4*7H2O", Tank = ConcentrateType.B },
            out _).Should().BeTrue();

        Fertilizer added = model.Catalogue.Single(f => f.Name.Value == "Shop zinc");
        FertilizerBundleGenerator.IsMicro(added).Should().BeTrue();
    }
}
