using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

public record PotassiumPpmTarget(double Value) : ElementFieldBase(Value);