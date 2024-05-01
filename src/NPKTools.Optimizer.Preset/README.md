# NPKTools.Optimizer.Preset
-  NPKTools.Optimizer.Preset is an preconfigured version of the  NPKTools.Optimizer. It includes:
- 17 basic types of macronutrient fertilizers combined into 18 different sets to optimize the selection process.
- 17 types of micronutrient fertilizers grouped into four main sets: basic, sulfate, nitrate, and chelated.
- During the selection of macronutrient fertilizers, the service conducts two searches: in the first, sulfur is accounted for as specified, while the second search excludes sulfur coefficients to expand the possible options.

## Developers
This tool was developed by **Anatoliy Yermakov**.
- **LinkedIn**: [Anatoliy Yermakov](https://www.linkedin.com/in/anatoliyyermakov)
- **GitHub**: [i7aket](https://github.com/i7aket)

Special thanks to **Artem Frolov** for his invaluable assistance and guidance in the development of this project.
- **LinkedIn**: [Artem Frolov](https://www.linkedin.com/in/artfrolov/)
- **GitHub**: [AqueGen](https://github.com/AqueGen)


## Installation and Configuration

This section guides you through setting up and configuring the necessary components for the project using Dependency Injection (DI).

### Direct Instantiation

To manually instantiate the components without using DI, use the following example:

```csharp
// Creating instances manually without DI
        IOptimizationProblemSolver solver = new GoogleOrToolsOptimizationSolver();
        IOptimizationProblemMapper mapper = new OptimizationProblemMapper();
        IFertilizerOptimizer optimizer = new FertilizerOptimizationAdapter(solver, mapper);
        IFertilizerBundleRepository bundles = new FertilizerBundleRepository();        
        IFertilizerOptimizationsService = new FertilizerOptimizationService(optimizer, bundles);
```
Using Dependency Injection
For integrating these components into a project that supports Dependency Injection, such as an ASP.NET Core application, configure your services in the Startup.cs or a similar configuration file as follows:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Registering services in the DI container
    services.AddSingleton<IOptimizationProblemSolver, GoogleOrToolsOptimizationSolver>();
    services.AddSingleton<IOptimizationProblemMapper, OptimizationProblemMapper>();
    services.AddSingleton<IFertilizerOptimizer, FertilizerOptimizationAdapter>();
    services.AddSingleton<IFertilizerBundleRepository, FertilizerBundleRepository>();
    services.AddSingleton<IFertilizerOptimizationService, FertilizerOptimizationService>();
}
```

## Example usage
```csharp
// Creating instances manually without DI
IOptimizationProblemSolver solver = new GoogleOrToolsOptimizationSolver();
IOptimizationProblemMapper mapper = new OptimizationProblemMapper();
IFertilizerOptimizer optimizer = new FertilizerOptimizationAdapter(solver, mapper);
IFertilizerBundleRepository bundles = new FertilizerBundleRepository();
IFertilizerOptimizationsService fertilizerOptimizationsService = new FertilizerOptimizationService(optimizer, bundles);

// Define your PPM target for optimization
PpmTarget target = new PpmTargetBuilder()
    .AddN(150)
    .AddP(50)
    .AddK(200)
    .AddMg(60)
    .AddCa(60)
    .AddS(80)
    .Build();

// Set the options for matching the macro elements precisely
SolutionFinderSettings settings = new SolutionFinderSettingsBuilder()
    .AddN(1)
    .AddP(1)
    .AddK(1)
    .AddCa(1)
    .AddMg(1)
    .AddS(1)
    .AddCl(1)
    .Build();

// Finding macro solutions based on the specified target
Solutions macroSolutions = fertilizerOptimizationsService.FindMacroSolutions(target);

// Finding micro solutions with a different target
PpmTarget microTarget = new PpmTargetBuilder()
    .AddFe(2)
    .AddCu(0.05)
    .AddMn(0.55)
    .AddZn(0.33)
    .AddB(0.28)
    .AddMo(0.05)
    .AddSi(0.01)
    .AddSe(0.01)
    .Build();

Solutions microSolutions = fertilizerOptimizationsService.FindMicroSolutions(microTarget);

// Combine both macro and micro nutrient optimizations
(Solutions Macro, Solutions Micro) results = fertilizerOptimizationsService.FindSolutions(new PpmTargetBuilder()
    .AddN(150)
    .AddP(50)
    .AddK(200)
    .AddMg(60)
    .AddCa(100)
    .AddS(100)
    .AddFe(2)
    .AddCu(0.05)
    .AddMn(0.55)
    .AddZn(0.33)
    .AddB(0.28)
    .AddMo(0.05)
    .AddCl(0.01)
    .AddSi(0.01)
    .AddSe(0.01)
    .Build());

// You can now access results.Macro and results.Micro for specific optimizations
```

## License:
This project is licensed under the MIT License.

## Dependencies
### NPKTools.Optimizer.Preset.Tests
- [**xUnit**](https://xunit.net/): Framework for unit testing.
- [**AutoFixture**](https://github.com/AutoFixture/AutoFixture): Generates test data.
- [**FluentAssertions**](https://fluentassertions.com/): Enhanced assertions for tests.
- [**Microsoft.AspNetCore.Mvc.Testing**](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0): Testing for ASP.NET MVC applications.
- [**NSubstitute**](https://nsubstitute.github.io/): Library for creating mock and stub objects.
- [**Microsoft.NET.Test.Sdk**](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/): Test SDK for .NET.

## Contact Information:
- **LinkedIn**: [Anatoliy Yermakov](https://www.linkedin.com/in/anatoliyyermakov)
