using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers the element groups an input form is built from.
/// </summary>
public class ElementGroupsTests
{
    /// <summary>
    /// An element in two groups would appear twice on screen and be entered twice.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Groups_Overlap_IsEmpty()
    {
        string[] all = [.. ElementGroups.Macro, .. ElementGroups.Micro, .. ElementGroups.CounterIons];

        all.Should().OnlyHaveUniqueItems();
    }

    /// <summary>
    /// The groups together are the whole input form. An element the parser accepts but no group
    /// carries would be silently unenterable, which is how sodium would have gone missing.
    /// </summary>
    /// <param name="symbol">The element symbol.</param>
    [Theory]
    [InlineData("N")]
    [InlineData("P")]
    [InlineData("K")]
    [InlineData("Ca")]
    [InlineData("Mg")]
    [InlineData("S")]
    [InlineData("Fe")]
    [InlineData("Cu")]
    [InlineData("Mn")]
    [InlineData("Zn")]
    [InlineData("B")]
    [InlineData("Mo")]
    [InlineData("Cl")]
    [InlineData("Si")]
    [InlineData("Se")]
    [InlineData("Na")]
    [Trait("Category", "Unit")]
    public void All_ContainsEverySymbolTheParserAccepts(string symbol)
    {
        ElementGroups.All.Should().Contain(symbol);
    }

    /// <summary>
    /// Sixteen elements and no seventeenth: a symbol added to the library without a group would
    /// break here rather than vanish from the interface.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void All_HasSixteenElements()
    {
        ElementGroups.All.Should().HaveCount(16);
    }

    /// <summary>
    /// Chlorine and sodium are not dosed for, and the bundle generator leaves them out of its micro
    /// list for that reason. They are still entered, so they are their own group rather than being
    /// folded into the micronutrients.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void CounterIons_AreChlorineAndSodium()
    {
        ElementGroups.CounterIons.Should().Equal("Cl", "Na");
    }
}
