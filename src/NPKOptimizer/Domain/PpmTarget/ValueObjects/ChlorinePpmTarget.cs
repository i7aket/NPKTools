using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.PpmTarget.ValueObjects;

public record ChlorinePpmTarget(double Value) : ElementFieldBase(Value);