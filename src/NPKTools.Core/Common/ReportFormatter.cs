using System.Globalization;
using System.Text;

namespace NPKTools.Core.Common;

/// <summary>
/// Provides methods to help format report strings.
/// </summary>
/// <remarks>
/// Reports always use <see cref="CultureInfo.InvariantCulture"/> so that the same solution
/// renders identically regardless of the machine's regional settings. Before 2.0.0 the
/// decimal branch honoured the current culture, which produced "100,000" on some machines
/// and "100.000" on others for the same fertilizer.
/// </remarks>
public static class ReportFormatter
{
    /// <summary>
    /// Appends a line to the StringBuilder if the value is greater than zero,
    /// formatting the line with the specified label and value. Allows specifying number format.
    /// </summary>
    /// <param name="stringBuilder">The StringBuilder to append to.</param>
    /// <param name="label">The label to prepend to the value.</param>
    /// <param name="value">The value to format and append.</param>
    /// <param name="decimalPlaces">The number of decimal places to format the value to. If negative, formats as integer.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stringBuilder"/> is null.</exception>
    public static void AppendLineIfNonZero(StringBuilder stringBuilder, string label, double value,
        int decimalPlaces = 3)
    {
        ArgumentNullException.ThrowIfNull(stringBuilder);

        if (value <= 0) return;

        string formattedValue = decimalPlaces < 0
            ? Math.Round(value).ToString(CultureInfo.InvariantCulture)
            : value.ToString($"F{decimalPlaces}", CultureInfo.InvariantCulture);

        stringBuilder.AppendLine(CultureInfo.InvariantCulture, $"{label}: {formattedValue}");
    }
}
