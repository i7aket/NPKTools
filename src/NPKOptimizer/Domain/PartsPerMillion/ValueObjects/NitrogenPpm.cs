using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.PartsPerMillion.ValueObjects;

/// <summary>
/// Represents the nitrogen content in a solution, expressed in parts per million (ppm).
/// This value object captures three common forms of nitrogen: nitrate, ammonium, and amine.
/// </summary>
public record NitrogenPpm 
{
    /// <summary>
    /// Gets the concentration of nitrate (NO3-) nitrogen in parts per million.
    /// </summary>
    public double Nitrate { get; }
    /// <summary>
    /// Gets the concentration of ammonium (NH4+) nitrogen in parts per million.
    /// </summary>
    public double Ammonium { get; }
    /// <summary>
    /// Gets the concentration of amine nitrogen in parts per million.
    /// </summary>
    public double Amine { get; }
    /// <summary>
    /// Calculates the total nitrogen content by summing the concentrations of nitrate, ammonium, and amine forms.
    /// </summary>
    public double Value => Nitrate + Ammonium + Amine; 

    /// <summary>
    /// Initializes a new instance of the <see cref="NitrogenPpm"/> class with specified values for nitrate, ammonium, and amine.
    /// </summary>
    /// <param name="nitrate">The concentration of nitrate nitrogen in ppm.</param>
    /// <param name="ammonium">The concentration of ammonium nitrogen in ppm.</param>
    /// <param name="amine">The concentration of amine nitrogen in ppm.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if any nitrogen value is negative.</exception>

    public NitrogenPpm(double nitrate = 0, double ammonium = 0, double amine = 0) 
    {
        ArgumentOutOfRangeException.ThrowIfNegative(nitrate);
        ArgumentOutOfRangeException.ThrowIfNegative(ammonium);
        ArgumentOutOfRangeException.ThrowIfNegative(amine);
        Nitrate = nitrate;
        Ammonium = ammonium;
        Amine = amine;
    }
}