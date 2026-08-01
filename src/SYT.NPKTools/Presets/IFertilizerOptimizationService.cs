using SYT.NPKTools.Nutrients;

namespace SYT.NPKTools;

/// <summary>
/// Defines a service for optimizing fertilizer usage based on target PPM values for macro and micro nutrients.
/// </summary>
/// <remarks>
/// These methods each run a linear program per fertilizer bundle, so a single call can take a
/// noticeable amount of time. Pass a <see cref="CancellationToken"/> to abandon the search early.
/// Before 2.0.0 they returned null when nothing was found; they now return <see cref="Solutions.Empty"/>.
/// </remarks>
public interface IFertilizerOptimizationService
{
    /// <summary>
    /// Finds optimization solutions for macro nutrients based on the specified target ppm values.
    /// </summary>
    /// <param name="target">The target PPM values for macro nutrients.</param>
    /// <param name="cancellationToken">Token used to cancel the search.</param>
    /// <returns>The optimized solutions, or <see cref="Solutions.Empty"/> when none satisfy the target.</returns>
    Solutions FindMacroSolutions(PpmTarget target, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds optimization solutions for micro nutrients based on the specified target ppm values.
    /// </summary>
    /// <param name="target">The target PPM values for micro nutrients.</param>
    /// <param name="cancellationToken">Token used to cancel the search.</param>
    /// <returns>The optimized solutions, or <see cref="Solutions.Empty"/> when none satisfy the target.</returns>
    Solutions FindMicroSolutions(PpmTarget target, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds optimization solutions for both macro and micro nutrients based on the specified target ppm values.
    /// </summary>
    /// <param name="target">The target PPM values for nutrients.</param>
    /// <param name="cancellationToken">Token used to cancel the search.</param>
    /// <returns>The macro and micro solution sets, each empty when none were found.</returns>
    FertilizerSolutions FindSolutions(PpmTarget target, CancellationToken cancellationToken = default);
}
