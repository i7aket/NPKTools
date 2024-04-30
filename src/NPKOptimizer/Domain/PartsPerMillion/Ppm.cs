using NPKOptimizer.Domain.PartsPerMillion.ValueObjects;

namespace NPKOptimizer.Domain.PartsPerMillion;

/// <summary>
/// Represents the concentration of various nutrients in parts per million (ppm),
/// along with the total volume of water in liters in which these nutrients are intended to be dissolved.
/// </summary>
public class Ppm
{
    /// <summary>
    /// Gets or sets the ppm value for nitrogen.
    /// </summary>
    public NitrogenPpm Nitrogen { get; set;}

    /// <summary>
    /// Gets or sets the ppm value for phosphorus.
    /// </summary>
    public PhosphorusPpm Phosphorus { get; set;}

    /// <summary>
    /// Gets or sets the ppm value for potassium.
    /// </summary>
    public PotassiumPpm Potassium { get; set;}

    /// <summary>
    /// Gets or sets the ppm value for calcium.
    /// </summary>
    public CalciumPpm Calcium { get; set;}

    /// <summary>
    /// Gets or sets the ppm value for magnesium.
    /// </summary>
    public MagnesiumPpm Magnesium { get; set;}

    /// <summary>
    /// Gets or sets the ppm value for sulfur.
    /// </summary>
    public SulfurPpm Sulfur { get; set;}

    /// <summary>
    /// Gets or sets the ppm value for iron.
    /// </summary>
    public IronPpm Iron { get; set;}

    /// <summary>
    /// Gets or sets the ppm value for copper.
    /// </summary>
    public CopperPpm Copper { get; set;}

    /// <summary>
    /// Gets or sets the ppm value for manganese.
    /// </summary>
    public ManganesePpm Manganese { get; set;}

    /// <summary>
    /// Gets or sets the ppm value for zinc.
    /// </summary>
    public ZincPpm Zinc { get; set;}

    /// <summary>
    /// Gets or sets the ppm value for boron.
    /// </summary>
    public BoronPpm Boron { get; set;}

    /// <summary>
    /// Gets or sets the ppm value for molybdenum.
    /// </summary>
    public MolybdenumPpm Molybdenum { get; set;}

    /// <summary>
    /// Gets or sets the ppm value for chlorine.
    /// </summary>
    public ChlorinePpm Chlorine { get; set;}

    /// <summary>
    /// Gets or sets the ppm value for silicon.
    /// </summary>
    public SiliconPpm Silicon { get; set;}

    /// <summary>
    /// Gets or sets the ppm value for selenium.
    /// </summary>
    public SeleniumPpm Selenium { get; set;}

    /// <summary>
    /// Gets or sets the ppm value for sodium.
    /// </summary>
    public SodiumPpm Sodium { get; set;}

    /// <summary>
    /// Gets or sets the volume of water in liters intended for dissolving the nutrients.
    /// </summary>
    public WaterVolumeLitersPpm Liters { get; set;}

    /// <summary>
    /// Calculates the combined ppm value of all the nutrients.
    /// </summary>
    public double Value => Nitrogen.Value + Phosphorus.Value + Potassium.Value + Calcium.Value + Magnesium.Value + Sulfur.Value +
                           Iron.Value + Copper.Value + Manganese.Value + Zinc.Value + Boron.Value + Molybdenum.Value +
                           Chlorine.Value + Sodium.Value + Silicon.Value + Selenium.Value;

    public  Ppm(){}

    public Ppm(NitrogenPpm nitrogen, PhosphorusPpm phosphorus, PotassiumPpm potassium, CalciumPpm calcium, MagnesiumPpm magnesium, SulfurPpm sulfur, IronPpm iron,
        CopperPpm copper, ManganesePpm manganese, ZincPpm zinc, BoronPpm boron, MolybdenumPpm molybdenum, ChlorinePpm chlorine, SiliconPpm silicon,
        SeleniumPpm selenium, SodiumPpm sodium, WaterVolumeLitersPpm liters)
    {
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
        
        ArgumentNullException.ThrowIfNull(liters);
        Liters = liters;
    }
}