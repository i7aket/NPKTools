namespace NPKOptimizer.Common;

/// <summary>
/// Serves as an abstract base record for defining nutrient elements with non-negative values.
/// This record ensures that all derived nutrient elements cannot have values below zero,
/// which aligns with the domain requirement that nutrient measurements must be positive or zero.
/// </summary>
public abstract record ElementFieldBase
{
    /// <summary>
    /// Gets the value of the nutrient element. This value is immutable and cannot be negative.
    /// </summary>
    public double Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElementFieldBase"/> record with a specified value.
    /// </summary>
    /// <param name="value">The initial value for the nutrient element, which must not be negative.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="value"/> is less than zero.</exception>
    protected ElementFieldBase(double value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        Value = value;
    }
}