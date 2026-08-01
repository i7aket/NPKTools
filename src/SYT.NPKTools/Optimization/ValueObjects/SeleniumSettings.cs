namespace SYT.NPKTools.Optimization;

/// <summary>
/// Precision setting for selenium (Se) during optimization. A value of 0 leaves the element unconstrained; values up to 1 tighten the allowed deviation.
/// </summary>
public record SeleniumSettings(double Value = 1) : SettingsFieldBase(Value);
