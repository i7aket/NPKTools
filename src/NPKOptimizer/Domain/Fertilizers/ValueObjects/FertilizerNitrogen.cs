using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerNitrogen : ElementFieldBase
{
    public double Nitrate { get; }
    public double Ammonium { get; }
    public double Amine { get; }

    public FertilizerNitrogen(double nitrate = 0, double ammonium = 0, double amine = 0) : base(nitrate + ammonium + amine)
    {
        ThrowIf.LowerThan(nitrate,0);
        ThrowIf.LowerThan(ammonium,0);
        ThrowIf.LowerThan(amine,0);
        Nitrate = nitrate;
        Ammonium = ammonium;
        Amine = amine;
    }
}