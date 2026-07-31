using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

/// <summary>
/// Represents the measured concentration of calcium (Ca) in parts per million.
/// </summary>
public record CalciumPpm(double Value) : ElementFieldBase(Value);