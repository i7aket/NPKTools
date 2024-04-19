using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Elements.ValueObjects;

public record IonizationEnergy
{
    public double Value { get; }
    public IonizationEnergy(double value)
    {
        Validate.Positive(value, nameof(value));
        Value = value;
    }
}