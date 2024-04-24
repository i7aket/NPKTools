using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.PpmTarget.ValueObjects;

public record CalciumPpmTarget(double Value) : ElementFieldBase(Value);