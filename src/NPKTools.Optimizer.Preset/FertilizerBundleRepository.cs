using NPKTools.Core.Domain.Fertilizers;

namespace NPKTools.Optimizer.Preset;
/// <summary>
/// Repository for managing collections of fertilizer bundles used in optimization processes. This repository provides
/// access to predefined sets of macro and micro nutrient fertilizers.
/// </summary>
public class FertilizerBundleRepository : IFertilizerBundleRepository
{
    /// <summary>
    /// Gets a collection of macro nutrient fertilizer bundles.
    /// </summary>
    /// <returns>A list of lists, each containing models of fertilizers for macro nutrient optimization.</returns>
    public IList<IList<Fertilizer>> Marco() => _marco.Value;
    private readonly Lazy<IList<IList<Fertilizer>>> _marco;

    /// <summary>
    /// Gets a collection of micro nutrient fertilizer bundles.
    /// </summary>
    /// <returns>A list of lists, each containing models of fertilizers for micro nutrient optimization.</returns>
    public IList<IList<Fertilizer>> Micro() => _micro.Value;
    private readonly Lazy<IList<IList<Fertilizer>>> _micro;

    /// <summary>
    /// Constructs a new instance of FertilizerBundleRepository initializing lazy loaders for macro and micro fertilizer collections.
    /// </summary>
    public FertilizerBundleRepository()
    {
        _marco = new Lazy<IList<IList<Fertilizer>>>(InitializeMarco);
        _micro = new Lazy<IList<IList<Fertilizer>>>(InitializeMicro);
    }

    private IList<IList<Fertilizer>> InitializeMarco()
    {
        IList<Fertilizer> baseMacroGroup = new FertilizerCollectionBuilder()
            .CalciumNitrate()
            .K()
            .Mgs()
            .Calc().Build();
        IList<Fertilizer> mkp = new FertilizerCollectionBuilder()
            .Mkp().Build();
        IList<Fertilizer> mag = new FertilizerCollectionBuilder()
            .Mag().Build();
        IList<Fertilizer> sop = new FertilizerCollectionBuilder()
            .Sop().Build();
        IList<Fertilizer> dkp = new FertilizerCollectionBuilder()
            .Dkp().Build();
        IList<Fertilizer> ammoniumNitrate = new FertilizerCollectionBuilder()
            .AmmoniumNitrate().Build();
        IList<Fertilizer> extendedMacroGroup = new FertilizerCollectionBuilder()
            .Urea()
            .UreaPhosphate()
            .Map()
            .Mop()
            .AmmoniumChloride()
            .AmmoniumSulfate()
            .PhosphoricAcid()
            .CalciumMonobasicPhosphate().Build();

        return new List<IList<Fertilizer>>
        {
            baseMacroGroup,
            CombineGroups(baseMacroGroup, mkp),
            CombineGroups(baseMacroGroup, mkp, mag),
            CombineGroups(baseMacroGroup, ammoniumNitrate),
            CombineGroups(baseMacroGroup, ammoniumNitrate, mkp),
            CombineGroups(baseMacroGroup, ammoniumNitrate, mkp, mag),

            CombineGroups(baseMacroGroup, mkp, sop),
            CombineGroups(baseMacroGroup, mkp, sop, mag),
            CombineGroups(baseMacroGroup, extendedMacroGroup, mkp, sop, mag),
            CombineGroups(baseMacroGroup, ammoniumNitrate, mkp, sop),
            CombineGroups(baseMacroGroup, ammoniumNitrate, mkp, sop, mag),
            CombineGroups(baseMacroGroup, ammoniumNitrate, extendedMacroGroup, mkp, sop, mag),

            CombineGroups(baseMacroGroup, mkp, dkp),
            CombineGroups(baseMacroGroup, mkp, dkp, mag),
            CombineGroups(baseMacroGroup, extendedMacroGroup, mkp, dkp, mag),
            CombineGroups(baseMacroGroup, ammoniumNitrate, mkp, dkp),
            CombineGroups(baseMacroGroup, ammoniumNitrate, mkp, dkp, mag),
            CombineGroups(baseMacroGroup, ammoniumNitrate, extendedMacroGroup, mkp, dkp, mag)
        };
    }

    private IList<IList<Fertilizer>> InitializeMicro()
    {
        IList<Fertilizer> baseMicroGroup = new FertilizerCollectionBuilder()
            .BoricAcid()
            .SodiumBorate()
            .SodiumMolybdate()
            .SodiumSilicate()
            .SodiumSelenate().Build();
        IList<Fertilizer> sulfateMicroGroup = new FertilizerCollectionBuilder()
            .IronSulfate()
            .CopperSulfate()
            .ManganeseSulfate()
            .ZincSulfate().Build();
        IList<Fertilizer> nitrateMicroGroup = new FertilizerCollectionBuilder()
            .CopperNitrate()
            .ZincNitrate()
            .IronNitrate()
            .ManganeseNitrate().Build();
        IList<Fertilizer> chelateMicroGroup = new FertilizerCollectionBuilder().CopperEdta()
            .ManganeseEdta()
            .ZincEdta()
            .IronEdta().Build();

        return new List<IList<Fertilizer>>
        {
            baseMicroGroup,
            CombineGroups(baseMicroGroup, sulfateMicroGroup),
            CombineGroups(baseMicroGroup, nitrateMicroGroup),
            CombineGroups(baseMicroGroup, chelateMicroGroup)
        };
    }
    
    private IList<Fertilizer> CombineGroups(params IList<Fertilizer>[] groups)
    {
        List<Fertilizer> combined = new List<Fertilizer>();
        foreach (IList<Fertilizer> group in groups)
        {
            combined.AddRange(group);
        }
        return combined;
    }
}