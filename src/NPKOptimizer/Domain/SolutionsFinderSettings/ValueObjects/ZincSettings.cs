namespace NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

public record ZincSettings(double Accuracy = 1) : SettingsFieldBase(Accuracy);