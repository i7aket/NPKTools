using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Elements.ValueObjects;

public record MeltingPoint
{
    public double? Value { get; }
    public MeltingPoint(double? value)
    {
        if (value.HasValue)
        {
            Validate.NotLowerThan(value.Value, -273.15);
        }
        Value = value;
    }
}