namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;
/// <summary>
/// Represents the magnesium content in the fertilizer, differentiated by non-chelated and EDTA forms.
/// </summary>
public record FertilizerMagnesium
{
    public double MgNonChelated { get; }
    public double MgEdta { get; }
    public double Value => MgNonChelated + MgEdta;


    public FertilizerMagnesium(double mgNonChelated = 0, double mgEdta = 0) 
    {
        ArgumentOutOfRangeException.ThrowIfNegative(mgNonChelated);
        ArgumentOutOfRangeException.ThrowIfNegative(mgEdta);

        MgNonChelated = mgNonChelated;
        MgEdta = mgEdta;
    }
}