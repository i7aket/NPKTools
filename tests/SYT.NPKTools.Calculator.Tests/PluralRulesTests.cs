using AwesomeAssertions;
using SYT.NPKTools.Calculator.Localisation;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers choosing a plural form, which is where a word-swap localisation gives itself away.
/// </summary>
/// <remarks>
/// Russian, Ukrainian and Polish take three forms and the boundaries are not intuitive: 21 behaves
/// like 1, 11 does not, and 22 behaves like 2 while 12 does not. The cases below are the ones that
/// catch a rule written from memory.
/// </remarks>
public class PluralRulesTests
{
    /// <summary>Languages with a simple singular and plural.</summary>
    /// <param name="language">The language tag.</param>
    [Theory]
    [InlineData("en")]
    [InlineData("de")]
    [InlineData("nl")]
    [InlineData("es")]
    [InlineData("tr")]
    [Trait("Category", "Unit")]
    public void Select_ForTwoFormLanguages_UsesOneOnlyForExactlyOne(string language)
    {
        PluralRules.Select(language, 1).Should().Be("one");
        PluralRules.Select(language, 0).Should().Be("other");
        PluralRules.Select(language, 2).Should().Be("other");
        PluralRules.Select(language, 21).Should().Be("other");
    }

    /// <summary>
    /// Russian and Ukrainian: 1 рецепт, 2 рецепта, 5 рецептов — and 21 goes back to рецепт while 11
    /// does not.
    /// </summary>
    /// <param name="language">The language tag.</param>
    /// <param name="count">The number.</param>
    /// <param name="expected">The form it should select.</param>
    [Theory]
    [InlineData("ru", 1, "one")]
    [InlineData("ru", 21, "one")]
    [InlineData("ru", 101, "one")]
    [InlineData("ru", 11, "many")]
    [InlineData("ru", 2, "few")]
    [InlineData("ru", 4, "few")]
    [InlineData("ru", 22, "few")]
    [InlineData("ru", 12, "many")]
    [InlineData("ru", 5, "many")]
    [InlineData("ru", 0, "many")]
    [InlineData("ru", 100, "many")]
    [InlineData("uk", 1, "one")]
    [InlineData("uk", 3, "few")]
    [InlineData("uk", 13, "many")]
    [InlineData("uk", 31, "one")]
    [Trait("Category", "Unit")]
    public void Select_ForEastSlavic_FollowsTheThreeFormRule(string language, long count, string expected)
    {
        PluralRules.Select(language, count).Should().Be(expected);
    }

    /// <summary>
    /// Polish differs from Russian in one place that matters: 21 is not "one", it is "many".
    /// </summary>
    /// <param name="count">The number.</param>
    /// <param name="expected">The form it should select.</param>
    [Theory]
    [InlineData(1, "one")]
    [InlineData(2, "few")]
    [InlineData(4, "few")]
    [InlineData(5, "many")]
    [InlineData(12, "many")]
    [InlineData(21, "many")]
    [InlineData(22, "few")]
    [Trait("Category", "Unit")]
    public void Select_ForPolish_TreatsTwentyOneAsMany(long count, string expected)
    {
        PluralRules.Select("pl", count).Should().Be(expected);
    }

    /// <summary>A language nobody taught it falls back to the simple rule rather than throwing.</summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Select_ForAnUnknownLanguage_FallsBackToTwoForms()
    {
        PluralRules.Select("ja", 1).Should().Be("one");
        PluralRules.Select("ja", 5).Should().Be("other");
    }

    /// <summary>Negative counts are not a thing this app produces, but they must not crash it.</summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Select_ForANegativeCount_UsesItsMagnitude()
    {
        PluralRules.Select("ru", -2).Should().Be("few");
    }
}
