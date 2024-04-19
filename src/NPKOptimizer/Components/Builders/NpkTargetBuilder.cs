using NPKOptimizer.Common;
using NPKOptimizer.Domain.NPK;
using NPKOptimizer.Domain.NPK.ValueObjects;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace NPKOptimizer.Components.Builders;

public class NpkTargetBuilder : BuilderBase<NpkTargetBuilder>
{
    private double _n, _p, _k, _ca, _mg, _s, _fe, _cu, _mn, _zn, _b, _mo, _cl, _si, _se, _na;

    public NpkTarget Build()
    {
        return new NpkTarget(
            new NitrogenNpkTarget(_n),
            new PhosphorusNpkTarget(_p),
            new PotassiumNpkTarget(_k),
            new CalciumNpkTarget(_ca),
            new MagnesiumNpkTarget(_mg),
            new SulfurNpkTarget(_s),
            new IronNpkTarget(_fe),
            new CopperNpkTarget(_cu),
            new ManganeseNpkTarget(_mn),
            new ZincNpkTarget(_zn),
            new BoronNpkTarget(_b),
            new MolybdenumNpkTarget(_mo),
            new ChlorineNpkTarget(_cl),
            new SiliconNpkTarget(_si),
            new SeleniumNpkTarget(_se),
            new SodiumNpkTarget(_na)
        );
    }
    
    public NpkTargetBuilder N(double value) => SetElementValue(ref _n, value, nameof(_n));
    public NpkTargetBuilder P(double value) => SetElementValue(ref _p, value, nameof(_p));
    public NpkTargetBuilder K(double value) => SetElementValue(ref _k, value, nameof(_k));
    public NpkTargetBuilder Ca(double value) => SetElementValue(ref _ca, value, nameof(_ca));
    public NpkTargetBuilder Mg(double value) => SetElementValue(ref _mg, value, nameof(_mg));
    public NpkTargetBuilder S(double value) => SetElementValue(ref _s, value, nameof(_s));
    public NpkTargetBuilder Fe(double value) => SetElementValue(ref _fe, value, nameof(_fe));
    public NpkTargetBuilder Cu(double value) => SetElementValue(ref _cu, value, nameof(_cu));
    public NpkTargetBuilder Mn(double value) => SetElementValue(ref _mn, value, nameof(_mn));
    public NpkTargetBuilder Zn(double value) => SetElementValue(ref _zn, value, nameof(_zn));
    public NpkTargetBuilder B(double value) => SetElementValue(ref _b, value, nameof(_b));
    public NpkTargetBuilder Mo(double value) => SetElementValue(ref _mo, value, nameof(_mo));
    public NpkTargetBuilder Cl(double value) => SetElementValue(ref _cl, value, nameof(_cl));
    public NpkTargetBuilder Si(double value) => SetElementValue(ref _si, value, nameof(_si));
    public NpkTargetBuilder Se(double value) => SetElementValue(ref _se, value, nameof(_se));
    public NpkTargetBuilder Na(double value) => SetElementValue(ref _na, value, nameof(_na));
}
