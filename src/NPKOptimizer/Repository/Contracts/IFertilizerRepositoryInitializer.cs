using NPKOptimizer.Domain.Fertilizers;

namespace NPKOptimizer.Repository.Contracts;

public interface IFertilizerRepositoryInitializer
{
    FertilizerCollection InitializeFertilizers();
}