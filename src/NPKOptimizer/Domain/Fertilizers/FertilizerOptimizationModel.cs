using NPKOptimizer.Common;
using NPKOptimizer.Domain.Fertilizers.ValueObjects;

namespace NPKOptimizer.Domain.Fertilizers;
/// <summary>
/// Extends the base FertilizerAttributes class to include a unique identifier, facilitating the optimization process.
/// This model serves as a comprehensive representation of a fertilizer, incorporating all essential nutrient attributes.
/// The addition of a FertilizerReferenceId allows for efficient tracking and comparison of input and output fertilizer data,
/// ensuring accurate matching and adjustments based on specified weights and other criteria during the optimization process.
/// </summary>
public class FertilizerOptimizationModel : FertilizerAttributes
{
    /// <summary>
    /// Represents a unique identifier for referencing a specific fertilizer entity.
    /// This ID is essential for linking the incoming fertilizer data to the corresponding outgoing fertilizer,
    /// ensuring that each fertilizer input is accurately associated with its relevant output processes.
    /// </summary>
    public FertilizerReferenceId RefId { get; set; }
    public FertilizerOptimizationModel (){}
    public FertilizerOptimizationModel(FertilizerPrice price, FertilizerNitrogen nitrogen, FertilizerPhosphorus phosphorus,
        FertilizerPotassium potassium, FertilizerCalcium calcium, FertilizerMagnesium magnesium, FertilizerSulfur sulfur, FertilizerIron iron,
        FertilizerCopper copper, FertilizerManganese manganese, FertilizerZinc zinc, FertilizerBoron boron, FertilizerMolybdenum molybdenum,
        FertilizerChlorine chlorine, FertilizerSilicon silicon, FertilizerSelenium selenium, FertilizerSodium sodium, FertilizerReferenceId refId) 
        : base(price, nitrogen, phosphorus, potassium, calcium, magnesium, sulfur, iron, copper, manganese, zinc, boron, molybdenum, chlorine, silicon, selenium, sodium)
    {
        ArgumentNullException.ThrowIfNull(refId);
        RefId = refId;
    }
}