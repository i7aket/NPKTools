using System.Text;
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
        AppendLineIfNonZero(stringBuilder, Labels.Weight, Weight.Value);
        AppendLineIfNonZero(stringBuilder, Labels.Nitrogen, Nitrogen.Value);
        AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.NitrateNo3}", Nitrogen.Nitrate);
        AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.AmmoniumNh4}", Nitrogen.Ammonium);
        AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.AmineNh2}", Nitrogen.Amine);

        AppendLineIfNonZero(stringBuilder, Labels.Phosphorus, Phosphorus.Value);
        AppendLineIfNonZero(stringBuilder, Labels.Potassium, Potassium.Value);

        AppendLineIfNonZero(stringBuilder, Labels.Calcium, Calcium.Value);
        AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.Edta}", Calcium.CaEdta);

        AppendLineIfNonZero(stringBuilder, Labels.Magnesium, Magnesium.Value);
        AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.Edta}", Magnesium.MgEdta);

        AppendLineIfNonZero(stringBuilder, Labels.Sulfur, Sulfur.Value);
        AppendLineIfNonZero(stringBuilder, Labels.Chlorine, Chlorine.Value);

        AppendLineIfNonZero(stringBuilder, Labels.Iron, Iron.Value);
        AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.Edta}", Iron.FeEdta);
        AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.Dtpa}", Iron.FeDtpa);
        AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.Eddha}", Iron.FeEddha);
        AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.Hbed}", Iron.FeHbed);
        AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.OrthoOrtho}", Iron.FeOrthoPart);

        AppendLineIfNonZero(stringBuilder, Labels.Copper, Copper.Value);
        AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.Edta}", Copper.CuEdta);

        AppendLineIfNonZero(stringBuilder, Labels.Manganese, Manganese.Value);
        AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.Edta}", Manganese.MnEdta);

        AppendLineIfNonZero(stringBuilder, Labels.Zinc, Zinc.Value);
        AppendLineIfNonZero(stringBuilder, $"{Labels.SubItemPrefix}{Labels.Edta}", Zinc.ZnEdta);

        AppendLineIfNonZero(stringBuilder, Labels.Boron, Boron.Value);
        AppendLineIfNonZero(stringBuilder, Labels.Molybdenum, Molybdenum.Value);
        AppendLineIfNonZero(stringBuilder, Labels.Silicon, Silicon.Value);
        AppendLineIfNonZero(stringBuilder, Labels.Selenium, Selenium.Value);
        AppendLineIfNonZero(stringBuilder, Labels.Sodium, Sodium.Value);
        return stringBuilder.ToString();
    }

    private static void AppendLineIfNonZero(StringBuilder stringBuilder, string label, double value)
    {
        if (value > 0)
        {
            stringBuilder.AppendLine($"{label}: {Math.Round(value, 3)}");
        }
    }
}