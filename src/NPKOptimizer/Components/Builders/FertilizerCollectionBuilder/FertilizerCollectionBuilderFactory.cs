using NPKOptimizer.Common;
using NPKOptimizer.Repository.Contracts;

namespace NPKOptimizer.Components.Builders.FertilizerCollectionBuilder;

public class FertilizerCollectionBuilderFactory : IFertilizerCollectionBuilderFactory
{
    private readonly IFertilizerRepository _fertilizerRepository;

    public FertilizerCollectionBuilderFactory(IFertilizerRepository fertilizerRepository)
    {
        Validate.NotNull(fertilizerRepository);
        _fertilizerRepository = fertilizerRepository;
    }

    public FertilizerCollectionBuilder CollectionBuilder()
    {
        return new FertilizerCollectionBuilder(_fertilizerRepository);
    }
}
