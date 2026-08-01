namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Represents the target concentration of sodium (Na) in parts per million.
/// </summary>
public record SodiumPpmTarget(double Value) : ElementFieldBase(Value);
