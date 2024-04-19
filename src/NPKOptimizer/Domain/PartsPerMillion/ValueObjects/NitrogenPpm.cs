using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.PartsPerMillion.ValueObjects;

public record NitrogenPpm : FieldBase
{
    public double Nitrate { get; }
    public double Ammonium { get; }
    public double Amine { get; }

    public NitrogenPpm(double nitrate = 0, double ammonium = 0, double amine = 0) : base(nitrate + ammonium + amine)
    {
        Validate.Positive(nitrate);
        Validate.Positive(ammonium);
        Validate.Positive(amine);
        Nitrate = nitrate;
        Ammonium = ammonium;
        Amine = amine;
    }
}