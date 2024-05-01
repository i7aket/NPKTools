using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

public record MagnesiumPpmTarget(double Value) : ElementFieldBase(Value);