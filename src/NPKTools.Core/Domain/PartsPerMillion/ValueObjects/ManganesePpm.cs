using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

/// <summary>
/// Represents the measured concentration of manganese (Mn) in parts per million.
/// </summary>
public record ManganesePpm(double Value) : ElementFieldBase(Value);
