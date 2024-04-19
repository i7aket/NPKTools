using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerName
{
    public string Value { get; }

    public FertilizerName(string value)
    {
        Validate.NotNullOrWhiteSpace(value);
        Value = value;
    }
}