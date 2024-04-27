using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;
/// <summary>
/// Represents the sulfur content in the fertilizer, expressed as a single value.
/// </summary>
public record FertilizerSulfur(double Value) : ElementFieldBase (Value);