using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

/// <summary>
/// Represents the target concentration of potassium (K) in parts per million.
/// </summary>
public record PotassiumPpmTarget(double Value) : ElementFieldBase(Value);
