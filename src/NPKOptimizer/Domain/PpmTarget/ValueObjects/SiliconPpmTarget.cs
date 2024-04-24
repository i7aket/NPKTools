using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.PpmTarget.ValueObjects;

public record SiliconPpmTarget(double Value) : ElementFieldBase(Value);