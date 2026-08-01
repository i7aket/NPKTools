namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Represents the measured concentration of iron (Fe) in parts per million.
/// </summary>
public record IronPpm(double Value) : ElementFieldBase(Value);
