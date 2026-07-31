using Microsoft.Extensions.DependencyInjection;
using NPKTools.Core.Domain.PpmTarget;
using NPKTools.Core.Domain.PpmTarget.Builder;
using NPKTools.Optimizer;
using NPKTools.Optimizer.Contracts;
using NPKTools.Optimizer.Preset;
using NPKTools.Optimizer.PpmTargetParser;
using NPKTools.PPMCalc;
using Xunit;

namespace NPKTools.IntegrationTests;

/// <summary>
/// Covers the DI registration helpers added in 2.0.0. Before then, consumers had to construct
/// the solver, mapper, adapter and bundle repository by hand in the right order.
/// </summary>
public class ServiceCollectionExtensionsTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkToolsPreset_ResolvesWorkingOptimizationService()
    {
        ServiceProvider provider = new ServiceCollection()
            .AddNpkToolsPreset()
            .BuildServiceProvider();

        IFertilizerOptimizationService service = provider.GetRequiredService<IFertilizerOptimizationService>();
        PpmTarget target = new PpmTargetBuilder().AddN(150).AddP(50).AddK(200).AddCa(100).AddMg(50).Build();

        Assert.NotEmpty(service.FindMacroSolutions(target));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkToolsOptimizer_RegistersEveryComponent()
    {
        ServiceProvider provider = new ServiceCollection()
            .AddNpkToolsOptimizer()
            .BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IOptimizationProblemSolver>());
        Assert.NotNull(provider.GetRequiredService<IOptimizationProblemMapper>());
        Assert.NotNull(provider.GetRequiredService<IFertilizerOptimizer>());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkToolsOptimizer_DoesNotOverrideAnExistingSolver()
    {
        CountingSolver custom = new();

        ServiceProvider provider = new ServiceCollection()
            .AddSingleton<IOptimizationProblemSolver>(custom)
            .AddNpkToolsOptimizer()
            .BuildServiceProvider();

        Assert.Same(custom, provider.GetRequiredService<IOptimizationProblemSolver>());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkTools_RegistrationsAreIdempotent()
    {
        ServiceProvider provider = new ServiceCollection()
            .AddNpkToolsPreset()
            .AddNpkToolsPreset()
            .BuildServiceProvider();

        Assert.Single(provider.GetServices<IFertilizerOptimizationService>());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkToolsPpmCalcAndParser_ResolveTheirServices()
    {
        ServiceProvider provider = new ServiceCollection()
            .AddNpkToolsPpmCalc()
            .AddNpkToolsPpmTargetParser()
            .BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IPpmCalculationService>());
        Assert.NotNull(provider.GetRequiredService<IPpmTargetParser>());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void FindMacroSolutions_CancelledToken_ThrowsBeforeSolvingAnyBundle()
    {
        CountingSolver solver = new();

        ServiceProvider provider = new ServiceCollection()
            .AddSingleton<IOptimizationProblemSolver>(solver)
            .AddNpkToolsPreset()
            .BuildServiceProvider();

        IFertilizerOptimizationService service = provider.GetRequiredService<IFertilizerOptimizationService>();
        PpmTarget target = new PpmTargetBuilder().AddN(150).Build();

        using CancellationTokenSource cts = new();
        cts.Cancel();

        Assert.Throws<OperationCanceledException>(() => service.FindMacroSolutions(target, cts.Token));
        Assert.Equal(0, solver.CallCount);
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
