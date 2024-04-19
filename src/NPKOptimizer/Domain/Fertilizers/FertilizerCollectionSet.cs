using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers;

public record FertilizerCollectionSet() : SetCollectionBase<FertilizerCollection>(new FertilizerCollectionComparer());
public class FertilizerCollectionComparer : IEqualityComparer<FertilizerCollection>
{
    public bool Equals(FertilizerCollection? x, FertilizerCollection? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.Id == y.Id;
    }
     
    public int GetHashCode(FertilizerCollection obj) => obj.Id.GetHashCode();
}


