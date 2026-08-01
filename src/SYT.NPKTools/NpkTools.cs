using SYT.NPKTools.Fertilizers;
using SYT.NPKTools.Nutrients;
using SYT.NPKTools.Optimization;

namespace SYT.NPKTools;

/// <summary>
/// Creates the library's services with their default wiring.
/// </summary>
/// <remarks>
/// <para>
/// The pieces are all constructible by hand — this only saves you assembling the four-object graph
/// the optimizer needs. Every returned type is stateless (the bundle repository caches lazily and
/// immutably), so a single instance can be shared for the lifetime of the process.
/// </para>
/// <para>
/// There is deliberately no <c>AddNpkTools()</c> extension method. Providing one would mean
/// depending on <c>Microsoft.Extensions.DependencyInjection.Abstractions</c>, and this package has no
/// dependencies at all. Registering with a container is one line per service, because your
/// application already has <c>IServiceCollection</c>:
/// </para>
/// <code>
/// services.AddSingleton(NpkTools.CreateOptimizationService());
/// services.AddSingleton(NpkTools.CreatePpmCalculator());
/// services.AddSingleton(NpkTools.CreateTargetParser());
/// </code>
/// </remarks>
public static class NpkTools
{
    /// <summary>
    /// Creates the preset optimization service: the curated fertilizer catalogue plus the optimizer
    /// that searches it.
    /// </summary>
    /// <param name="solver">
    /// The linear solver to use. Defaults to <see cref="SimplexOptimizationSolver"/>, which is fully
    /// managed and therefore works everywhere .NET runs, including WebAssembly. Pass your own to
    /// substitute a different backend — see <see cref="IOptimizationProblemSolver"/>.
    /// </param>
    /// <returns>A ready-to-use <see cref="IFertilizerOptimizationService"/>.</returns>
    public static IFertilizerOptimizationService CreateOptimizationService(
        IOptimizationProblemSolver? solver = null) =>
        new FertilizerOptimizationService(CreateOptimizer(solver), new FertilizerBundleRepository());

    /// <summary>
    /// Creates the optimizer without the preset catalogue, for callers supplying their own
    /// fertilizers.
    /// </summary>
    /// <param name="solver">
    /// The linear solver to use. Defaults to <see cref="SimplexOptimizationSolver"/>.
    /// </param>
    /// <returns>A ready-to-use <see cref="IFertilizerOptimizer"/>.</returns>
    public static IFertilizerOptimizer CreateOptimizer(IOptimizationProblemSolver? solver = null) =>
        new FertilizerOptimizationAdapter(
            solver ?? new SimplexOptimizationSolver(),
            new OptimizationProblemMapper());

    /// <summary>
    /// Creates the ppm calculator, which measures the nutrient concentrations a fertilizer mixture
    /// produces in a given volume of water.
    /// </summary>
    /// <returns>A ready-to-use <see cref="IPpmCalculationService"/>.</returns>
    public static IPpmCalculationService CreatePpmCalculator() => new PpmCalculationService();

    /// <summary>
    /// Creates the parser for target strings such as <c>"N=150 P=50 K=200 L=100"</c>.
    /// </summary>
    /// <returns>A ready-to-use <see cref="IPpmTargetParser"/>.</returns>
    public static IPpmTargetParser CreateTargetParser() => new PpmTargetParser();

    /// <summary>
    /// Creates the fertilizer catalogue on its own: 17 macronutrient fertilizers in 18 bundles and 17
    /// micronutrient fertilizers in 4 sets.
    /// </summary>
    /// <returns>A ready-to-use <see cref="IFertilizerBundleRepository"/>.</returns>
    /// <remarks>
    /// Bundles are built lazily and cached, so share the returned instance rather than creating one
    /// per call.
    /// </remarks>
    public static IFertilizerBundleRepository CreateBundleRepository() =>
        new FertilizerBundleRepository();

    /// <summary>
    /// Creates an optimization service that searches a person's own salts instead of the preset
    /// catalogue.
    /// </summary>
    /// <param name="salts">
    /// The salts on hand, in any order and any mix of macro and micro. Weights are irrelevant — the
    /// optimizer computes them — so a salt need only carry its composition.
    /// </param>
    /// <param name="settings">
    /// Bounds on how many bundles to generate from those salts. Defaults to
    /// <see cref="BundleGenerationSettings.Default"/>.
    /// </param>
    /// <param name="solver">The linear solver to use. Defaults to <see cref="SimplexOptimizationSolver"/>.</param>
    /// <returns>An <see cref="IFertilizerOptimizationService"/> over the supplied salts.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="salts"/> is null.</exception>
    /// <remarks>
    /// To see what generation left out — an element none of the salts supply, a salt that carries
    /// nothing usable — construct <see cref="CustomFertilizerBundleRepository"/> directly and read its
    /// properties, then pass it to <see cref="FertilizerOptimizationService"/>. This overload is for the
    /// common case where the salts are known to be sufficient.
    /// </remarks>
    public static IFertilizerOptimizationService CreateOptimizationService(
        IEnumerable<Fertilizer> salts,
        BundleGenerationSettings? settings = null,
        IOptimizationProblemSolver? solver = null) =>
        new FertilizerOptimizationService(
            CreateOptimizer(solver),
            new CustomFertilizerBundleRepository(salts, settings));

    /// <summary>
    /// Creates a bundle repository over a person's own salts, generating the bundles from them.
    /// </summary>
    /// <param name="salts">The salts on hand.</param>
    /// <param name="settings">
    /// Bounds on how many bundles to generate. Defaults to <see cref="BundleGenerationSettings.Default"/>.
    /// </param>
    /// <returns>
    /// The repository. Its concrete type is returned rather than the interface so that callers can read
    /// what generation left out.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="salts"/> is null.</exception>
    public static CustomFertilizerBundleRepository CreateBundleRepository(
        IEnumerable<Fertilizer> salts,
        BundleGenerationSettings? settings = null) =>
        new(salts, settings);
}
