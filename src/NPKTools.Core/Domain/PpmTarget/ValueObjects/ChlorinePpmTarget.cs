using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

/// <summary>
/// Represents the target concentration of chlorine (Cl) in parts per million.
/// </summary>
public record ChlorinePpmTarget(double Value) : ElementFieldBase(Value);