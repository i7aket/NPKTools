using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Elements.ValueObjects;

public record AtomicMass
{
    public double Value { get; }
    public AtomicMass(double value)
    {
        Validate.Positive(value);
        Value = value;
    }
}