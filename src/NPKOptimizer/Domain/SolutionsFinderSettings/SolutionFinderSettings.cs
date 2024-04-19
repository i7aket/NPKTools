using NPKOptimizer.Common;
using NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

// ReSharper disable UnusedMember.Global

namespace NPKOptimizer.Domain.SolutionsFinderSettings;

public sealed record SolutionFinderSettings
{
    public NitrogenSettings N { get; }
    public PhosphorusSettings P { get; }
    public PotassiumSettings K { get; }
    public CalciumSettings Ca { get; }
    public MagnesiumSettings Mg { get; }
    public SulfurSettings S { get; }
    public ChlorineSettings Cl { get; }
    public IronSettings Fe { get; }
    public CopperSettings Cu { get; }
    public ManganeseSettings Mn { get; }
    public ZincSettings Zn { get; }
    public BoronSettings B { get; }
    public MolybdenumSettings Mo { get; }
    public SiliconSettings Si { get; }
    public SeleniumSettings Se { get; }
    public SodiumSettings Na { get; }

    public SolutionFinderSettings(NitrogenSettings n, PhosphorusSettings p, PotassiumSettings k, CalciumSettings ca,
        MagnesiumSettings mg, SulfurSettings s, ChlorineSettings cl, IronSettings fe, CopperSettings cu,
        ManganeseSettings mn, ZincSettings zn, BoronSettings b, MolybdenumSettings mo, SiliconSettings si,
        SeleniumSettings se, SodiumSettings na)
    {
        Validate.NotNull(n);
        Validate.NotNull(p);
        Validate.NotNull(k);
        Validate.NotNull(ca);
        Validate.NotNull(mg);
        Validate.NotNull(s);
        Validate.NotNull(cl);
        Validate.NotNull(fe);
        Validate.NotNull(cu);
        Validate.NotNull(mn);
        Validate.NotNull(zn);
        Validate.NotNull(b);
        Validate.NotNull(mo);
        Validate.NotNull(si);
        Validate.NotNull(se);
        Validate.NotNull(na);

        N = n;
        P = p;
        K = k;
        Ca = ca;
        Mg = mg;
        S = s;
        Cl = cl;
        Fe = fe;
        Cu = cu;
        Mn = mn;
        Zn = zn;
        B = b;
        Mo = mo;
        Si = si;
        Se = se;
        Na = na;
    }

    public static SolutionFinderSettings CreateDefault()
    {
        return new SolutionFinderSettings(
            new NitrogenSettings(),
            new PhosphorusSettings(),
            new PotassiumSettings(),
            new CalciumSettings(),
            new MagnesiumSettings(),
            new SulfurSettings(0),
            new ChlorineSettings(),
            new IronSettings(),
            new CopperSettings(),
            new ManganeseSettings(),
            new ZincSettings(),
            new BoronSettings(),
            new MolybdenumSettings(),
            new SiliconSettings(),
            new SeleniumSettings(),
            new SodiumSettings()
        );
    }

    public static SolutionFinderSettings CreateDefaultMacro()
    {
        return new SolutionFinderSettings(
            new NitrogenSettings(),
            new PhosphorusSettings(),
            new PotassiumSettings(),
            new CalciumSettings(),
            new MagnesiumSettings(),
            new SulfurSettings(),
            new ChlorineSettings(),
            new IronSettings(),
            new CopperSettings(),
            new ManganeseSettings(),
            new ZincSettings(),
            new BoronSettings(),
            new MolybdenumSettings(),
            new SiliconSettings(),
            new SeleniumSettings(),
            new SodiumSettings(0)
        );
    }

    public static SolutionFinderSettings CreateDefaultMacroNoSulfur()
    {
        return new SolutionFinderSettings(
            new NitrogenSettings(),
            new PhosphorusSettings(),
            new PotassiumSettings(),
            new CalciumSettings(),
            new MagnesiumSettings(),
            new SulfurSettings(0),
            new ChlorineSettings(),
            new IronSettings(),
            new CopperSettings(),
            new ManganeseSettings(),
            new ZincSettings(),
            new BoronSettings(),
            new MolybdenumSettings(),
            new SiliconSettings(),
            new SeleniumSettings(),
            new SodiumSettings(0)
        );
    }

    public static SolutionFinderSettings CreateDefaultMicro()
    {
        return new SolutionFinderSettings(
            new NitrogenSettings(0),
            new PhosphorusSettings(0),
            new PotassiumSettings(0),
            new CalciumSettings(0),
            new MagnesiumSettings(0),
            new SulfurSettings(0),
            new ChlorineSettings(),
            new IronSettings(),
            new CopperSettings(),
            new ManganeseSettings(),
            new ZincSettings(),
            new BoronSettings(),
            new MolybdenumSettings(),
            new SiliconSettings(),
            new SeleniumSettings(),
            new SodiumSettings(0)
        );
    }
}