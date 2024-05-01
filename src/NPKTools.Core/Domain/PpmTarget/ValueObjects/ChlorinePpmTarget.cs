using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

public record ChlorinePpmTarget(double Value) : ElementFieldBase(Value);