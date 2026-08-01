using SYT.NPKTools.Fertilizers;

namespace SYT.NPKTools;
/// <summary>
/// Repository for managing collections of fertilizer bundles used in optimization processes. This repository provides
/// access to predefined sets of macro and micro nutrient fertilizers.
/// </summary>
public sealed class FertilizerBundleRepository : IFertilizerBundleRepository
{
    private readonly Lazy<IReadOnlyList<IReadOnlyList<Fertilizer>>> _macro;
    private readonly Lazy<IReadOnlyList<IReadOnlyList<Fertilizer>>> _micro;

    /// <summary>
    /// Constructs a new instance of FertilizerBundleRepository initializing lazy loaders for macro and micro fertilizer collections.
    /// </summary>
    public FertilizerBundleRepository()
    {
        _macro = new Lazy<IReadOnlyList<IReadOnlyList<Fertilizer>>>(InitializeMacro);
        _micro = new Lazy<IReadOnlyList<IReadOnlyList<Fertilizer>>>(InitializeMicro);
    }

    /// <summary>
    /// Gets a collection of macro nutrient fertilizer bundles. Built once and cached.
    /// </summary>
    /// <returns>A list of lists, each containing models of fertilizers for macro nutrient optimization.</returns>
    public IReadOnlyList<IReadOnlyList<Fertilizer>> Macro() => _macro.Value;

    /// <summary>
    /// Gets a collection of micro nutrient fertilizer bundles. Built once and cached.
    /// </summary>
    /// <returns>A list of lists, each containing models of fertilizers for micro nutrient optimization.</returns>
    public IReadOnlyList<IReadOnlyList<Fertilizer>> Micro() => _micro.Value;

    private static List<IReadOnlyList<Fertilizer>> InitializeMacro()
    {
        IReadOnlyList<Fertilizer> baseMacroGroup = new FertilizerCollectionBuilder()
            .CalciumNitrate()
            .K()
            .Mgs()
            .Calc().Build();
        IReadOnlyList<Fertilizer> mkp = new FertilizerCollectionBuilder()
            .Mkp().Build();
        IReadOnlyList<Fertilizer> mag = new FertilizerCollectionBuilder()
            .Mag().Build();
        IReadOnlyList<Fertilizer> sop = new FertilizerCollectionBuilder()
            .Sop().Build();
        IReadOnlyList<Fertilizer> dkp = new FertilizerCollectionBuilder()
            .Dkp().Build();
        IReadOnlyList<Fertilizer> ammoniumNitrate = new FertilizerCollectionBuilder()
            .AmmoniumNitrate().Build();
        IReadOnlyList<Fertilizer> extendedMacroGroup = new FertilizerCollectionBuilder()
            .Urea()
            .UreaPhosphate()
            .Map()
            .Mop()
            .AmmoniumChloride()
            .AmmoniumSulfate()
            .PhosphoricAcid()
            .CalciumMonobasicPhosphate().Build();

        return new List<IReadOnlyList<Fertilizer>>
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

    private static List<IReadOnlyList<Fertilizer>> InitializeMicro()
    {
        IReadOnlyList<Fertilizer> baseMicroGroup = new FertilizerCollectionBuilder()
            .BoricAcid()
            .SodiumBorate()
            .SodiumMolybdate()
            .SodiumSilicate()
            .SodiumSelenate()
            .Build();
        IReadOnlyList<Fertilizer> sulfateMicroGroup = new FertilizerCollectionBuilder()
            .IronSulfate()
            .CopperSulfate()
            .ManganeseSulfate()
            .ZincSulfate()
            .Build();
        IReadOnlyList<Fertilizer> nitrateMicroGroup = new FertilizerCollectionBuilder()
            .CopperNitrate()
            .ZincNitrate()
            .IronNitrate()
            .ManganeseNitrate()
            .Build();
        IReadOnlyList<Fertilizer> chelateMicroGroup = new FertilizerCollectionBuilder()
            .CopperEdta()
            .ManganeseEdta()
            .ZincEdta()
            .IronEdta()
            .Build();

        return new List<IReadOnlyList<Fertilizer>>
        {
            baseMicroGroup,
            CombineGroups(baseMicroGroup, sulfateMicroGroup),
            CombineGroups(baseMicroGroup, nitrateMicroGroup),
            CombineGroups(baseMicroGroup, chelateMicroGroup)
        };
    }

    private static List<Fertilizer> CombineGroups(params IReadOnlyList<Fertilizer>[] groups)
    {
        List<Fertilizer> combined = [];
        foreach (IReadOnlyList<Fertilizer> group in groups)
        {
            combined.AddRange(group);
        }
        return combined;
    }
}
