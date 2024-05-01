using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

public record CalciumPpmTarget(double Value) : ElementFieldBase(Value);