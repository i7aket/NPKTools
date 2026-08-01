
namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Represents the target concentrations for various nutrients in parts per million (ppm),
/// along with the target volume of water in which these nutrients should be dissolved.
/// This class is essential for specifying the nutrient requirements that the optimization process aims to achieve.
/// </summary>
public sealed class PpmTarget
{
    /// <summary>
    /// Gets or sets the target ppm for nitrogen.
    /// </summary>
    public NitrogenPpmTarget N { get; }

    /// <summary>
    /// Gets or sets the target ppm for phosphorus.
    /// </summary>
    public PhosphorusPpmTarget P { get; }

    /// <summary>
    /// Gets or sets the target ppm for potassium.
    /// </summary>
    public PotassiumPpmTarget K { get; }

    /// <summary>
    /// Gets or sets the target ppm for calcium.
    /// </summary>
    public CalciumPpmTarget Ca { get; }

    /// <summary>
    /// Gets or sets the target ppm for magnesium.
    /// </summary>
    public MagnesiumPpmTarget Mg { get; }

    /// <summary>
    /// Gets or sets the target ppm for sulfur.
    /// </summary>
    public SulfurPpmTarget S { get; }

    /// <summary>
    /// Gets or sets the target ppm for iron.
    /// </summary>
    public IronPpmTarget Fe { get; }

    /// <summary>
    /// Gets or sets the target ppm for copper.
    /// </summary>
    public CopperPpmTarget Cu { get; }

    /// <summary>
    /// Gets or sets the target ppm for manganese.
    /// </summary>
    public ManganesePpmTarget Mn { get; }

    /// <summary>
    /// Gets or sets the target ppm for zinc.
    /// </summary>
    public ZincPpmTarget Zn { get; }

    /// <summary>
    /// Gets or sets the target ppm for boron.
    /// </summary>
    public BoronPpmTarget B { get; }

    /// <summary>
    /// Gets or sets the target ppm for molybdenum.
    /// </summary>
    public MolybdenumPpmTarget Mo { get; }

    /// <summary>
    /// Gets or sets the target ppm for chlorine.
    /// </summary>
    public ChlorinePpmTarget Cl { get; }

    /// <summary>
    /// Gets or sets the target ppm for silicon.
    /// </summary>
    public SiliconPpmTarget Si { get; }

    /// <summary>
    /// Gets or sets the target ppm for selenium.
    /// </summary>
    public SeleniumPpmTarget Se { get; }

    /// <summary>
    /// Gets or sets the target ppm for sodium.
    /// </summary>
    public SodiumPpmTarget Na { get; }

    /// <summary>
    /// Gets or sets the volume of water in liters for dissolving the fertilizers.
    /// </summary>
    public WaterVolumeLitersPpmTarget Liters { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PpmTarget"/> class.
    /// </summary>
    /// <param name="n">Nitrogen (N) target in ppm.</param>
    /// <param name="p">Phosphorus (P) target in ppm.</param>
    /// <param name="k">Potassium (K) target in ppm.</param>
    /// <param name="ca">Calcium (Ca) target in ppm.</param>
    /// <param name="mg">Magnesium (Mg) target in ppm.</param>
    /// <param name="s">Sulfur (S) target in ppm.</param>
    /// <param name="fe">Iron (Fe) target in ppm.</param>
    /// <param name="cu">Copper (Cu) target in ppm.</param>
    /// <param name="mn">Manganese (Mn) target in ppm.</param>
    /// <param name="zn">Zinc (Zn) target in ppm.</param>
    /// <param name="b">Boron (B) target in ppm.</param>
    /// <param name="mo">Molybdenum (Mo) target in ppm.</param>
    /// <param name="cl">Chlorine (Cl) target in ppm.</param>
    /// <param name="si">Silicon (Si) target in ppm.</param>
    /// <param name="se">Selenium (Se) target in ppm.</param>
    /// <param name="na">Sodium (Na) target in ppm.</param>
    /// <param name="liters">The volume of water, in liters, the targets apply to.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    public PpmTarget(
        NitrogenPpmTarget n,
        PhosphorusPpmTarget p,
        PotassiumPpmTarget k,
        CalciumPpmTarget ca,
        MagnesiumPpmTarget mg,
        SulfurPpmTarget s,
        IronPpmTarget fe,
        CopperPpmTarget cu,
        ManganesePpmTarget mn,
        ZincPpmTarget zn,
        BoronPpmTarget b,
        MolybdenumPpmTarget mo,
        ChlorinePpmTarget cl,
        SiliconPpmTarget si,
        SeleniumPpmTarget se,
        SodiumPpmTarget na,
        WaterVolumeLitersPpmTarget liters)
    {
        ArgumentNullException.ThrowIfNull(n);
        N = n;

        ArgumentNullException.ThrowIfNull(p);
        P = p;

        ArgumentNullException.ThrowIfNull(k);
        K = k;

        ArgumentNullException.ThrowIfNull(ca);
        Ca = ca;

        ArgumentNullException.ThrowIfNull(mg);
        Mg = mg;

        ArgumentNullException.ThrowIfNull(s);
        S = s;

        ArgumentNullException.ThrowIfNull(fe);
        Fe = fe;

        ArgumentNullException.ThrowIfNull(cu);
        Cu = cu;

        ArgumentNullException.ThrowIfNull(mn);
        Mn = mn;

        ArgumentNullException.ThrowIfNull(zn);
        Zn = zn;

        ArgumentNullException.ThrowIfNull(b);
        B = b;

        ArgumentNullException.ThrowIfNull(mo);
        Mo = mo;

        ArgumentNullException.ThrowIfNull(cl);
        Cl = cl;

        ArgumentNullException.ThrowIfNull(si);
        Si = si;

        ArgumentNullException.ThrowIfNull(se);
        Se = se;

        ArgumentNullException.ThrowIfNull(na);
        Na = na;

        ArgumentNullException.ThrowIfNull(liters);
        Liters = liters;
    }
}
