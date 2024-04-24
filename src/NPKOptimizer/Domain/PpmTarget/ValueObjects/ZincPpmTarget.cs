using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.PpmTarget.ValueObjects;

public record ZincPpmTarget(double Value) : ElementFieldBase(Value);