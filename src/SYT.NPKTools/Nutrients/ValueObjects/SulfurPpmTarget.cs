namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Represents the target concentration of sulfur (S) in parts per million.
/// </summary>
public record SulfurPpmTarget(double Value) : ElementFieldBase(Value);
