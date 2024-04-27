using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;
/// <summary>
/// Represents the boron content in the fertilizer, expressed as a single value.
/// </summary>
public record FertilizerBoron(double Value) : ElementFieldBase (Value);