using NPKOptimizer.Domain.Fertilizers.ValueObjects;
namespace NPKOptimizer.Domain.Fertilizers;
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
    public FertilizerPrice Price { get; set; }

    /// <summary>
    /// Represents the nitrogen (N) content in the fertilizer.
    /// </summary>
    public FertilizerNitrogen Nitrogen { get; set; }

    /// <summary>
    /// Represents the phosphorus (P) content in the fertilizer.
    /// </summary>
    public FertilizerPhosphorus Phosphorus { get; set; }

    /// <summary>
    /// Represents the potassium (K) content in the fertilizer.
    /// </summary>
    public FertilizerPotassium Potassium { get; set; }

    /// <summary>
    /// Represents the calcium (Ca) content in the fertilizer.
    /// </summary>
    public FertilizerCalcium Calcium { get; set; }

    /// <summary>
    /// Represents the magnesium (Mg) content in the fertilizer.
    /// </summary>
    public FertilizerMagnesium Magnesium { get; set; }

    /// <summary>
    /// Represents the sulfur (S) content in the fertilizer.
    /// </summary>
    public FertilizerSulfur Sulfur { get; set; }

    /// <summary>
    /// Represents the iron (Fe) content in the fertilizer.
    /// </summary>
    public FertilizerIron Iron { get; set; }

    /// <summary>
    /// Represents the copper (Cu) content in the fertilizer.
    /// </summary>
    public FertilizerCopper Copper { get; set; }

    /// <summary>
    /// Represents the manganese (Mn) content in the fertilizer.
    /// </summary>
    public FertilizerManganese Manganese { get; set; }

    /// <summary>
    /// Represents the zinc (Zn) content in the fertilizer.
    /// </summary>
    public FertilizerZinc Zinc { get; set; }

    /// <summary>
    /// Represents the boron (B) content in the fertilizer.
    /// </summary>
    public FertilizerBoron Boron { get; set; }

    /// <summary>
    /// Represents the molybdenum (Mo) content in the fertilizer.
    /// </summary>
    public FertilizerMolybdenum Molybdenum { get; set; }

    /// <summary>
    /// Represents the chlorine (Cl) content in the fertilizer.
    /// </summary>
    public FertilizerChlorine Chlorine { get; set; }

    /// <summary>
    /// Represents the silicon (Si) content in the fertilizer.
    /// </summary>
    public FertilizerSilicon Silicon { get; set; }

    /// <summary>
    /// Represents the selenium (Se) content in the fertilizer.
    /// </summary>
    public FertilizerSelenium Selenium { get; set; }

    /// <summary>
    /// Represents the sodium (Na) content in the fertilizer.
    /// </summary>
    public FertilizerSodium Sodium { get; set; }
    
    public FertilizerAttributes(){}
    
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