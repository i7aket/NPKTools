using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Elements.ValueObjects;

public record AtomicSymbol
{
    public string Value { get; }
    public AtomicSymbol(string value)
    {
        Validate.NotNullOrWhiteSpace(value);
        Value = value;
    }
}