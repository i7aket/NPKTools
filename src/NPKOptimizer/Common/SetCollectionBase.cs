using System.Collections;

namespace NPKOptimizer.Common;

public abstract record SetCollectionBase<T> : ISet<T>
{
    protected readonly HashSet<T> Collection = new ();

    protected SetCollectionBase(IEqualityComparer<T> comparer)
    {
        Collection = new HashSet<T>(comparer);
    }
    
    public bool Add(T item) => Collection.Add(item);
    void ICollection<T>.Add(T item) => Add(item);
    public void Clear() => Collection.Clear();
    public bool Contains(T item) => Collection.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => Collection.CopyTo(array, arrayIndex);
    public bool Remove(T item) => Collection.Remove(item);
    public int Count => Collection.Count;
    public bool IsReadOnly => false;
    public IEnumerator<T> GetEnumerator() => Collection.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public void ExceptWith(IEnumerable<T> other) => Collection.ExceptWith(other);
    public void IntersectWith(IEnumerable<T> other) => Collection.IntersectWith(other);
    public bool IsProperSubsetOf(IEnumerable<T> other) => Collection.IsProperSubsetOf(other);
    public bool IsProperSupersetOf(IEnumerable<T> other) => Collection.IsProperSupersetOf(other);
    public bool IsSubsetOf(IEnumerable<T> other) => Collection.IsSubsetOf(other);
    public bool IsSupersetOf(IEnumerable<T> other) => Collection.IsSupersetOf(other);
    public bool Overlaps(IEnumerable<T> other) => Collection.Overlaps(other);
    public bool SetEquals(IEnumerable<T> other) => Collection.SetEquals(other);
    public void SymmetricExceptWith(IEnumerable<T> other) => Collection.SymmetricExceptWith(other);
    public void UnionWith(IEnumerable<T> other) => Collection.UnionWith(other);
    public void AddRange(IEnumerable<T> items)
    {
        foreach (T item in items)
        {
            Add(item);
        }
    }
}