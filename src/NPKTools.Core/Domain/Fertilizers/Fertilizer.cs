using System.Text;
using NPKTools.Core.Common;
using NPKTools.Core.Const;
using NPKTools.Core.Domain.Fertilizers.Enums;
using NPKTools.Core.Domain.Fertilizers.ValueObjects;

namespace NPKTools.Core.Domain.Fertilizers;

public class Fertilizer : FertilizerAttributes

{
    public FertilizerReferenceId RefId { get; set; }
    public FertilizerWeight Weight { get; set; }
    public FertilizerName Name { get; set; }
    public FertilizerFormula Formula { get; set; }
    public ConcentrateType Type { get; set; }

    public Fertilizer()
    {
    }

    public Fertilizer(FertilizerName name, FertilizerFormula formula, ConcentrateType type, FertilizerReferenceId refId,
        FertilizerWeight weight, FertilizerPrice price, FertilizerNitrogen nitrogen, FertilizerPhosphorus phosphorus,
        FertilizerPotassium potassium, FertilizerCalcium calcium, FertilizerMagnesium magnesium,
        FertilizerSulfur sulfur, FertilizerIron iron,
        FertilizerCopper copper, FertilizerManganese manganese, FertilizerZinc zinc, FertilizerBoron boron,
        FertilizerMolybdenum molybdenum,
        FertilizerChlorine chlorine, FertilizerSilicon silicon, FertilizerSelenium selenium, FertilizerSodium sodium)
        : base(price, nitrogen, phosphorus, potassium, calcium, magnesium, sulfur, iron, copper, manganese, zinc, boron,
            molybdenum, chlorine, silicon, selenium, sodium)
    {
        ArgumentNullException.ThrowIfNull(name);
        Name = name;
        ArgumentNullException.ThrowIfNull(formula);
        Formula = formula;
        ArgumentNullException.ThrowIfNull(type);
        Type = type;
        ArgumentNullException.ThrowIfNull(refId);
        RefId = refId;
        ArgumentNullException.ThrowIfNull(weight);
        Weight = weight;
    }

    public Fertilizer With(FertilizerWeight newWeight)
    {
        return new Fertilizer(Name, Formula, Type, RefId, newWeight, Price, Nitrogen, Phosphorus, Potassium, Calcium,
            Magnesium, Sulfur, Iron, Copper, Manganese, Zinc, Boron, Molybdenum, Chlorine, Silicon, Selenium, Sodium);
    }
    
    public string Report()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine($"{Labels.Name}: {Name.Value}");
        stringBuilder.AppendLine($"{Labels.Formula}: {Formula.Value}");
        stringBuilder.AppendLine($"{Labels.ConcentrateType}: {Type}");
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Labels.Weight, Weight.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Labels.Nitrogen, Nitrogen.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.NitrateNo3}", Nitrogen.Nitrate);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.AmmoniumNh4}", Nitrogen.Ammonium);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.AmineNh2}", Nitrogen.Amine);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Labels.Phosphorus, Phosphorus.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Labels.Potassium, Potassium.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Labels.Calcium, Calcium.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.Edta}", Calcium.CaEdta);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Labels.Magnesium, Magnesium.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.Edta}", Magnesium.MgEdta);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Labels.Sulfur, Sulfur.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Labels.Chlorine, Chlorine.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Labels.Iron, Iron.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.Edta}", Iron.FeEdta);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.Dtpa}", Iron.FeDtpa);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.Eddha}", Iron.FeEddha);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.Hbed}", Iron.FeHbed);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.OrthoOrtho}", Iron.FeOrthoPart);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Labels.Copper, Copper.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.Edta}", Copper.CuEdta);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Labels.Manganese, Manganese.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.Edta}", Manganese.MnEdta);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Labels.Zinc, Zinc.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.Edta}", Zinc.ZnEdta);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Labels.Boron, Boron.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Labels.Molybdenum, Molybdenum.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Labels.Silicon, Silicon.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Labels.Selenium, Selenium.Value);
        ReportFormatter.AppendLineIfNonZero(stringBuilder, Labels.Sodium, Sodium.Value);
        return stringBuilder.ToString();
    }
}