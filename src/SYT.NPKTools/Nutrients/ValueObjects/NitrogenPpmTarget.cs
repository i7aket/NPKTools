namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Represents the target concentration of nitrogen (N) in parts per million.
/// </summary>
public record NitrogenPpmTarget(double Value) : ElementFieldBase(Value);
