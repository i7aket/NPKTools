namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Represents the target concentration of chlorine (Cl) in parts per million.
/// </summary>
public record ChlorinePpmTarget(double Value) : ElementFieldBase(Value);
