using NPKOptimizer.Common;
using NPKOptimizer.Domain.PpmTarget.ValueObjects;

namespace NPKOptimizer.Domain.PpmTarget.Builder;

public class PpmTargetBuilderBase<TBuilder> : BuilderBase<TBuilder>
    where TBuilder : PpmTargetBuilderBase<TBuilder>
{
    protected double N, P, K, Ca, Mg, S, Fe, Cu, Mn, Zn, B, Mo, Cl, Si, Se, Na;
    protected double Liters = 1;
    protected override TBuilder Self => (TBuilder)this;

    public override PpmTarget Build()
    {
        return new PpmTarget(
            new NitrogenPpmTarget(N),
            new PhosphorusPpmTarget(P),
            new PotassiumPpmTarget(K),
            new CalciumPpmTarget(Ca),
            new MagnesiumPpmTarget(Mg),
            new SulfurPpmTarget(S),
            new IronPpmTarget(Fe),
            new CopperPpmTarget(Cu),
            new ManganesePpmTarget(Mn),
            new ZincPpmTarget(Zn),
            new BoronPpmTarget(B),
            new MolybdenumPpmTarget(Mo),
            new ChlorinePpmTarget(Cl),
            new SiliconPpmTarget(Si),
            new SeleniumPpmTarget(Se),
            new SodiumPpmTarget(Na),
            new WaterVolumeLitersPpmTarget(Liters)
        );
    }

    public TBuilder AddN(double value) => SetValue(ref N, value, nameof(N));
    public TBuilder AddP(double value) => SetValue(ref P, value, nameof(P));
    public TBuilder AddK(double value) => SetValue(ref K, value, nameof(K));
    public TBuilder AddCa(double value) => SetValue(ref Ca, value, nameof(Ca));
    public TBuilder AddMg(double value) => SetValue(ref Mg, value, nameof(Mg));
    public TBuilder AddS(double value) => SetValue(ref S, value, nameof(S));
    public TBuilder AddFe(double value) => SetValue(ref Fe, value, nameof(Fe));
    public TBuilder AddCu(double value) => SetValue(ref Cu, value, nameof(Cu));
    public TBuilder AddMn(double value) => SetValue(ref Mn, value, nameof(Mn));
    public TBuilder AddZn(double value) => SetValue(ref Zn, value, nameof(Zn));
    public TBuilder AddB(double value) => SetValue(ref B, value, nameof(B));
    public TBuilder AddMo(double value) => SetValue(ref Mo, value, nameof(Mo));
    public TBuilder AddCl(double value) => SetValue(ref Cl, value, nameof(Cl));
    public TBuilder AddSi(double value) => SetValue(ref Si, value, nameof(Si));
    public TBuilder AddSe(double value) => SetValue(ref Se, value, nameof(Se));
    public TBuilder AddNa(double value) => SetValue(ref Na, value, nameof(Na));
    public TBuilder AddLitters(double value) => SetValue(ref Liters, value, nameof(Liters));
}