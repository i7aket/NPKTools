using AwesomeAssertions;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers the app owning its own error messages.
/// </summary>
/// <remarks>
/// The model used to surface exception text straight from the library, which is written for developers
/// and exists only in English. A grower needs a sentence they can act on, in their own language, so the
/// app names the failure and translates it itself.
/// </remarks>
public class ErrorMessageTests
{
    /// <summary>A malformed target names the failure rather than passing the exception through.</summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TargetText_WhenMalformed_SetsAKeyNotLibraryProse()
    {
        CalculatorModel model = new();

        model.TargetText = "N=oops K=210";

        model.ErrorKey.Should().Be("error.target.unreadable");
    }

    /// <summary>An empty shelf is its own failure, distinct from a malformed target.</summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_WithNoSaltsSelected_SetsItsOwnKey()
    {
        CalculatorModel model = new();
        model.Selected.Clear();

        model.Recalculate();

        model.ErrorKey.Should().Be("error.shelf.empty");
    }

    /// <summary>A target the shelf cannot reach is a third, different failure.</summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_WhenNothingSuppliesAnElement_SetsTheUncoveredKey()
    {
        CalculatorModel model = new();
        model.Selected.Clear();
        model.Selected.Add("Potassium Nitrate");

        model.Recalculate();

        model.ErrorKey.Should().Be("error.recipe.uncovered");
    }

    /// <summary>A working setup reports nothing.</summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_WhenItWorks_HasNoError()
    {
        CalculatorModel model = new();

        model.Recalculate();

        model.ErrorKey.Should().BeNull();
    }

    /// <summary>
    /// Fixing a malformed target clears the failure rather than leaving it on screen.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TargetText_WhenCorrected_ClearsTheKey()
    {
        CalculatorModel model = new();
        model.TargetText = "N=oops";
        model.ErrorKey.Should().NotBeNull();

        model.TargetText = "N=150 K=210 L=100";

        model.ErrorKey.Should().BeNull();
    }
}
