namespace NPKTools.Core.Domain.SolutionsFinderSettings.ValueObjects;

/// <summary>
/// Precision setting for calcium (Ca) during optimization. A value of 0 leaves the element unconstrained; values up to 1 tighten the allowed deviation.
/// </summary>
public record CalciumSettings(double Value = 1) : SettingsFieldBase(Value);