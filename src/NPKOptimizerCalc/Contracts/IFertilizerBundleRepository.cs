using NPKOptimizer.Domain.Fertilizers;

namespace NPKOptimizerCalc.Contracts;

public interface IFertilizerBundleRepository
{
    IList<IList<FertilizerOptimizationModel>> Marco();
    IList<IList<FertilizerOptimizationModel>> Micro();
}