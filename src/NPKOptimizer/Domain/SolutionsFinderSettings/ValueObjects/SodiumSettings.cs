namespace NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

public record SodiumSettings(double Accuracy = 1) : SettingsFieldBase(Accuracy);