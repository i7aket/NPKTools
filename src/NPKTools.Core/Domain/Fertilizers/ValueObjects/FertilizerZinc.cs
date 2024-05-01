

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKTools.Core.Domain.Fertilizers.ValueObjects;
/// <summary>
/// Represents the zinc content in the fertilizer, differentiated by non-chelated and EDTA forms.
/// </summary>
public record FertilizerZinc 
{
    public double ZnNonChelated { get; }
    public double ZnEdta { get; }
    public double Value => ZnNonChelated + ZnEdta;

    public FertilizerZinc(double znNonChelated = 0, double znEdta = 0)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(znNonChelated);
        ZnNonChelated = znNonChelated;

        ArgumentOutOfRangeException.ThrowIfNegative(znEdta);
        ZnEdta = znEdta;
    }
}