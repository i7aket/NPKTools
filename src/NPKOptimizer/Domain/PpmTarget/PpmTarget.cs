using NPKOptimizer.Common;
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
    public WaterVolumeLitersPpm Liters { get; set;}

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
        WaterVolumeLitersPpm liters)
    {
        ArgumentNullException.ThrowIfNull(n);
        ArgumentOutOfRangeException.ThrowIfNegative(n.Value);
        N = n;

        ArgumentNullException.ThrowIfNull(p);
        ArgumentOutOfRangeException.ThrowIfNegative(p.Value);
        P = p;

        ArgumentNullException.ThrowIfNull(k);
        ArgumentOutOfRangeException.ThrowIfNegative(k.Value);
        K = k;

        ArgumentNullException.ThrowIfNull(ca);
        ArgumentOutOfRangeException.ThrowIfNegative(ca.Value);
        Ca = ca;

        ArgumentNullException.ThrowIfNull(mg);
        ArgumentOutOfRangeException.ThrowIfNegative(mg.Value);
        Mg = mg;

        ArgumentNullException.ThrowIfNull(s);
        ArgumentOutOfRangeException.ThrowIfNegative(s.Value);
        S = s;

        ArgumentNullException.ThrowIfNull(fe);
        ArgumentOutOfRangeException.ThrowIfNegative(fe.Value);
        Fe = fe;

        ArgumentNullException.ThrowIfNull(cu);
        ArgumentOutOfRangeException.ThrowIfNegative(cu.Value);
        Cu = cu;

        ArgumentNullException.ThrowIfNull(mn);
        ArgumentOutOfRangeException.ThrowIfNegative(mn.Value);
        Mn = mn;

        ArgumentNullException.ThrowIfNull(zn);
        ArgumentOutOfRangeException.ThrowIfNegative(zn.Value);
        Zn = zn;

        ArgumentNullException.ThrowIfNull(b);
        ArgumentOutOfRangeException.ThrowIfNegative(b.Value);
        B = b;

        ArgumentNullException.ThrowIfNull(mo);
        ArgumentOutOfRangeException.ThrowIfNegative(mo.Value);
        Mo = mo;

        ArgumentNullException.ThrowIfNull(cl);
        ArgumentOutOfRangeException.ThrowIfNegative(cl.Value);
        Cl = cl;

        ArgumentNullException.ThrowIfNull(si);
        ArgumentOutOfRangeException.ThrowIfNegative(si.Value);
        Si = si;

        ArgumentNullException.ThrowIfNull(se);
        ArgumentOutOfRangeException.ThrowIfNegative(se.Value);
        Se = se;

        ArgumentNullException.ThrowIfNull(na);
        ArgumentOutOfRangeException.ThrowIfNegative(na.Value);
        Na = na;
        
        ArgumentNullException.ThrowIfNull(liters);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(liters.Value);
        Liters = liters;
    }
}