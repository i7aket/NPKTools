using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerPrice : FieldBase
{
    public FertilizerPrice(double value = 1) : base(value)
    {
        Validate.NotDefault(value);
    }
}