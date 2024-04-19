namespace NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

public record CalciumSettings(double Accuracy = 1) : SettingsFieldBase(Accuracy);