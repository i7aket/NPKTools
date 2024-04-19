using NPKOptimizer.Common;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.Fertilizers.ValueObjects;
using NPKOptimizer.Enums;

// ReSharper disable UnusedMember.Global

namespace NPKOptimizer.Components.Builders;

public sealed class FertilizerBuilder : BuilderBase<FertilizerBuilder>
{
    private Guid _id;
    private string _name = null!;
    private string? _formula;
    private double _price = 1;
    private double _weight;
    private ConcentrateType _concentrateType;
    private double _no3, _nh4, _nh2, _p, _k;
    private double _caNonChelated, _caEdta, _mgNonChelated, _mgEdta, _s;
    private double _feNonChelated, _feEdta, _feDtpa, _feEddha, _feHbed, _feOrthoPart;
    private double _cuNonChelated, _cuEdta, _mnNonChelated, _mnEdta, _znNonChelated, _znEdta;
    private double _b, _mo, _cl, _si, _se, _na;
    private FertilizerComposition? _fertilizerComposition;

    public Fertilizer Build()
    {
        return new Fertilizer(
            new FertilizerId(_id),
            new FertilizerName(_name),
            new FertilizerFormula(_formula),
            new FertilizerPrice(_price),
            new FertilizerWeight(_weight),
            _concentrateType,
            new FertilizerNitrogen(_no3, _nh4, _nh2),
            new FertilizerPhosphorus(_p),
            new FertilizerPotassium(_k),
            new FertilizerCalcium(_caNonChelated, _caEdta),
            new FertilizerMagnesium(_mgNonChelated, _mgEdta),
            new FertilizerSulfur(_s),
            new FertilizerIron(_feNonChelated, _feEdta, _feDtpa, _feEddha, _feHbed, _feOrthoPart),
            new FertilizerCopper(_cuNonChelated, _cuEdta),
            new FertilizerManganese(_mnNonChelated, _mnEdta),
            new FertilizerZinc(_znNonChelated, _znEdta),
            new FertilizerBoron(_b),
            new FertilizerMolybdenum(_mo),
            new FertilizerChlorine(_cl),
            new FertilizerSilicon(_si),
            new FertilizerSelenium(_se),
            new FertilizerSodium(_na),
            _fertilizerComposition);
    }
    
    public FertilizerBuilder Id(Guid value) => SetElementValue(ref _id, value, nameof(_id));
    public FertilizerBuilder Name(string value) => SetElementValue(ref _name, value, nameof(_name));
    public FertilizerBuilder Formula(string? value) => SetElementValue(ref _formula, value, nameof(_formula));
    public FertilizerBuilder ConcentrateType(ConcentrateType value) => SetElementValue(ref _concentrateType, value, nameof(_concentrateType));
    public FertilizerBuilder Price(double value) => SetElementValue(ref _price, value, nameof(_price));
    public FertilizerBuilder Weight(double value) => SetElementValue(ref _weight, value, nameof(_weight));
    public FertilizerBuilder No3(double value) => SetElementValue(ref _no3, value, nameof(_no3));
    public FertilizerBuilder Nh4(double value) => SetElementValue(ref _nh4, value, nameof(_nh4));
    public FertilizerBuilder Nh2(double value) => SetElementValue(ref _nh2, value, nameof(_nh2));
    public FertilizerBuilder P(double value) => SetElementValue(ref _p, value, nameof(_p));
    public FertilizerBuilder K(double value) => SetElementValue(ref _k, value, nameof(_k));
    public FertilizerBuilder CaNonChelated(double value) => SetElementValue(ref _caNonChelated, value, nameof(_caNonChelated));
    public FertilizerBuilder CaEdta(double value) => SetElementValue(ref _caEdta, value, nameof(_caEdta));
    public FertilizerBuilder MgNonChelated(double value) => SetElementValue(ref _mgNonChelated, value, nameof(_mgNonChelated));
    public FertilizerBuilder MgEdta(double value) => SetElementValue(ref _mgEdta, value, nameof(_mgEdta));
    public FertilizerBuilder S(double value) => SetElementValue(ref _s, value, nameof(_s));
    public FertilizerBuilder FeNonChelated(double value) => SetElementValue(ref _feNonChelated, value, nameof(_feNonChelated));
    public FertilizerBuilder FeEdta(double value) => SetElementValue(ref _feEdta, value, nameof(_feEdta));
    public FertilizerBuilder FeDtpa(double value) => SetElementValue(ref _feDtpa, value, nameof(_feDtpa));
    public FertilizerBuilder FeEddha(double value) => SetElementValue(ref _feEddha, value, nameof(_feEddha));
    public FertilizerBuilder FeHbed(double value) => SetElementValue(ref _feHbed, value, nameof(_feHbed));
    public FertilizerBuilder FeOrthoPart(double value) => SetElementValue(ref _feOrthoPart, value, nameof(_feOrthoPart));
    public FertilizerBuilder CuNonChelated(double value) => SetElementValue(ref _cuNonChelated, value, nameof(_cuNonChelated));
    public FertilizerBuilder CuEdta(double value) => SetElementValue(ref _cuEdta, value, nameof(_cuEdta));
    public FertilizerBuilder MnNonChelated(double value) => SetElementValue(ref _mnNonChelated, value, nameof(_mnNonChelated));
    public FertilizerBuilder MnEdta(double value) => SetElementValue(ref _mnEdta, value, nameof(_mnEdta));
    public FertilizerBuilder ZnNonChelated(double value) => SetElementValue(ref _znNonChelated, value, nameof(_znNonChelated));
    public FertilizerBuilder ZnEdta(double value) => SetElementValue(ref _znEdta, value, nameof(_znEdta));
    public FertilizerBuilder B(double value) => SetElementValue(ref _b, value, nameof(_b));
    public FertilizerBuilder Mo(double value) => SetElementValue(ref _mo, value, nameof(_mo));
    public FertilizerBuilder Cl(double value) => SetElementValue(ref _cl, value, nameof(_cl));
    public FertilizerBuilder Si(double value) => SetElementValue(ref _si, value, nameof(_si));
    public FertilizerBuilder Se(double value) => SetElementValue(ref _se, value, nameof(_se));
    public FertilizerBuilder Na(double value) => SetElementValue(ref _na, value, nameof(_na));
    public FertilizerBuilder Composition(FertilizerComposition value) =>
        SetElementValue(ref _fertilizerComposition, value, nameof(_fertilizerComposition));
}