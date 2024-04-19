using NPKOptimizer.Common;
using NPKOptimizer.Components.Builders.FertilizerCollectionBuilder;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Repository.Contracts;

namespace NPKOptimizer.Repository;

public class FertilizerBundleRepository : IFertilizerBundleRepository
{
    private readonly IFertilizerCollectionBuilderFactory _factory;

    public FertilizerBundleRepository (IFertilizerCollectionBuilderFactory fertilizerCollectionBuilderFactory)
    {
        Validate.NotNull(fertilizerCollectionBuilderFactory);
        _factory = fertilizerCollectionBuilderFactory;
    }

    public async Task<FertilizerCollectionSet> MarcoBundle()
    {
        return new FertilizerCollectionSet
        {
            await _factory.CollectionBuilder().BaseMacroGroup().Build(),
            await _factory.CollectionBuilder().BaseMacroGroup().Mkp().Build(),
            await _factory.CollectionBuilder().BaseMacroGroup().Mkp().Mag().Build(),
            await _factory.CollectionBuilder().BaseMacroGroup().AmmoniumNitrate().Build(),
            await _factory.CollectionBuilder().BaseMacroGroup().AmmoniumNitrate().Mkp().Build(),
            await _factory.CollectionBuilder().BaseMacroGroup().AmmoniumNitrate().Mkp().Mag().Build(),

            await _factory.CollectionBuilder().BaseMacroGroup().Mkp().Sop().Build(),
            await _factory.CollectionBuilder().BaseMacroGroup().Mkp().Sop().Mag().Build(),
            await _factory.CollectionBuilder().BaseMacroGroup().ExtendedMacroGroup().Mkp().Sop().Mag().Build(),
            await _factory.CollectionBuilder().BaseMacroGroup().AmmoniumNitrate().Mkp().Sop().Build(),
            await _factory.CollectionBuilder().BaseMacroGroup().AmmoniumNitrate().Mkp().Sop().Mag().Build(),
            await _factory.CollectionBuilder().BaseMacroGroup().AmmoniumNitrate().ExtendedMacroGroup().Mkp().Sop().Mag().Build(),

            await _factory.CollectionBuilder().BaseMacroGroup().Mkp().Dkp().Build(),
            await _factory.CollectionBuilder().BaseMacroGroup().Mkp().Dkp().Mag().Build(),
            await _factory.CollectionBuilder().BaseMacroGroup().ExtendedMacroGroup().Mkp().Dkp().Mag().Build(),
            await _factory.CollectionBuilder().BaseMacroGroup().AmmoniumNitrate().Mkp().Dkp().Build(),
            await _factory.CollectionBuilder().BaseMacroGroup().AmmoniumNitrate().Mkp().Dkp().Mag().Build(),
            await _factory.CollectionBuilder().BaseMacroGroup().AmmoniumNitrate().ExtendedMacroGroup().Mkp().Dkp().Mag().Build()
        };    }

    public async Task<FertilizerCollectionSet> MicroBundle()
    {
        return new FertilizerCollectionSet
        {
            await _factory.CollectionBuilder().BaseMacroGroup().Build(),
            await _factory.CollectionBuilder().BaseMicroGroup().SulfateMicroGroup().Build(),
            await _factory.CollectionBuilder().BaseMicroGroup().NitrateMicroGroup().Build(),
            await _factory.CollectionBuilder().BaseMicroGroup().ChelateMicroGroup().Build()
        };
    }
}

