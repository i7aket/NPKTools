using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

public record PhosphorusPpmTarget(double Value) : ElementFieldBase(Value);