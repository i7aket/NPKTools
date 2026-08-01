namespace SYT.NPKTools.Nutrients;

/// <summary>
/// The nutrients already present in the source water, in parts per million.
/// </summary>
/// <remarks>
/// <para>
/// Tap and well water is rarely blank: calcium, magnesium, sulfur and sodium are routinely present in
/// quantities that matter, and ignoring them makes every calculated mix wrong by that amount. Subtract
/// a profile from a target with <see cref="PpmTargetExtensions.AdjustFor"/> before optimizing.
/// </para>
/// <para>
/// Unlike <see cref="Ppm"/> this carries no water volume, because a water analysis is a concentration
/// and holds regardless of how much of it you use. It reuses the same value objects, so the numbers on
/// a laboratory report go in unchanged.
/// </para>
/// <para>
/// Build one with <see cref="WaterProfileBuilder"/>. An all-zero profile — reverse osmosis or
/// distilled — leaves a target untouched.
/// </para>
/// </remarks>
public sealed class WaterProfile
{
    /// <summary>
    /// Gets the nitrogen already in the water. Water analyses normally report nitrate; ammonium and
    /// amine are unusual in a municipal supply but are representable.
    /// </summary>
    public NitrogenPpm Nitrogen { get; }

    /// <summary>
    /// Gets the phosphorus already in the water.
    /// </summary>
    public PhosphorusPpm Phosphorus { get; }

    /// <summary>
    /// Gets the potassium already in the water.
    /// </summary>
    public PotassiumPpm Potassium { get; }

    /// <summary>
    /// Gets the calcium already in the water. Together with magnesium this is what "hard water" means,
    /// and it is the most common reason a target cannot be met without dilution.
    /// </summary>
    public CalciumPpm Calcium { get; }

    /// <summary>
    /// Gets the magnesium already in the water.
    /// </summary>
    public MagnesiumPpm Magnesium { get; }

    /// <summary>
    /// Gets the sulfur already in the water, as elemental S rather than sulfate.
    /// </summary>
    public SulfurPpm Sulfur { get; }

    /// <summary>
    /// Gets the iron already in the water.
    /// </summary>
    public IronPpm Iron { get; }

    /// <summary>
    /// Gets the copper already in the water.
    /// </summary>
    public CopperPpm Copper { get; }

    /// <summary>
    /// Gets the manganese already in the water.
    /// </summary>
    public ManganesePpm Manganese { get; }

    /// <summary>
    /// Gets the zinc already in the water.
    /// </summary>
    public ZincPpm Zinc { get; }

    /// <summary>
    /// Gets the boron already in the water.
    /// </summary>
    public BoronPpm Boron { get; }

    /// <summary>
    /// Gets the molybdenum already in the water.
    /// </summary>
    public MolybdenumPpm Molybdenum { get; }

    /// <summary>
    /// Gets the chlorine already in the water. Along with sodium this is what accumulates and is the
    /// usual reason growers move to reverse osmosis.
    /// </summary>
    public ChlorinePpm Chlorine { get; }

    /// <summary>
    /// Gets the silicon already in the water.
    /// </summary>
    public SiliconPpm Silicon { get; }

    /// <summary>
    /// Gets the selenium already in the water.
    /// </summary>
    public SeleniumPpm Selenium { get; }

    /// <summary>
    /// Gets the sodium already in the water.
    /// </summary>
    public SodiumPpm Sodium { get; }

    /// <summary>
    /// A profile with every value zero: reverse osmosis, distilled, or rain water. Subtracting it
    /// leaves a target unchanged.
    /// </summary>
    public static readonly WaterProfile Pure = new WaterProfileBuilder().Build();

    /// <summary>
    /// Initializes a new instance of the <see cref="WaterProfile"/> class.
    /// </summary>
    /// <param name="nitrogen">Nitrogen already in the water.</param>
    /// <param name="phosphorus">Phosphorus already in the water.</param>
    /// <param name="potassium">Potassium already in the water.</param>
    /// <param name="calcium">Calcium already in the water.</param>
    /// <param name="magnesium">Magnesium already in the water.</param>
    /// <param name="sulfur">Sulfur already in the water.</param>
    /// <param name="iron">Iron already in the water.</param>
    /// <param name="copper">Copper already in the water.</param>
    /// <param name="manganese">Manganese already in the water.</param>
    /// <param name="zinc">Zinc already in the water.</param>
    /// <param name="boron">Boron already in the water.</param>
    /// <param name="molybdenum">Molybdenum already in the water.</param>
    /// <param name="chlorine">Chlorine already in the water.</param>
    /// <param name="silicon">Silicon already in the water.</param>
    /// <param name="selenium">Selenium already in the water.</param>
    /// <param name="sodium">Sodium already in the water.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    public WaterProfile(
        NitrogenPpm nitrogen,
        PhosphorusPpm phosphorus,
        PotassiumPpm potassium,
        CalciumPpm calcium,
        MagnesiumPpm magnesium,
        SulfurPpm sulfur,
        IronPpm iron,
        CopperPpm copper,
        ManganesePpm manganese,
        ZincPpm zinc,
        BoronPpm boron,
        MolybdenumPpm molybdenum,
        ChlorinePpm chlorine,
        SiliconPpm silicon,
        SeleniumPpm selenium,
        SodiumPpm sodium)
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
    }
}
