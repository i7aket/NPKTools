using NPKOptimizer.Common;
namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerReferenceId
{
    public Guid Value { get; }

    public FertilizerReferenceId(Guid value)
    {
        ThrowIf.Default(value);
        Value = value;
    }
}