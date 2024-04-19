namespace NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

public record SiliconSettings(double Accuracy = 1) : SettingsFieldBase(Accuracy);