using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerCopper : FieldBase
{
    public double CuNonChelated { get; }
    public double CuEdta { get; }

    public FertilizerCopper(double cuNonChelated = 0, double cuEdta = 0) : base(cuNonChelated + cuEdta)
    {
        Validate.Positive(cuNonChelated);
        CuNonChelated = cuNonChelated;

        Validate.Positive(cuEdta);
        CuEdta = cuEdta;
    }
}