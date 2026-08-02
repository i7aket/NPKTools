namespace SYT.NPKTools.Nutrients;

/// <summary>
/// How much acid a tank needs, and what the acid brings with it.
/// </summary>
public sealed record AcidPlan
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AcidPlan"/> record.
    /// </summary>
    /// <param name="milliequivalentsPerLitre">Acidity to add, in meq/L.</param>
    /// <param name="millilitres">Volume of the liquid acid, for the whole tank.</param>
    /// <param name="nutrientSymbol">The element the acid contributes.</param>
    /// <param name="nutrientPpm">How much of that element it contributes, in ppm.</param>
    internal AcidPlan(
        double milliequivalentsPerLitre,
        double millilitres,
        string nutrientSymbol,
        double nutrientPpm)
    {
        MilliequivalentsPerLitre = milliequivalentsPerLitre;
        Millilitres = millilitres;
        NutrientSymbol = nutrientSymbol;
        NutrientPpm = nutrientPpm;
    }

    /// <summary>Gets the acidity to add, in milliequivalents per litre.</summary>
    public double MilliequivalentsPerLitre { get; }

    /// <summary>Gets the volume of liquid acid for the whole tank, in millilitres.</summary>
    public double Millilitres { get; }

    /// <summary>Gets the element symbol the acid contributes.</summary>
    public string NutrientSymbol { get; }

    /// <summary>
    /// Gets how much of that element the acid contributes, in ppm.
    /// </summary>
    /// <remarks>
    /// Subtract this from the target alongside the water. Nitric acid on moderately hard water carries
    /// around 29 ppm of nitrogen, a fifth of an ordinary target, and phosphoric acid can carry more
    /// phosphorus than the target asks for in total.
    /// </remarks>
    public double NutrientPpm { get; }
}
