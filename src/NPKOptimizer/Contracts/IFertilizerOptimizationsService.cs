using NPKOptimizer.Common;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.NPK;
using NPKOptimizer.Domain.PartsPerMillion;
using NPKOptimizer.Domain.SolutionsFinderSettings;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace NPKOptimizer.Contracts;

/// <summary>
/// Defines an interface for calculating optimal fertilizer mixes based on target NPK values and other nutrients.
/// </summary>
public interface IFertilizerOptimizationsService
{
    /// <summary>
    /// Finds all possible solutions for the given NPK target.
    /// </summary>
    /// <param name="target">The target NPK and other nutrient levels.</param>
    /// <returns>A result containing a collection of fertilizer mixes that meet the target, or an error message.</returns>
    Task<ActionResult<Solutions>> FindAllSolutions(NpkTarget target);

    /// <summary>
    /// Finds a single solution for the given NPK target and mix, applying specific settings.
    /// </summary>
    /// <param name="target">The target NPK and other nutrient levels.</param>
    /// <param name="collection">The initial mix of fertilizers to consider.</param>
    /// <param name="settings">Settings that specify which nutrients to include and their constraints.</param>
    /// <returns>A result containing the optimized fertilizer mix that meets the target, or an error message.</returns>
    ActionResult<FertilizerCollection> FindSolution(NpkTarget target, FertilizerCollection collection,
        SolutionFinderSettings settings);

    /// <summary>
    /// Calculates the parts per million (PPM) concentration of nutrients in a given fertilizer mix.
    /// </summary>
    /// <param name="collection">The mix of fertilizers to analyze.</param>
    /// <param name="waterLiters">The volume of water in liters the mix is dissolved in. Defaults to 1 liter.</param>
    /// <returns>A result containing the PPM values for each nutrient, or an error message.</returns>
    ActionResult<Ppm> CalculatePpm(FertilizerCollection collection, double waterLiters = 1);
}