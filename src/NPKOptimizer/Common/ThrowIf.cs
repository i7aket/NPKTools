using System.Runtime.CompilerServices;

namespace NPKOptimizer.Common;

public static class ThrowIf
{
    public static void Default<T>(T value, [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (EqualityComparer<T>.Default.Equals(value, default))
        {
            throw new ArgumentException("Value cannot be the default value.", parameterName);
        }
    }

    public static void GreaterThan(double value, double max,
        [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (value > max)
        {
            throw new ArgumentException($"Value cannot be greater than {max}.", parameterName);
        }
    }

    public static void LowerThan(double value, double min,
        [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (value < min)
        {
            throw new ArgumentException($"Value cannot be lower than {min}.", parameterName);
        }
    }
    
    public static void NotInRange(double value, double min, double max, [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (value < min || value > max)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, $"Value must be between {min} and {max}.");
        }
    }
    
    public static void NullOrEmpty<T>(IEnumerable<T> collection, [CallerArgumentExpression("collection")] string? parameterName = null)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(parameterName, "The collection cannot be null.");
        }
        if (!collection.Any())
        {
            throw new ArgumentException("The collection cannot be empty.", parameterName);
        }
    }
    
    public static void LowerThanOrEqual(double value, double min, [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (value <= min)
        {
            throw new ArgumentException($"Value must be greater than {min}.", parameterName);
        }
    }
}