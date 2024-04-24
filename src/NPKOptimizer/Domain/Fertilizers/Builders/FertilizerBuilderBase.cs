using NPKOptimizer.Common;
using NPKOptimizer.Domain.Fertilizers.ValueObjects;

namespace NPKOptimizer.Domain.Fertilizers.Builders;

public class FertilizerBuilderBase<TBuilder> : BuilderBase<TBuilder> where TBuilder : FertilizerBuilderBase<TBuilder>
{
    protected Guid Id = Guid.NewGuid();
    protected double Weight;
    protected double Price  = 1;
    protected double No3, Nh4, Nh2, P, K;
    protected double CaNonChelated, CaEdta, MgNonChelated, MgEdta, S;
    protected double FeNonChelated, FeEdta, FeDtpa, FeEddha, FeHbed, FeOrthoPart;
    protected double CuNonChelated, CuEdta, MnNonChelated, MnEdta, ZnNonChelated, ZnEdta;
    protected double B, Mo, Cl, Si, Se, Na;
    
    protected override TBuilder Self => (TBuilder)this;
    public override Fertilizer Build()
    {
        return new Fertilizer(
            new FertilizerReferenceId(Id),
            new FertilizerWeight(Weight),
            new FertilizerPrice(Price),
            new FertilizerNitrogen(No3, Nh4, Nh2),
            new FertilizerPhosphorus(P),
            new FertilizerPotassium(K),
            new FertilizerCalcium(CaNonChelated, CaEdta),
            new FertilizerMagnesium(MgNonChelated, MgEdta),
            new FertilizerSulfur(S),
            new FertilizerIron(FeNonChelated, FeEdta, FeDtpa, FeEddha, FeHbed, FeOrthoPart),
            new FertilizerCopper(CuNonChelated, CuEdta),
            new FertilizerManganese(MnNonChelated, MnEdta),
            new FertilizerZinc(ZnNonChelated, ZnEdta),
            new FertilizerBoron(B),
            new FertilizerMolybdenum(Mo),
            new FertilizerChlorine(Cl),
            new FertilizerSilicon(Si),
            new FertilizerSelenium(Se),
            new FertilizerSodium(Na)
        );
    }

    public TBuilder AddId(Guid value) => SetValue(ref Id, value, nameof(Id));
    public TBuilder AddPrice(double value) => SetValue(ref Price, value, nameof(Price));
    public TBuilder AddNo3(double value) => SetValue(ref No3, value, nameof(No3));
    public TBuilder AddNh4(double value) => SetValue(ref Nh4, value, nameof(Nh4));
    public TBuilder AddNh2(double value) => SetValue(ref Nh2, value, nameof(Nh2));
    public TBuilder AddP(double value) => SetValue(ref P, value, nameof(P));
    public TBuilder AddK(double value) => SetValue(ref K, value, nameof(K));
    public TBuilder AddCaNonChelated(double value) => SetValue(ref CaNonChelated, value, nameof(CaNonChelated));
    public TBuilder AddCaEdta(double value) => SetValue(ref CaEdta, value, nameof(CaEdta));
    public TBuilder AddMgNonChelated(double value) => SetValue(ref MgNonChelated, value, nameof(MgNonChelated));
    public TBuilder AddMgEdta(double value) => SetValue(ref MgEdta, value, nameof(MgEdta));
    public TBuilder AddS(double value) => SetValue(ref S, value, nameof(S));
    public TBuilder AddFeNonChelated(double value) => SetValue(ref FeNonChelated, value, nameof(FeNonChelated));
    public TBuilder AddFeEdta(double value) => SetValue(ref FeEdta, value, nameof(FeEdta));
    public TBuilder AddFeDtpa(double value) => SetValue(ref FeDtpa, value, nameof(FeDtpa));
    public TBuilder AddFeEddha(double value) => SetValue(ref FeEddha, value, nameof(FeEddha));
    public TBuilder AddFeHbed(double value) => SetValue(ref FeHbed, value, nameof(FeHbed));
    public TBuilder AddFeOrthoPart(double value) => SetValue(ref FeOrthoPart, value, nameof(FeOrthoPart));
    public TBuilder AddCuNonChelated(double value) => SetValue(ref CuNonChelated, value, nameof(CuNonChelated));
    public TBuilder AddCuEdta(double value) => SetValue(ref CuEdta, value, nameof(CuEdta));
    public TBuilder AddMnNonChelated(double value) => SetValue(ref MnNonChelated, value, nameof(MnNonChelated));
    public TBuilder AddMnEdta(double value) => SetValue(ref MnEdta, value, nameof(MnEdta));
    public TBuilder AddZnNonChelated(double value) => SetValue(ref ZnNonChelated, value, nameof(ZnNonChelated));
    public TBuilder AddZnEdta(double value) => SetValue(ref ZnEdta, value, nameof(ZnEdta));
    public TBuilder AddB(double value) => SetValue(ref B, value, nameof(B));
    public TBuilder AddMo(double value) => SetValue(ref Mo, value, nameof(Mo));
    public TBuilder AddCl(double value) => SetValue(ref Cl, value, nameof(Cl));
    public TBuilder AddSi(double value) => SetValue(ref Si, value, nameof(Si));
    public TBuilder AddSe(double value) => SetValue(ref Se, value, nameof(Se));
    public TBuilder AddNa(double value) => SetValue(ref Na, value, nameof(Na));
    public TBuilder AddWeight(double value) => SetValue(ref Weight, value, nameof(Weight));
}
