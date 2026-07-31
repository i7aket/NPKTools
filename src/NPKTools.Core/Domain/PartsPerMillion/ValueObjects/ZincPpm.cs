using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

/// <summary>
/// Represents the measured concentration of zinc (Zn) in parts per million.
/// </summary>
public record ZincPpm(double Value) : ElementFieldBase(Value);