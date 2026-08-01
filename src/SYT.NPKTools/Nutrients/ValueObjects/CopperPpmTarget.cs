namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Represents the target concentration of copper (Cu) in parts per million.
/// </summary>
public record CopperPpmTarget(double Value) : ElementFieldBase(Value);
