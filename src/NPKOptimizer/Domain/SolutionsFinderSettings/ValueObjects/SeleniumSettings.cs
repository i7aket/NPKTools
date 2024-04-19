namespace NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

public record SeleniumSettings(double Accuracy = 1) : SettingsFieldBase(Accuracy);