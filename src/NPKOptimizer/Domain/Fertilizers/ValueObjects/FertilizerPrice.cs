namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;
/// <summary>
/// Represents the monetary price of the fertilizer. The price must be a positive value.
/// </summary>
public record FertilizerPrice
{
    public double Value;

    public FertilizerPrice(double value = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
        Value = value;
    }
}