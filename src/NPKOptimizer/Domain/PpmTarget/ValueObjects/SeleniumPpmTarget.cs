using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.PpmTarget.ValueObjects;

public record SeleniumPpmTarget(double Value) : ElementFieldBase(Value);