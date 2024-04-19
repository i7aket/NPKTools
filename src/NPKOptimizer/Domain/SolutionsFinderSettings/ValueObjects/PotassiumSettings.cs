namespace NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

public record PotassiumSettings(double Accuracy = 1) : SettingsFieldBase(Accuracy);