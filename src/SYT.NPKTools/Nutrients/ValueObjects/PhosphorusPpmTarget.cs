namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Represents the target concentration of phosphorus (P) in parts per million.
/// </summary>
public record PhosphorusPpmTarget(double Value) : ElementFieldBase(Value);
