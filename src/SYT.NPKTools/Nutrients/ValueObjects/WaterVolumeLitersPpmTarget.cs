namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Represents the volume of water, in liters, the target concentrations apply to.
/// </summary>
public record WaterVolumeLitersPpmTarget
{
    public double Value { get; }
    public WaterVolumeLitersPpmTarget(double value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
        Value = value;
    }
}
