using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

/// <summary>
/// Represents the target concentration of nitrogen (N) in parts per million.
/// </summary>
public record NitrogenPpmTarget(double Value) : ElementFieldBase(Value);
