using System.Runtime.CompilerServices;

namespace NPKOptimizer.Common;

public static class Validate
{
    public static void NotNull(object? parameter, [CallerArgumentExpression("parameter")] string? parameterName = null)
    {
        if (parameter == null)
        {
            throw new ArgumentNullException(parameterName, "Provided parameter cannot be null.");
        }
    }

    public static void Positive(double value, [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "Provided value cannot be negative.");
        }
    }

    public static void NotNullOrWhiteSpace(string? str, [CallerArgumentExpression("str")] string? parameterName = null)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            throw new ArgumentException("Provided string cannot be null or empty.", parameterName);
        }
    }
    
    public static void NotDefault<T>(T value, [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (EqualityComparer<T>.Default.Equals(value, default))
        {
            throw new ArgumentException("Provided value cannot be the default value.", parameterName);
        }
    }

    public static void NotGreaterThan(double value, double max,
        [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (value > max)
        {
            throw new ArgumentException($"Value cannot be greater than {max}.", parameterName);
        }
    }

    public static void NotLowerThan(double value, double min,
        [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (value < min)
        {
            throw new ArgumentException($"Value cannot be lower than {min}.", parameterName);
        }
    }
    
    public static void InRange(double value, double min, double max, [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (value < min || value > max)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, $"Value must be between {min} and {max}.");
        }
    }
}