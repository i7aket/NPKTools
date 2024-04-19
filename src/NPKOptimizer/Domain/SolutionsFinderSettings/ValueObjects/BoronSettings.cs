namespace NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

public record BoronSettings(double Accuracy = 1) : SettingsFieldBase(Accuracy);