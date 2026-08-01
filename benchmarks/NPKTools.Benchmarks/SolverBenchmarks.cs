using BenchmarkDotNet.Attributes;
using NPKTools.Core.Domain.Collections;
using NPKTools.Core.Domain.PpmTarget;
using NPKTools.Optimizer.Components;
using NPKTools.Optimizer.Contracts;
using NPKTools.Optimizer.OrTools;
using NPKTools.Optimizer.Preset;
using NPKTools.Optimizer.PpmTargetParser;

namespace NPKTools.Benchmarks;

/// <summary>
/// Compares the managed simplex against the OR-Tools GLOP backend on the work the library actually
/// does: a full preset search, which solves 40 linear programs (the 18 macro bundles twice, with and
/// without the sulfur target, plus the 4 micro bundles).
/// </summary>
/// <remarks>
/// The managed solver is the shipped default, chosen because OR-Tools cannot run under WebAssembly.
/// These benchmarks exist to keep that choice honest — if the managed solver were markedly slower on
/// server workloads, that would be a reason to reconsider the default rather than something to
/// discover later.
/// </remarks>
[MemoryDiagnoser]
public class SolverBenchmarks
{
    private PpmTarget _target = null!;
    private IFertilizerOptimizationService _simplexService = null!;
    private IFertilizerOptimizationService _orToolsService = null!;

    [GlobalSetup]
    public void Setup()
    {
        _target = new PpmTargetParser()
            .Parse("N=150 P=50 K=200 Ca=100 Mg=50 S=60 Fe=2 Mn=0.55 Zn=0.33 B=0.28 Cu=0.05 Mo=0.05 L=100");

        _simplexService = Build(new SimplexOptimizationSolver());
        _orToolsService = Build(new GoogleOrToolsOptimizationSolver());
    }

    private static IFertilizerOptimizationService Build(IOptimizationProblemSolver solver) =>
        new FertilizerOptimizationService(
            new FertilizerOptimizationAdapter(solver, new OptimizationProblemMapper()),
            new FertilizerBundleRepository());

    [Benchmark(Baseline = true, Description = "Full search — managed simplex")]
    public int Simplex()
    {
        (Solutions Macro, Solutions Micro) result = _simplexService.FindSolutions(_target);
        return result.Macro.Count + result.Micro.Count;
    }

    [Benchmark(Description = "Full search — OR-Tools GLOP")]
    public int OrTools()
    {
        (Solutions Macro, Solutions Micro) result = _orToolsService.FindSolutions(_target);
        return result.Macro.Count + result.Micro.Count;
    }

    [Benchmark(Description = "Macro only — managed simplex")]
    public int SimplexMacroOnly() => _simplexService.FindMacroSolutions(_target).Count;

    [Benchmark(Description = "Macro only — OR-Tools GLOP")]
    public int OrToolsMacroOnly() => _orToolsService.FindMacroSolutions(_target).Count;
}
