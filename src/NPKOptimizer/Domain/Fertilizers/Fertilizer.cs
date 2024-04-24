using NPKOptimizer.Common;
using NPKOptimizer.Domain.Fertilizers.ValueObjects;

namespace NPKOptimizer.Domain.Fertilizers;

public class Fertilizer : FertilizerOptimizationModel
{
    public FertilizerWeight Weight { get; set; }
    public Fertilizer(){}
    public Fertilizer(FertilizerReferenceId refId, FertilizerWeight weight, FertilizerPrice price, FertilizerNitrogen nitrogen, FertilizerPhosphorus phosphorus,
        FertilizerPotassium potassium, FertilizerCalcium calcium, FertilizerMagnesium magnesium, FertilizerSulfur sulfur, FertilizerIron iron,
        FertilizerCopper copper, FertilizerManganese manganese, FertilizerZinc zinc, FertilizerBoron boron, FertilizerMolybdenum molybdenum,
        FertilizerChlorine chlorine, FertilizerSilicon silicon, FertilizerSelenium selenium, FertilizerSodium sodium) 
        : base(price, nitrogen, phosphorus, potassium, calcium, magnesium, sulfur, iron, copper, manganese, zinc, boron, molybdenum, chlorine, silicon, selenium, sodium, refId)
    {
        ArgumentNullException.ThrowIfNull(weight);
        ThrowIf.LowerThan(weight.Value, 0);
        Weight = weight;
    }
    
}