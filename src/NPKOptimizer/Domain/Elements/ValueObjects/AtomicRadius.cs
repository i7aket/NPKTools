using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Elements.ValueObjects;

public record AtomicRadius
{
    public double Value { get; }
    
    public AtomicRadius(double value)
    {
        Validate.Positive(value);
        Value = value;
    }
}