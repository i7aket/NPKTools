using System.Text;
using NPKOptimizer.Contracts;
using NPKOptimizer.Domain.Collections;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.PpmTarget;
using NPKOptimizer.Domain.SolutionsFinderSettings;
using NPKOptimizer.Domain.SolutionsFinderSettings.Builder;
using NPKOptimizerCalc.Contracts;

namespace NPKOptimizerCalc.Components;

public class FertilizerOptimizationService : IFertilizerOptimizationsService
{
    private readonly IFertilizerOptimizer _fertilizerOptimizer;
    private readonly IFertilizerBundleRepository _fertilizerBundleRepository;
    
    public FertilizerOptimizationService(IFertilizerOptimizer fertilizerOptimizer,
        IFertilizerBundleRepository fertilizerBundleRepository)
    {
        ArgumentNullException.ThrowIfNull(fertilizerOptimizer);
        _fertilizerOptimizer = fertilizerOptimizer;
        
        ArgumentNullException.ThrowIfNull(fertilizerBundleRepository);
        _fertilizerBundleRepository = fertilizerBundleRepository;
    }
    
    public Solutions FindMacroSolutions(PpmTarget target)
    {
        ArgumentNullException.ThrowIfNull(target);

        IList<IList<FertilizerOptimizationModel>> bundle = _fertilizerBundleRepository.Marco();
        
        SolutionFinderSettings settingsPrecise = new SolutionFinderSettingsBuilder()
            .AddN(1)
            .AddP(1)
            .AddK(1)
            .AddCa(1)
            .AddMg(1)
            .AddS(1)
            .AddCl(1)
            .Build();
        
        Solutions solutions = FindSolutions(bundle, settingsPrecise, target);
        
        SolutionFinderSettings settingsNoSulfur = new SolutionFinderSettingsBuilder()
            .AddN(1)
            .AddP(1)
            .AddK(1)
            .AddCa(1)
            .AddMg(1)
            .AddCl(1)
            .Build();

        Solutions solutionsNoSulfur = FindSolutions(bundle, settingsNoSulfur, target);

        solutions.AddRange(solutionsNoSulfur);
        
        return RemoveDuplicates(solutions);
    }

    public Solutions FindMicroSolutions(PpmTarget target)
    {
        ArgumentNullException.ThrowIfNull(target);

        IList<IList<FertilizerOptimizationModel>> bundle = _fertilizerBundleRepository.Micro();
        
        SolutionFinderSettings settings = new SolutionFinderSettingsBuilder()
            .AddFe(1)
            .AddCu(1)
            .AddMn(1)
            .AddZn(1)
            .AddB(1)
            .AddMo(1)
            .AddSi(1)
            .AddSe(1)
            .Build();

        Solutions solutions = FindSolutions(bundle, settings, target);

        return RemoveDuplicates(solutions);    
    }

    public (Solutions Macro, Solutions Micro) FindSolutions(PpmTarget target)
    {
        ArgumentNullException.ThrowIfNull(target, nameof(target));

        Solutions macroSolutions = FindMacroSolutions(target);
    
        Solutions microSolutions = FindMicroSolutions(target);

        return (macroSolutions, microSolutions);
    }
    
    private Solutions FindSolutions (IList<IList<FertilizerOptimizationModel>> bundle,
        SolutionFinderSettings settings,
        PpmTarget target)
    {
        Solutions solutions = new ();

        foreach (IList<FertilizerOptimizationModel> collection in bundle)
        {
            Solution? solution = _fertilizerOptimizer.Optimize(target, collection, settings);
            if (solution != null)
            {
                solutions.Add(solution);    
            }
        }

        return solutions;
    }
    
    private Solutions RemoveDuplicates(Solutions solutions)
    {
        Dictionary<string, Solution> uniqueSolutions = new Dictionary<string, Solution>();

        foreach (Solution solution in solutions)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Fertilizer fertilizer in solution.OrderBy(f => f.RefId.Value))
            {
                stringBuilder.Append($"{fertilizer.RefId.Value}-{fertilizer.Weight.Value};");
            }
            string key = stringBuilder.ToString();
            uniqueSolutions.TryAdd(key, solution);
        }

        Solutions result = new Solutions();
        foreach (Solution solution in uniqueSolutions.Values)
        {
            result.Add(solution);
        }

        return result;
    }
}