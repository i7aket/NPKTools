using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerZinc : FieldBase
{
    public double ZnNonChelated { get; }
    public double ZnEdta { get; }

    public FertilizerZinc(double znNonChelated = 0, double znEdta = 0) : base(znNonChelated + znEdta)
    {
        Validate.Positive(znNonChelated);
        ZnNonChelated = znNonChelated;

        Validate.Positive(znEdta);
        ZnEdta = znEdta;
    }
}