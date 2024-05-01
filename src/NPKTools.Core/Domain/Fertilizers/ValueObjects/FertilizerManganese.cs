namespace NPKTools.Core.Domain.Fertilizers.ValueObjects;
/// <summary>
/// Represents the manganese content in the fertilizer, differentiated by non-chelated and EDTA forms.
/// </summary>
public record FertilizerManganese 
{
    public double MnNonChelated { get; }
    public double MnEdta { get; }
    public double Value => MnNonChelated + MnEdta;
    
    public FertilizerManganese(double mnNonChelated = 0, double mnEdta = 0) 
    {
        ArgumentOutOfRangeException.ThrowIfNegative(mnNonChelated);
        MnNonChelated = mnNonChelated;

        ArgumentOutOfRangeException.ThrowIfNegative(mnEdta);
        MnEdta = mnEdta;
    }
}