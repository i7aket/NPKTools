using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;
/// <summary>
/// Represents the copper content in the fertilizer, detailing both non-chelated and EDTA-chelated forms.
/// This class calculates the total copper content by summing both forms, providing a comprehensive view of available copper nutrients.
/// </summary>
public record FertilizerCopper 
{
    public double CuNonChelated { get; }
    public double CuEdta { get; }
    public double Value => CuNonChelated + CuEdta;


    public FertilizerCopper(double cuNonChelated = 0, double cuEdta = 0) 
    {
        ArgumentOutOfRangeException.ThrowIfNegative(cuNonChelated);
        CuNonChelated = cuNonChelated;

        ArgumentOutOfRangeException.ThrowIfNegative(cuEdta);
        CuEdta = cuEdta;
    }
}