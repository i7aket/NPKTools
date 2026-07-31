using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

/// <summary>
/// Represents the measured concentration of sulfur (S) in parts per million.
/// </summary>
public record SulfurPpm(double Value) : ElementFieldBase(Value);