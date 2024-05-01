using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

public record SodiumPpmTarget(double Value) : ElementFieldBase(Value);