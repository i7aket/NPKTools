namespace NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

public record ChlorineSettings(double Accuracy = 1) : SettingsFieldBase(Accuracy);