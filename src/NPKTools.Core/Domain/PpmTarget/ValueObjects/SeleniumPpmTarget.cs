using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

/// <summary>
/// Represents the target concentration of selenium (Se) in parts per million.
/// </summary>
public record SeleniumPpmTarget(double Value) : ElementFieldBase(Value);