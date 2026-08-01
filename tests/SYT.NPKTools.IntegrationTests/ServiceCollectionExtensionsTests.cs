using Microsoft.Extensions.DependencyInjection;
using SYT.NPKTools.Nutrients;
using SYT.NPKTools.Optimization;
using Xunit;

namespace SYT.NPKTools.IntegrationTests;

/// <summary>
/// Covers <see cref="NpkToolsServiceCollectionExtensions.AddNpkTools"/>. Without it, consumers had to
/// construct the solver, mapper, adapter and bundle repository by hand in the right order.
/// </summary>
public class ServiceCollectionExtensionsTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkTools_RegistersEveryService()
    {
        using ServiceProvider provider = new ServiceCollection()
            .AddNpkTools()
            .BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IOptimizationProblemSolver>());
        Assert.NotNull(provider.GetRequiredService<IOptimizationProblemMapper>());
        Assert.NotNull(provider.GetRequiredService<IFertilizerOptimizer>());
        Assert.NotNull(provider.GetRequiredService<IFertilizerBundleRepository>());
        Assert.NotNull(provider.GetRequiredService<IFertilizerOptimizationService>());
        Assert.NotNull(provider.GetRequiredService<IPpmCalculationService>());
        Assert.NotNull(provider.GetRequiredService<IPpmTargetParser>());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkTools_ResolvesWorkingOptimizationService()
    {
        using ServiceProvider provider = new ServiceCollection()
            .AddNpkTools()
            .BuildServiceProvider();

        IFertilizerOptimizationService service = provider.GetRequiredService<IFertilizerOptimizationService>();
        PpmTarget target = new PpmTargetBuilder().AddN(150).AddP(50).AddK(200).AddCa(100).AddMg(50).Build();

        Assert.NotEmpty(service.FindMacroSolutions(target));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkTools_DefaultSolverIsTheManagedSimplex()
    {
        using ServiceProvider provider = new ServiceCollection()
            .AddNpkTools()
            .BuildServiceProvider();

        Assert.IsType<SimplexOptimizationSolver>(provider.GetRequiredService<IOptimizationProblemSolver>());
    }

    /// <summary>
    /// Registrations use <c>TryAdd</c>, which is what makes substituting a solver possible — and also
    /// makes the outcome order-dependent. Both directions are pinned so neither can change silently.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkTools_DoesNotOverrideASolverRegisteredFirst()
    {
        CountingSolver custom = new();

        using ServiceProvider provider = new ServiceCollection()
            .AddSingleton<IOptimizationProblemSolver>(custom)
            .AddNpkTools()
            .BuildServiceProvider();

        Assert.Same(custom, provider.GetRequiredService<IOptimizationProblemSolver>());
    }

    /// <summary>
    /// Registering afterwards also resolves to the custom solver — <c>GetRequiredService</c> returns the
    /// last descriptor — but it leaves two registrations behind, so anything resolving
    /// <c>IEnumerable&lt;IOptimizationProblemSolver&gt;</c> sees both. Registering first is documented
    /// because it leaves exactly one.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkTools_ASolverRegisteredAfterwardsWinsButLeavesTwoRegistrations()
    {
        CountingSolver custom = new();

        using ServiceProvider provider = new ServiceCollection()
            .AddNpkTools()
            .AddSingleton<IOptimizationProblemSolver>(custom)
            .BuildServiceProvider();

        Assert.Same(custom, provider.GetRequiredService<IOptimizationProblemSolver>());
        Assert.Equal(2, provider.GetServices<IOptimizationProblemSolver>().Count());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkTools_ASolverRegisteredFirst_LeavesExactlyOneRegistration()
    {
        CountingSolver custom = new();

        using ServiceProvider provider = new ServiceCollection()
            .AddSingleton<IOptimizationProblemSolver>(custom)
            .AddNpkTools()
            .BuildServiceProvider();

        Assert.Single(provider.GetServices<IOptimizationProblemSolver>());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkTools_IsIdempotent()
    {
        using ServiceProvider provider = new ServiceCollection()
            .AddNpkTools()
            .AddNpkTools()
            .BuildServiceProvider();

        Assert.Single(provider.GetServices<IFertilizerOptimizationService>());
        Assert.Single(provider.GetServices<IOptimizationProblemSolver>());
        Assert.Single(provider.GetServices<IPpmCalculationService>());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkTools_NullServices_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => ((IServiceCollection)null!).AddNpkTools());

    [Fact]
    [Trait("Category", "Integration")]
    public void FindMacroSolutions_CancelledToken_ThrowsBeforeSolvingAnyBundle()
    {
        CountingSolver solver = new();

        using ServiceProvider provider = new ServiceCollection()
            .AddSingleton<IOptimizationProblemSolver>(solver)
            .AddNpkTools()
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
