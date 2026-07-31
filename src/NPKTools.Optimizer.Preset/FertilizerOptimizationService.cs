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
    /// Precision settings for the macronutrient search, honouring the sulfur target.
    /// </summary>
    private static readonly SolutionFinderSettings MacroSettings = new SolutionFinderSettingsBuilder()
        .AddN(1).AddP(1).AddK(1).AddCa(1).AddMg(1).AddS(1).AddCl(1)
        .Build();

    /// <summary>
    /// The same search with sulfur left unconstrained, which widens the set of feasible mixes.
    /// </summary>
    private static readonly SolutionFinderSettings MacroSettingsWithoutSulfur = new SolutionFinderSettingsBuilder()
        .AddN(1).AddP(1).AddK(1).AddCa(1).AddMg(1).AddCl(1)
        .Build();

    /// <summary>
    /// Precision settings for the micronutrient search.
    /// </summary>
    private static readonly SolutionFinderSettings MicroSettings = new SolutionFinderSettingsBuilder()
        .AddFe(1).AddCu(1).AddMn(1).AddZn(1).AddB(1).AddMo(1).AddSi(1).AddSe(1)
        .Build();

    /// <summary>
    /// Initializes a new instance of the <see cref="FertilizerOptimizationService"/> class.
    /// </summary>
    /// <param name="fertilizerOptimizer">The optimizer used to find optimal fertilizer solutions.</param>
    /// <param name="fertilizerBundleRepository">The repository to access bundles of fertilizers.</param>
    /// <exception cref="ArgumentNullException">Thrown when either dependency is null.</exception>
    public FertilizerOptimizationService(IFertilizerOptimizer fertilizerOptimizer,
        IFertilizerBundleRepository fertilizerBundleRepository)
    {
        ArgumentNullException.ThrowIfNull(fertilizerOptimizer);
        ArgumentNullException.ThrowIfNull(fertilizerBundleRepository);

        _fertilizerOptimizer = fertilizerOptimizer;
        _fertilizerBundleRepository = fertilizerBundleRepository;
    }

    /// <summary>
    /// Finds optimization solutions for macro nutrients based on the given target ppm values.
    /// The search runs twice — once honouring the sulfur target and once ignoring it — and the
    /// combined results are de-duplicated.
    /// </summary>
    /// <param name="target">The target ppm values for macro nutrients.</param>
    /// <param name="cancellationToken">Token used to cancel the search.</param>
    /// <returns>The distinct solutions found, or <see cref="Solutions.Empty"/> when there are none.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="target"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is signalled.</exception>
    public Solutions FindMacroSolutions(PpmTarget target, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(target);

        IReadOnlyList<IReadOnlyList<Fertilizer>> bundle = _fertilizerBundleRepository.Macro();

        List<Solution> found = [];
        found.AddRange(Solve(bundle, MacroSettings, target, cancellationToken));
        found.AddRange(Solve(bundle, MacroSettingsWithoutSulfur, target, cancellationToken));

        return Distinct(found);
    }

    /// <summary>
    /// Finds optimization solutions for micro nutrients based on the given target ppm values.
    /// </summary>
    /// <param name="target">The target ppm values for micro nutrients.</param>
    /// <param name="cancellationToken">Token used to cancel the search.</param>
    /// <returns>The distinct solutions found, or <see cref="Solutions.Empty"/> when there are none.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="target"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is signalled.</exception>
    public Solutions FindMicroSolutions(PpmTarget target, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(target);

        IReadOnlyList<IReadOnlyList<Fertilizer>> bundle = _fertilizerBundleRepository.Micro();

        return Distinct(Solve(bundle, MicroSettings, target, cancellationToken));
    }

    /// <summary>
    /// Finds optimization solutions for both macro and micro nutrients based on the given target ppm values.
    /// </summary>
    /// <param name="target">The target ppm values for nutrients.</param>
    /// <param name="cancellationToken">Token used to cancel the search.</param>
    /// <returns>A tuple with the macro and micro solution sets, each empty when none were found.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="target"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is signalled.</exception>
    public (Solutions Macro, Solutions Micro) FindSolutions(PpmTarget target,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(target);

        return (FindMacroSolutions(target, cancellationToken), FindMicroSolutions(target, cancellationToken));
    }

    private List<Solution> Solve(IReadOnlyList<IReadOnlyList<Fertilizer>> bundle,
        SolutionFinderSettings settings,
        PpmTarget target,
        CancellationToken cancellationToken)
    {
        List<Solution> solutions = [];

        foreach (IReadOnlyList<Fertilizer> collection in bundle)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Solution? solution = _fertilizerOptimizer.Optimize(target, collection, settings);
            if (solution != null)
            {
                solutions.Add(solution);
            }
        }

        return solutions;
    }

    /// <summary>
    /// Collapses solutions that prescribe the same fertilizers at the same weights.
    /// </summary>
    private static Solutions Distinct(List<Solution> solutions)
    {
        if (solutions.Count == 0)
        {
            return Solutions.Empty;
        }

        Dictionary<string, Solution> unique = [];

        foreach (Solution solution in solutions)
        {
            StringBuilder key = new();
            foreach (Fertilizer fertilizer in solution.OrderBy(f => f.RefId.Value))
            {
                key.Append(fertilizer.RefId.Value).Append('-').Append(fertilizer.Weight.Value).Append(';');
            }

            unique.TryAdd(key.ToString(), solution);
        }

        return new Solutions(unique.Values);
    }
}
