using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

public record ZincPpmTarget(double Value) : ElementFieldBase(Value);