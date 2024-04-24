using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerManganese : ElementFieldBase
{
    public double MnNonChelated { get; }
    public double MnEdta { get; }

    public FertilizerManganese(double mnNonChelated = 0, double mnEdta = 0) : base(mnNonChelated + mnEdta)
    {
        ThrowIf.LowerThan(mnNonChelated,0);
        MnNonChelated = mnNonChelated;

        ThrowIf.LowerThan(mnEdta,0);
        MnEdta = mnEdta;
    }
}