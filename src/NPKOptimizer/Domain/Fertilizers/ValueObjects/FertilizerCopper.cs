using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerCopper : ElementFieldBase
{
    public double CuNonChelated { get; }
    public double CuEdta { get; }

    public FertilizerCopper(double cuNonChelated = 0, double cuEdta = 0) : base(cuNonChelated + cuEdta)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(cuNonChelated);
        CuNonChelated = cuNonChelated;

        ArgumentOutOfRangeException.ThrowIfNegative(cuEdta);
        CuEdta = cuEdta;
    }
}