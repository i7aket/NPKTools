using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.PpmTarget.ValueObjects;

public record PhosphorusPpmTarget(double Value) : ElementFieldBase(Value);