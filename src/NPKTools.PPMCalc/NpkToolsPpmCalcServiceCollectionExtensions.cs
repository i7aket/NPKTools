using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NPKTools.PPMCalc;

/// <summary>
/// Registers the ppm calculation service with a dependency injection container.
/// </summary>
public static class NpkToolsPpmCalcServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IPpmCalculationService"/>.
    /// </summary>
    /// <param name="services">The service collection to add the registration to.</param>
    /// <returns>The same <paramref name="services"/> instance, to allow chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    public static IServiceCollection AddNpkToolsPpmCalc(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<IPpmCalculationService, PpmCalculationService>();

        return services;
    }
}
