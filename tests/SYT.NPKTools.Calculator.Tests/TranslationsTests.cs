using AwesomeAssertions;
using SYT.NPKTools.Calculator.Localisation;
using SYT.NPKTools.Concentrates;
using SYT.NPKTools.Fertilizers;
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
    /// Every notice the save and transfer card can show has words behind it.
    /// </summary>
    /// <remarks>
    /// These six are chosen in C# rather than in markup, and four of them need a clipboard, a file or a
    /// link from an older catalogue to appear — none of which a headless browser can be walked through.
    /// So they are checked here instead: a key that reaches the screen without a value in the resource
    /// files shows the key.
    /// </remarks>
    [Theory]
    [InlineData("storage.msg.saltsIgnored")]
    [InlineData("storage.msg.copied")]
    [InlineData("storage.msg.clipboardFailed")]
    [InlineData("storage.msg.notARecipe")]
    [InlineData("storage.msg.loaded")]
    [InlineData("storage.msg.forgotten")]
    [Trait("Category", "Unit")]
    public void StorageNotices_HaveWords(string key)
    {
        Translations t = new();

        t[key].Should().NotBe(key);
    }

    /// <summary>
    /// The name of a loaded file lands in the notice about it.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Format_PutsAFileNameIntoTheLoadedNotice()
    {
        Translations t = new();

        t.Format("storage.msg.loaded", "feed-chart.json").Should().Be("Loaded feed-chart.json.");
    }

    /// <summary>
    /// Every kind of concentrate warning has words in the interface.
    /// </summary>
    /// <remarks>
    /// Two of the four — a precipitation risk, and a tank the library had to infer — could not be
    /// provoked in a browser, so this is what stands behind them. It also catches a kind added to the
    /// library and not to the resource files, which would reach the screen as its key.
    /// </remarks>
    [Fact]
    [Trait("Category", "Unit")]
    public void EveryConcentrateWarningKind_HasWords()
    {
        Translations t = new();

        foreach (ConcentrateWarningKind kind in Enum.GetValues<ConcentrateWarningKind>())
        {
            t[$"concentrate.warn.{kind}"].Should().NotBe($"concentrate.warn.{kind}");
        }
    }

    /// <summary>
    /// A recipe of one salt says "1 salt", which the markup it replaced could not.
    /// </summary>
    /// <remarks>
    /// The count was interpolated into a fixed plural before — "1 salts" — so this is a small fix
    /// rather than only a move behind a key.
    /// </remarks>
    [Fact]
    [Trait("Category", "Unit")]
    public void Plural_CountsSalts()
    {
        Translations t = new();

        t.Plural("recipe.salts", 1).Should().Be("1 salt");
        t.Plural("recipe.salts", 6).Should().Be("6 salts");
    }

    /// <summary>
    /// Every way a formula can be refused has words in the interface, and the offending text travels
    /// with it.
    /// </summary>
    /// <remarks>
    /// The parser reports a kind; the app writes the sentence. Walking the enum is what catches a kind
    /// added to the library and not to the resource files — the alternative is a grower who has just
    /// mistyped a formula being shown <c>salt.error.UnexpectedCharacter</c>.
    /// </remarks>
    [Fact]
    [Trait("Category", "Unit")]
    public void EveryFormulaProblemKind_HasWords()
    {
        Translations t = new();

        foreach (FormulaProblemKind kind in Enum.GetValues<FormulaProblemKind>())
        {
            t[$"salt.error.{kind}"].Should().NotBe($"salt.error.{kind}");
        }
    }

    /// <summary>
    /// The character and its position both reach the sentence about them.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void From_CarriesTheCharacterAndItsPosition()
    {
        Translations t = new();
        ChemicalFormula.TryParse("KNO3!", out _, out FormulaProblem? problem).Should().BeFalse();

        SaltProblem described = SaltProblem.From(problem!);

        described.Key.Should().Be("salt.error.UnexpectedCharacter");
        t.Format(described.Key, [.. described.Values]).Should().Be("Unexpected '!' at position 5.");
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
