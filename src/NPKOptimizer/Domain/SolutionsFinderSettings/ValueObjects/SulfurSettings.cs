namespace NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

public record SulfurSettings(double Accuracy = 1) : SettingsFieldBase(Accuracy);