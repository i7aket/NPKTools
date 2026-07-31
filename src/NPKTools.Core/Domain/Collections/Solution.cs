using System.Collections;
using NPKTools.Core.Domain.Fertilizers;

namespace NPKTools.Core.Domain.Collections;

/// <summary>
/// One result of the fertilizer optimization process: the fertilizers to use, each carrying its
/// computed weight, together with the volume of water they are to be dissolved in.
/// </summary>
/// <remarks>
/// Up to 2.0.0 this type derived from <see cref="List{T}"/>, which let callers mutate a solution
/// after the optimizer had produced it and exposed the whole list API as part of the contract.
/// A solution is now an immutable read-only view over its fertilizers.
/// </remarks>
public sealed class Solution : IReadOnlyList<Fertilizer>
{
    private readonly Fertilizer[] _fertilizers;

    /// <summary>
    /// Initializes a new instance of the <see cref="Solution"/> class.
    /// </summary>
    /// <param name="fertilizers">The fertilizers making up the solution, each with its weight already applied.</param>
    /// <param name="waterLiters">The volume of water the fertilizers are dissolved in. Must be greater than zero.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="fertilizers"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="waterLiters"/> is zero or negative.</exception>
    public Solution(IEnumerable<Fertilizer> fertilizers, double waterLiters)
    {
        ArgumentNullException.ThrowIfNull(fertilizers);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(waterLiters);

        _fertilizers = [.. fertilizers];
        WaterLiters = waterLiters;
    }

    /// <summary>
    /// Gets the volume of water (in liters) the fertilizers are dissolved in.
    /// This determines the final nutrient concentration of the solution.
    /// </summary>
    public double WaterLiters { get; }

    /// <summary>
    /// Gets the fertilizer at the specified position.
    /// </summary>
    /// <param name="index">The zero-based index of the fertilizer to get.</param>
    public Fertilizer this[int index] => _fertilizers[index];

    /// <summary>
    /// Gets the number of fertilizers in the solution.
    /// </summary>
    public int Count => _fertilizers.Length;

    /// <inheritdoc />
    public IEnumerator<Fertilizer> GetEnumerator() => ((IEnumerable<Fertilizer>)_fertilizers).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
