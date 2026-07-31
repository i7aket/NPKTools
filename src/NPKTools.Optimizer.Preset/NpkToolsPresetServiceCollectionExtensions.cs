using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NPKTools.Optimizer.Preset;

/// <summary>
/// Registers the preconfigured fertilizer optimization service with a dependency injection container.
/// </summary>
public static class NpkToolsPresetServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IFertilizerOptimizationService"/> and its bundle repository,
    /// along with the underlying optimizer from <c>NPKTools.Optimizer</c>.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <returns>The same <paramref name="services"/> instance, to allow chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    /// <remarks>
    /// The bundle repository builds its 18 macro and 4 micro bundles lazily and caches them,
    /// so it is registered as a singleton to avoid rebuilding them per request.
    /// </remarks>
    public static IServiceCollection AddNpkToolsPreset(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddNpkToolsOptimizer();
        services.TryAddSingleton<IFertilizerBundleRepository, FertilizerBundleRepository>();
        services.TryAddSingleton<IFertilizerOptimizationService, FertilizerOptimizationService>();

        return services;
    }
}
