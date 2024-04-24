using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerPrice (double Value = 1) : ElementFieldBase (Value);