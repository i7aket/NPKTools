using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

/// <summary>
/// Represents the measured concentration of magnesium (Mg) in parts per million.
/// </summary>
public record MagnesiumPpm(double Value) : ElementFieldBase(Value);