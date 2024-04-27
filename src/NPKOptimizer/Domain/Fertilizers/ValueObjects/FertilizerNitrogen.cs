namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;
/// <summary>
/// Represents the nitrogen content in the fertilizer, differentiated by nitrate, ammonium, and amine forms.
/// </summary>
public record FertilizerNitrogen 
{
    public double Nitrate { get; }
    public double Ammonium { get; }
    public double Amine { get; }
    public double Value => Nitrate + Ammonium + Amine;


    public FertilizerNitrogen(double nitrate = 0, double ammonium = 0, double amine = 0)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(nitrate);
        ArgumentOutOfRangeException.ThrowIfNegative(ammonium);
        ArgumentOutOfRangeException.ThrowIfNegative(amine);
        Nitrate = nitrate;
        Ammonium = ammonium;
        Amine = amine;
    }
}