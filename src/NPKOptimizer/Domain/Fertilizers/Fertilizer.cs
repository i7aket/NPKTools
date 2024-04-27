using NPKOptimizer.Domain.Fertilizers.ValueObjects;

namespace NPKOptimizer.Domain.Fertilizers;
/// <summary>
/// Represents the final configured state of a fertilizer after optimization, including a set weight.
/// This class extends the FertilizerOptimizationModel to include the physical weight of the fertilizer.
/// The weight is typically determinedas part of the optimization process to ensure the correct dosage
/// and ratio of nutrients per unit weight.
/// </summary>
public class Fertilizer : FertilizerOptimizationModel
{
    /// <summary>
    /// The weight of the fertilizer, typically measured in kilograms.
    /// </summary>
    public FertilizerWeight Weight { get; set; }
    public Fertilizer(){}
    public Fertilizer(FertilizerReferenceId refId, FertilizerWeight weight, FertilizerPrice price, FertilizerNitrogen nitrogen, FertilizerPhosphorus phosphorus,
        FertilizerPotassium potassium, FertilizerCalcium calcium, FertilizerMagnesium magnesium, FertilizerSulfur sulfur, FertilizerIron iron,
        FertilizerCopper copper, FertilizerManganese manganese, FertilizerZinc zinc, FertilizerBoron boron, FertilizerMolybdenum molybdenum,
        FertilizerChlorine chlorine, FertilizerSilicon silicon, FertilizerSelenium selenium, FertilizerSodium sodium) 
        : base(price, nitrogen, phosphorus, potassium, calcium, magnesium, sulfur, iron, copper, manganese, zinc, boron, molybdenum, chlorine, silicon, selenium, sodium, refId)
    {
        ArgumentNullException.ThrowIfNull(weight);
        Weight = weight;
    }
    
}