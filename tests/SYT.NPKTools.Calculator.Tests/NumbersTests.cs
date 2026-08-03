using AwesomeAssertions;
using SYT.NPKTools.Calculator.Localisation;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers reading a number the way a grower types it.
/// </summary>
/// <remarks>
/// Half of Europe types a comma for the decimal point. Accepting it explicitly is safe in a way that
/// culture-aware parsing is not: the danger the app was built to avoid is a parser deciding a comma
/// means thousands, turning 1,5 grams into 15.
/// </remarks>
public class NumbersTests
{
    /// <summary>A comma and a dot mean the same thing.</summary>
    /// <param name="typed">What was typed.</param>
    [Theory]
    [InlineData("1.5")]
    [InlineData("1,5")]
    [Trait("Category", "Unit")]
    public void ParseOrZero_ReadsEitherDecimalSeparator(string typed)
    {
        Numbers.ParseOrZero(typed).Should().Be(1.5);
    }

    /// <summary>Group separators are noise, including the non-breaking space a spreadsheet pastes in.</summary>
    /// <param name="typed">What was typed.</param>
    [Theory]
    [InlineData("1 500")]
    [InlineData("1 500")]
    [InlineData("1 500")]
    [Trait("Category", "Unit")]
    public void ParseOrZero_IgnoresGroupSeparators(string typed)
    {
        Numbers.ParseOrZero(typed).Should().Be(1500);
    }

    /// <summary>
    /// Half-typed and empty input reads as nothing rather than throwing, which is what lets a field
    /// be cleared.
    /// </summary>
    /// <param name="typed">What was typed.</param>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("-")]
    [InlineData("abc")]
    [InlineData(null)]
    [Trait("Category", "Unit")]
    public void ParseOrZero_ForUnreadableInput_IsZero(string? typed)
    {
        Numbers.ParseOrZero(typed).Should().Be(0);
    }

    /// <summary>Nothing this app asks for can be negative, so a negative reads as nothing.</summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ParseOrZero_ForANegative_IsZero()
    {
        Numbers.ParseOrZero("-5").Should().Be(0);
    }

    /// <summary>
    /// An unmeasured drop test is not a measurement of zero, so the nullable form keeps the
    /// difference.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ParseOrNull_KeepsEmptyDistinctFromZero()
    {
        Numbers.ParseOrNull("").Should().BeNull();
        Numbers.ParseOrNull("0").Should().Be(0);
    }

    /// <summary>Two commas is not a number in any language.</summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryParse_ForTwoSeparators_Fails()
    {
        Numbers.TryParse("1,5,5", out _).Should().BeFalse();
    }
}
