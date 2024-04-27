namespace NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

public record RangeFactorSettings
{
    public double Value;
    public RangeFactorSettings (double value=1)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 1);
        Value = value;
    }
}
    
    
