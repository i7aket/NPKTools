using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

/// <summary>
/// Represents the measured concentration of iron (Fe) in parts per million.
/// </summary>
public record IronPpm(double Value) : ElementFieldBase(Value);
