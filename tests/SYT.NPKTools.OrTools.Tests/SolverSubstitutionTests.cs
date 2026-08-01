using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using SYT.NPKTools.Optimization;
using SYT.NPKTools.OrToolsOracle;
using Xunit;

namespace SYT.NPKTools.OrTools.Tests;

/// <summary>
/// <see cref="IOptimizationProblemSolver"/> is the package's documented extension point: pass one to
/// <see cref="NpkTools.CreateOptimizationService"/> and it replaces the managed default.
/// </summary>
/// <remarks>
/// These tests substitute a real alternative — the GLOP oracle — rather than a stub, so they prove the
/// seam works with a solver that has different numerical behaviour and a native dependency, not just
/// that a delegate gets called. That is the path a consumer wanting GLOP on a server would follow.
/// </remarks>
public class SolverSubstitutionTests
{
    private static readonly PpmTarget Target = new PpmTargetBuilder()
        .AddN(150).AddP(50).AddK(200).AddCa(100).AddMg(50).AddS(60).AddLiters(100)
        .Build();

    [Fact]
    [Trait("Category", "Integration")]
    public void SuppliedSolver_ProducesSolutionsThroughTheWholePipeline()
    {
        // Arrange
        IFertilizerOptimizationService service =
            NpkTools.CreateOptimizationService(new GoogleOrToolsOptimizationSolver());

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
        // Act
        Solutions viaSimplex = NpkTools
            .CreateOptimizationService(new SimplexOptimizationSolver())
            .FindMacroSolutions(Target);
        Solutions viaGlop = NpkTools
            .CreateOptimizationService(new GoogleOrToolsOptimizationSolver())
            .FindMacroSolutions(Target);

        // Assert
        viaSimplex.Should().NotBeEmpty();
        viaGlop.Count.Should().Be(viaSimplex.Count);
    }

    /// <summary>
    /// The default and an explicitly-passed managed simplex must behave identically — otherwise the
    /// default is not what the documentation says it is.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void OmittingTheSolver_MatchesPassingTheManagedSimplex()
    {
        // Act
        Solutions viaDefault = NpkTools.CreateOptimizationService().FindMacroSolutions(Target);
        Solutions viaExplicit = NpkTools
            .CreateOptimizationService(new SimplexOptimizationSolver())
            .FindMacroSolutions(Target);

        // Assert
        viaDefault.Count.Should().Be(viaExplicit.Count);
        viaDefault.Should().NotBeEmpty();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void SuppliedSolver_IsAlsoHonouredByTheBareOptimizer()
    {
        // Arrange
        IFertilizerOptimizer viaGlop = NpkTools.CreateOptimizer(new GoogleOrToolsOptimizationSolver());
        IFertilizerOptimizer viaSimplex = NpkTools.CreateOptimizer(new SimplexOptimizationSolver());
        SolutionFinderSettings settings = new SolutionFinderSettingsBuilder()
            .AddN(1).AddP(1).AddK(1).AddCa(1).AddMg(1).AddS(1)
            .Build();
        IReadOnlyList<Fertilizers.Fertilizer> bundle = NpkTools.CreateBundleRepository().Macro()[0];

        // Act
        Solution? glop = viaGlop.Optimize(Target, bundle, settings);
        Solution? simplex = viaSimplex.Optimize(Target, bundle, settings);

        // Assert: the two agree on whether this bundle can hit the target at all.
        (glop is null).Should().Be(simplex is null);
    }
}
