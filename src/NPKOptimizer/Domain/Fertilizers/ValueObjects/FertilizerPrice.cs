using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerPrice
{
    public double Value;

    public FertilizerPrice(double value = 1)
    {
        ThrowIf.LowerThanOrEqual(value,0);
        Value = value;
    }
    
}