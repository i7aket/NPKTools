using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using NPKTools.Optimizer.Components;
using NPKTools.Optimizer.Contracts;
using NPKTools.Optimizer.Preset;
using Xunit;

namespace NPKTools.Optimizer.OrTools.Tests;

/// <summary>
/// Pins the substitution snippet printed in three READMEs and the migration table:
/// <c>AddNpkToolsOrToolsSolver().AddNpkToolsPreset()</c> must actually produce GLOP.
/// </summary>
/// <remarks>
/// The whole mechanism rests on the default solver being registered with <c>TryAdd</c>, which makes
/// the outcome depend on registration order. That is a footgun worth a test: if the default were ever
/// registered with <c>Add</c> or the order requirement were dropped, every documented example would
/// silently keep using the managed simplex.
/// </remarks>
public class OrToolsRegistrationTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void AddNpkToolsOrToolsSolver_BeforePreset_ResolvesTheGlopSolver()
    {
        // Arrange
        using ServiceProvider provider = new ServiceCollection()
            .AddNpkToolsOrToolsSolver()
            .AddNpkToolsPreset()
            .BuildServiceProvider();

        // Act
        IOptimizationProblemSolver solver = provider.GetRequiredService<IOptimizationProblemSolver>();

        // Assert
        solver.Should().BeOfType<GoogleOrToolsOptimizationSolver>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddNpkToolsOrToolsSolver_BeforeOptimizer_ResolvesTheGlopSolver()
    {
        // Arrange
        using ServiceProvider provider = new ServiceCollection()
            .AddNpkToolsOrToolsSolver()
            .AddNpkToolsOptimizer()
            .BuildServiceProvider();

        // Act
        IOptimizationProblemSolver solver = provider.GetRequiredService<IOptimizationProblemSolver>();

        // Assert
        solver.Should().BeOfType<GoogleOrToolsOptimizationSolver>();
    }

    /// <summary>
    /// The order documented in the READMEs is not decoration. Registering the preset first wins,
    /// because the managed default gets in via <c>TryAdd</c> before the OR-Tools call is reached.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AddNpkToolsOrToolsSolver_AfterPreset_LeavesTheManagedDefaultInPlace()
    {
        // Arrange
        using ServiceProvider provider = new ServiceCollection()
            .AddNpkToolsPreset()
            .AddNpkToolsOrToolsSolver()
            .BuildServiceProvider();

        // Act
        IOptimizationProblemSolver solver = provider.GetRequiredService<IOptimizationProblemSolver>();

        // Assert
        solver.Should().BeOfType<SimplexOptimizationSolver>();
    }

    /// <summary>
    /// Substituting the solver must not disturb the rest of the graph: the preset service still
    /// resolves and still finds solutions, now through GLOP.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void AddNpkToolsOrToolsSolver_WithPreset_StillProducesSolutions()
    {
        // Arrange
        using ServiceProvider provider = new ServiceCollection()
            .AddNpkToolsOrToolsSolver()
            .AddNpkToolsPreset()
            .BuildServiceProvider();

        IFertilizerOptimizationService service =
            provider.GetRequiredService<IFertilizerOptimizationService>();

        // Act
        Core.Domain.Collections.Solutions solutions = service.FindMacroSolutions(
            new Core.Domain.PpmTarget.Builder.PpmTargetBuilder()
                .AddN(150)
                .AddP(50)
                .AddK(200)
                .AddCa(100)
                .AddMg(50)
                .AddS(60)
                .AddLiters(100)
                .Build());

        // Assert
        solutions.Should().NotBeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddNpkToolsOrToolsSolver_CalledTwice_RegistersOneSolver()
    {
        // Arrange
        ServiceCollection services = new();

        // Act
        services.AddNpkToolsOrToolsSolver().AddNpkToolsOrToolsSolver();

        // Assert
        services.Count(descriptor => descriptor.ServiceType == typeof(IOptimizationProblemSolver))
            .Should().Be(1);
    }
}
