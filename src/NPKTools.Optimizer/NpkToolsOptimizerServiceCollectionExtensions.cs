using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NPKTools.Optimizer.Components;
using NPKTools.Optimizer.Contracts;

namespace NPKTools.Optimizer;

/// <summary>
/// Registers the fertilizer optimizer with a dependency injection container.
/// </summary>
public static class NpkToolsOptimizerServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IFertilizerOptimizer"/> together with the mapper and solver it needs.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <returns>The same <paramref name="services"/> instance, to allow chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    /// <remarks>
    /// <para>
    /// The default solver is the managed <see cref="SimplexOptimizationSolver"/>, which has no native
    /// dependencies and therefore works on every .NET target including WebAssembly.
    /// </para>
    /// <para>
    /// All three components are stateless, so they are registered as singletons. Registrations use
    /// <c>TryAdd</c>, so a solver registered beforehand wins — this is how
    /// <c>NPKTools.Optimizer.OrTools</c> substitutes the GLOP backend:
    /// </para>
    /// <code>
    /// services.AddNpkToolsOrToolsSolver().AddNpkToolsOptimizer();
    /// </code>
    /// </remarks>
    public static IServiceCollection AddNpkToolsOptimizer(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<IOptimizationProblemSolver, SimplexOptimizationSolver>();
        services.TryAddSingleton<IOptimizationProblemMapper, OptimizationProblemMapper>();
        services.TryAddSingleton<IFertilizerOptimizer, FertilizerOptimizationAdapter>();

        return services;
    }
}
