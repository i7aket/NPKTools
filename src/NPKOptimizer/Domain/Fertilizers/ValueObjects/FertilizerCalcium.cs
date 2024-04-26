using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerCalcium : ElementFieldBase
{
    public double CaNonChelated { get; set; }
    public double CaEdta { get; set; }

    public FertilizerCalcium(double caNonChelated = 0, double caEdta = 0) : base(caNonChelated + caEdta)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(caNonChelated);
        ArgumentOutOfRangeException.ThrowIfNegative(caEdta);

        CaNonChelated = caNonChelated;
        CaEdta = caEdta;
    }
}