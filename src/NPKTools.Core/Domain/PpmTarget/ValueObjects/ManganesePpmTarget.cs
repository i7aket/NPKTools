using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

/// <summary>
/// Represents the target concentration of manganese (Mn) in parts per million.
/// </summary>
public record ManganesePpmTarget(double Value) : ElementFieldBase(Value);