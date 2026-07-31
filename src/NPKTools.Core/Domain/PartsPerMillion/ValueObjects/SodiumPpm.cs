using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

/// <summary>
/// Represents the measured concentration of sodium (Na) in parts per million.
/// </summary>
public record SodiumPpm(double Value) : ElementFieldBase(Value);
