
namespace SYT.NPKTools.Fertilizers;
/// <summary>
/// Represents the base type for a fertilizer detailing all fundamental nutrient attributes.
/// This class serves as a central collection of data points, each representing a specific nutrient or property of the fertilizer.
/// These attributes are essential for the optimization of fertilizers
/// </summary>
public class FertilizerAttributes
{
    /// <summary>
    /// Represents the monetary price of the fertilizer.
    /// </summary>
    public FertilizerPrice Price { get; }

    /// <summary>
    /// Represents the nitrogen (N) content in the fertilizer.
    /// </summary>
    public FertilizerNitrogen Nitrogen { get; }

    /// <summary>
    /// Represents the phosphorus (P) content in the fertilizer.
    /// </summary>
    public FertilizerPhosphorus Phosphorus { get; }

    /// <summary>
    /// Represents the potassium (K) content in the fertilizer.
    /// </summary>
    public FertilizerPotassium Potassium { get; }

    /// <summary>
    /// Represents the calcium (Ca) content in the fertilizer.
    /// </summary>
    public FertilizerCalcium Calcium { get; }

    /// <summary>
    /// Represents the magnesium (Mg) content in the fertilizer.
    /// </summary>
    public FertilizerMagnesium Magnesium { get; }

    /// <summary>
    /// Represents the sulfur (S) content in the fertilizer.
    /// </summary>
    public FertilizerSulfur Sulfur { get; }

    /// <summary>
    /// Represents the iron (Fe) content in the fertilizer.
    /// </summary>
    public FertilizerIron Iron { get; }

    /// <summary>
    /// Represents the copper (Cu) content in the fertilizer.
    /// </summary>
    public FertilizerCopper Copper { get; }

    /// <summary>
    /// Represents the manganese (Mn) content in the fertilizer.
    /// </summary>
    public FertilizerManganese Manganese { get; }

    /// <summary>
    /// Represents the zinc (Zn) content in the fertilizer.
    /// </summary>
    public FertilizerZinc Zinc { get; }

    /// <summary>
    /// Represents the boron (B) content in the fertilizer.
    /// </summary>
    public FertilizerBoron Boron { get; }

    /// <summary>
    /// Represents the molybdenum (Mo) content in the fertilizer.
    /// </summary>
    public FertilizerMolybdenum Molybdenum { get; }

    /// <summary>
    /// Represents the chlorine (Cl) content in the fertilizer.
    /// </summary>
    public FertilizerChlorine Chlorine { get; }

    /// <summary>
    /// Represents the silicon (Si) content in the fertilizer.
    /// </summary>
    public FertilizerSilicon Silicon { get; }

    /// <summary>
    /// Represents the selenium (Se) content in the fertilizer.
    /// </summary>
    public FertilizerSelenium Selenium { get; }

    /// <summary>
    /// Represents the sodium (Na) content in the fertilizer.
    /// </summary>
    public FertilizerSodium Sodium { get; }


    /// <summary>
    /// Initializes a new instance of the <see cref="FertilizerAttributes"/> class.
    /// </summary>
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
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    public FertilizerAttributes(
        FertilizerPrice price,
        FertilizerNitrogen nitrogen,
        FertilizerPhosphorus phosphorus,
        FertilizerPotassium potassium,
        FertilizerCalcium calcium,
        FertilizerMagnesium magnesium,
        FertilizerSulfur sulfur,
        FertilizerIron iron,
        FertilizerCopper copper,
        FertilizerManganese manganese,
        FertilizerZinc zinc,
        FertilizerBoron boron,
        FertilizerMolybdenum molybdenum,
        FertilizerChlorine chlorine,
        FertilizerSilicon silicon,
        FertilizerSelenium selenium,
        FertilizerSodium sodium)
    {
        ArgumentNullException.ThrowIfNull(price);
        Price = price;

        ArgumentNullException.ThrowIfNull(nitrogen);
        Nitrogen = nitrogen;

        ArgumentNullException.ThrowIfNull(phosphorus);
        Phosphorus = phosphorus;

        ArgumentNullException.ThrowIfNull(potassium);
        Potassium = potassium;

        ArgumentNullException.ThrowIfNull(calcium);
        Calcium = calcium;

        ArgumentNullException.ThrowIfNull(magnesium);
        Magnesium = magnesium;

        ArgumentNullException.ThrowIfNull(sulfur);
        Sulfur = sulfur;

        ArgumentNullException.ThrowIfNull(iron);
        Iron = iron;

        ArgumentNullException.ThrowIfNull(copper);
        Copper = copper;

        ArgumentNullException.ThrowIfNull(manganese);
        Manganese = manganese;

        ArgumentNullException.ThrowIfNull(zinc);
        Zinc = zinc;

        ArgumentNullException.ThrowIfNull(boron);
        Boron = boron;

        ArgumentNullException.ThrowIfNull(molybdenum);
        Molybdenum = molybdenum;

        ArgumentNullException.ThrowIfNull(chlorine);
        Chlorine = chlorine;

        ArgumentNullException.ThrowIfNull(silicon);
        Silicon = silicon;

        ArgumentNullException.ThrowIfNull(selenium);
        Selenium = selenium;

        ArgumentNullException.ThrowIfNull(sodium);
        Sodium = sodium;
    }
}
