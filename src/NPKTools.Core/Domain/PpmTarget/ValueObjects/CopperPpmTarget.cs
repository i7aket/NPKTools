using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

public record CopperPpmTarget(double Value) : ElementFieldBase(Value);