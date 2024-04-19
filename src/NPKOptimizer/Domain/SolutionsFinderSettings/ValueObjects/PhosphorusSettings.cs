namespace NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

public record PhosphorusSettings(double Accuracy = 1) : SettingsFieldBase(Accuracy);