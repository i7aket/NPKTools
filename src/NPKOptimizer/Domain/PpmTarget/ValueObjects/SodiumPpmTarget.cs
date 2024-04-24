using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.PpmTarget.ValueObjects;

public record SodiumPpmTarget(double Value) : ElementFieldBase(Value);