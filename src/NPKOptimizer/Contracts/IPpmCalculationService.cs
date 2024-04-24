using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.PartsPerMillion;

namespace NPKOptimizer.Contracts;

public interface IPpmCalculationService
{
    Ppm CalculatePpm(IEnumerable<Fertilizer> collection, double waterLiters = 1);
}