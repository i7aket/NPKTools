namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Represents the target concentration of iron (Fe) in parts per million.
/// </summary>
public record IronPpmTarget(double Value) : ElementFieldBase(Value);
