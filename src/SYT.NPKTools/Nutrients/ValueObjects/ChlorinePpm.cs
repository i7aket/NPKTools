namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Represents the measured concentration of chlorine (Cl) in parts per million.
/// </summary>
public record ChlorinePpm(double Value) : ElementFieldBase(Value);
