using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Elements.ValueObjects;

public record BoilingPoint
{
    public double Value { get; }
    public BoilingPoint(double value)
    {
        Validate.NotLowerThan(value, -273.15);
        Value = value;
    }
}