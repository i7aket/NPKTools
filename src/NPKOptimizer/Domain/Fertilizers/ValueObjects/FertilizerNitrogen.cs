using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerNitrogen : FieldBase
{
    public double Nitrate { get; }
    public double Ammonium { get; }
    public double Amine { get; }

    public FertilizerNitrogen(double nitrate = 0, double ammonium = 0, double amine = 0) : base(nitrate + ammonium + amine)
    {
        Validate.Positive(nitrate);
        Validate.Positive(ammonium);
        Validate.Positive(amine);
        Nitrate = nitrate;
        Ammonium = ammonium;
        Amine = amine;
    }
}