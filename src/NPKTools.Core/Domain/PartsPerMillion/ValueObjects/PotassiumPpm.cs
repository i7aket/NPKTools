using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

/// <summary>
/// Represents the measured concentration of potassium (K) in parts per million.
/// </summary>
public record PotassiumPpm(double Value) : ElementFieldBase(Value);
