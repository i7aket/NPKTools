using NPKOptimizer.Common;
using NPKOptimizer.Domain.Collections;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.Fertilizers.Builders;

namespace NPKOptimizerCalc.Builders;

public class FertilizerCollectionBuilder
{
    private readonly HashSet<FertilizerOptimizationModel> _selectedFertilizer = new (new FertilizerAttributesComparer());
    public FertilizerCollectionBuilder Add(FertilizerOptimizationModel fertilizer)
    {
        ThrowIf.Duplicate(_selectedFertilizer, fertilizer);
        return this;
    }
    
    public FertilizerCollectionBuilder CalciumNitrate() => Add(new FertilizerBuilder()
        .AddCaNonChelated(16.972)
        .AddNo3(11.863).Build());

    public FertilizerCollectionBuilder K() => Add(new FertilizerBuilder()
        .AddNo3(13.854)
        .AddK(38.672).Build());

    public FertilizerCollectionBuilder Mgs() => Add(new FertilizerBuilder()
        .AddMgNonChelated(9.861)
        .AddS(13.008).Build());

    public FertilizerCollectionBuilder Mkp() => Add(new FertilizerBuilder()
        .AddP(22.761)
        .AddK(28.731).Build());

    public FertilizerCollectionBuilder Calc() => Add(new FertilizerBuilder()
        .AddCaNonChelated(18.295)
        .AddCl(32.364).Build());

    public FertilizerCollectionBuilder Sop() => Add(new FertilizerBuilder()
        .AddK(44.875)
        .AddS(18.399).Build());

    public FertilizerCollectionBuilder Dkp() => Add(new FertilizerBuilder()
        .AddP(17.783)
        .AddK(44.896).Build());

    public FertilizerCollectionBuilder Mag() => Add(new FertilizerBuilder()
        .AddMgNonChelated(9.479)
        .AddNo3(10.926).Build());

    public FertilizerCollectionBuilder AmmoniumNitrate() => Add(new FertilizerBuilder()
        .AddNh4(17.499)
        .AddNo3(17.499).Build());

    public FertilizerCollectionBuilder Urea() => Add(new FertilizerBuilder()
        .AddNh2(46.646).Build());

    public FertilizerCollectionBuilder UreaPhosphate() => Add(new FertilizerBuilder()
        .AddNh2(17.725)
        .AddP(19.597).Build());

    public FertilizerCollectionBuilder Map() => Add(new FertilizerBuilder()
        .AddNh4(12.177)
        .AddP(26.928).Build());

    public FertilizerCollectionBuilder Mop() => Add(new FertilizerBuilder()
        .AddK(52.447)
        .AddCl(47.553).Build());

    public FertilizerCollectionBuilder AmmoniumChloride() => Add(new FertilizerBuilder()
        .AddNh4(26.187)
        .AddCl(66.275).Build());

    public FertilizerCollectionBuilder AmmoniumSulfate() => Add(new FertilizerBuilder()
        .AddNh4(21.201)
        .AddS(24.263).Build());

    public FertilizerCollectionBuilder PhosphoricAcid() => Add(new FertilizerBuilder()
        .AddP(31.608).Build());

    public FertilizerCollectionBuilder CalciumMonobasicPhosphate() => Add(new FertilizerBuilder()
        .AddCaNonChelated(15.9)
        .AddP(24.576).Build());

    public FertilizerCollectionBuilder BoricAcid() => Add(new FertilizerBuilder()
        .AddB(17.483).Build());

    public FertilizerCollectionBuilder SodiumBorate() => Add(new FertilizerBuilder()
        .AddB(11.338)
        .AddNa(12.057).Build());

    public FertilizerCollectionBuilder SodiumMolybdate() => Add(new FertilizerBuilder()
        .AddMo(39.656)
        .AddNa(19.003).Build());

    public FertilizerCollectionBuilder SodiumSilicate() => Add(new FertilizerBuilder()
        .AddSi(23.009)
        .AddNa(37.669).Build());

    public FertilizerCollectionBuilder SodiumSelenate() => Add(new FertilizerBuilder()
        .AddSe(41.795)
        .AddNa(24.335).Build());

    public FertilizerCollectionBuilder IronSulfate() => Add(new FertilizerBuilder()
        .AddFeNonChelated(20.088)
        .AddS(11.532).Build());

    public FertilizerCollectionBuilder CopperSulfate() => Add(new FertilizerBuilder()
        .AddCuNonChelated(25.451)
        .AddS(12.841).Build());

    public FertilizerCollectionBuilder ManganeseSulfate() => Add(new FertilizerBuilder()
        .AddMnNonChelated(32.506)
        .AddS(18.969).Build());

    public FertilizerCollectionBuilder ZincSulfate() => Add(new FertilizerBuilder()
        .AddZnNonChelated(36.433)
        .AddS(17.866).Build());

    public FertilizerCollectionBuilder CopperNitrate() => Add(new FertilizerBuilder()
        .AddCuNonChelated(21.494)
        .AddNo3(9.476).Build());

    public FertilizerCollectionBuilder ZincNitrate() => Add(new FertilizerBuilder()
        .AddZnNonChelated(21.978)
        .AddNo3(9.417).Build());

    public FertilizerCollectionBuilder IronNitrate() => Add(new FertilizerBuilder()
        .AddFeNonChelated(13.823)
        .AddNo3(10.401).Build());

    public FertilizerCollectionBuilder ManganeseNitrate() => Add(new FertilizerBuilder()
        .AddMnNonChelated(30.701)
        .AddNo3(15.655).Build());

    public FertilizerCollectionBuilder CopperEdta() => Add(new FertilizerBuilder()
        .AddCuEdta(14.65).Build());

    public FertilizerCollectionBuilder ManganeseEdta() => Add(new FertilizerBuilder()
        .AddMnEdta(12.922).Build());

    public FertilizerCollectionBuilder ZincEdta() => Add(new FertilizerBuilder()
        .AddZnEdta(15.009).Build());

    public FertilizerCollectionBuilder IronEdta() => Add(new FertilizerBuilder()
        .AddFeEdta(13.262).Build());
    
    public IList<FertilizerOptimizationModel> Build() => _selectedFertilizer.ToList();
}