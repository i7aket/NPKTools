namespace SYT.NPKTools.Optimization;

/// <summary>
/// Precision setting for sulfur (S) during optimization. A value of 0 leaves the element unconstrained; values up to 1 tighten the allowed deviation.
/// </summary>
public record SulfurSettings(double Value = 1) : SettingsFieldBase(Value);
