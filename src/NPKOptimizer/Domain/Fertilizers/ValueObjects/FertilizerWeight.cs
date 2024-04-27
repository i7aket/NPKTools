using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;
/// <summary>
/// Represents the physical weight of the fertilizer, usually measured in kilograms or pounds.
/// </summary>
public record FertilizerWeight (double Value = 0) : ElementFieldBase (Value);

    
    