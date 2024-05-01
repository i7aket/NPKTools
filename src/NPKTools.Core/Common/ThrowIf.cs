using System.Runtime.CompilerServices;

namespace NPKTools.Core.Common;

public static class ThrowIf
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
    public static void NullOrEmpty<T>(IEnumerable<T> collection, [CallerArgumentExpression("collection")] string? parameterName = null)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(parameterName, "The collection cannot be null.");
        }
        if (!collection.Any())
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