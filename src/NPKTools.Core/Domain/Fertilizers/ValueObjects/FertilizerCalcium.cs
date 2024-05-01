namespace NPKTools.Core.Domain.Fertilizers.ValueObjects;
/// <summary>
/// Represents the calcium content in the fertilizer, detailing both non-chelated and EDTA-chelated forms.
/// This class provides a total combined value of both forms of calcium for comprehensive nutrient analysis.
/// </summary>
public record FertilizerCalcium 
{
    public double CaNonChelated { get; set; }
    public double CaEdta { get; set; }
    public double Value => CaNonChelated + CaEdta;

    public FertilizerCalcium(double caNonChelated = 0, double caEdta = 0) 
    {
        ArgumentOutOfRangeException.ThrowIfNegative(caNonChelated);
        ArgumentOutOfRangeException.ThrowIfNegative(caEdta);

        CaNonChelated = caNonChelated;
        CaEdta = caEdta;
    }
}