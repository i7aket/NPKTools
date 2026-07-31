using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

/// <summary>
/// Represents the target concentration of copper (Cu) in parts per million.
/// </summary>
public record CopperPpmTarget(double Value) : ElementFieldBase(Value);
