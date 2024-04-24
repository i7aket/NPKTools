using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerPhosphorus(double Value) : ElementFieldBase (Value);