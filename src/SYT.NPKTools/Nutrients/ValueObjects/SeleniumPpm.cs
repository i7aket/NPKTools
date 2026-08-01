namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Represents the measured concentration of selenium (Se) in parts per million.
/// </summary>
public record SeleniumPpm(double Value) : ElementFieldBase(Value);
