using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.PpmTarget.ValueObjects;

public record WaterVolumeLitersPpm
{
    public double Value { get; }
    public WaterVolumeLitersPpm (double value)
    {
        ThrowIf.LowerThanOrEqual(value, 0);
        Value = value;
    }
};