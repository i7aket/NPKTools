using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.PpmTarget.ValueObjects;

public record CopperPpmTarget(double Value) : ElementFieldBase(Value);