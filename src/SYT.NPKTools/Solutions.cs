using System.Collections;

namespace SYT.NPKTools;

/// <summary>
/// An immutable set of optimized <see cref="Solution"/> results. Each entry is a distinct
/// combination of fertilizers and quantities that meets the requested nutrient targets.
/// </summary>
/// <remarks>
/// Up to 2.0.0 this type derived from <see cref="List{T}"/>. Optimizer APIs also returned
/// <c>null</c> to mean "nothing found"; they now return <see cref="Empty"/> instead, so callers
/// no longer have to null-check before enumerating.
/// </remarks>
public sealed class Solutions : IReadOnlyList<Solution>
{
    private readonly Solution[] _solutions;

    /// <summary>
    /// An empty result set. Returned instead of null when no solution satisfies the targets.
    /// </summary>
    public static readonly Solutions Empty = new([]);

    /// <summary>
    /// Initializes a new instance of the <see cref="Solutions"/> class.
    /// </summary>
    /// <param name="solutions">The solutions to expose.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="solutions"/> is null.</exception>
    public Solutions(IEnumerable<Solution> solutions)
    {
        ArgumentNullException.ThrowIfNull(solutions);
        _solutions = [.. solutions];
    }

    /// <summary>
    /// Gets the solution at the specified position.
    /// </summary>
    /// <param name="index">The zero-based index of the solution to get.</param>
    public Solution this[int index] => _solutions[index];

    /// <summary>
    /// Gets the number of solutions found.
    /// </summary>
    public int Count => _solutions.Length;

    /// <inheritdoc />
    public IEnumerator<Solution> GetEnumerator() => ((IEnumerable<Solution>)_solutions).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
