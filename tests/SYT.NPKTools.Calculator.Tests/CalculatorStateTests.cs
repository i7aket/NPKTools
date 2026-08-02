using AwesomeAssertions;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers the three ways a setup is carried between sessions: local storage, a link, and a file.
/// </summary>
/// <remarks>
/// These are compatibility tests before they are anything else. A link someone saved is a promise, and
/// the version-2 format added later in this change must not break it.
/// </remarks>
public class CalculatorStateTests
{
    private static readonly string[] Catalogue =
        ["Calcium nitrate", "Potassium nitrate", "Magnesium sulfate"];

    /// <summary>
    /// A file written now reads back identically.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ToJson_RoundTrips()
    {
        CalculatorState state = new()
        {
            Target = "N=150 P=50 K=210 L=100",
            Water = new Dictionary<string, double> { ["Ca"] = 40 },
            Salts = ["Calcium nitrate"],
            ConcentrateLiters = 2,
        };

        CalculatorState? read = CalculatorState.FromJson(state.ToJson());

        read.Should().NotBeNull();
        read!.Target.Should().Be(state.Target);
        read.Water.Should().ContainKey("Ca").WhoseValue.Should().Be(40);
        read.Salts.Should().Equal("Calcium nitrate");
        read.ConcentrateLiters.Should().Be(2);
    }

    /// <summary>
    /// A link written now reads back identically, salts included.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ToFragment_RoundTrips()
    {
        CalculatorState state = new()
        {
            Target = "N=150 K=210 L=50",
            Water = new Dictionary<string, double> { ["Ca"] = 30 },
            Salts = ["Potassium nitrate"],
            ConcentrateLiters = 1,
        };

        (CalculatorState? read, bool saltsUsable) =
            CalculatorState.FromFragment(state.ToFragment(Catalogue), Catalogue);

        read.Should().NotBeNull();
        read!.Target.Should().Be(state.Target);
        read.Salts.Should().Equal("Potassium nitrate");
        saltsUsable.Should().BeTrue();
    }

    /// <summary>
    /// Text that is not a state file is an everyday event, not an error.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void FromJson_ForUnreadableText_ReturnsNull()
    {
        CalculatorState.FromJson("not json at all").Should().BeNull();
    }
}
