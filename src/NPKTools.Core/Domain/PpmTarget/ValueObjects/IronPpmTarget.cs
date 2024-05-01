using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

public record IronPpmTarget(double Value) : ElementFieldBase(Value);