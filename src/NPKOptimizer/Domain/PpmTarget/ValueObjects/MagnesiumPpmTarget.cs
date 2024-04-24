using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.PpmTarget.ValueObjects;

public record MagnesiumPpmTarget(double Value) : ElementFieldBase(Value);