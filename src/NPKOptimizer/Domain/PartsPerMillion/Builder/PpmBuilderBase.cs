using NPKOptimizer.Common;
using NPKOptimizer.Domain.PartsPerMillion.ValueObjects;

namespace NPKOptimizer.Domain.PartsPerMillion.Builder;

public class PpmBuilderBase<TBuilder> : BuilderBase<TBuilder> where TBuilder : PpmBuilderBase<TBuilder>
{
    protected double Nitrate, Ammonium, Amine, P, K, Ca, Mg, S, Fe, Cu, Mn, Zn, B, Mo, Cl, Si, Se, Na;

    protected override TBuilder Self => (TBuilder)this;

    public override Ppm Build()
    {
        return new Ppm(
            new NitrogenPpm(Nitrate, Ammonium, Amine),
            new PhosphorusPpm(P),
            new PotassiumPpm(K),
            new CalciumPpm(Ca),
            new MagnesiumPpm(Mg),
            new SulfurPpm(S),
            new IronPpm(Fe),
            new CopperPpm(Cu),
            new ManganesePpm(Mn),
            new ZincPpm(Zn),
            new BoronPpm(B),
            new MolybdenumPpm(Mo),
            new ChlorinePpm(Cl),
            new SiliconPpm(Si),
            new SeleniumPpm(Se),
            new SodiumPpm(Na)
        );
    }

    public TBuilder AddNitrate(double value) => SetValue(ref Nitrate, value, nameof(Nitrate));
    public TBuilder AddAmmonium(double value) => SetValue(ref Ammonium, value, nameof(Ammonium));
    public TBuilder AddAmine(double value) => SetValue(ref Amine, value, nameof(Amine));
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
}