using System.Text;
using NPKTools.Core.Common;
using static NPKTools.Core.Const.Labels;

namespace NPKTools.Core.Domain.Fertilizers.Extensions;

public static class FertilizerExtensions
{
    public static string Report(this Fertilizer fertilizer)
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine($"{Name}: {fertilizer.Name.Value}");
        stringBuilder.AppendLine($"{Formula}: {fertilizer.Formula.Value}");
        stringBuilder.AppendLine($"{ConcentrateType}: {fertilizer.Type}");
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

    public static string GetNutrientSummary(this Fertilizer fertilizer)
    {
        List<string> parts = new();

        if (fertilizer.Nitrogen.Value > 0) parts.Add($"N {fertilizer.Nitrogen.Value:N2}");
        if (fertilizer.Phosphorus.Value > 0) parts.Add($"P {fertilizer.Phosphorus.Value:N2}");
        if (fertilizer.Potassium.Value > 0) parts.Add($"K {fertilizer.Potassium.Value:N2}");
        if (fertilizer.Calcium.Value > 0) parts.Add($"Ca {fertilizer.Calcium.Value:N2}");
        if (fertilizer.Magnesium.Value > 0) parts.Add($"Mg {fertilizer.Magnesium.Value:N2}");
        if (fertilizer.Sulfur.Value > 0) parts.Add($"S {fertilizer.Sulfur.Value:N2}");
        if (fertilizer.Iron.Value > 0) parts.Add($"Fe {fertilizer.Iron.Value:N2}");
        if (fertilizer.Copper.Value > 0) parts.Add($"Cu {fertilizer.Copper.Value:N2}");
        if (fertilizer.Manganese.Value > 0) parts.Add($"Mn {fertilizer.Manganese.Value:N2}");
        if (fertilizer.Zinc.Value > 0) parts.Add($"Zn {fertilizer.Zinc.Value:N2}");
        if (fertilizer.Boron.Value > 0) parts.Add($"B {fertilizer.Boron.Value:N2}");
        if (fertilizer.Molybdenum.Value > 0) parts.Add($"Mo {fertilizer.Molybdenum.Value:N2}");
        if (fertilizer.Chlorine.Value > 0) parts.Add($"Cl {fertilizer.Chlorine.Value:N2}");
        if (fertilizer.Silicon.Value > 0) parts.Add($"Si {fertilizer.Silicon.Value:N2}");
        if (fertilizer.Selenium.Value > 0) parts.Add($"Se {fertilizer.Selenium.Value:N2}");
        if (fertilizer.Sodium.Value > 0) parts.Add($"Na {fertilizer.Sodium.Value:N2}");

        return string.Join(" | ", parts);
    }
}