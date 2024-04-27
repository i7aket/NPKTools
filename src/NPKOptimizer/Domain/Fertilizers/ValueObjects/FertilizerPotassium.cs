using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;
/// <summary>
/// Represents the potassium content in the fertilizer, expressed as a single value.
/// </summary>
public record FertilizerPotassium(double Value) : ElementFieldBase (Value);