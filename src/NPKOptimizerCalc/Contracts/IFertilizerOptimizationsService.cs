using NPKOptimizer.Domain.PpmTarget;

namespace NPKOptimizerCalc.Contracts;

public interface IFertilizerOptimizationsService
{
    Solutions FindMacroSolutions (PpmTarget target);
    Solutions FindMicroSolutions (PpmTarget target);
    (Solutions Macro, Solutions Micro) FindSolutions(PpmTarget target);
}
