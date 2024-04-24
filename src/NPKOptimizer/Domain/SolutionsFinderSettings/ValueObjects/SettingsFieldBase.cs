using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

/// <summary>
/// Base class for settings fields that require a precision setting.
/// </summary>
/// <remarks>
/// The value parameter is used to set the precision of calculations:
/// - A value of 0 sets the range to be infinite.
/// - A value greater than 0 and up to 1 is interpreted as the allowable precision percentage.
///   For example, a value of 0.9 allows for a deviation of ±10% from the target value.
/// </remarks>
public abstract record SettingsFieldBase
{
    /// <summary>
    /// Gets the precision value for the settings field.
    /// </summary>
    public double Value { get; }

    /// <summary>
    /// Initializes a new instance of the SettingsFieldBase with a specified precision value.
    /// </summary>
    /// <param name="value">The precision value, ranging from 0 to 1.
    /// A value of 0 indicates infinite precision, while values greater than 0 specify the allowable precision percentage.</param>
    protected SettingsFieldBase(double value = 1)
    {
        ThrowIf.NotInRange(value, 0, 1);
        Value = value;
    }
}