namespace NPKTools.Core.Domain.SolutionsFinderSettings.ValueObjects;

/// <summary>
/// Caps how far the optimizer may deviate from a nutrient target.
/// The effective tolerance for an element is the smaller of this factor and that element's own precision setting.
/// </summary>
public record RangeFactorSettings
{
    /// <summary>
    /// Gets the range factor, in the range (0, 1].
    /// </summary>
    public double Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeFactorSettings"/> record.
    /// </summary>
    /// <param name="value">The range factor. Must be greater than zero and at most one.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is outside (0, 1].</exception>
    public RangeFactorSettings(double value = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 1);
        Value = value;
    }
}
