using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.PartsPerMillion;

namespace NPKOptimizer.Contracts;

/// <summary>
/// Defines a service responsible for calculating parts per million (ppm) values for fertilizers.
/// </summary>
public interface IPpmCalculationService
{
    /// <summary>
    /// Calculates the ppm values for a given collection of fertilizers and volume of water.
    /// </summary>
    /// <param name="collection">The collection of fertilizers to calculate ppm values for.</param>
    /// <param name="waterLiters">The volume of water in liters. Must be greater than 0.</param>
    /// <returns>A <see cref="Ppm"/> object containing calculated ppm values for the fertilizers.</returns>
    Ppm CalculatePpm(IList<Fertilizer> collection, double waterLiters = 1);
}