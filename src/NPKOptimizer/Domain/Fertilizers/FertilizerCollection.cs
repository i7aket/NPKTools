using System.Security.Cryptography;
using System.Text;
using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers;

public sealed record FertilizerCollection : SetCollectionBase<Fertilizer>
{
    public Guid Id => _id.Value;
    private readonly Lazy<Guid> _id;

    public FertilizerCollection() : base(new FertilizerComparer())
    {
        _id = new Lazy<Guid>(CreateHashId);
    }

    private Guid CreateHashId()
    {
        List<Fertilizer> sortedComponents = Collection.OrderBy(c => c.Id.Value).ToList();
        string componentsString = string.Join(",", sortedComponents.Select(c => $"{c.Id.Value}:{c.Weight.Value}"));
        byte[] hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(componentsString));
        return new Guid(hashBytes);
    }
}

public class FertilizerComparer : IEqualityComparer<Fertilizer>
{
    public bool Equals(Fertilizer? x, Fertilizer? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x == null || y == null) return false;
        return x.Id == y.Id && x.Weight == y.Weight;
    }

    public int GetHashCode(Fertilizer obj)
    {
        unchecked 
        {
            int hash = 17;
            hash = hash * 23 + obj.Id.GetHashCode();
            hash = hash * 23 + obj.Weight.GetHashCode();
            return hash;
        }
    }
}
