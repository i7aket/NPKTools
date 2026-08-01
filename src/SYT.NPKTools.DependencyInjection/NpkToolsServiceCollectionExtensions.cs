using Microsoft.Extensions.DependencyInjection.Extensions;
using SYT.NPKTools;
using SYT.NPKTools.Nutrients;
using SYT.NPKTools.Optimization;

// Deliberately in the Microsoft.Extensions.DependencyInjection namespace, the convention for
// IServiceCollection extensions: a typical Program.cs already imports it, so `services.AddNpkTools()`
// is available with no extra using directive.
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Registers NPKTools with a dependency injection container in one call.
/// </summary>
/// <remarks>
/// This lives in its own package so that <c>SYT.NPKTools</c> itself has no dependencies. Everything
/// here is a thin wrapper over the <see cref="NpkTools"/> factory, which is public — so if you would
/// rather not take a second package, register the factory results directly and you lose nothing:
/// <code>
/// services.AddSingleton(NpkTools.CreateOptimizationService());
/// services.AddSingleton(NpkTools.CreatePpmCalculator());
/// services.AddSingleton(NpkTools.CreateTargetParser());
/// </code>
/// </remarks>
public static class NpkToolsServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IFertilizerOptimizationService"/>, <see cref="IPpmCalculationService"/>,
    /// <see cref="IPpmTargetParser"/> and <see cref="IFertilizerBundleRepository"/> as singletons.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <param name="solver">
    /// The linear solver to use. Defaults to the managed <c>SimplexOptimizationSolver</c>, which has no
    /// native dependencies and therefore works on every .NET target including WebAssembly.
    /// </param>
    /// <returns>The same <paramref name="services"/> instance, to allow chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    /// <remarks>
    /// <para>
    /// Everything registered is stateless, apart from the bundle repository's immutable lazy cache, so
    /// singletons are correct and the registrations are safe to share across threads.
    /// </para>
    /// <para>
    /// Substituting a solver is the <paramref name="solver"/> argument rather than a registration-order
    /// rule, so it cannot be silently ineffective. Registrations use <c>TryAdd</c>, which only makes
    /// calling this twice idempotent — it is not the substitution mechanism.
    /// </para>
    /// </remarks>
    public static IServiceCollection AddNpkTools(
        this IServiceCollection services,
        IOptimizationProblemSolver? solver = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton(_ => NpkTools.CreateBundleRepository());
        services.TryAddSingleton(_ => NpkTools.CreateOptimizer(solver));
        services.TryAddSingleton(_ => NpkTools.CreatePpmCalculator());
        services.TryAddSingleton(_ => NpkTools.CreateTargetParser());

        // Composed from the resolved registrations rather than from NpkTools.CreateOptimizationService,
        // which would build its own optimizer and catalogue — leaving the container handing out a
        // different IFertilizerBundleRepository than the service actually uses. Harmless, since the
        // catalogue is immutable and value-identical, but two lazy copies of it is not what anyone
        // reading the registrations would expect.
        services.TryAddSingleton<IFertilizerOptimizationService>(provider =>
            new FertilizerOptimizationService(
                provider.GetRequiredService<IFertilizerOptimizer>(),
                provider.GetRequiredService<IFertilizerBundleRepository>()));

        return services;
    }
}
