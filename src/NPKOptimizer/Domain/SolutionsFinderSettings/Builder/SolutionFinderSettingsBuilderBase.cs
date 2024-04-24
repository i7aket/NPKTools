using NPKOptimizer.Common;
using NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

namespace NPKOptimizer.Domain.SolutionsFinderSettings.Builder;

public class SolutionFinderSettingsBuilderBase<TBuilder> : BuilderBase<TBuilder>
    where TBuilder : SolutionFinderSettingsBuilderBase<TBuilder>
{
    protected double Accuracy, N, P, K, Ca, Mg, S, Cl, Fe, Cu, Mn, Zn, B, Mo, Si, Se, Na;
    protected override TBuilder Self => (TBuilder)this;

    public override SolutionFinderSettings Build()
    {
        return new SolutionFinderSettings(
            new RangeFactorSettings(Accuracy),
            new NitrogenSettings(N),
            new PhosphorusSettings(P),
            new PotassiumSettings( K),
            new CalciumSettings(Ca),
            new MagnesiumSettings(Mg),
            new SulfurSettings(S),
            new ChlorineSettings(Cl),
            new IronSettings(Fe),
            new CopperSettings(Cu),
            new ManganeseSettings(Mn),
            new ZincSettings(Zn),
            new BoronSettings(B),
            new MolybdenumSettings(Mo),
            new SiliconSettings(Si),
            new SeleniumSettings(Se),
            new SodiumSettings(Na)
        );    
    }
    public TBuilder AddRangeFactor(double value) => SetValue(ref Accuracy, value, nameof(Accuracy));
    public TBuilder AddN(double value) => SetValue(ref N, value, nameof(N));
    public TBuilder AddP(double value) => SetValue(ref P, value, nameof(P));
    public TBuilder AddK(double value) => SetValue(ref K, value, nameof(K));
    public TBuilder AddCa(double value) => SetValue(ref Ca, value, nameof(Ca));
    public TBuilder AddMg(double value) => SetValue(ref Mg, value, nameof(Mg));
    public TBuilder AddS(double value) => SetValue(ref S, value, nameof(S));
    public TBuilder AddCl(double value) => SetValue(ref Cl, value, nameof(Cl));
    public TBuilder AddFe(double value) => SetValue(ref Fe, value, nameof(Fe));
    public TBuilder AddCu(double value) => SetValue(ref Cu, value, nameof(Cu));
    public TBuilder AddMn(double value) => SetValue(ref Mn, value, nameof(Mn));
    public TBuilder AddZn(double value) => SetValue(ref Zn, value, nameof(Zn));
    public TBuilder AddB(double value) => SetValue(ref B, value, nameof(B));
    public TBuilder AddMo(double value) => SetValue(ref Mo, value, nameof(Mo));
    public TBuilder AddSi(double value) => SetValue(ref Si, value, nameof(Si));
    public TBuilder AddSe(double value) => SetValue(ref Se, value, nameof(Se));
    public TBuilder AddNa(double value) => SetValue(ref Na, value, nameof(Na));
}