using System.Globalization;
using System.Text;

namespace NPKTools.Core.Common;

/// <summary>
/// Provides methods to help format report strings.
/// </summary>
public static class ReportFormatter
{
    /// <summary>
    /// Appends a line to the StringBuilder if the value is greater than zero,
    /// formatting the line with the specified label and value. Allows specifying number format.
    /// </summary>
    /// <param name="stringBuilder">The StringBuilder to append to.</param>
    /// <param name="label">The label to prepend to the value.</param>
    /// <param name="value">The value to format and append.</param>
    /// <param name="decimalPlaces">The number of decimal places to format the value to. If -1, formats as integer.</param>
    public static void AppendLineIfNonZero(StringBuilder stringBuilder, string label, double value,
        int decimalPlaces = 3)
    {
        if (value <= 0) return;
        string formattedValue =
            decimalPlaces < 0
                ? Math.Round(value).ToString(CultureInfo.InvariantCulture)
                : value.ToString($"F{decimalPlaces}");
        stringBuilder.AppendLine($"{label}: {formattedValue}");
    }
}