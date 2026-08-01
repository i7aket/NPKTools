using Microsoft.Extensions.DependencyInjection;
using SYT.NPKTools.Nutrients;
using SYT.NPKTools.Optimization;
using Xunit;

namespace SYT.NPKTools.IntegrationTests;

/// <summary>
/// Covers <see cref="NpkTools"/>, the factory that assembles the library's services.
/// </summary>
/// <remarks>
/// The package has no dependencies, so it ships no <c>IServiceCollection</c> extension. These tests
/// therefore also pin the documented container pattern — registering the factory results directly —
/// using the DI implementation as a test-only reference, to prove the README's snippet really works.
/// </remarks>
public class NpkToolsFactoryTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public void CreateOptimizationService_ResolvesWorkingService()
    {
        IFertilizerOptimizationService service = NpkTools.CreateOptimizationService();
        PpmTarget target = new PpmTargetBuilder().AddN(150).AddP(50).AddK(200).AddCa(100).AddMg(50).Build();

        Assert.NotEmpty(service.FindMacroSolutions(target));
    }

    /// <summary>
    /// The default is observable rather than merely declared: the real solver finds mixes and a
    /// substituted one that never succeeds finds none.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void CreateOptimizationService_DefaultsToAWorkingManagedSolver()
    {
        CountingSolver custom = new();

        Assert.NotEmpty(NpkTools.CreateOptimizationService().FindMacroSolutions(Target()));
        Assert.Empty(NpkTools.CreateOptimizationService(custom).FindMacroSolutions(Target()));
        Assert.True(custom.CallCount > 0, "the substituted solver should have been called");
    }

    /// <summary>
    /// Substitution is an explicit argument rather than a registration-order rule, so it cannot be
    /// silently ineffective the way a TryAdd-based DI helper could.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void CreateOptimizationService_UsesTheSuppliedSolverForEverySolve()
    {
        CountingSolver custom = new();

        NpkTools.CreateOptimizationService(custom).FindMacroSolutions(Target());

        // 18 macro bundles, solved once honouring the sulfur target and once ignoring it.
        Assert.Equal(36, custom.CallCount);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void CreateOptimizer_UsesTheSuppliedSolver()
    {
        CountingSolver custom = new();

        NpkTools.CreateOptimizer(custom)
            .Optimize(Target(), NpkTools.CreateBundleRepository().Macro()[0], Settings());

        Assert.Equal(1, custom.CallCount);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void CreatePpmCalculatorAndTargetParser_Work()
    {
        IPpmTargetParser parser = NpkTools.CreateTargetParser();
        IPpmCalculationService calculator = NpkTools.CreatePpmCalculator();

        PpmTarget target = parser.Parse("N=150 P=50 K=200 L=100");

        Assert.Equal(150, target.N.Value);
        Assert.NotNull(calculator);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void CreateBundleRepository_ReturnsTheCuratedCatalogue()
    {
        IFertilizerBundleRepository repository = NpkTools.CreateBundleRepository();

        Assert.Equal(18, repository.Macro().Count);
        Assert.Equal(4, repository.Micro().Count);
    }

    /// <summary>
    /// The pattern the README documents in place of an <c>AddNpkTools()</c> extension. It works because
    /// <c>IServiceCollection</c> belongs to the consuming application, not to this package.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void FactoryResults_RegisterAndResolveFromAContainer()
    {
        using ServiceProvider provider = new ServiceCollection()
            .AddSingleton(NpkTools.CreateOptimizationService())
            .AddSingleton(NpkTools.CreatePpmCalculator())
            .AddSingleton(NpkTools.CreateTargetParser())
            .BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IFertilizerOptimizationService>());
        Assert.NotNull(provider.GetRequiredService<IPpmCalculationService>());
        Assert.NotNull(provider.GetRequiredService<IPpmTargetParser>());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void RegisteredAsSingleton_ResolvesTheSameInstance()
    {
        using ServiceProvider provider = new ServiceCollection()
            .AddSingleton(NpkTools.CreateOptimizationService())
            .BuildServiceProvider();

        Assert.Same(
            provider.GetRequiredService<IFertilizerOptimizationService>(),
            provider.GetRequiredService<IFertilizerOptimizationService>());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void FindMacroSolutions_CancelledToken_ThrowsBeforeSolvingAnyBundle()
    {
        CountingSolver solver = new();
        IFertilizerOptimizationService service = NpkTools.CreateOptimizationService(solver);

        using CancellationTokenSource cts = new();
        cts.Cancel();

        Assert.Throws<OperationCanceledException>(() => service.FindMacroSolutions(Target(), cts.Token));
        Assert.Equal(0, solver.CallCount);
    }

    private static PpmTarget Target() => new PpmTargetBuilder().AddN(150).AddP(50).AddK(200).Build();

    private static SolutionFinderSettings Settings() =>
        new SolutionFinderSettingsBuilder().AddN(1).AddP(1).AddK(1).Build();

    /// <summary>
    /// A solver that records how many times it was asked to solve and never finds a solution.
    /// </summary>
    private sealed class CountingSolver : IOptimizationProblemSolver
    {
        public int CallCount { get; private set; }

        public Dictionary<string, double>? Solve(OptimizationProblem problem)
        {
            CallCount++;
            return null;
        }
    }
}
