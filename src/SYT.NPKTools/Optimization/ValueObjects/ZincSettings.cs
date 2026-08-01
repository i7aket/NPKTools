namespace SYT.NPKTools.Optimization;

/// <summary>
/// Precision setting for zinc (Zn) during optimization. A value of 0 leaves the element unconstrained; values up to 1 tighten the allowed deviation.
/// </summary>
public record ZincSettings(double Value = 1) : SettingsFieldBase(Value);
