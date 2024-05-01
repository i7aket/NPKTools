using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

public record SiliconPpmTarget(double Value) : ElementFieldBase(Value);