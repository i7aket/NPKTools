using NPKTools.Core.Domain.Fertilizers;

namespace NPKTools.Core.Domain.Collections;

/// <summary>
/// Represents a collection of <see cref="FertilizerResultModel"/> objects used in a fertilization solution,
/// with the additional property of water liters to indicate the volume of water in which the fertilizers will be dissolved.
/// This class is designed to store the final solution of the fertilizer optimization process.
/// </summary>
public class Solution : List<Fertilizer>
{
    /// <summary>
    /// Gets or sets the volume of water (in liters) that the fertilizers are to be dissolved in.
    /// This is crucial for determining the final concentration of nutrients in the solution
    /// that will be applied to crops.
    /// </summary>
    public double WaterLiters { get; set; }
}