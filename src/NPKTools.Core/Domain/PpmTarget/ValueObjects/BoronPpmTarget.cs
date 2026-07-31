using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

/// <summary>
/// Represents the target concentration of boron (B) in parts per million.
/// </summary>
public record BoronPpmTarget(double Value) : ElementFieldBase(Value);
