using NPKOptimizer.Common;
using NPKOptimizer.Domain.Fertilizers.ValueObjects;

namespace NPKOptimizer.Domain.Fertilizers;

public class FertilizerOptimizationModel : FertilizerAttributes
{
    public FertilizerReferenceId RefId { get; set; }
    public FertilizerOptimizationModel (){}
    public FertilizerOptimizationModel(FertilizerPrice price, FertilizerNitrogen nitrogen, FertilizerPhosphorus phosphorus,
        FertilizerPotassium potassium, FertilizerCalcium calcium, FertilizerMagnesium magnesium, FertilizerSulfur sulfur, FertilizerIron iron,
        FertilizerCopper copper, FertilizerManganese manganese, FertilizerZinc zinc, FertilizerBoron boron, FertilizerMolybdenum molybdenum,
        FertilizerChlorine chlorine, FertilizerSilicon silicon, FertilizerSelenium selenium, FertilizerSodium sodium, FertilizerReferenceId refId) 
        : base(price, nitrogen, phosphorus, potassium, calcium, magnesium, sulfur, iron, copper, manganese, zinc, boron, molybdenum, chlorine, silicon, selenium, sodium)
    {
        ArgumentNullException.ThrowIfNull(refId);
        ThrowIf.Default(refId.Value);
        RefId = refId;
    }
}