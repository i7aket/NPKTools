using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

public record SeleniumPpmTarget(double Value) : ElementFieldBase(Value);