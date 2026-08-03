using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace SYT.NPKTools.Calculator.Localisation;

/// <summary>
/// The words the interface shows, in the language it was asked for.
/// </summary>
/// <remarks>
/// <para>
/// Every language is an embedded JSON file, parsed once. Fetching them from <c>wwwroot</c> was the
/// first instinct and the wrong one: this app has no <c>HttpClient</c> on purpose, and a runtime fetch
/// would add a failure mode — offline, or a 404 — for text that cannot change while the page is open.
/// Eight languages come to about 80 KB against an 11 MB payload, so lazy loading buys nothing worth a
/// loading state.
/// </para>
/// <para>
/// A missing key returns the key. That puts <c>water.mode.osmosis</c> on the screen, which is ugly and
/// gets reported immediately; a blank space is neither.
/// </para>
/// </remarks>
public sealed class Translations
{
    private readonly Dictionary<string, Dictionary<string, JsonElement>> _byLanguage =
        new(StringComparer.Ordinal);

    /// <summary>
    /// Initializes a new instance of the <see cref="Translations"/> class, reading every language.
    /// </summary>
    public Translations()
    {
        Assembly assembly = typeof(Translations).Assembly;
        string prefix = assembly.GetName().Name ?? nameof(SYT);

        foreach (Language language in Language.All)
        {
            using Stream? stream = assembly.GetManifestResourceStream(
                $"{prefix}.Resources.{language.Tag}.json");

            _byLanguage[language.Tag] = stream is null
                ? []
                : JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(stream) ?? [];
        }
    }

    /// <summary>The language currently shown.</summary>
    public Language Current { get; private set; } = Language.Default;

    /// <summary>Raised when the language changes, so the interface can redraw.</summary>
    public event Action? Changed;

    /// <summary>
    /// The text for a key, falling back to English and then to the key itself.
    /// </summary>
    /// <param name="key">The key, for example <c>water.mode.osmosis</c>.</param>
    public string this[string key] => Text(key) ?? key;

    /// <summary>
    /// Shows the interface in a different language.
    /// </summary>
    /// <param name="language">The language to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="language"/> is null.</exception>
    public void Use(Language language)
    {
        ArgumentNullException.ThrowIfNull(language);

        Current = language;
        Changed?.Invoke();
    }

    /// <summary>
    /// The text for a key, with the count substituted into the right plural form.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="count">How many.</param>
    /// <returns>The text, with <c>{0}</c> replaced by the count.</returns>
    public string Plural(string key, long count)
    {
        string form = PluralRules.Select(Current.Tag, count);
        JsonElement? found = Element(key);

        string? template = found is { ValueKind: JsonValueKind.Object } forms
            ? Form(forms, form)
            : Text(key);

        return (template ?? key).Replace(
            "{0}",
            count.ToString(CultureInfo.InvariantCulture),
            StringComparison.Ordinal);
    }

    /// <summary>
    /// The text for a key, with <c>{0}</c>, <c>{1}</c> and so on replaced by the values given.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="values">The values, already formatted for display.</param>
    /// <returns>The text, with every placeholder it recognises filled in.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is null.</exception>
    /// <remarks>
    /// <para>
    /// Numbers arrive already formatted, as strings. That is deliberate: the caller knows how many
    /// decimals its number deserves, and formatting stays invariant rather than becoming this class's
    /// business.
    /// </para>
    /// <para>
    /// The scan is a single pass, so a value that happens to contain a placeholder is not substituted
    /// again — which is what lets one sentence be filled with a list built from another.
    /// </para>
    /// </remarks>
    public string Format(string key, params string[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        string text = this[key];
        StringBuilder built = new(text.Length + 16);

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '{')
            {
                int close = text.IndexOf('}', i + 1);

                if (close > i
                    && int.TryParse(
                        text.AsSpan(i + 1, close - i - 1),
                        NumberStyles.None,
                        CultureInfo.InvariantCulture,
                        out int index)
                    && index < values.Length)
                {
                    built.Append(values[index]);
                    i = close;
                    continue;
                }
            }

            built.Append(text[i]);
        }

        return built.ToString();
    }

    /// <summary>
    /// Every key a language defines.
    /// </summary>
    /// <param name="language">The language to list.</param>
    /// <returns>Its keys, or nothing when the language carries no file.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="language"/> is null.</exception>
    public IReadOnlyCollection<string> KeysFor(Language language)
    {
        ArgumentNullException.ThrowIfNull(language);

        return _byLanguage.TryGetValue(language.Tag, out Dictionary<string, JsonElement>? keys)
            ? keys.Keys
            : [];
    }

    private static string? Form(JsonElement forms, string form) =>
        forms.TryGetProperty(form, out JsonElement chosen) ? chosen.GetString()
        : forms.TryGetProperty("other", out JsonElement other) ? other.GetString()
        : null;

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
