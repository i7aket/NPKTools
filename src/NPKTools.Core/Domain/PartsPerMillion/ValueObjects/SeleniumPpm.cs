using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

/// <summary>
/// Represents the measured concentration of selenium (Se) in parts per million.
/// </summary>
public record SeleniumPpm(double Value) : ElementFieldBase(Value);
