namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Represents the target concentration of zinc (Zn) in parts per million.
/// </summary>
public record ZincPpmTarget(double Value) : ElementFieldBase(Value);
