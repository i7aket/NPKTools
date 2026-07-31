using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NPKTools.Optimizer.PpmTargetParser;

/// <summary>
/// Registers the ppm target parser with a dependency injection container.
/// </summary>
public static class NpkToolsPpmTargetParserServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IPpmTargetParser"/>.
    /// </summary>
    /// <param name="services">The service collection to add the registration to.</param>
    /// <returns>The same <paramref name="services"/> instance, to allow chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    public static IServiceCollection AddNpkToolsPpmTargetParser(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<IPpmTargetParser, PpmTargetParser>();

        return services;
    }
}
