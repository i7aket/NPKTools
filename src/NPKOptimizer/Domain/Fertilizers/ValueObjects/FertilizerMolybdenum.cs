using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;
/// <summary>
/// Represents the molybdenum content in the fertilizer, expressed as a single value.
/// </summary>
public record FertilizerMolybdenum(double Value) : ElementFieldBase (Value);