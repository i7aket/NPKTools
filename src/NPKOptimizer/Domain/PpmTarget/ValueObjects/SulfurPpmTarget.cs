using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.PpmTarget.ValueObjects;

public record SulfurPpmTarget(double Value) : ElementFieldBase(Value);