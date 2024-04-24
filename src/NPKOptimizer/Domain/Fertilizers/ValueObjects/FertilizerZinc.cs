using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerZinc : ElementFieldBase
{
    public double ZnNonChelated { get; }
    public double ZnEdta { get; }

    public FertilizerZinc(double znNonChelated = 0, double znEdta = 0) : base(znNonChelated + znEdta)
    {
        ThrowIf.LowerThan(znNonChelated,0);
        ZnNonChelated = znNonChelated;

        ThrowIf.LowerThan(znEdta,0);
        ZnEdta = znEdta;
    }
}