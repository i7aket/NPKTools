namespace NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

public record MagnesiumSettings(double Accuracy = 1) : SettingsFieldBase(Accuracy);