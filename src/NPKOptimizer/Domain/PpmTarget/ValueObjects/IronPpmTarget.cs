using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.PpmTarget.ValueObjects;

public record IronPpmTarget(double Value) : ElementFieldBase(Value);