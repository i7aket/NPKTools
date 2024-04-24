using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.PpmTarget.ValueObjects;

public record PotassiumPpmTarget(double Value) : ElementFieldBase(Value);