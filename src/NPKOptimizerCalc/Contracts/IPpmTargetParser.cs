using NPKOptimizer.Domain.PpmTarget;

namespace NPKOptimizerCalc.Contracts;

public interface IPpmTargetParser
{
    PpmTarget Parse(string input);
}