namespace NPKOptimizer.Domain.PartsPerMillion;

public record WaterVolumeLitersPpm
{
    public double Value { get; }
    public WaterVolumeLitersPpm (double value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
        Value = value;
    }
};