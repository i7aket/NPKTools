using NPKOptimizer.Common;
using NPKOptimizer.Domain.NPK.ValueObjects;

namespace NPKOptimizer.Domain.NPK;

public record NpkTarget
{
    public NitrogenNpkTarget N { get; }
    public PhosphorusNpkTarget P { get; }
    public PotassiumNpkTarget K { get; }
    public CalciumNpkTarget Ca { get; }
    public MagnesiumNpkTarget Mg { get; }
    public SulfurNpkTarget S { get; }
    public IronNpkTarget Fe { get; }
    public CopperNpkTarget Cu { get; }
    public ManganeseNpkTarget Mn { get; }
    public ZincNpkTarget Zn { get; }
    public BoronNpkTarget B { get; }
    public MolybdenumNpkTarget Mo { get; }
    public ChlorineNpkTarget Cl { get; }
    public SiliconNpkTarget Si { get; }
    public SeleniumNpkTarget Se { get; }
    public SodiumNpkTarget Na { get; }

    public NpkTarget(NitrogenNpkTarget n, PhosphorusNpkTarget p, PotassiumNpkTarget k, CalciumNpkTarget ca, MagnesiumNpkTarget mg, SulfurNpkTarget s, IronNpkTarget fe, CopperNpkTarget cu,
        ManganeseNpkTarget mn, ZincNpkTarget zn, BoronNpkTarget b, MolybdenumNpkTarget mo, ChlorineNpkTarget cl, SiliconNpkTarget si, SeleniumNpkTarget se, SodiumNpkTarget na)
    {
        Validate.NotNull(n);
        N = n;
        Validate.NotNull(p);
        P = p;
        Validate.NotNull(k);
        K = k;
        Validate.NotNull(ca);
        Ca = ca;
        Validate.NotNull(mg);
        Mg = mg;
        Validate.NotNull(s);
        S = s;
        Validate.NotNull(fe);
        Fe = fe;
        Validate.NotNull(cu);
        Cu = cu;
        Validate.NotNull(mn);
        Mn = mn;
        Validate.NotNull(zn);
        Zn = zn;
        Validate.NotNull(b);
        B = b;
        Validate.NotNull(mo);
        Mo = mo;
        Validate.NotNull(cl);
        Cl = cl;
        Validate.NotNull(si);
        Si = si;
        Validate.NotNull(se);
        Se = se;
        Validate.NotNull(na);
        Na = na;
    }
}