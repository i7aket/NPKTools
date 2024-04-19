using Microsoft.Extensions.DependencyInjection;
using NPKOptimizer.Components;
using NPKOptimizer.Components.Builders.FertilizerCollectionBuilder;
using NPKOptimizer.Components.OptimizationProblemSolvers;
using NPKOptimizer.Components.OptimizationProblemSolvers.Contracts;
using NPKOptimizer.Contracts;
using NPKOptimizer.Repository;
using NPKOptimizer.Repository.Contracts;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace NPKOptimizer;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFertilizerOptimizationService(this IServiceCollection services)
    {
        services.AddSingleton<IPpmCalculationService, PpmCalculationService>();
        services.AddSingleton<INpkTargetParser, NpkTargetParser>();

        services.AddSingleton<IFertilizerRepositoryInitializer, FertilizerRepositoryInitializer>();
        services.AddSingleton<IFertilizerRepository, InMemoryFertilizerRepository>();
        services.AddSingleton<IFertilizerCollectionBuilderFactory, FertilizerCollectionBuilderFactory>();
        services.AddSingleton<IFertilizerBundleRepository, FertilizerBundleRepository>();
        services.AddSingleton<IOptimizationProblemSolver, GoogleOrToolsOptimizationSolver>();
        services.AddSingleton<IFertilizerOptimizer, FertilizerOptimizationAdapter>();
        services.AddSingleton<IFertilizerOptimizationsService, FertilizerOptimizationsService>();
        
        return services;
    }
}