namespace NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

public record IronSettings(double Accuracy = 1) : SettingsFieldBase(Accuracy);