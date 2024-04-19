using NPKOptimizer.Domain.Fertilizers;

namespace NPKOptimizer.Repository.Contracts;

public interface IFertilizerBundleRepository
{
    Task<FertilizerCollectionSet> MarcoBundle();
    Task<FertilizerCollectionSet> MicroBundle();
}