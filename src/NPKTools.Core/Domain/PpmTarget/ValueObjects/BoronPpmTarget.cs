using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

public record BoronPpmTarget(double Value) : ElementFieldBase(Value);