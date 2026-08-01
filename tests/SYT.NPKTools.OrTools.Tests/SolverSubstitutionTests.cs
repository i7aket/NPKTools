using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using SYT.NPKTools.Nutrients;
using SYT.NPKTools.Optimization;
using SYT.NPKTools.OrToolsOracle;
using Xunit;

namespace SYT.NPKTools.OrTools.Tests;

/// <summary>
/// <see cref="IOptimizationProblemSolver"/> is the package's documented extension point: the shipped
/// managed simplex is registered with <c>TryAdd</c>, so registering another solver first replaces it.
/// </summary>
/// <remarks>
/// These tests substitute a real alternative — the GLOP oracle — rather than a stub, so they prove the
/// seam works with a solver that has different numerical behaviour and a native dependency, not just
/// that DI resolves the type. That is the scenario a consumer wanting GLOP on a server would follow.
/// </remarks>
public class SolverSubstitutionTests
{
    private static readonly PpmTarget Target = new PpmTargetBuilder()
        .AddN(150).AddP(50).AddK(200).AddCa(100).AddMg(50).AddS(60).AddLiters(100)
        .Build();

    [Fact]
    [Trait("Category", "Integration")]
    public void SolverRegisteredBeforeAddNpkTools_Replaces_TheManagedDefault()
    {
        // Arrange
        using ServiceProvider provider = new ServiceCollection()
            .AddSingleton<IOptimizationProblemSolver, GoogleOrToolsOptimizationSolver>()
            .AddNpkTools()
            .BuildServiceProvider();

        // Act
        IOptimizationProblemSolver solver = provider.GetRequiredService<IOptimizationProblemSolver>();

        // Assert
        solver.Should().BeOfType<GoogleOrToolsOptimizationSolver>();
    }

    /// <summary>
    /// Registering afterwards resolves to GLOP as well, because <c>GetRequiredService</c> returns the
    /// last descriptor — but the managed default is still registered underneath it. Registering first
    /// is the documented order because it leaves exactly one solver in the container.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void SolverRegisteredAfterAddNpkTools_WinsButLeavesTheDefaultRegistered()
    {
        // Arrange
        using ServiceProvider provider = new ServiceCollection()
            .AddNpkTools()
            .AddSingleton<IOptimizationProblemSolver, GoogleOrToolsOptimizationSolver>()
            .BuildServiceProvider();

        // Act
        IOptimizationProblemSolver[] solvers = [.. provider.GetServices<IOptimizationProblemSolver>()];

        // Assert
        provider.GetRequiredService<IOptimizationProblemSolver>()
            .Should().BeOfType<GoogleOrToolsOptimizationSolver>();
        solvers.Should().HaveCount(2);
        solvers[0].Should().BeOfType<SimplexOptimizationSolver>();
    }

    /// <summary>
    /// Substituting the solver must not disturb the rest of the graph: the preset service still
    /// resolves and still finds mixes, now computed by GLOP.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void SubstitutedSolver_StillProducesSolutionsThroughTheWholePipeline()
    {
        // Arrange
        using ServiceProvider provider = new ServiceCollection()
            .AddSingleton<IOptimizationProblemSolver, GoogleOrToolsOptimizationSolver>()
            .AddNpkTools()
            .BuildServiceProvider();

        IFertilizerOptimizationService service =
            provider.GetRequiredService<IFertilizerOptimizationService>();

        // Act
        Solutions solutions = service.FindMacroSolutions(Target);

        // Assert
        solutions.Should().NotBeEmpty();
    }

    /// <summary>
    /// Both backends drive the identical pipeline, so a caller swapping them gets the same number of
    /// distinct mixes for this target. Individual weights may differ where an optimum is degenerate,
    /// which is why this asserts the count rather than the contents.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void BothBackends_FindTheSameNumberOfMacroSolutions()
    {
        // Arrange
        Solutions viaSimplex = BuildService(new SimplexOptimizationSolver()).FindMacroSolutions(Target);
        Solutions viaGlop = BuildService(new GoogleOrToolsOptimizationSolver()).FindMacroSolutions(Target);

        // Assert
        viaSimplex.Should().NotBeEmpty();
        viaGlop.Count.Should().Be(viaSimplex.Count);
    }

    private static IFertilizerOptimizationService BuildService(IOptimizationProblemSolver solver) =>
        new FertilizerOptimizationService(
            new FertilizerOptimizationAdapter(solver, new OptimizationProblemMapper()),
            new FertilizerBundleRepository());
}
