using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Elements.ValueObjects;

public record AtomicNumber
{
    public int Value { get; }
    public AtomicNumber(int value)
    {
        Validate.InRange(value, 1, 118);
        Value = value;
    }
}