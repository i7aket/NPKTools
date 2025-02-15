using System.Text;
using NPKTools.Core.Common;
using NPKTools.Core.Const;
using NPKTools.Core.Domain.Fertilizers.Enums;
using NPKTools.Core.Domain.Fertilizers.ValueObjects;
using static NPKTools.Core.Const.Labels;

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
}