using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerFormula
{
    public string? Value { get; }

    public FertilizerFormula(string? value = "")
    {
        Validate.NotNull(value);
        Value = value;
    }
}