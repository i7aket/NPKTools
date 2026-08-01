namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Represents the target concentration of selenium (Se) in parts per million.
/// </summary>
public record SeleniumPpmTarget(double Value) : ElementFieldBase(Value);
