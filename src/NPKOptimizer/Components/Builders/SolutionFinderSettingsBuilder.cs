using NPKOptimizer.Common;
using NPKOptimizer.Domain.SolutionsFinderSettings;
using NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace NPKOptimizer.Components.Builders;

public class SolutionFinderSettingsBuilder : BuilderBase<SolutionFinderSettingsBuilder>
{
    private double _n, _p, _k, _ca, _mg, _s, _cl, _fe, _cu, _mn, _zn, _b, _mo, _si, _se, _na;
    
    public SolutionFinderSettings Build()
    {
        return new SolutionFinderSettings(
            new NitrogenSettings(_n),
            new PhosphorusSettings(_p),
            new PotassiumSettings( _k),
            new CalciumSettings(_ca),
            new MagnesiumSettings(_mg),
            new SulfurSettings(_s),
            new ChlorineSettings(_cl),
            new IronSettings(_fe),
            new CopperSettings(_cu),
            new ManganeseSettings(_mn),
            new ZincSettings(_zn),
            new BoronSettings(_b),
            new MolybdenumSettings(_mo),
            new SiliconSettings(_si),
            new SeleniumSettings(_se),
            new SodiumSettings(_na)
        );
    }
    
    public SolutionFinderSettingsBuilder N(double value) => SetElementValue(ref _n, value, nameof(_n));
    public SolutionFinderSettingsBuilder P(double value) => SetElementValue(ref _p, value, nameof(_p));
    public SolutionFinderSettingsBuilder K(double value) => SetElementValue(ref _k, value, nameof(_k));
    public SolutionFinderSettingsBuilder Ca(double value) => SetElementValue(ref _ca, value, nameof(_ca));
    public SolutionFinderSettingsBuilder Mg(double value) => SetElementValue(ref _mg, value, nameof(_mg));
    public SolutionFinderSettingsBuilder S(double value) => SetElementValue(ref _s, value, nameof(_s));
    public SolutionFinderSettingsBuilder Cl(double value) => SetElementValue(ref _cl, value, nameof(_cl));
    public SolutionFinderSettingsBuilder Fe(double value) => SetElementValue(ref _fe, value, nameof(_fe));
    public SolutionFinderSettingsBuilder Cu(double value) => SetElementValue(ref _cu, value, nameof(_cu));
    public SolutionFinderSettingsBuilder Mn(double value) => SetElementValue(ref _mn, value, nameof(_mn));
    public SolutionFinderSettingsBuilder Zn(double value) => SetElementValue(ref _zn, value, nameof(_zn));
    public SolutionFinderSettingsBuilder B(double value) => SetElementValue(ref _b, value, nameof(_b));
    public SolutionFinderSettingsBuilder M(double value) => SetElementValue(ref _mo, value, nameof(_mo));
    public SolutionFinderSettingsBuilder Si(double value) => SetElementValue(ref _si, value, nameof(_si));
    public SolutionFinderSettingsBuilder Se(double value) => SetElementValue(ref _se, value, nameof(_se));
    public SolutionFinderSettingsBuilder Na(double value) => SetElementValue(ref _na, value, nameof(_na));
}