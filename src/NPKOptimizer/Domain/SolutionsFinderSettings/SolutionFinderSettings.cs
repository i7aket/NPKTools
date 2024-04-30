using NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

namespace NPKOptimizer.Domain.SolutionsFinderSettings;

/// <summary>
/// Encapsulates all settings used to find optimized solutions for fertilizer applications, including settings for each specific nutrient.
/// </summary>
public class SolutionFinderSettings
{
    /// <summary>
    /// Gets or sets the range factor settings which might include tolerances and other factors that influence the overall optimization constraints.
    /// </summary>
    public RangeFactorSettings RangeFactor { get; set; }

    /// <summary>
    /// Gets or sets the settings for nitrogen optimization.
    /// </summary>
    public NitrogenSettings Nitrogen { get; set; }

    /// <summary>
    /// Gets or sets the settings for phosphorus optimization.
    /// </summary>
    public PhosphorusSettings Phosphorus { get; set; }

    /// <summary>
    /// Gets or sets the settings for potassium optimization.
    /// </summary>
    public PotassiumSettings Potassium { get; set; }

    /// <summary>
    /// Gets or sets the settings for calcium optimization.
    /// </summary>
    public CalciumSettings Calcium { get; set; }

    /// <summary>
    /// Gets or sets the settings for magnesium optimization.
    /// </summary>
    public MagnesiumSettings Magnesium { get; set; }

    /// <summary>
    /// Gets or sets the settings for sulfur optimization.
    /// </summary>
    public SulfurSettings Sulfur { get; set; }

    /// <summary>
    /// Gets or sets the settings for chlorine optimization.
    /// </summary>
    public ChlorineSettings Chlorine { get; set; }

    /// <summary>
    /// Gets or sets the settings for iron optimization.
    /// </summary>
    public IronSettings Iron { get; set; }

    /// <summary>
    /// Gets or sets the settings for copper optimization.
    /// </summary>
    public CopperSettings Copper { get; set; }

    /// <summary>
    /// Gets or sets the settings for manganese optimization.
    /// </summary>
    public ManganeseSettings Manganese { get; set; }

    /// <summary>
    /// Gets or sets the settings for zinc optimization.
    /// </summary>
    public ZincSettings Zinc { get; set; }

    /// <summary>
    /// Gets or sets the settings for boron optimization.
    /// </summary>
    public BoronSettings Boron { get; set; }

    /// <summary>
    /// Gets or sets the settings for molybdenum optimization.
    /// </summary>
    public MolybdenumSettings Molybdenum { get; set; }

    /// <summary>
    /// Gets or sets the settings for silicon optimization.
    /// </summary>
    public SiliconSettings Silicon { get; set; }

    /// <summary>
    /// Gets or sets the settings for selenium optimization.
    /// </summary>
    public SeleniumSettings Selenium { get; set; }

    /// <summary>
    /// Gets or sets the settings for sodium optimization.
    /// </summary>
    public SodiumSettings Sodium { get; set; }
    public SolutionFinderSettings(){} 
    public SolutionFinderSettings(RangeFactorSettings rangeFactor, NitrogenSettings nitrogen, PhosphorusSettings phosphorus, PotassiumSettings potassium, CalciumSettings calcium,
        MagnesiumSettings magnesium, SulfurSettings sulfur, ChlorineSettings chlorine, IronSettings iron, CopperSettings copper,
        ManganeseSettings manganese, ZincSettings zinc, BoronSettings boron, MolybdenumSettings molybdenum, SiliconSettings silicon,
        SeleniumSettings selenium, SodiumSettings sodium)
    {
        ArgumentNullException.ThrowIfNull(rangeFactor);
        RangeFactor = rangeFactor;
        
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