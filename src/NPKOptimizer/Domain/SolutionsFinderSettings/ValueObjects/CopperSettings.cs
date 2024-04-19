namespace NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

public record CopperSettings(double Accuracy = 1) : SettingsFieldBase(Accuracy);