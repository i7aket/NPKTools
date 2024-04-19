using NPKOptimizer.Common;
using NPKOptimizer.Components;
using NPKOptimizer.Components.OptimizationProblemSolvers.Contracts;
using NPKOptimizer.Contracts;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.NPK;
using NPKOptimizer.Domain.PartsPerMillion;
using NPKOptimizer.Domain.SolutionsFinderSettings;
using NPKOptimizer.Repository.Contracts;

namespace NPKOptimizer;

public sealed class FertilizerOptimizationsService : IFertilizerOptimizationsService
{
    private readonly IFertilizerOptimizer _fertilizerOptimizer;
    private readonly IPpmCalculationService _ppmCalculationService;
    private readonly IFertilizerBundleRepository _fertilizerBundleRepository;

    public FertilizerOptimizationsService(IFertilizerOptimizer fertilizerOptimizer,
        IPpmCalculationService ppmCalculationService, 
        IFertilizerBundleRepository fertilizerBundleRepository)
    {
        Validate.NotNull(fertilizerOptimizer);
        _fertilizerOptimizer = fertilizerOptimizer;

        Validate.NotNull(ppmCalculationService);
        _ppmCalculationService = ppmCalculationService;

        Validate.NotNull(fertilizerBundleRepository);
        _fertilizerBundleRepository = fertilizerBundleRepository;
    }

    public ActionResult<Ppm> CalculatePpm(FertilizerCollection collection, double waterLiters = 1)
    {
        ActionResult<Ppm> result = _ppmCalculationService.CalculatePpm(collection, waterLiters);

        return !result.IsSuccess 
            ? ActionResult<Ppm>.Fail(result.ErrorMessage) 
            : ActionResult<Ppm>.Success(result.Payload);
    }

    public async Task<ActionResult<Solutions>> FindAllSolutions(NpkTarget target)
    {
        FertilizerCollectionSet macroFertilizerCollection = FindSolutionsSet(
            await _fertilizerBundleRepository.MarcoBundle(), SolutionFinderSettings.CreateDefaultMacro(), target);

        FertilizerCollectionSet macroFertilizerCollectionNoSulfur = FindSolutionsSet(
            await _fertilizerBundleRepository.MarcoBundle(), SolutionFinderSettings.CreateDefaultMacroNoSulfur(),
            target);
        macroFertilizerCollection.UnionWith(macroFertilizerCollectionNoSulfur);

        FertilizerCollectionSet microFertilizerCollection = FindSolutionsSet(
            await _fertilizerBundleRepository.MicroBundle(), SolutionFinderSettings.CreateDefaultMicro(), target);

        return ActionResult<Solutions>.Success(new Solutions
        {
            Macro = macroFertilizerCollection,
            Micro = microFertilizerCollection
        });
    }

    public ActionResult<FertilizerCollection> FindSolution(NpkTarget target,
        FertilizerCollection collection,
        SolutionFinderSettings settings)
    {
        return _fertilizerOptimizer.Optimize(target, collection, settings);
    }

    private FertilizerCollectionSet FindSolutionsSet(FertilizerCollectionSet sets,
        SolutionFinderSettings settings,
        NpkTarget target)
    {
        FertilizerCollectionSet fertilizerCollectionSet = new ();

        foreach (FertilizerCollection set in sets)
        {
            ActionResult<FertilizerCollection> actionResult = FindSolution(target, set, settings);
            if (actionResult.IsSuccess)
            {
                fertilizerCollectionSet.Add(actionResult.Payload);
            }
        }

        return fertilizerCollectionSet;
    }
}