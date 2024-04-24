using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.PpmTarget.ValueObjects;

public record NitrogenPpmTarget(double Value) : ElementFieldBase(Value);