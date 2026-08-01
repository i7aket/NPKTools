using Microsoft.Extensions.DependencyInjection;
using SYT.NPKTools.Nutrients;
using SYT.NPKTools.Optimization;
using Xunit;

namespace SYT.NPKTools.IntegrationTests;

/// <summary>
/// Covers <c>AddNpkTools()</c> from the optional <c>SYT.NPKTools.DependencyInjection</c> package.
/// </summary>
/// <remarks>
/// The extension lives in the <c>Microsoft.Extensions.DependencyInjection</c> namespace by convention,
/// so a typical <c>Program.cs</c> needs no extra using directive — which is exactly what these tests
/// demonstrate, since this file imports only that namespace to reach it.
/// </remarks>
public class AddNpkToolsTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkTools_RegistersEveryService()
    {
        using ServiceProvider provider = new ServiceCollection()
            .AddNpkTools()
            .BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IFertilizerOptimizationService>());
        Assert.NotNull(provider.GetRequiredService<IFertilizerOptimizer>());
        Assert.NotNull(provider.GetRequiredService<IFertilizerBundleRepository>());
        Assert.NotNull(provider.GetRequiredService<IPpmCalculationService>());
        Assert.NotNull(provider.GetRequiredService<IPpmTargetParser>());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkTools_ResolvesAWorkingPipeline()
    {
        using ServiceProvider provider = new ServiceCollection()
            .AddNpkTools()
            .BuildServiceProvider();

        IPpmTargetParser parser = provider.GetRequiredService<IPpmTargetParser>();
        IFertilizerOptimizationService optimizer =
            provider.GetRequiredService<IFertilizerOptimizationService>();

        FertilizerSolutions result = optimizer.FindSolutions(parser.Parse("N=150 P=50 K=200 Ca=100 Mg=50 L=100"));

        Assert.NotEmpty(result.Macro);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkTools_RegistersSingletons()
    {
        using ServiceProvider provider = new ServiceCollection()
            .AddNpkTools()
            .BuildServiceProvider();

        Assert.Same(
            provider.GetRequiredService<IFertilizerOptimizationService>(),
            provider.GetRequiredService<IFertilizerOptimizationService>());
        Assert.Same(
            provider.GetRequiredService<IFertilizerBundleRepository>(),
            provider.GetRequiredService<IFertilizerBundleRepository>());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkTools_CalledTwice_RegistersEachServiceOnce()
    {
        using ServiceProvider provider = new ServiceCollection()
            .AddNpkTools()
            .AddNpkTools()
            .BuildServiceProvider();

        Assert.Single(provider.GetServices<IFertilizerOptimizationService>());
        Assert.Single(provider.GetServices<IPpmCalculationService>());
        Assert.Single(provider.GetServices<IPpmTargetParser>());
    }

    /// <summary>
    /// The solver is an argument, not a registration-order rule, so substitution cannot silently fail
    /// the way the previous <c>TryAdd</c>-ordering design could.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkTools_UsesTheSuppliedSolver()
    {
        CountingSolver custom = new();

        using ServiceProvider provider = new ServiceCollection()
            .AddNpkTools(custom)
            .BuildServiceProvider();

        Solutions solutions = provider
            .GetRequiredService<IFertilizerOptimizationService>()
            .FindMacroSolutions(new PpmTargetBuilder().AddN(150).AddP(50).AddK(200).Build());

        Assert.Empty(solutions);
        // 18 macro bundles, solved once honouring the sulfur target and once ignoring it.
        Assert.Equal(36, custom.CallCount);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkTools_WithoutASolver_ResolvesTheManagedDefault()
    {
        using ServiceProvider provider = new ServiceCollection()
            .AddNpkTools()
            .BuildServiceProvider();

        Solutions solutions = provider
            .GetRequiredService<IFertilizerOptimizationService>()
            .FindMacroSolutions(new PpmTargetBuilder()
                .AddN(150).AddP(50).AddK(200).AddCa(100).AddMg(50).Build());

        Assert.NotEmpty(solutions);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkTools_NullServices_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => ((IServiceCollection)null!).AddNpkTools());

    /// <summary>
    /// Registrations are lazy, so nothing is constructed for a service the application never resolves.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkTools_DoesNotConstructAnythingUntilResolved()
    {
        CountingSolver custom = new();

        using ServiceProvider provider = new ServiceCollection()
            .AddNpkTools(custom)
            .BuildServiceProvider();

        Assert.Equal(0, custom.CallCount);
    }

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
