namespace NPKTools.Core.Domain.PpmTarget.ValueObjects;

public record WaterVolumeLitersPpmTarget
{
    public double Value { get; }
    public WaterVolumeLitersPpmTarget (double value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
        Value = value;
    }
};