using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

/// <summary>
/// Represents the measured concentration of boron (B) in parts per million.
/// </summary>
public record BoronPpm(double Value) : ElementFieldBase(Value);
