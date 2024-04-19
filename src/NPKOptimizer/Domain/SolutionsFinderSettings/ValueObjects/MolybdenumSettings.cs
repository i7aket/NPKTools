namespace NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

public record MolybdenumSettings(double Accuracy = 1) : SettingsFieldBase(Accuracy);