using System.Runtime.CompilerServices;

namespace SYT.NPKTools.Internal;

/// <summary>
/// Guard clauses for argument checks the BCL does not provide out of the box.
/// </summary>
internal static class ThrowIf
{
    /// <summary>
    /// Throws an ArgumentException if the specified value is the default for its type.
    /// </summary>
    /// <typeparam name="T">The type of the value being checked.</typeparam>
    /// <param name="value">The value to check against the default.</param>
    /// <param name="parameterName">The name of the parameter being checked. This is captured automatically.</param>
    /// <exception cref="ArgumentException">Thrown if the value is the default for its type.</exception>
    public static void Default<T>(T value, [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (EqualityComparer<T>.Default.Equals(value, default))
        {
            throw new ArgumentException("Value cannot be the default value.", parameterName);
        }
    }

    /// <summary>
    /// Throws an ArgumentNullException if the specified collection is null, or an ArgumentException if it is empty.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    /// <param name="collection">The collection to check.</param>
    /// <param name="parameterName">The name of the parameter being checked. This is captured automatically.</param>
    /// <exception cref="ArgumentNullException">Thrown if the collection is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the collection is empty.</exception>
    /// <remarks>
    /// Emptiness is established from a count where the sequence exposes one, so the common case
    /// never enumerates. <see cref="Enumerable.TryGetNonEnumeratedCount{TSource}"/> alone is not
    /// enough: it recognises <see cref="ICollection{T}"/> but not <see cref="IReadOnlyCollection{T}"/>,
    /// and the library's own <c>Solution</c> and <c>Solutions</c> are read-only collections.
    /// Only a genuinely lazy sequence falls through to <c>Any()</c>.
    /// </remarks>
    public static void NullOrEmpty<T>(IEnumerable<T> collection, [CallerArgumentExpression(nameof(collection))] string? parameterName = null)
    {
        if (collection is null)
        {
            throw new ArgumentNullException(parameterName, "The collection cannot be null.");
        }

        bool isEmpty = collection switch
        {
            IReadOnlyCollection<T> readOnly => readOnly.Count == 0,
            ICollection<T> mutable => mutable.Count == 0,
            _ => !collection.TryGetNonEnumeratedCount(out int count) ? !collection.Any() : count == 0
        };

        if (isEmpty)
        {
            throw new ArgumentException("The collection cannot be empty.", parameterName);
        }
    }

    /// <summary>
    /// Throws an InvalidOperationException if the item cannot be added to the set (indicating a duplicate).
    /// </summary>
    /// <typeparam name="T">The type of the elements in the set.</typeparam>
    /// <param name="set">The set to which the item is being added.</param>
    /// <param name="item">The item to add.</param>
    /// <param name="parameterName">The name of the parameter being checked. This is captured automatically.</param>
    /// <exception cref="InvalidOperationException">Thrown if the item is already in the set.</exception>
    public static void Duplicate<T>(HashSet<T> set, T item, [CallerArgumentExpression("item")] string? parameterName = null)
    {
        if (!set.Add(item))
        {
            throw new InvalidOperationException($"Duplicate {parameterName} detected with identical attributes.");
        }
    }
}
