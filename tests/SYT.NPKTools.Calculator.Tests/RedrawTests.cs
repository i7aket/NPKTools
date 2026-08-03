using AwesomeAssertions;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers the model announcing that it has changed.
/// </summary>
/// <remarks>
/// Blazor does not re-render a child component whose parameters have not changed, and every panel on
/// this page takes one parameter — a callback to the same method on the same page, equal on every
/// pass. So each panel only redrew when it handled the event itself, and the acidification card, which
/// appears when the water has alkalinity, was drawn once at startup on osmosis and never again. These
/// tests cover the announcement; that the panels listen to it is markup, and shows up in the browser.
/// </remarks>
public class RedrawTests
{
    /// <summary>
    /// A recalculation tells whoever is listening.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_RaisesChanged()
    {
        CalculatorModel model = new();
        int raised = 0;
        model.Changed += () => raised++;

        model.Recalculate();

        raised.Should().Be(1);
    }

    /// <summary>
    /// A recalculation that fails still tells them, because a stale screen is worse than a lost
    /// calculation.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_WhenItCannotSolve_StillRaisesChanged()
    {
        CalculatorModel model = new();
        model.Selected.Clear();
        int raised = 0;
        model.Changed += () => raised++;

        model.Recalculate();

        model.ErrorKey.Should().Be("error.shelf.empty");
        raised.Should().Be(1);
    }

    /// <summary>
    /// The alkalinity the acidification card keys off is set by the recalculation that announces it,
    /// so a listener redrawing on the announcement sees the new value rather than the old one.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Changed_IsRaisedAfterTheResultsAreSet()
    {
        CalculatorModel model = new()
        {
            Mode = WaterInputMode.Conductivity,
            WaterPresetId = "CalciumBicarbonateHard",
        };
        model.SeedReadingFromPreset();

        double seen = -1;
        model.Changed += () => seen = model.Alkalinity;

        model.Recalculate();

        seen.Should().BeGreaterThan(0).And.Be(model.Alkalinity);
    }
}
