using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

/// <summary>
/// Represents the target concentration of phosphorus (P) in parts per million.
/// </summary>
public record PhosphorusPpmTarget(double Value) : ElementFieldBase(Value);
