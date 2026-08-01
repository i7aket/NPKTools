using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SYT.NPKTools.Nutrients;
using SYT.NPKTools.Optimization;

namespace SYT.NPKTools;

/// <summary>
/// Registers NPKTools with a dependency injection container.
/// </summary>
public static class NpkToolsServiceCollectionExtensions
{
    /// <summary>
    /// Registers every NPKTools service: the optimizer and the components beneath it, the preset
    /// fertilizer catalogue, the ppm calculator and the ppm target parser.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <returns>The same <paramref name="services"/> instance, to allow chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    /// <remarks>
    /// <para>
    /// Everything registered here is stateless — or, in the bundle repository's case, an immutable
    /// lazily-built cache — so all registrations are singletons.
    /// </para>
    /// <para>
    /// Registrations use <c>TryAdd</c>, so anything you register first wins. That is the supported way
    /// to substitute your own linear solver:
    /// </para>
    /// <code>
    /// services.AddSingleton&lt;IOptimizationProblemSolver, MyOwnSolver&gt;()
    ///         .AddNpkTools();
    /// </code>
    /// </remarks>
    public static IServiceCollection AddNpkTools(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<IOptimizationProblemSolver, SimplexOptimizationSolver>();
        services.TryAddSingleton<IOptimizationProblemMapper, OptimizationProblemMapper>();
        services.TryAddSingleton<IFertilizerOptimizer, FertilizerOptimizationAdapter>();

        services.TryAddSingleton<IFertilizerBundleRepository, FertilizerBundleRepository>();
        services.TryAddSingleton<IFertilizerOptimizationService, FertilizerOptimizationService>();

        services.TryAddSingleton<IPpmCalculationService, PpmCalculationService>();
        services.TryAddSingleton<IPpmTargetParser, PpmTargetParser>();

        return services;
    }
}
