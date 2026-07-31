using System.Text;
using NPKTools.Core.Common;
using NPKTools.Core.Constants;
using NPKTools.Core.Domain.Fertilizers.Enums;
using NPKTools.Core.Domain.Fertilizers.ValueObjects;
using static NPKTools.Core.Constants.Labels;

namespace NPKTools.Core.Domain.Fertilizers;

/// <summary>
/// A fertilizer: its identity (name, chemical formula, concentrate tank), the weight prescribed
/// for a solution, and the full nutrient composition inherited from <see cref="FertilizerAttributes"/>.
/// </summary>
/// <remarks>
/// Immutable. Build one with <see cref="Builders.FertilizerBuilder"/>, and use
/// <see cref="With"/> to obtain a copy carrying a different weight.
/// </remarks>
public class Fertilizer : FertilizerAttributes
{
    /// <summary>
    /// Gets the identifier distinguishing this fertilizer within a source collection.
    /// The optimizer requires every candidate fertilizer to carry a unique value.
    /// </summary>
    public FertilizerReferenceId RefId { get; }

    /// <summary>
    /// Gets the weight, in grams, prescribed for the solution. Zero for a catalogue entry
    /// that has not yet been through the optimizer.
    /// </summary>
    public FertilizerWeight Weight { get; }

    /// <summary>
    /// Gets the human-readable name of the fertilizer.
    /// </summary>
    public FertilizerName Name { get; }

    /// <summary>
    /// Gets the chemical formula of the fertilizer.
    /// </summary>
    public FertilizerFormula Formula { get; }

    /// <summary>
    /// Gets the concentrate tank this fertilizer belongs in.
    /// </summary>
    public ConcentrateType Type { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Fertilizer"/> class.
    /// </summary>
    /// <param name="name">The human-readable name.</param>
    /// <param name="formula">The chemical formula.</param>
    /// <param name="type">The concentrate tank the fertilizer belongs in.</param>
    /// <param name="refId">The identifier distinguishing this fertilizer within a collection.</param>
    /// <param name="weight">The weight prescribed for the solution.</param>
    /// <param name="price">The monetary price of the fertilizer, used as the optimizer's cost objective.</param>
    /// <param name="nitrogen">Nitrogen content, split into nitrate, ammonium and amine forms.</param>
    /// <param name="phosphorus">Phosphorus (P) content.</param>
    /// <param name="potassium">Potassium (K) content.</param>
    /// <param name="calcium">Calcium (Ca) content, chelated and non-chelated.</param>
    /// <param name="magnesium">Magnesium (Mg) content, chelated and non-chelated.</param>
    /// <param name="sulfur">Sulfur (S) content.</param>
    /// <param name="iron">Iron (Fe) content, by chelation form.</param>
    /// <param name="copper">Copper (Cu) content, chelated and non-chelated.</param>
    /// <param name="manganese">Manganese (Mn) content, chelated and non-chelated.</param>
    /// <param name="zinc">Zinc (Zn) content, chelated and non-chelated.</param>
    /// <param name="boron">Boron (B) content.</param>
    /// <param name="molybdenum">Molybdenum (Mo) content.</param>
    /// <param name="chlorine">Chlorine (Cl) content.</param>
    /// <param name="silicon">Silicon (Si) content.</param>
    /// <param name="selenium">Selenium (Se) content.</param>
    /// <param name="sodium">Sodium (Na) content.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument other than <paramref name="type"/> is null.</exception>
    public Fertilizer(FertilizerName name, FertilizerFormula formula, ConcentrateType type, FertilizerReferenceId refId,
        FertilizerWeight weight, FertilizerPrice price, FertilizerNitrogen nitrogen, FertilizerPhosphorus phosphorus,
        FertilizerPotassium potassium, FertilizerCalcium calcium, FertilizerMagnesium magnesium,
        FertilizerSulfur sulfur, FertilizerIron iron,
        FertilizerCopper copper, FertilizerManganese manganese, FertilizerZinc zinc, FertilizerBoron boron,
        FertilizerMolybdenum molybdenum,
        FertilizerChlorine chlorine, FertilizerSilicon silicon, FertilizerSelenium selenium, FertilizerSodium sodium)
        : base(price, nitrogen, phosphorus, potassium, calcium, magnesium, sulfur, iron, copper, manganese, zinc, boron,
            molybdenum, chlorine, silicon, selenium, sodium)
    {
        ArgumentNullException.ThrowIfNull(name);
        Name = name;
        ArgumentNullException.ThrowIfNull(formula);
        Formula = formula;
        // ConcentrateType is an enum, so a null check would be a no-op (CA2264).
        Type = type;
        ArgumentNullException.ThrowIfNull(refId);
        RefId = refId;
        ArgumentNullException.ThrowIfNull(weight);
        Weight = weight;
    }

    /// <summary>
    /// Returns a copy of this fertilizer carrying a different weight. Used by the optimizer to
    /// attach the solved quantity to a catalogue entry without mutating it.
    /// </summary>
    /// <param name="newWeight">The weight the copy should carry.</param>
    /// <returns>A new <see cref="Fertilizer"/> identical to this one apart from its weight.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="newWeight"/> is null.</exception>
    public Fertilizer With(FertilizerWeight newWeight)
    {
        return new Fertilizer(Name, Formula, Type, RefId, newWeight, Price, Nitrogen, Phosphorus, Potassium, Calcium,
            Magnesium, Sulfur, Iron, Copper, Manganese, Zinc, Boron, Molybdenum, Chlorine, Silicon, Selenium, Sodium);
    }
}