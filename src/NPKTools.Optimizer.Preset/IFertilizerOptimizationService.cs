using NPKTools.Core.Domain.Collections;
using NPKTools.Core.Domain.PpmTarget;

namespace NPKTools.Optimizer.Preset;

/// <summary>
/// Defines a service for optimizing fertilizer usage based on target PPM values for macro and micro nutrients.
/// </summary>
public interface IFertilizerOptimizationService
{
    /// <summary>
    /// Finds optimization solutions for macro nutrients based on the specified target ppm values.
    /// </summary>
    /// <param name="target">The target PPM values for macro nutrients.</param>
    /// <returns>A collection of optimized solutions.</returns>
    Solutions? FindMacroSolutions(PpmTarget target);

    /// <summary>
    /// Finds optimization solutions for micro nutrients based on the specified target ppm values.
    /// </summary>
    /// <param name="target">The target PPM values for micro nutrients.</param>
    /// <returns>A collection of optimized solutions.</returns>
    Solutions? FindMicroSolutions(PpmTarget target);

    /// <summary>
    /// Finds optimization solutions for both macro and micro nutrients based on the specified target ppm values.
    /// </summary>
    /// <param name="target">The target PPM values for nutrients.</param>
    /// <returns>A tuple containing collections of solutions for macro and micro nutrients.</returns>
    (Solutions? Macro, Solutions? Micro) FindSolutions(PpmTarget target);
}