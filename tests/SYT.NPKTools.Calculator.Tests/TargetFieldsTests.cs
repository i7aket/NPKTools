using AwesomeAssertions;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers the target held as fields, with the string as a projection of them.
/// </summary>
/// <remarks>
/// The string stays the transport format — it is what a link and a file carry — so the two
/// representations have to agree in both directions. A field set that does not survive the trip
/// through a string would be silently lost the next time someone opened their own link.
/// </remarks>
public class TargetFieldsTests
{
    /// <summary>
    /// The default target is the one the app has always opened with.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TargetText_ByDefault_IsTheUsualStarterTarget()
    {
        CalculatorModel model = new();

        model.TargetFields["N"].Should().Be(150);
        model.TargetFields["K"].Should().Be(210);
        model.Liters.Should().Be(100);
    }

    /// <summary>
    /// Editing a field rewrites the string.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TargetText_FollowsTheFields()
    {
        CalculatorModel model = new();

        model.TargetFields["N"] = 175;

        model.TargetText.Should().Contain("N=175").And.NotContain("N=150");
    }

    /// <summary>
    /// Pasting a string rewrites the fields — the reason the string field is kept at all.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TargetText_WhenSet_RewritesTheFields()
    {
        CalculatorModel model = new();

        model.TargetText = "N=100 P=30 K=140 L=25";

        model.TargetFields["N"].Should().Be(100);
        model.TargetFields["P"].Should().Be(30);
        model.TargetFields["K"].Should().Be(140);
        model.Liters.Should().Be(25);
    }

    /// <summary>
    /// An element left out of the string is zero, not left at its previous value. Otherwise pasting a
    /// shorter target would silently keep parts of the old one.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TargetText_WhenSet_ClearsElementsItOmits()
    {
        CalculatorModel model = new();

        model.TargetText = "N=100 L=10";

        model.TargetFields["Ca"].Should().Be(0);
        model.TargetFields["S"].Should().Be(0);
    }

    /// <summary>
    /// Every field set survives the round trip through the string.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TargetText_RoundTripsEveryField()
    {
        CalculatorModel written = new();
        foreach ((string symbol, int index) in written.TargetFields.Keys.ToList().Select((s, i) => (s, i)))
        {
            written.TargetFields[symbol] = (index + 1) * 1.5;
        }

        written.Liters = 37.5;

        CalculatorModel read = new()
        {
            TargetText = written.TargetText,
        };

        read.Liters.Should().Be(37.5);
        foreach (string symbol in written.TargetFields.Keys)
        {
            read.TargetFields[symbol].Should().BeApproximately(written.TargetFields[symbol], 1e-9);
        }
    }

    /// <summary>
    /// A malformed string reports an error and leaves the table alone. Before this change a typo
    /// destroyed the whole target; now the table is the state and only the paste box can be wrong.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TargetText_WhenMalformed_ReportsAnErrorAndKeepsTheFields()
    {
        CalculatorModel model = new();
        double before = model.TargetFields["N"];

        model.TargetText = "N=oops K=210";

        model.Error.Should().NotBeNull();
        model.TargetFields["N"].Should().Be(before);
    }

    /// <summary>
    /// Zeros are left out of the string, so a link stays short and a file stays readable.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TargetText_OmitsZeroes()
    {
        CalculatorModel model = new();

        model.TargetText = "N=100 L=10";

        model.TargetText.Should().Be("N=100 L=10");
    }
}
