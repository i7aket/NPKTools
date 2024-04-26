using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.PartsPerMillion.ValueObjects;

public record NitrogenPpm : ElementFieldBase
{
    public double Nitrate { get; }
    public double Ammonium { get; }
    public double Amine { get; }

    public NitrogenPpm(double nitrate = 0, double ammonium = 0, double amine = 0) : base(nitrate + ammonium + amine)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(nitrate);
        ArgumentOutOfRangeException.ThrowIfNegative(ammonium);
        ArgumentOutOfRangeException.ThrowIfNegative(amine);
        Nitrate = nitrate;
        Ammonium = ammonium;
        Amine = amine;
    }
}