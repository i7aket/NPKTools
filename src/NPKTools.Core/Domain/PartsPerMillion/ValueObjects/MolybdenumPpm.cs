using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

/// <summary>
/// Represents the measured concentration of molybdenum (Mo) in parts per million.
/// </summary>
public record MolybdenumPpm(double Value) : ElementFieldBase(Value);