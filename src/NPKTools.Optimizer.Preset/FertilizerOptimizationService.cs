using System.Text;
using NPKTools.Core.Domain.Collections;
using NPKTools.Core.Domain.Fertilizers;
using NPKTools.Core.Domain.PpmTarget;
using NPKTools.Core.Domain.SolutionsFinderSettings;
using NPKTools.Core.Domain.SolutionsFinderSettings.Builder;
using NPKTools.Optimizer.Contracts;

namespace NPKTools.Optimizer.Preset;

/// <summary>
/// Provides services for optimizing fertilizer combinations based on target ppm values
/// for various elements. This service supports finding solutions for both macro and micro
/// nutrient requirements.
/// </summary>
public class FertilizerOptimizationService : IFertilizerOptimizationService
{
    private readonly IFertilizerOptimizer _fertilizerOptimizer;
    private readonly IFertilizerBundleRepository _fertilizerBundleRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="FertilizerOptimizationService"/> class.
    /// </summary>
    /// <param name="fertilizerOptimizer">The optimizer used to find optimal fertilizer solutions.</param>
    /// <param name="fertilizerBundleRepository">The repository to access bundles of fertilizers.</param>
    public FertilizerOptimizationService(IFertilizerOptimizer fertilizerOptimizer,
        IFertilizerBundleRepository fertilizerBundleRepository)
    {
        ArgumentNullException.ThrowIfNull(fertilizerOptimizer);
        _fertilizerOptimizer = fertilizerOptimizer;
        
        ArgumentNullException.ThrowIfNull(fertilizerBundleRepository);
        _fertilizerBundleRepository = fertilizerBundleRepository;
    }
    
    /// <summary>
    /// Finds optimization solutions for macro nutrients based on the given target ppm values.
    /// </summary>
    /// <param name="target">The target ppm values for macro nutrients.</param>
    /// <returns>A collection of solutions without duplicates.</returns>
    public Solutions FindMacroSolutions(PpmTarget target)
    {
        ArgumentNullException.ThrowIfNull(target);

        IList<IList<Fertilizer>> bundle = _fertilizerBundleRepository.Marco();
        
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

    /// <summary>
    /// Finds optimization solutions for micro nutrients based on the given target ppm values.
    /// </summary>
    /// <param name="target">The target ppm values for micro nutrients.</param>
    /// <returns>A collection of solutions without duplicates.</returns>
    public Solutions FindMicroSolutions(PpmTarget target)
    {
        ArgumentNullException.ThrowIfNull(target);

        IList<IList<Fertilizer>> bundle = _fertilizerBundleRepository.Micro();
        
        SolutionFinderSettings settings = new SolutionFinderSettingsBuilder()
            .AddFe(1)
            .AddCu(1)
            .AddMn(1)
            .AddZn(1)
            .AddB(1)
            .AddMo(1)
            .AddSi(1)
            .AddSe(1)
            .AddCl(1)
            .Build();

        Solutions solutions = FindSolutions(bundle, settings, target);

        return RemoveDuplicates(solutions);    
    }

    /// <summary>
    /// Finds optimization solutions for both macro and micro nutrients based on the given target ppm values.
    /// </summary>
    /// <param name="target">The target ppm values for nutrients.</param>
    /// <returns>A tuple containing collections of solutions for macro and micro nutrients without duplicates.</returns>
    public (Solutions Macro, Solutions Micro) FindSolutions(PpmTarget target)
    {
        ArgumentNullException.ThrowIfNull(target, nameof(target));

        Solutions macroSolutions = FindMacroSolutions(target);
    
        Solutions microSolutions = FindMicroSolutions(target);

        return (macroSolutions, microSolutions);
    }
    
    private Solutions FindSolutions (IList<IList<Fertilizer>> bundle,
        SolutionFinderSettings settings,
        PpmTarget target)
    {
        Solutions solutions = [];

        foreach (IList<Fertilizer> collection in bundle)
        {
            Solution? solution = _fertilizerOptimizer.Optimize(target, collection, settings);
            if (solution != null)
            {
                solutions.Add(solution);    
            }
        }

        return solutions;
    }
    
    private static Solutions RemoveDuplicates(Solutions solutions)
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