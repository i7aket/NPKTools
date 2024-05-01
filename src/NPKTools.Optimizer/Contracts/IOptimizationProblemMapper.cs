using NPKTools.Core.Domain.Collections;
using NPKTools.Core.Domain.Fertilizers;
using NPKTools.Core.Domain.PpmTarget;
using NPKTools.Core.Domain.SolutionsFinderSettings;

namespace NPKTools.Optimizer.Contracts;

/// <summary>
/// Provides methods for mapping between the domain model and the optimization problem format.
/// </summary>
public interface IOptimizationProblemMapper
{
    /// <summary>
    /// Creates an optimization problem based on target PPM values, a collection of fertilizers, and specific finder settings.
    /// </summary>
    /// <param name="target">The target ppm values for each nutrient.</param>
    /// <param name="sourceCollection">A list of available fertilizers for the optimization.</param>
    /// <param name="settings">Settings that influence the optimization process.</param>
    /// <returns>An <see cref="OptimizationProblem"/> that represents the formulated optimization problem.</returns>
    OptimizationProblem CreateOptimizationProblem(PpmTarget target, IList<FertilizerOptimizationModel> sourceCollection, SolutionFinderSettings settings);

    /// <summary>
    /// Creates a solution from the given solution values and the original source collection of fertilizers.
    /// </summary>
    /// <param name="solutionValues">The optimized values obtained from solving the optimization problem.</param>
    /// <param name="originalSourceCollection">The original collection of fertilizers used in the optimization problem.</param>
    /// <param name="waterLiters">The amount of water in liters to be used with the fertilizers.</param>
    /// <returns>A <see cref="Solution"/> containing the optimized amounts of each fertilizer.</returns>
    Solution CreateSolution(Dictionary<string, double> solutionValues, IList<FertilizerOptimizationModel> originalSourceCollection, double waterLiters = 1);
}