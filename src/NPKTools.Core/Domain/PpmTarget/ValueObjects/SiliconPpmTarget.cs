using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

/// <summary>
/// Represents the target concentration of silicon (Si) in parts per million.
/// </summary>
public record SiliconPpmTarget(double Value) : ElementFieldBase(Value);
