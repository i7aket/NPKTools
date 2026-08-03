# Localisation machinery — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Everything needed to show the interface in eight languages, with the interface still in
English and behaving exactly as it does now.

**Architecture:** A `Translations` singleton parses one embedded JSON file per language at startup and
answers `t("key")`. Plural forms go through a per-language CLDR selector. Number parsing moves into one
shared helper that accepts a comma. No ICU, no `CultureInfo`, no `HttpClient`.

**Tech Stack:** .NET 10, Blazor WebAssembly, `System.Text.Json`, xUnit, AwesomeAssertions.

Spec: `docs/superpowers/specs/2026-08-03-localisation-design.md`

## Global Constraints

- Target `net10.0`. Solution `SYT.NPKTools.slnx`. `TreatWarningsAsErrors=true`.
- CS1591 (XML docs on all public members) is enforced in `src/` only, **not** in `web/` or `tests/`.
- `InvariantGlobalization=true` stays. **Do not add ICU, `CultureInfo`-aware formatting, resx or
  satellite assemblies.**
- **No `HttpClient`.** `Program.cs` states the app has "no HttpClient and no backend of any kind";
  translations are `EmbeddedResource`, read with `Assembly.GetManifestResourceStream`.
- Display formatting stays invariant, always a dot. Only *parsing* becomes tolerant.
- The eight languages are exactly `en ru uk nl de es pl tr`. `en` is the fallback.
- This PR adds no translated text. Every non-English file is a copy of the English one, so the app
  looks identical whatever is selected. Words arrive in the second PR.
- Tests: xUnit, `using AwesomeAssertions;`, `Method_Scenario_Expectation`, `[Trait("Category", "Unit")]`.
- Run `dotnet format` before every commit; CI runs `--verify-no-changes`.

**Build:** `dotnet build SYT.NPKTools.slnx -c Release`
**Test:** `dotnet test SYT.NPKTools.slnx -c Release`

---

### Task 1: Plural rules

The piece most likely to be skipped and most visible when it is. Pure function, no dependencies.

**Files:**
- Create: `web/SYT.NPKTools.Calculator/Localisation/PluralRules.cs`
- Test: `tests/SYT.NPKTools.Calculator.Tests/PluralRulesTests.cs`

**Interfaces:**
- Produces: `PluralRules.Select(string language, long count) -> string` returning one of
  `"one"`, `"few"`, `"many"`, `"other"`.

- [ ] **Step 1: Write the failing test**

```csharp
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
    /// <summary>
    /// Languages with a simple singular and plural.
    /// </summary>
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

    /// <summary>
    /// A language nobody taught it falls back to the simple rule rather than throwing.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Select_ForAnUnknownLanguage_FallsBackToTwoForms()
    {
        PluralRules.Select("ja", 1).Should().Be("one");
        PluralRules.Select("ja", 5).Should().Be("other");
    }

    /// <summary>
    /// Negative counts are not a thing this app produces, but they must not crash it.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Select_ForANegativeCount_UsesItsMagnitude()
    {
        PluralRules.Select("ru", -2).Should().Be("few");
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release --filter "FullyQualifiedName~PluralRulesTests"`
Expected: build failure — `PluralRules` does not exist.

- [ ] **Step 3: Write the implementation**

```csharp
namespace SYT.NPKTools.Calculator.Localisation;

/// <summary>
/// Chooses which plural form a count needs.
/// </summary>
/// <remarks>
/// <para>
/// "8 recipes" is one word in English and three different words in Russian, Ukrainian and Polish:
/// 1 рецепт, 2 рецепта, 5 рецептов. A dictionary lookup alone produces "5 рецепт", which is the
/// clearest sign of a localisation nobody checked.
/// </para>
/// <para>
/// The rules follow CLDR for integers, which is all this app counts — recipes, salts, elements. The
/// boundaries are the unintuitive part and are worth stating: 21 behaves like 1 in Russian but not in
/// Polish, and 11 behaves like neither in either.
/// </para>
/// </remarks>
public static class PluralRules
{
    /// <summary>The form for a count in a language.</summary>
    public static string Select(string language, long count)
    {
        long n = Math.Abs(count);
        long mod10 = n % 10;
        long mod100 = n % 100;

        return language switch
        {
            "ru" or "uk" => mod10 == 1 && mod100 != 11 ? "one"
                : mod10 >= 2 && mod10 <= 4 && (mod100 < 12 || mod100 > 14) ? "few"
                : "many",

            // Polish keeps "one" for exactly one, so 21 is "many" where Russian would say "one".
            "pl" => n == 1 ? "one"
                : mod10 >= 2 && mod10 <= 4 && (mod100 < 12 || mod100 > 14) ? "few"
                : "many",

            _ => n == 1 ? "one" : "other",
        };
    }
}
```

- [ ] **Step 4: Run the tests**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release --filter "FullyQualifiedName~PluralRulesTests"`
Expected: PASS, 33 tests.

- [ ] **Step 5: Format and commit**

```bash
dotnet format
git add web/SYT.NPKTools.Calculator/Localisation/PluralRules.cs tests/SYT.NPKTools.Calculator.Tests/PluralRulesTests.cs
git commit -m "feat(calculator): choose plural forms the way each language does"
```

---

### Task 2: Tolerant number parsing, in one place

Five components each carry their own copy of a `Parse` helper. They all need the same change, so they
become one helper.

**Files:**
- Create: `web/SYT.NPKTools.Calculator/Localisation/Numbers.cs`
- Test: `tests/SYT.NPKTools.Calculator.Tests/NumbersTests.cs`

**Interfaces:**
- Produces: `Numbers.TryParse(object? value, out double result) -> bool`,
  `Numbers.ParseOrZero(object? value) -> double`,
  `Numbers.ParseOrNull(object? value) -> double?`.

- [ ] **Step 1: Write the failing test**

```csharp
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
    /// <summary>
    /// A comma and a dot mean the same thing.
    /// </summary>
    /// <param name="typed">What was typed.</param>
    [Theory]
    [InlineData("1.5")]
    [InlineData("1,5")]
    [Trait("Category", "Unit")]
    public void ParseOrZero_ReadsEitherDecimalSeparator(string typed)
    {
        Numbers.ParseOrZero(typed).Should().Be(1.5);
    }

    /// <summary>
    /// Group separators are noise, including the non-breaking space a spreadsheet pastes in.
    /// </summary>
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

    /// <summary>
    /// Nothing this app asks for can be negative, so a negative reads as nothing.
    /// </summary>
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

    /// <summary>
    /// Two commas is not a number in any language.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryParse_ForTwoSeparators_Fails()
    {
        Numbers.TryParse("1,5,5", out _).Should().BeFalse();
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release --filter "FullyQualifiedName~NumbersTests"`
Expected: build failure — `Numbers` does not exist.

- [ ] **Step 3: Write the implementation**

```csharp
using System.Globalization;

namespace SYT.NPKTools.Calculator.Localisation;

/// <summary>
/// Reads a number the way a grower types it, and never formats one.
/// </summary>
/// <remarks>
/// <para>
/// Parsing is tolerant, display is not. Half of Europe types a comma for the decimal point, so the
/// comma is accepted — explicitly, which is the safe way. What the app was built to avoid is a
/// culture-aware parser deciding a comma means thousands and reading 1,5 grams as 15; deciding for
/// ourselves that a comma is always a decimal point cannot do that.
/// </para>
/// <para>
/// Output stays invariant with a dot everywhere. A recipe is a set of weights that gets read aloud,
/// photographed and pasted across borders, and one unambiguous form is worth more there than local
/// familiarity.
/// </para>
/// </remarks>
public static class Numbers
{
    /// <summary>Reads a number, reporting whether it could.</summary>
    public static bool TryParse(object? value, out double result)
    {
        result = 0;

        string? text = value?.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        // Spaces group digits in most of these languages, including the non-breaking and narrow
        // no-break kinds a spreadsheet pastes in.
        string cleaned = text
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace(',', '.');

        // Two separators is not a number anybody meant.
        if (cleaned.IndexOf('.', StringComparison.Ordinal) != cleaned.LastIndexOf('.'))
        {
            return false;
        }

        return double.TryParse(cleaned, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
    }

    /// <summary>Reads a number, treating anything unreadable or negative as zero.</summary>
    public static double ParseOrZero(object? value) =>
        TryParse(value, out double parsed) && parsed >= 0 ? parsed : 0;

    /// <summary>
    /// Reads a number, keeping the difference between empty and zero.
    /// </summary>
    /// <remarks>
    /// An unmeasured drop test is not a measurement of zero, and the water estimator treats the two
    /// differently.
    /// </remarks>
    public static double? ParseOrNull(object? value) =>
        TryParse(value, out double parsed) && parsed >= 0 ? parsed : null;
}
```

- [ ] **Step 4: Run the tests**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release --filter "FullyQualifiedName~NumbersTests"`
Expected: PASS, 16 tests.

- [ ] **Step 5: Replace the five private copies**

In each of these, delete the local `Parse` method and call `Numbers` instead. Add
`@using SYT.NPKTools.Calculator.Localisation` to `_Imports.razor` first, so no file needs its own.

| File | Replace |
|---|---|
| `Pages/Home.razor` | `Parse(e.Value)` → `Numbers.ParseOrZero(e.Value)`; delete `private static double Parse` |
| `Components/ElementGrid.razor` | same |
| `Components/WaterPanel.razor` | `Parse(e.Value)` → `Numbers.ParseOrNull(e.Value)`; delete its `Parse` |
| `Components/AcidPanel.razor` | `Parse(e.Value, fallback)` → `Numbers.TryParse(e.Value, out double v) && v is >= 0 and <= 14 ? v : fallback`; delete its `Parse` |
| `Components/CustomSaltForm.razor` | the solubility and percentage handlers → `Numbers.ParseOrNull` and `Numbers.ParseOrZero` |

- [ ] **Step 6: Run everything**

Run: `dotnet test SYT.NPKTools.slnx -c Release`
Expected: PASS. The existing target and water tests exercise these paths, so a wrong replacement
shows up here rather than in the browser.

- [ ] **Step 7: Format and commit**

```bash
dotnet format
git add web/SYT.NPKTools.Calculator tests/SYT.NPKTools.Calculator.Tests
git commit -m "feat(calculator): read a comma as a decimal point, in one place instead of five"
```

---

### Task 3: The translation store

**Files:**
- Create: `web/SYT.NPKTools.Calculator/Localisation/Language.cs`
- Create: `web/SYT.NPKTools.Calculator/Localisation/Translations.cs`
- Create: `web/SYT.NPKTools.Calculator/Resources/en.json`
- Modify: `web/SYT.NPKTools.Calculator/SYT.NPKTools.Calculator.csproj`
- Test: `tests/SYT.NPKTools.Calculator.Tests/TranslationsTests.cs`

**Interfaces:**
- Consumes: `PluralRules.Select` (Task 1).
- Produces: `Language` record with `Tag`, `EnglishName`, `NativeName`, and `Language.All`
  (`IReadOnlyList<Language>`), `Language.Default` (`en`), `Language.Match(string? browserTag) -> Language`;
  `Translations` with `Current` (`Language`), `Use(Language)`, `string this[string key]`,
  `Plural(string key, long count) -> string`, and `event Action? Changed`.

`en.json` in this task carries only the handful of keys the tests need. Task 4 fills it from the
markup, which is where the real list comes from.

- [ ] **Step 1: Write the failing test**

```csharp
using AwesomeAssertions;
using SYT.NPKTools.Calculator.Localisation;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers the store the interface reads its words from.
/// </summary>
public class TranslationsTests
{
    /// <summary>
    /// The eight languages, and English as the one everything falls back to.
    /// </summary>
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

    /// <summary>
    /// A key returns its text.
    /// </summary>
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

    /// <summary>
    /// Plural forms come from the selector, and the count is substituted.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Plural_UsesTheFormForTheCount()
    {
        Translations t = new();

        t.Plural("recipes.count", 1).Should().Be("1 recipe");
        t.Plural("recipes.count", 8).Should().Be("8 recipes");
    }

    /// <summary>
    /// Switching language raises the event the interface redraws on.
    /// </summary>
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
    /// Every language carries every key English does. This is the test that earns its place: the
    /// failure it prevents — adding a string and forgetting seven files — is the one that happens.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EveryLanguage_CarriesEveryEnglishKey()
    {
        Translations t = new();
        IReadOnlyCollection<string> english = t.KeysFor(Language.Default);

        foreach (Language language in Language.All)
        {
            IReadOnlyCollection<string> keys = t.KeysFor(language);
            keys.Should().BeEquivalentTo(english, because: $"{language.Tag} must match English");
        }
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release --filter "FullyQualifiedName~TranslationsTests"`
Expected: build failure — `Language` does not exist.

- [ ] **Step 3: Write `Language`**

```csharp
namespace SYT.NPKTools.Calculator.Localisation;

/// <summary>
/// A language the interface is available in.
/// </summary>
/// <param name="Tag">The two-letter code, and the name of its resource file.</param>
/// <param name="EnglishName">The name in English, for anyone reading the code.</param>
/// <param name="NativeName">The name as its own speakers write it, for the picker.</param>
public sealed record Language(string Tag, string EnglishName, string NativeName)
{
    /// <summary>
    /// The eight, chosen by where the industry is rather than by population.
    /// </summary>
    /// <remarks>
    /// The Netherlands is the centre of greenhouse hydroponics; Almería and Antalya are the largest
    /// greenhouse concentrations in Europe; Poland has the largest sector in Central Europe.
    /// </remarks>
    public static IReadOnlyList<Language> All { get; } =
    [
        new("en", "English", "English"),
        new("ru", "Russian", "Русский"),
        new("uk", "Ukrainian", "Українська"),
        new("nl", "Dutch", "Nederlands"),
        new("de", "German", "Deutsch"),
        new("es", "Spanish", "Español"),
        new("pl", "Polish", "Polski"),
        new("tr", "Turkish", "Türkçe"),
    ];

    /// <summary>The language everything falls back to.</summary>
    public static Language Default { get; } = All[0];

    /// <summary>
    /// Picks the closest available language to what a browser reports.
    /// </summary>
    /// <param name="browserTag">A tag such as <c>uk-UA</c>, or null.</param>
    /// <returns>The match, or <see cref="Default"/>.</returns>
    /// <remarks>
    /// Matched on language and not on region: a Ukrainian speaker in Canada reports <c>uk-CA</c> and
    /// wants Ukrainian, and there is nothing region-specific in this interface to justify the extra
    /// distinction.
    /// </remarks>
    public static Language Match(string? browserTag)
    {
        if (string.IsNullOrWhiteSpace(browserTag))
        {
            return Default;
        }

        string primary = browserTag.Split('-')[0].ToLowerInvariant();
        return All.FirstOrDefault(l => l.Tag == primary) ?? Default;
    }
}
```

- [ ] **Step 4: Write `Translations`**

```csharp
using System.Reflection;
using System.Text.Json;

namespace SYT.NPKTools.Calculator.Localisation;

/// <summary>
/// The words the interface shows, in the language it was asked for.
/// </summary>
/// <remarks>
/// <para>
/// Every language is an embedded JSON file, parsed once. Fetching them from <c>wwwroot</c> was the
/// first instinct and the wrong one: this app has no <see cref="System.Net.Http.HttpClient"/> on
/// purpose, and a runtime fetch would add a failure mode — offline, or a 404 — for text that cannot
/// change while the page is open. Eight languages come to about 80 KB against an 11 MB payload.
/// </para>
/// <para>
/// A missing key returns the key. That puts <c>water.mode.osmosis</c> on the screen, which is ugly
/// and immediately reported; a blank space is neither.
/// </para>
/// </remarks>
public sealed class Translations
{
    private readonly Dictionary<string, Dictionary<string, JsonElement>> _byLanguage = new(StringComparer.Ordinal);

    /// <summary>
    /// Initializes a new instance of the <see cref="Translations"/> class, reading every language.
    /// </summary>
    public Translations()
    {
        Assembly assembly = typeof(Translations).Assembly;

        foreach (Language language in Language.All)
        {
            string name = $"{assembly.GetName().Name}.Resources.{language.Tag}.json";
            using Stream? stream = assembly.GetManifestResourceStream(name);

            _byLanguage[language.Tag] = stream is null
                ? []
                : JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(stream)
                  ?? [];
        }
    }

    /// <summary>The language currently shown.</summary>
    public Language Current { get; private set; } = Language.Default;

    /// <summary>Raised when the language changes, so the interface can redraw.</summary>
    public event Action? Changed;

    /// <summary>Shows the interface in a different language.</summary>
    /// <param name="language">The language to use.</param>
    public void Use(Language language)
    {
        ArgumentNullException.ThrowIfNull(language);

        Current = language;
        Changed?.Invoke();
    }

    /// <summary>The text for a key, falling back to English and then to the key itself.</summary>
    /// <param name="key">The key, for example <c>water.mode.osmosis</c>.</param>
    public string this[string key] => Text(key) ?? key;

    /// <summary>
    /// The text for a key, with the count substituted into the right plural form.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="count">How many.</param>
    /// <returns>The text, with <c>{0}</c> replaced by the count.</returns>
    public string Plural(string key, long count)
    {
        JsonElement? forms = Element(key);
        string form = PluralRules.Select(Current.Tag, count);

        string? template = forms is { ValueKind: JsonValueKind.Object } obj
            ? obj.TryGetProperty(form, out JsonElement chosen) ? chosen.GetString()
              : obj.TryGetProperty("other", out JsonElement other) ? other.GetString()
              : null
            : Text(key);

        return (template ?? key).Replace("{0}", count.ToString(System.Globalization.CultureInfo.InvariantCulture), StringComparison.Ordinal);
    }

    /// <summary>Every key a language defines. Used by the completeness test.</summary>
    /// <param name="language">The language to list.</param>
    public IReadOnlyCollection<string> KeysFor(Language language)
    {
        ArgumentNullException.ThrowIfNull(language);

        return _byLanguage.TryGetValue(language.Tag, out Dictionary<string, JsonElement>? keys)
            ? keys.Keys
            : [];
    }

    private string? Text(string key) =>
        Element(key) is { ValueKind: JsonValueKind.String } value ? value.GetString() : null;

    private JsonElement? Element(string key)
    {
        if (_byLanguage.TryGetValue(Current.Tag, out Dictionary<string, JsonElement>? current)
            && current.TryGetValue(key, out JsonElement found))
        {
            return found;
        }

        return _byLanguage.TryGetValue(Language.Default.Tag, out Dictionary<string, JsonElement>? fallback)
            && fallback.TryGetValue(key, out JsonElement english)
            ? english
            : null;
    }
}
```

- [ ] **Step 5: Create `Resources/en.json` with the keys the tests need**

```json
{
  "app.name": "NPKTools",
  "recipes.count": {
    "one": "{0} recipe",
    "other": "{0} recipes"
  }
}
```

- [ ] **Step 6: Register the resources and copy English to the other seven**

In the csproj, inside a new `ItemGroup`:

```xml
  <!--
    Translations are embedded rather than served from wwwroot: this app has no HttpClient on purpose,
    and a runtime fetch would add an offline failure mode for text that cannot change while the page
    is open.
  -->
  <ItemGroup>
    <EmbeddedResource Include="Resources\*.json" />
  </ItemGroup>
```

Then create the other seven as byte-for-byte copies of `en.json`. They are placeholders: this pull
request adds no translated text, so the app looks the same whatever is selected.

```bash
cd web/SYT.NPKTools.Calculator/Resources
for lang in ru uk nl de es pl tr; do cp en.json "$lang.json"; done
```

- [ ] **Step 7: Run the tests**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release --filter "FullyQualifiedName~TranslationsTests"`
Expected: PASS, 13 tests.

If the resource name does not resolve, print what is actually embedded and match it —
`Assembly.GetManifestResourceNames()` — the prefix follows `RootNamespace`, which may differ from the
assembly name.

- [ ] **Step 8: Format and commit**

```bash
dotnet format
git add web/SYT.NPKTools.Calculator tests/SYT.NPKTools.Calculator.Tests
git commit -m "feat(calculator): hold the interface text for eight languages"
```

---

### Task 4: Move every string out of the markup

The mechanical bulk of the work: about 135 strings become keys. The interface must look **identical**
after this — same words, same order, same punctuation.

**Files:**
- Modify: `web/SYT.NPKTools.Calculator/Resources/en.json` — the real key list
- Modify: `Layout/MainLayout.razor`, `Pages/Home.razor`, `Pages/NotFound.razor`,
  `Components/WaterPanel.razor`, `Components/AcidPanel.razor`, `Components/SaltPicker.razor`,
  `Components/CustomSaltForm.razor`, `Components/StoragePanel.razor`, `Components/RecipeCard.razor`
- Modify: `web/SYT.NPKTools.Calculator/Program.cs` — register `Translations`
- Modify: `web/SYT.NPKTools.Calculator/_Imports.razor`

**Interfaces:**
- Consumes: `Translations` (Task 3).
- Produces: `en.json` holding every user-visible string, keyed by area.

- [ ] **Step 1: Register the store and import the namespace**

`Program.cs`, beside the existing registration:

```csharp
builder.Services.AddSingleton<Translations>();
```

`_Imports.razor`:

```razor
@using SYT.NPKTools.Calculator.Localisation
```

- [ ] **Step 2: Adopt a key naming scheme and write it down in `en.json`**

Keys are `area.thing`, where area is the card or component:

```json
{
  "app.name": "NPKTools",
  "app.brand": "Green Secrets",
  "app.tagline": "Fertilizer calculator — runs entirely in your browser",

  "target.macro.title": "Target — macro",
  "target.micro.title": "Target — micro",
  "target.litres": "Reservoir, litres",
  "target.asString": "As a string",
  "target.asString.note": "Element=ppm, separated by spaces. L is the reservoir volume in litres. Paste one from a feed chart, or copy this one out.",
  "target.counterIons": "Counter-ions",
  "target.counterIons.note": "Not dosed for — they arrive with other salts — but a target may still name them.",
  "target.cannotSolve": "Cannot solve.",

  "water.title": "Source water",
  "water.mode.osmosis": "Osmosis",
  "water.mode.ec": "EC",
  "water.mode.ecTests": "EC + tests",
  "water.mode.analysis": "Analysis"
}
```

The full file follows the same shape. Two rules, both worth stating because breaking either is what
makes a translation impossible to do well:

- **One key per sentence, not per fragment.** Never assemble a sentence from pieces at runtime: word
  order differs between these eight languages, and a sentence built from fragments cannot be fixed by
  a translator.
- **Counts go through `Plural`,** never string concatenation.

#### Ten sentences are currently built from fragments and have to be restructured

Found by capturing the rendered text rather than by reading the markup, which is why it was missed
when this plan was written. These are not string swaps — each is a sentence whose pieces are
concatenated around a value, and in at least one of the eight languages the value does not sit where
English puts it:

| Rendered today | Has to become |
|---|---|
| `Element=ppm, separated by spaces.` + `L` + `is the reservoir volume in litres. Paste one from a feed chart, or copy this one out.` | one key, with `L` inside it |
| `The link` + `carries the whole setup in the address. …` | one key per sentence |
| `The file` + `is plain JSON you can keep, edit and re-load. …` | one key per sentence |
| `Concentrate — 1:` + n + `ml of each per litre` | `concentrate.summary` with `{0}` and `{1}` |
| `Salts you have —` + n + `/` + n | `salts.title` with `{0}` and `{1}` |
| n + `recipes —` + litres + `L` | `recipes.title`, plural, with `{1}` for litres |
| `Tank A — 118 g/L, 14 % saturated` | `tank.summary` with `{0}`, `{1}`, `{2}` |
| `Solubility allows up to 1:` + n | one key with `{0}` |
| `Target` / `In tank` / `Off by` table headers | fine as they are — single words |
| `g total`, `ml of each per litre` | fold into the sentence that carries them |

Doing these as fragment-by-fragment replacements would produce eight translations that cannot be
written correctly, so they are the part of this task to do carefully rather than quickly.

#### Two library messages also reach the screen

Task 6 covers `Error = ex.Message`. The rendered text shows two more coming from the library, both
from the concentrate plan and both full sentences:

- "Tank B is at 136 % of saturation: its salts together need more water than the tank holds, because
  they compete for the same water. Use a larger concentrate volume."
- "Tank B needs 20.3 g/L of 'Calcium Monobasic Phosphate', which dissolves to 18 g/L at 20 °C. Use a
  larger concentrate volume, or a more soluble source of that element."

They need the same treatment as the error keys: the library keeps its English prose for developers,
and the app names the condition and writes its own sentence. `ConcentratePlan` exposes the numbers
already, so nothing needs parsing out of the message.

- [ ] **Step 3: Replace the strings, component by component**

`MainLayout.razor` becomes:

```razor
@inherits LayoutComponentBase
@inject Translations T

<header class="app-header">
    <img class="brand-mark" src="img/greensecrets.png" width="32" height="32" alt="" />
    <span class="brand">@T["app.brand"]</span>
    <h1>SYT @T["app.name"]</h1>
    <span class="tagline">@T["app.tagline"]</span>
</header>

<main class="app-main">
    @Body
</main>
```

Work through the other eight files the same way. Where a count appears — `@Model.Recipes.Count recipes`,
`@Model.Selected.Count / @Model.Catalogue.Count`, `6 salts` — use
`@T.Plural("recipes.count", Model.Recipes.Count)`.

Element symbols, chemical formulas and `°dH` stay as they are: they are written the same on a bag in
all eight countries.

- [ ] **Step 4: Verify nothing moved by eye and by diff**

```bash
dotnet build SYT.NPKTools.slnx -c Release
dotnet publish web/SYT.NPKTools.Calculator -c Release -o /tmp/i18n-after
```

Serve it and compare against the live site at the same widths. The rendered text must be identical —
this task changes where words come from, not what they say.

- [ ] **Step 5: Run everything**

Run: `dotnet test SYT.NPKTools.slnx -c Release`
Expected: PASS. The completeness test from Task 3 now covers the real key list, so a string moved into
`en.json` and not into the other seven fails here.

- [ ] **Step 6: Re-copy English over the placeholders and commit**

```bash
cd web/SYT.NPKTools.Calculator/Resources
for lang in ru uk nl de es pl tr; do cp en.json "$lang.json"; done
cd -
dotnet format
git add web/SYT.NPKTools.Calculator tests/SYT.NPKTools.Calculator.Tests
git commit -m "refactor(calculator): move every string out of the markup and behind a key"
```

---

### Task 5: The picker, and remembering the choice

**Files:**
- Create: `web/SYT.NPKTools.Calculator/Components/LanguagePicker.razor`
- Modify: `web/SYT.NPKTools.Calculator/wwwroot/js/storage.js`
- Modify: `web/SYT.NPKTools.Calculator/Layout/MainLayout.razor`
- Modify: `web/SYT.NPKTools.Calculator/wwwroot/css/app.css`

**Interfaces:**
- Consumes: `Translations`, `Language` (Tasks 3–4).
- Produces: `storage.js` exports `language()` and `setLanguage(tag)`; `<LanguagePicker />` in the header.

- [ ] **Step 1: Add the two storage helpers**

Append to `storage.js`, following the shape of the existing `load` and `save`:

```javascript
const LANGUAGE_KEY = 'npktools.language';

// The interface language is remembered separately from the recipe, because it belongs to the reader
// rather than to the calculation — a link from another country must not change it.
export function language() {
  try {
    return localStorage.getItem(LANGUAGE_KEY) ?? navigator.language ?? '';
  } catch {
    return '';
  }
}

export function setLanguage(tag) {
  try {
    localStorage.setItem(LANGUAGE_KEY, tag);
  } catch {
    // Private browsing refuses to store. The choice then lasts for this tab, which is enough.
  }
}
```

- [ ] **Step 2: Write the picker**

```razor
@inject Translations T
@inject IJSRuntime Js

@*
    Reads the remembered choice, or the browser's own language the first time. Deliberately not part
    of the saved recipe: a link sent to another country should not impose the sender's language on
    the reader.
*@

<select class="language" aria-label="@T["app.language"]" value="@T.Current.Tag" @onchange="Choose">
    @foreach (Language language in Language.All)
    {
        <option value="@language.Tag">@language.NativeName</option>
    }
</select>

@code {
    private IJSObjectReference? _module;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _module = await Js.InvokeAsync<IJSObjectReference>("import", "./js/storage.js");
        string? remembered = await _module.InvokeAsync<string?>("language");
        Language chosen = Language.Match(remembered);

        if (chosen != T.Current)
        {
            T.Use(chosen);
        }
    }

    private async Task Choose(ChangeEventArgs e)
    {
        Language chosen = Language.Match(e.Value?.ToString());
        T.Use(chosen);

        if (_module is not null)
        {
            await _module.InvokeAsync<object>("setLanguage", chosen.Tag);
        }
    }
}
```

- [ ] **Step 3: Put it in the header and make the layout redraw on a change**

`MainLayout.razor` gains `<LanguagePicker />` after the tagline, and subscribes so every component
redraws when the language changes:

```razor
@code {
    protected override void OnInitialized() => T.Changed += Redraw;

    private void Redraw() => InvokeAsync(StateHasChanged);

    public void Dispose() => T.Changed -= Redraw;
}
```

Add `@implements IDisposable` at the top of the file. `App.razor` renders the layout above everything,
so one subscription there redraws the whole page.

- [ ] **Step 4: Add the style**

```css
/* Sits at the end of the header, small: it is chosen once and then ignored. */
.app-header .language {
  width: auto;
  margin-left: auto;
  min-height: 28px;
  padding: 2px 6px;
  font-size: var(--text-xs);
}

@media (max-width: 620px) {
  .app-header .language { margin-left: 0; }
}
```

- [ ] **Step 5: Build, run and check**

```bash
dotnet build SYT.NPKTools.slnx -c Release
dotnet run --project web/SYT.NPKTools.Calculator
```

Check: the picker lists eight languages in their own names; choosing one changes nothing visible yet,
because every file is still English; reloading keeps the choice; the recipe and the link are unchanged
by it.

- [ ] **Step 6: Format and commit**

```bash
dotnet format
git add web/SYT.NPKTools.Calculator
git commit -m "feat(calculator): let the reader pick their language and remember it"
```

---

### Task 6: Library messages stop reaching the screen

**Files:**
- Modify: `web/SYT.NPKTools.Calculator/CalculatorModel.cs`
- Modify: `web/SYT.NPKTools.Calculator/Resources/en.json`
- Test: `tests/SYT.NPKTools.Calculator.Tests/ErrorMessageTests.cs`

**Interfaces:**
- Consumes: `Translations` (Task 3).
- Produces: `CalculatorModel.ErrorKey` (`string?`) alongside the existing `Error`.

- [ ] **Step 1: Write the failing test**

```csharp
using AwesomeAssertions;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers the app owning its own error messages.
/// </summary>
/// <remarks>
/// The model used to surface exception text straight from the library, which is written for
/// developers and only exists in English. A grower needs a message they can act on, in their own
/// language, so the app names the failure and translates it itself.
/// </remarks>
public class ErrorMessageTests
{
    /// <summary>
    /// A malformed target names the failure rather than passing the exception through.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TargetText_WhenMalformed_SetsAKeyNotLibraryProse()
    {
        CalculatorModel model = new();

        model.TargetText = "N=oops K=210";

        model.ErrorKey.Should().Be("error.target.unreadable");
    }

    /// <summary>
    /// An empty shelf is its own failure, distinct from a malformed target.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_WithNoSaltsSelected_SetsItsOwnKey()
    {
        CalculatorModel model = new();
        model.Selected.Clear();

        model.Recalculate();

        model.ErrorKey.Should().Be("error.shelf.empty");
    }

    /// <summary>
    /// A target the shelf cannot reach is a third, different failure.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_WhenNothingSuppliesAnElement_SetsTheUncoveredKey()
    {
        CalculatorModel model = new();
        model.Selected.Clear();
        model.Selected.Add(model.Catalogue.First(f => f.Name.Value == "Potassium Nitrate").Name.Value);

        model.Recalculate();

        model.ErrorKey.Should().Be("error.recipe.uncovered");
    }

    /// <summary>
    /// A working setup reports nothing.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_WhenItWorks_HasNoError()
    {
        CalculatorModel model = new();

        model.Recalculate();

        model.ErrorKey.Should().BeNull();
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release --filter "FullyQualifiedName~ErrorMessageTests"`
Expected: build failure — `ErrorKey` does not exist.

- [ ] **Step 3: Add the keys**

```json
  "error.target.unreadable": "That target cannot be read. Use element=ppm pairs separated by spaces, for example N=150 K=210 L=100.",
  "error.shelf.empty": "No salts selected. Tick at least one.",
  "error.recipe.uncovered": "Nothing on the shelf supplies {0}.",
  "error.recipe.unreachable": "The selected salts cannot reach this target. Add more, or relax it."
```

- [ ] **Step 4: Set the key wherever `Error` is set**

`ErrorKey` is set alongside `Error` at each of the four places `Error` is assigned in
`CalculatorModel`. `Error` keeps carrying the English text so nothing that reads it breaks; the
components switch to `ErrorKey` in the next step.

```csharp
    /// <summary>
    /// Which failure this is, as a translation key, or null when there is none.
    /// </summary>
    /// <remarks>
    /// Named rather than described, because the library's own exception text is written for
    /// developers and only exists in English. <see cref="UncoveredElements"/> carries the detail the
    /// message needs to name.
    /// </remarks>
    public string? ErrorKey { get; private set; }
```

In the `TargetText` setter's catch block: `ErrorKey = "error.target.unreadable";`
Where the shelf is empty: `ErrorKey = "error.shelf.empty";`
Where no recipe was found: `ErrorKey = UncoveredElements.Count > 0 ? "error.recipe.uncovered" : "error.recipe.unreachable";`
And `ErrorKey = null;` at the top of `Recalculate` beside `Error = null;`.

- [ ] **Step 5: Show the translated message**

In `Home.razor`, the error notice reads from `ErrorKey`, substituting the uncovered elements:

```razor
            @if (Model.ErrorKey is { } errorKey)
            {
                <div class="notice error" role="alert">
                    <Icon Kind="IconKind.Error" />
                    <span>
                        <strong>@T["target.cannotSolve"]</strong>
                        @T[errorKey].Replace("{0}", string.Join(", ", Model.UncoveredElements))
                    </span>
                </div>
            }
```

- [ ] **Step 6: Run everything**

Run: `dotnet test SYT.NPKTools.slnx -c Release`
Expected: PASS.

- [ ] **Step 7: Copy English over the placeholders, format and commit**

```bash
cd web/SYT.NPKTools.Calculator/Resources && for lang in ru uk nl de es pl tr; do cp en.json "$lang.json"; done && cd -
dotnet format
git add web/SYT.NPKTools.Calculator tests/SYT.NPKTools.Calculator.Tests
git commit -m "feat(calculator): own the error messages instead of passing the library's through"
```

---

### Task 7: Prove the interface did not change, and open the pull request

**Files:**
- Modify: `CHANGELOG.md`

- [ ] **Step 1: Verify everything**

```bash
dotnet format --verify-no-changes
dotnet build SYT.NPKTools.slnx -c Release
dotnet test SYT.NPKTools.slnx -c Release
```

- [ ] **Step 2: Compare the rendered text against the live site**

The whole claim of this pull request is that nothing visible changed. Publish, serve locally, and run
the screenshot harness against both the local build and `https://i7aket.github.io/NPKTools/` at 390,
768 and 1440. The pairs should be indistinguishable apart from the new picker in the header.

- [ ] **Step 3: Add the changelog entry under `## [Unreleased]`**

```markdown
- The interface is ready to be translated: every string sits behind a key, plural forms follow each
  language's own rules rather than English's, and numbers accept a comma as a decimal point while
  still displaying a dot. Eight languages are selectable — English, Russian, Ukrainian, Dutch,
  German, Spanish, Polish, Turkish — and all eight currently show English text. No ICU: the app is
  still built with `InvariantGlobalization=true`.
```

- [ ] **Step 4: Commit and open the pull request**

```bash
git add CHANGELOG.md
git commit -m "docs: record the localisation machinery"
git push -u origin feat/localisation-machinery
```

---

## Self-review

**Spec coverage.** Symbolic keys → Task 4. Embedded JSON with English fallback → Task 3. Completeness
test → Task 3, made real by Task 4. Plural rules → Task 1. Tolerant parsing, invariant display →
Task 2. Symbols and formulas untranslated → stated in Task 4. Library messages stop leaking → Task 6.
Detection, picker, local storage, not in the link → Task 5. Eight languages → Task 3. Layout audit
across languages belongs to the second pull request, when there is text of different lengths to audit;
Task 7 checks only that nothing changed.

**Type consistency.** `PluralRules.Select(language, count)` is used in Tasks 1 and 3.
`Numbers.ParseOrZero` / `ParseOrNull` / `TryParse` in Tasks 2, 5 and 6. `Language.All`, `.Default`,
`.Match`, `.Tag`, `.NativeName` in Tasks 3, 4 and 5. `Translations`, `T[key]`, `T.Plural(key, count)`,
`T.Current`, `T.Use`, `T.Changed`, `T.KeysFor` in Tasks 3, 4, 5 and 6.

**Known soft spots**, stated rather than hidden:

- The embedded resource name depends on `RootNamespace`, which is not set in this csproj and therefore
  defaults to the assembly name. Task 3 Step 7 says what to do if it does not resolve.
- `Translations.Plural` substitutes `{0}` by string replacement rather than `string.Format`, because a
  translator may legitimately write a form with no placeholder at all and `string.Format` would not
  care either way. It also means a stray `{0}` in prose would be replaced; no current string has one.
- Seven of the eight resource files are copies of English in this pull request. The completeness test
  therefore passes trivially until the second one. That is the intended split — code first, words
  second — but it means the test proves nothing yet.
- No component tests: there is no bUnit here, so Task 5 and Task 7 end with specific things to look at.
