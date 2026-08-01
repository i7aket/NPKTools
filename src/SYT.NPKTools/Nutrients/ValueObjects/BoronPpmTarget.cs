namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Represents the target concentration of boron (B) in parts per million.
/// </summary>
public record BoronPpmTarget(double Value) : ElementFieldBase(Value);
