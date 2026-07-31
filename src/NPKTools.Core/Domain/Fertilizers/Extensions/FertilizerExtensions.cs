using System.Globalization;
using System.Text;
using NPKTools.Core.Common;
using NPKTools.Core.Constants;
using static NPKTools.Core.Constants.Labels;

namespace NPKTools.Core.Domain.Fertilizers.Extensions;

/// <summary>
/// Human-readable renderings of a <see cref="Fertilizer"/>.
/// All output uses the invariant culture so reports are reproducible across machines.
/// </summary>
public static class FertilizerExtensions
{
    /// <summary>
    /// Renders a multi-line report listing the fertilizer's identity and every non-zero nutrient.
    /// </summary>
    /// <param name="fertilizer">The fertilizer to describe.</param>
    /// <returns>A multi-line report string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="fertilizer"/> is null.</exception>
    public static string Report(this Fertilizer fertilizer)
    {
        ArgumentNullException.ThrowIfNull(fertilizer);

        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine(CultureInfo.InvariantCulture, $"{Name}: {fertilizer.Name.Value}");
        stringBuilder.AppendLine(CultureInfo.InvariantCulture, $"{Formula}: {fertilizer.Formula.Value}");
        stringBuilder.AppendLine(CultureInfo.InvariantCulture, $"{ConcentrateType}: {fertilizer.Type}");
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Weight, fertilizer.Weight.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Nitrogen, fertilizer.Nitrogen.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{SubItemPrefix}{NitrateNo3}", fertilizer.Nitrogen.Nitrate);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{SubItemPrefix}{AmmoniumNh4}",
            fertilizer.Nitrogen.Ammonium);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{SubItemPrefix}{AmineNh2}", fertilizer.Nitrogen.Amine);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Phosphorus, fertilizer.Phosphorus.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Potassium, fertilizer.Potassium.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Calcium, fertilizer.Calcium.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{SubItemPrefix}{Edta}", fertilizer.Calcium.CaEdta);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Magnesium, fertilizer.Magnesium.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{SubItemPrefix}{Edta}", fertilizer.Magnesium.MgEdta);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Sulfur, fertilizer.Sulfur.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Chlorine, fertilizer.Chlorine.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Iron, fertilizer.Iron.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{SubItemPrefix}{Edta}", fertilizer.Iron.FeEdta);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{SubItemPrefix}{Dtpa}", fertilizer.Iron.FeDtpa);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{SubItemPrefix}{Eddha}", fertilizer.Iron.FeEddha);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{SubItemPrefix}{Hbed}", fertilizer.Iron.FeHbed);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{SubItemPrefix}{OrthoOrtho}", fertilizer.Iron.FeOrthoPart);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Copper, fertilizer.Copper.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{SubItemPrefix}{Edta}", fertilizer.Copper.CuEdta);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Manganese, fertilizer.Manganese.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{SubItemPrefix}{Edta}", fertilizer.Manganese.MnEdta);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Zinc, fertilizer.Zinc.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{SubItemPrefix}{Edta}", fertilizer.Zinc.ZnEdta);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Boron, fertilizer.Boron.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Molybdenum, fertilizer.Molybdenum.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Silicon, fertilizer.Silicon.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Selenium, fertilizer.Selenium.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Sodium, fertilizer.Sodium.Value);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Renders a single-line summary of the fertilizer's non-zero nutrients,
    /// for example <c>"N 11.86 | Ca 16.97"</c>.
    /// </summary>
    /// <param name="fertilizer">The fertilizer to summarize.</param>
    /// <returns>A pipe-separated summary, or an empty string when the fertilizer carries no nutrients.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="fertilizer"/> is null.</exception>
    public static string GetNutrientSummary(this Fertilizer fertilizer)
    {
        ArgumentNullException.ThrowIfNull(fertilizer);

        (string Symbol, double Value)[] nutrients =
        [
            (Names.N, fertilizer.Nitrogen.Value),
            (Names.P, fertilizer.Phosphorus.Value),
            (Names.K, fertilizer.Potassium.Value),
            (Names.Ca, fertilizer.Calcium.Value),
            (Names.Mg, fertilizer.Magnesium.Value),
            (Names.S, fertilizer.Sulfur.Value),
            (Names.Fe, fertilizer.Iron.Value),
            (Names.Cu, fertilizer.Copper.Value),
            (Names.Mn, fertilizer.Manganese.Value),
            (Names.Zn, fertilizer.Zinc.Value),
            (Names.B, fertilizer.Boron.Value),
            (Names.Mo, fertilizer.Molybdenum.Value),
            (Names.Cl, fertilizer.Chlorine.Value),
            (Names.Si, fertilizer.Silicon.Value),
            (Names.Se, fertilizer.Selenium.Value),
            (Names.Na, fertilizer.Sodium.Value)
        ];

        return string.Join(" | ", nutrients
            .Where(nutrient => nutrient.Value > 0)
            .Select(nutrient => string.Create(CultureInfo.InvariantCulture, $"{nutrient.Symbol} {nutrient.Value:N2}")));
    }
}