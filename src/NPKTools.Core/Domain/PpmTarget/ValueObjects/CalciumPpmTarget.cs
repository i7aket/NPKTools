using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

/// <summary>
/// Represents the target concentration of calcium (Ca) in parts per million.
/// </summary>
public record CalciumPpmTarget(double Value) : ElementFieldBase(Value);
