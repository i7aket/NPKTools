using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

public record SulfurPpmTarget(double Value) : ElementFieldBase(Value);