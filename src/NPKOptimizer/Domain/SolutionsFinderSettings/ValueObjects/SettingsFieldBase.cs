using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

public abstract record SettingsFieldBase
{
    public double Accuracy { get; }

    protected SettingsFieldBase(double accuracy = 1)
    {
        Validate.Positive(accuracy);
        Validate.NotGreaterThan(accuracy, 1);
        Accuracy = accuracy;
    }
}