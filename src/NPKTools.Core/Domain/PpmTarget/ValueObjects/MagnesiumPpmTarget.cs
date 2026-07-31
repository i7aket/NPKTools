using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

/// <summary>
/// Represents the target concentration of magnesium (Mg) in parts per million.
/// </summary>
public record MagnesiumPpmTarget(double Value) : ElementFieldBase(Value);
