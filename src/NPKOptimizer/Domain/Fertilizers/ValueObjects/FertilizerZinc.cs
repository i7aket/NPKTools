using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerZinc : ElementFieldBase
{
    public double ZnNonChelated { get; }
    public double ZnEdta { get; }

    public FertilizerZinc(double znNonChelated = 0, double znEdta = 0) : base(znNonChelated + znEdta)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(znNonChelated);
        ZnNonChelated = znNonChelated;

        ArgumentOutOfRangeException.ThrowIfNegative(znEdta);
        ZnEdta = znEdta;
    }
}