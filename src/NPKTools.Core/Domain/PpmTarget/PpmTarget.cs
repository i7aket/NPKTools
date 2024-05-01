using NPKTools.Core.Domain.PpmTarget.ValueObjects;

namespace NPKTools.Core.Domain.PpmTarget;

/// <summary>
/// Represents the target concentrations for various nutrients in parts per million (ppm),
/// along with the target volume of water in which these nutrients should be dissolved.
/// This class is essential for specifying the nutrient requirements that the optimization process aims to achieve.
/// </summary>
public class PpmTarget
{
    /// <summary>
    /// Gets or sets the target ppm for nitrogen.
    /// </summary>
    public NitrogenPpmTarget N { get; set;}

    /// <summary>
    /// Gets or sets the target ppm for phosphorus.
    /// </summary>
    public PhosphorusPpmTarget P { get; set;}

    /// <summary>
    /// Gets or sets the target ppm for potassium.
    /// </summary>
    public PotassiumPpmTarget K { get; set;}

    /// <summary>
    /// Gets or sets the target ppm for calcium.
    /// </summary>
    public CalciumPpmTarget Ca { get; set;}

    /// <summary>
    /// Gets or sets the target ppm for magnesium.
    /// </summary>
    public MagnesiumPpmTarget Mg { get; set;}

    /// <summary>
    /// Gets or sets the target ppm for sulfur.
    /// </summary>
    public SulfurPpmTarget S { get; set;}

    /// <summary>
    /// Gets or sets the target ppm for iron.
    /// </summary>
    public IronPpmTarget Fe { get; set;}

    /// <summary>
    /// Gets or sets the target ppm for copper.
    /// </summary>
    public CopperPpmTarget Cu { get; set;}

    /// <summary>
    /// Gets or sets the target ppm for manganese.
    /// </summary>
    public ManganesePpmTarget Mn { get; set;}

    /// <summary>
    /// Gets or sets the target ppm for zinc.
    /// </summary>
    public ZincPpmTarget Zn { get; set;}

    /// <summary>
    /// Gets or sets the target ppm for boron.
    /// </summary>
    public BoronPpmTarget B { get; set;}

    /// <summary>
    /// Gets or sets the target ppm for molybdenum.
    /// </summary>
    public MolybdenumPpmTarget Mo { get; set;}

    /// <summary>
    /// Gets or sets the target ppm for chlorine.
    /// </summary>
    public ChlorinePpmTarget Cl { get; set;}

    /// <summary>
    /// Gets or sets the target ppm for silicon.
    /// </summary>
    public SiliconPpmTarget Si { get; set;}

    /// <summary>
    /// Gets or sets the target ppm for selenium.
    /// </summary>
    public SeleniumPpmTarget Se { get; set;}

    /// <summary>
    /// Gets or sets the target ppm for sodium.
    /// </summary>
    public SodiumPpmTarget Na { get; set;}

    /// <summary>
    /// Gets or sets the volume of water in liters for dissolving the fertilizers.
    /// </summary>
    public WaterVolumeLitersPpmTarget Liters { get; set;}

    public PpmTarget(){}
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