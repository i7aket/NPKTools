namespace NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

public record NitrogenSettings(double Accuracy = 1) : SettingsFieldBase(Accuracy);