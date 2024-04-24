using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerCopper : ElementFieldBase
{
    public double CuNonChelated { get; }
    public double CuEdta { get; }

    public FertilizerCopper(double cuNonChelated = 0, double cuEdta = 0) : base(cuNonChelated + cuEdta)
    {
        ThrowIf.LowerThan(cuNonChelated,0);
        CuNonChelated = cuNonChelated;

        ThrowIf.LowerThan(cuEdta,0);
        CuEdta = cuEdta;
    }
}