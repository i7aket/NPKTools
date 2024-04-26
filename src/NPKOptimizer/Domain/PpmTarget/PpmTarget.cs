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
        ThrowIf.LowerThan(n.Value, 0);
        N = n;

        ArgumentNullException.ThrowIfNull(p);
        ThrowIf.LowerThan(p.Value, 0);
        P = p;

        ArgumentNullException.ThrowIfNull(k);
        ThrowIf.LowerThan(k.Value, 0);
        K = k;

        ArgumentNullException.ThrowIfNull(ca);
        ThrowIf.LowerThan(ca.Value, 0);
        Ca = ca;

        ArgumentNullException.ThrowIfNull(mg);
        ThrowIf.LowerThan(mg.Value, 0);
        Mg = mg;

        ArgumentNullException.ThrowIfNull(s);
        ThrowIf.LowerThan(s.Value, 0);
        S = s;

        ArgumentNullException.ThrowIfNull(fe);
        ThrowIf.LowerThan(fe.Value, 0);
        Fe = fe;

        ArgumentNullException.ThrowIfNull(cu);
        ThrowIf.LowerThan(cu.Value, 0);
        Cu = cu;

        ArgumentNullException.ThrowIfNull(mn);
        ThrowIf.LowerThan(mn.Value, 0);
        Mn = mn;

        ArgumentNullException.ThrowIfNull(zn);
        ThrowIf.LowerThan(zn.Value, 0);
        Zn = zn;

        ArgumentNullException.ThrowIfNull(b);
        ThrowIf.LowerThan(b.Value, 0);
        B = b;

        ArgumentNullException.ThrowIfNull(mo);
        ThrowIf.LowerThan(mo.Value, 0);
        Mo = mo;

        ArgumentNullException.ThrowIfNull(cl);
        ThrowIf.LowerThan(cl.Value, 0);
        Cl = cl;

        ArgumentNullException.ThrowIfNull(si);
        ThrowIf.LowerThan(si.Value, 0);
        Si = si;

        ArgumentNullException.ThrowIfNull(se);
        ThrowIf.LowerThan(se.Value, 0);
        Se = se;

        ArgumentNullException.ThrowIfNull(na);
        ThrowIf.LowerThan(na.Value, 0);
        Na = na;
        
        ArgumentNullException.ThrowIfNull(liters);
        ThrowIf.LowerThanOrEqual(liters.Value, 0);
        Liters = liters;
    }
}