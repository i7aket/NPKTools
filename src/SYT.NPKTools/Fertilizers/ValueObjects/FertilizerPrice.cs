namespace SYT.NPKTools.Fertilizers;

/// <summary>
/// Represents the monetary price of the fertilizer. The price must be a positive value.
/// </summary>
public record FertilizerPrice
{
    /// <summary>
    /// Gets the price of the fertilizer. Always greater than zero.
    /// </summary>
    public double Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FertilizerPrice"/> record.
    /// </summary>
    /// <param name="value">The price of the fertilizer. Must be greater than zero.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is zero or negative.</exception>
    public FertilizerPrice(double value = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
        Value = value;
    }
}
