using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

/// <summary>
/// Represents the measured concentration of copper (Cu) in parts per million.
/// </summary>
public record CopperPpm(double Value) : ElementFieldBase(Value);