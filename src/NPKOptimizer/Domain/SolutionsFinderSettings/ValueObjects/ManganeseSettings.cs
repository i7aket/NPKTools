namespace NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

public record ManganeseSettings(double Accuracy = 1) : SettingsFieldBase(Accuracy);