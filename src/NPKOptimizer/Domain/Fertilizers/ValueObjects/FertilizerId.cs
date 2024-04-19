using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerId
{
    public Guid Value { get; }

    public FertilizerId(Guid value)
    {
        Validate.NotDefault(value);
        Value = value;
    }
}