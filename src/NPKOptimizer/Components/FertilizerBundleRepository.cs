using NPKOptimizer.Contracts;
using NPKOptimizer.Domain.Fertilizers;

namespace NPKOptimizer.Components;
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
    public IList<IList<FertilizerOptimizationModel>> Marco() => _marco.Value;
    private readonly Lazy<IList<IList<FertilizerOptimizationModel>>> _marco;

    /// <summary>
    /// Gets a collection of micro nutrient fertilizer bundles.
    /// </summary>
    /// <returns>A list of lists, each containing models of fertilizers for micro nutrient optimization.</returns>
    public IList<IList<FertilizerOptimizationModel>> Micro() => _micro.Value;
    private readonly Lazy<IList<IList<FertilizerOptimizationModel>>> _micro;

    /// <summary>
    /// Constructs a new instance of FertilizerBundleRepository initializing lazy loaders for macro and micro fertilizer collections.
    /// </summary>
    public FertilizerBundleRepository()
    {
        _marco = new Lazy<IList<IList<FertilizerOptimizationModel>>>(InitializeMarco);
        _micro = new Lazy<IList<IList<FertilizerOptimizationModel>>>(InitializeMicro);
    }

    private IList<IList<FertilizerOptimizationModel>> InitializeMarco()
    {
        IList<FertilizerOptimizationModel> baseMacroGroup = new FertilizerCollectionBuilder()
            .CalciumNitrate()
            .K()
            .Mgs()
            .Calc().Build();
        IList<FertilizerOptimizationModel> mkp = new FertilizerCollectionBuilder()
            .Mkp().Build();
        IList<FertilizerOptimizationModel> mag = new FertilizerCollectionBuilder()
            .Mag().Build();
        IList<FertilizerOptimizationModel> sop = new FertilizerCollectionBuilder()
            .Sop().Build();
        IList<FertilizerOptimizationModel> dkp = new FertilizerCollectionBuilder()
            .Dkp().Build();
        IList<FertilizerOptimizationModel> ammoniumNitrate = new FertilizerCollectionBuilder()
            .AmmoniumNitrate().Build();
        IList<FertilizerOptimizationModel> extendedMacroGroup = new FertilizerCollectionBuilder()
            .Urea()
            .UreaPhosphate()
            .Map()
            .Mop()
            .AmmoniumChloride()
            .AmmoniumSulfate()
            .PhosphoricAcid()
            .CalciumMonobasicPhosphate().Build();

        return new List<IList<FertilizerOptimizationModel>>
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

    private IList<IList<FertilizerOptimizationModel>> InitializeMicro()
    {
        IList<FertilizerOptimizationModel> baseMicroGroup = new FertilizerCollectionBuilder()
            .BoricAcid()
            .SodiumBorate()
            .SodiumMolybdate()
            .SodiumSilicate()
            .SodiumSelenate().Build();
        IList<FertilizerOptimizationModel> sulfateMicroGroup = new FertilizerCollectionBuilder()
            .IronSulfate()
            .CopperSulfate()
            .ManganeseSulfate()
            .ZincSulfate().Build();
        IList<FertilizerOptimizationModel> nitrateMicroGroup = new FertilizerCollectionBuilder()
            .CopperNitrate()
            .ZincNitrate()
            .IronNitrate()
            .ManganeseNitrate().Build();
        IList<FertilizerOptimizationModel> chelateMicroGroup = new FertilizerCollectionBuilder().CopperEdta()
            .ManganeseEdta()
            .ZincEdta()
            .IronEdta().Build();

        return new List<IList<FertilizerOptimizationModel>>
        {
            baseMicroGroup,
            CombineGroups(baseMicroGroup, sulfateMicroGroup),
            CombineGroups(baseMicroGroup, nitrateMicroGroup),
            CombineGroups(baseMicroGroup, chelateMicroGroup)
        };
    }
    
    private IList<FertilizerOptimizationModel> CombineGroups(params IList<FertilizerOptimizationModel>[] groups)
    {
        List<FertilizerOptimizationModel> combined = new List<FertilizerOptimizationModel>();
        foreach (IList<FertilizerOptimizationModel> group in groups)
        {
            combined.AddRange(group);
        }
        return combined;
    }
}