using AwesomeAssertions;
using SYT.NPKTools.Calculator.Localisation;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers the store the interface reads its words from.
/// </summary>
public class TranslationsTests
{
    /// <summary>The eight languages, and English as the one everything falls back to.</summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void All_IsTheEightLanguagesWithEnglishDefault()
    {
        Language.All.Select(l => l.Tag).Should().Equal("en", "ru", "uk", "nl", "de", "es", "pl", "tr");
        Language.Default.Tag.Should().Be("en");
    }

    /// <summary>
    /// A browser tag is matched on its language, not its region: a Ukrainian in Canada still gets
    /// Ukrainian.
    /// </summary>
    /// <param name="browserTag">What the browser reports.</param>
    /// <param name="expected">The language it should select.</param>
    [Theory]
    [InlineData("uk-UA", "uk")]
    [InlineData("uk", "uk")]
    [InlineData("de-AT", "de")]
    [InlineData("es-419", "es")]
    [InlineData("pt-BR", "en")]
    [InlineData("", "en")]
    [InlineData(null, "en")]
    [Trait("Category", "Unit")]
    public void Match_UsesTheLanguageAndIgnoresTheRegion(string? browserTag, string expected)
    {
        Language.Match(browserTag).Tag.Should().Be(expected);
    }

    /// <summary>A key returns its text.</summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Indexer_ReturnsTheText()
    {
        Translations t = new();

        t["app.name"].Should().Be("NPKTools");
    }

    /// <summary>
    /// A key nobody defined returns the key itself, so the gap is visible on screen rather than
    /// silently blank.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Indexer_ForAnUnknownKey_ReturnsTheKey()
    {
        Translations t = new();

        t["nothing.here"].Should().Be("nothing.here");
    }

    /// <summary>Plural forms come from the selector, and the count is substituted.</summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Plural_UsesTheFormForTheCount()
    {
        Translations t = new();

        t.Plural("recipes.count", 1).Should().Be("1 recipe");
        t.Plural("recipes.count", 8).Should().Be("8 recipes");
    }

    /// <summary>Switching language raises the event the interface redraws on.</summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Use_RaisesChanged()
    {
        Translations t = new();
        int raised = 0;
        t.Changed += () => raised++;

        t.Use(Language.All.Single(l => l.Tag == "ru"));

        raised.Should().Be(1);
        t.Current.Tag.Should().Be("ru");
    }

    /// <summary>
    /// Values land in the placeholders that name them.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Format_FillsThePlaceholders()
    {
        Translations t = new();

        t.Format("water.excess.item", "Cl", "30", "0").Should().Be("Cl (30 vs target 0)");
    }

    /// <summary>
    /// A value that itself contains a placeholder is left alone, which is what lets one sentence be
    /// filled with a list assembled from another.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Format_DoesNotSubstituteIntoItsOwnOutput()
    {
        Translations t = new();

        t.Format("water.excess.detail", "{1} ppm").Should().StartWith("{1} ppm. Fertilizer only adds");
    }

    /// <summary>
    /// A placeholder nobody supplied a value for stays visible rather than becoming a blank.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Format_WithTooFewValues_LeavesThePlaceholderInPlace()
    {
        Translations t = new();

        t.Format("water.excess.item", "Cl").Should().Be("Cl ({1} vs target {2})");
    }

    /// <summary>
    /// Every water preset the library offers has a name in the interface.
    /// </summary>
    /// <remarks>
    /// The presets are named by <see cref="WaterPreset.Id"/> rather than by their own
    /// <see cref="WaterPreset.Label"/>, which is English prose written for developers. That leaves one
    /// failure worth a test: a preset added to the library and not to the resource files would show
    /// its key in the picker.
    /// </remarks>
    [Fact]
    [Trait("Category", "Unit")]
    public void EveryWaterPreset_HasAName()
    {
        Translations t = new();
        IReadOnlyCollection<string> keys = t.KeysFor(Language.Default);

        foreach (WaterPreset preset in WaterPreset.All)
        {
            keys.Should().Contain($"water.preset.{preset.Id}");
        }
    }

    /// <summary>
    /// Every acid the library offers has a name in the interface.
    /// </summary>
    /// <remarks>
    /// Keyed by <see cref="Acid.Kind"/> with the strength filled in, so the six built-in acids need
    /// three keys rather than six — and Turkish, which writes the percentage as <c>%60</c>, can put it
    /// where it belongs.
    /// </remarks>
    [Fact]
    [Trait("Category", "Unit")]
    public void EveryAcid_HasAName()
    {
        Translations t = new();
        IReadOnlyCollection<string> keys = t.KeysFor(Language.Default);

        foreach (Acid acid in Acid.All)
        {
            keys.Should().Contain($"acid.kind.{acid.Kind}");
        }
    }

    /// <summary>
    /// Every language carries every key English does. This is the test that earns its place: the
    /// failure it prevents — adding a string and forgetting seven files — is the one that happens.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EveryLanguage_CarriesEveryEnglishKey()
    {
        Translations t = new();
        IReadOnlyCollection<string> english = t.KeysFor(Language.Default);

        english.Should().NotBeEmpty(because: "the resources must actually be embedded");

        foreach (Language language in Language.All)
        {
            t.KeysFor(language).Should().BeEquivalentTo(
                english, because: $"{language.Tag} must match English");
        }
    }
}
