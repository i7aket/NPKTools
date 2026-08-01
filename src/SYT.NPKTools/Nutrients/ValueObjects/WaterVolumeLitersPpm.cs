namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Represents the volume of water, in liters, the measured concentrations apply to.
/// </summary>
public record WaterVolumeLitersPpm
{
    public double Value { get; }
    public WaterVolumeLitersPpm(double value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
        Value = value;
    }
}
