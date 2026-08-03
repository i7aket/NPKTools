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
