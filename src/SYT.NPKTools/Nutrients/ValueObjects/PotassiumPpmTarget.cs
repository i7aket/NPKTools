namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Represents the target concentration of potassium (K) in parts per million.
/// </summary>
public record PotassiumPpmTarget(double Value) : ElementFieldBase(Value);
