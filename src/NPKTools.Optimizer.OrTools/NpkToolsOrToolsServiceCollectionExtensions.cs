using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NPKTools.Optimizer.Contracts;

namespace NPKTools.Optimizer.OrTools;

/// <summary>
/// Registers the Google OR-Tools solver backend with a dependency injection container.
/// </summary>
public static class NpkToolsOrToolsServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="GoogleOrToolsOptimizationSolver"/> as the <see cref="IOptimizationProblemSolver"/>,
    /// replacing the managed default.
    /// </summary>
    /// <param name="services">The service collection to add the registration to.</param>
    /// <returns>The same <paramref name="services"/> instance, to allow chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    /// <remarks>
    /// Call this <em>before</em> <c>AddNpkToolsOptimizer()</c> or <c>AddNpkToolsPreset()</c>. Those
    /// methods register the managed solver with <c>TryAdd</c>, so whichever solver is registered
    /// first is the one that gets used:
    /// <code>
    /// services.AddNpkToolsOrToolsSolver().AddNpkToolsPreset();
    /// </code>
    /// </remarks>
    public static IServiceCollection AddNpkToolsOrToolsSolver(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<IOptimizationProblemSolver, GoogleOrToolsOptimizationSolver>();

        return services;
    }
}
