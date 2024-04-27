using NPKOptimizer.Domain.PpmTarget.ValueObjects;

namespace NPKOptimizer.Domain.PpmTarget;

public class PpmTarget
{
    public NitrogenPpmTarget N { get; set;}
    public PhosphorusPpmTarget P { get; set;}
    public PotassiumPpmTarget K { get; set;}
    public CalciumPpmTarget Ca { get; set;}
    public MagnesiumPpmTarget Mg { get; set;}
    public SulfurPpmTarget S { get; set;}
    public IronPpmTarget Fe { get; set;}
    public CopperPpmTarget Cu { get; set;}
    public ManganesePpmTarget Mn { get; set;}
    public ZincPpmTarget Zn { get; set;}
    public BoronPpmTarget B { get; set;}
    public MolybdenumPpmTarget Mo { get; set;}
    public ChlorinePpmTarget Cl { get; set;}
    public SiliconPpmTarget Si { get; set;}
    public SeleniumPpmTarget Se { get; set;}
    public SodiumPpmTarget Na { get; set;}
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