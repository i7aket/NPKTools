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
    /// <summary>
    /// Reads a number, reporting whether it could.
    /// </summary>
    /// <param name="value">What was typed, from a change event.</param>
    /// <param name="result">The number, or zero when it could not be read.</param>
    /// <returns><see langword="true"/> when the input was a number.</returns>
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

    /// <summary>
    /// Reads a number, treating anything unreadable or negative as zero.
    /// </summary>
    /// <param name="value">What was typed.</param>
    /// <returns>The number, or zero.</returns>
    public static double ParseOrZero(object? value) =>
        TryParse(value, out double parsed) && parsed >= 0 ? parsed : 0;

    /// <summary>
    /// Reads a number, keeping the difference between empty and zero.
    /// </summary>
    /// <param name="value">What was typed.</param>
    /// <returns>The number, or null when the field was empty or unreadable.</returns>
    /// <remarks>
    /// An unmeasured drop test is not a measurement of zero, and the water estimator treats the two
    /// differently.
    /// </remarks>
    public static double? ParseOrNull(object? value) =>
        TryParse(value, out double parsed) && parsed >= 0 ? parsed : null;
}
