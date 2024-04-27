using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;
/// <summary>
/// Represents the chlorine content in the fertilizer, expressed as a single value.
/// </summary>
public record FertilizerChlorine(double Value) : ElementFieldBase (Value);