using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerCalcium : FieldBase
{
    public double CaNonChelated { get; }
    public double CaEdta { get; }

    public FertilizerCalcium(double caNonChelated = 0, double caEdta = 0) : base(caNonChelated + caEdta)
    {
        Validate.Positive(caNonChelated);
        Validate.Positive(caEdta);

        CaNonChelated = caNonChelated;
        CaEdta = caEdta;
    }
}