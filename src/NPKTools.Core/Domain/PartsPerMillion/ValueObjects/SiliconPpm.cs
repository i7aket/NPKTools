using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

/// <summary>
/// Represents the measured concentration of silicon (Si) in parts per million.
/// </summary>
public record SiliconPpm(double Value) : ElementFieldBase(Value);
