using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

/// <summary>
/// Represents the target concentration of molybdenum (Mo) in parts per million.
/// </summary>
public record MolybdenumPpmTarget(double Value) : ElementFieldBase(Value);