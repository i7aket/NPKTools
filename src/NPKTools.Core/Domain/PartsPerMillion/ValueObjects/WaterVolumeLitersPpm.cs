namespace NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

public record WaterVolumeLitersPpm
{
    public double Value { get; }
    public WaterVolumeLitersPpm (double value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
        Value = value;
    }
}