using NPKOptimizer.Common;
using NPKOptimizer.Domain.PartsPerMillion;
using NPKOptimizer.Domain.PartsPerMillion.ValueObjects;

// ReSharper disable UnusedMember.Global

namespace NPKOptimizer.Components.Builders;

public class PpmBuilder : BuilderBase<PpmBuilder>
{
    private double _nitrate, _ammonium, _amine;
    private double _p, _k, _ca, _mg, _s, _fe, _cu, _mn, _zn, _b, _mo, _cl, _si, _se, _na, _totalWeight;

    public Ppm Build()
    {
        return new Ppm(
            
            new NitrogenPpm(_nitrate, _ammonium, _amine),
            new PhosphorusPpm(_p),
            new PotassiumPpm(_k),
            new CalciumPpm(_ca),
            new MagnesiumPpm(_mg),
            new SulfurPpm(_s),
            new IronPpm(_fe),
            new CopperPpm(_cu),
            new ManganesePpm(_mn),
            new ZincPpm(_zn),
            new BoronPpm(_b),
            new MolybdenumPpm(_mo),
            new ChlorinePpm(_cl),
            new SiliconPpm(_si),
            new SeleniumPpm(_se),
            new SodiumPpm(_na),
            new TotalMassPpm(_totalWeight)
        );
    }
    public PpmBuilder Nitrate(double value) => SetElementValue(ref _nitrate, value, nameof(_nitrate));
    public PpmBuilder Ammonium(double value) => SetElementValue(ref _ammonium, value, nameof(_ammonium));
    public PpmBuilder Amine(double value) => SetElementValue(ref _amine, value, nameof(_amine));
    public PpmBuilder P(double value) => SetElementValue(ref _p, value, nameof(_p));
    public PpmBuilder K(double value) => SetElementValue(ref _k, value, nameof(_k));
    public PpmBuilder Ca(double value) => SetElementValue(ref _ca, value, nameof(_ca));
    public PpmBuilder Mg(double value) => SetElementValue(ref _mg, value, nameof(_mg));
    public PpmBuilder S(double value) => SetElementValue(ref _s, value, nameof(_s));
    public PpmBuilder Fe(double value) => SetElementValue(ref _fe, value, nameof(_fe));
    public PpmBuilder Cu(double value) => SetElementValue(ref _cu, value, nameof(_cu));
    public PpmBuilder Mn(double value) => SetElementValue(ref _mn, value, nameof(_mn));
    public PpmBuilder Zn(double value) => SetElementValue(ref _zn, value, nameof(_zn));
    public PpmBuilder B(double value) => SetElementValue(ref _b, value, nameof(_b));
    public PpmBuilder Mo(double value) => SetElementValue(ref _mo, value, nameof(_mo));
    public PpmBuilder Cl(double value) => SetElementValue(ref _cl, value, nameof(_cl));
    public PpmBuilder Si(double value) => SetElementValue(ref _si, value, nameof(_si));
    public PpmBuilder Se(double value) => SetElementValue(ref _se, value, nameof(_se));
    public PpmBuilder Na(double value) => SetElementValue(ref _na, value, nameof(_na));
    public PpmBuilder TotalWeight(double value) => SetElementValue(ref _totalWeight, value, nameof(_totalWeight));
}
