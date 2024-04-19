using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Elements.ValueObjects;

public record AtomicName
{
    public string Value { get; }
    public AtomicName(string value)
    {
        Validate.NotNullOrWhiteSpace(value);
        Value = value;
    }
}