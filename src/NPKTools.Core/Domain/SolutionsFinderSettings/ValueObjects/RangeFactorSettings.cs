namespace NPKTools.Core.Domain.SolutionsFinderSettings.ValueObjects;

/// <summary>
/// A global precision floor applied alongside each element's own precision setting.
/// </summary>
/// <remarks>
/// <para>
/// The optimizer constrains an element to <c>target ± target × (1 − p)</c>, where
/// <c>p = min(rangeFactor, elementPrecision)</c>. Because the smaller of the two precisions wins, the
/// looser setting decides, and the allowed deviation is the wider of the two the settings imply.
/// </para>
/// <para>
/// Raising this factor therefore <b>tightens</b> the search rather than widening it: at
/// <c>1</c> — the default — every element with a precision of <c>1</c> becomes an exact equality.
/// Lowering it loosens every element at once. An element whose own precision is <c>0</c> is left
/// unconstrained regardless of this value.
/// </para>
/// </remarks>
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
