using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

/// <summary>
/// Represents the measured concentration of phosphorus (P) in parts per million.
/// </summary>
public record PhosphorusPpm(double Value) : ElementFieldBase(Value);