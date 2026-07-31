using NPKTools.Core.Domain.SolutionsFinderSettings.ValueObjects;

namespace NPKTools.Core.Domain.SolutionsFinderSettings;

/// <summary>
/// Encapsulates all settings used to find optimized solutions for fertilizer applications, including settings for each specific nutrient.
/// </summary>
public class SolutionFinderSettings
{
    /// <summary>
    /// Gets or sets the range factor settings which might include tolerances and other factors that influence the overall optimization constraints.
    /// </summary>
    public RangeFactorSettings RangeFactor { get; }

    /// <summary>
    /// Gets or sets the settings for nitrogen optimization.
    /// </summary>
    public NitrogenSettings Nitrogen { get; }

    /// <summary>
    /// Gets or sets the settings for phosphorus optimization.
    /// </summary>
    public PhosphorusSettings Phosphorus { get; }

    /// <summary>
    /// Gets or sets the settings for potassium optimization.
    /// </summary>
    public PotassiumSettings Potassium { get; }

    /// <summary>
    /// Gets or sets the settings for calcium optimization.
    /// </summary>
    public CalciumSettings Calcium { get; }

    /// <summary>
    /// Gets or sets the settings for magnesium optimization.
    /// </summary>
    public MagnesiumSettings Magnesium { get; }

    /// <summary>
    /// Gets or sets the settings for sulfur optimization.
    /// </summary>
    public SulfurSettings Sulfur { get; }

    /// <summary>
    /// Gets or sets the settings for chlorine optimization.
    /// </summary>
    public ChlorineSettings Chlorine { get; }

    /// <summary>
    /// Gets or sets the settings for iron optimization.
    /// </summary>
    public IronSettings Iron { get; }

    /// <summary>
    /// Gets or sets the settings for copper optimization.
    /// </summary>
    public CopperSettings Copper { get; }

    /// <summary>
    /// Gets or sets the settings for manganese optimization.
    /// </summary>
    public ManganeseSettings Manganese { get; }

    /// <summary>
    /// Gets or sets the settings for zinc optimization.
    /// </summary>
    public ZincSettings Zinc { get; }

    /// <summary>
    /// Gets or sets the settings for boron optimization.
    /// </summary>
    public BoronSettings Boron { get; }

    /// <summary>
    /// Gets or sets the settings for molybdenum optimization.
    /// </summary>
    public MolybdenumSettings Molybdenum { get; }

    /// <summary>
    /// Gets or sets the settings for silicon optimization.
    /// </summary>
    public SiliconSettings Silicon { get; }

    /// <summary>
    /// Gets or sets the settings for selenium optimization.
    /// </summary>
    public SeleniumSettings Selenium { get; }

    /// <summary>
    /// Gets or sets the settings for sodium optimization.
    /// </summary>
    public SodiumSettings Sodium { get; }
    /// <summary>
    /// Initializes a new instance of the <see cref="SolutionFinderSettings"/> class.
    /// </summary>
    /// <param name="rangeFactor">Caps the deviation allowed for every element. The effective tolerance
    /// for an element is the smaller of this factor and that element's own precision setting.</param>
    /// <param name="nitrogen">Nitrogen (N) precision, where 0 leaves the element unconstrained.</param>
    /// <param name="phosphorus">Phosphorus (P) precision, where 0 leaves the element unconstrained.</param>
    /// <param name="potassium">Potassium (K) precision, where 0 leaves the element unconstrained.</param>
    /// <param name="calcium">Calcium (Ca) precision, where 0 leaves the element unconstrained.</param>
    /// <param name="magnesium">Magnesium (Mg) precision, where 0 leaves the element unconstrained.</param>
    /// <param name="sulfur">Sulfur (S) precision, where 0 leaves the element unconstrained.</param>
    /// <param name="iron">Iron (Fe) precision, where 0 leaves the element unconstrained.</param>
    /// <param name="copper">Copper (Cu) precision, where 0 leaves the element unconstrained.</param>
    /// <param name="manganese">Manganese (Mn) precision, where 0 leaves the element unconstrained.</param>
    /// <param name="zinc">Zinc (Zn) precision, where 0 leaves the element unconstrained.</param>
    /// <param name="boron">Boron (B) precision, where 0 leaves the element unconstrained.</param>
    /// <param name="molybdenum">Molybdenum (Mo) precision, where 0 leaves the element unconstrained.</param>
    /// <param name="chlorine">Chlorine (Cl) precision, where 0 leaves the element unconstrained.</param>
    /// <param name="silicon">Silicon (Si) precision, where 0 leaves the element unconstrained.</param>
    /// <param name="selenium">Selenium (Se) precision, where 0 leaves the element unconstrained.</param>
    /// <param name="sodium">Sodium (Na) precision, where 0 leaves the element unconstrained.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
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