# NPKTools.Optimizer
NPKTools.Optimizer is a tool developed on the .NET platform that helps adjust the ratio of each fertilizer to achieve the desired PPM (parts per million) profile. Additionally, it is capable of calculating the PPM of a mixture of fertilizers.
- The FertilizerOptimizer is composed of three main components: an adapter, a mapper, and a solver. It takes a nutrient target in NPK values, a list of available fertilizers, and specific precision settings as inputs to calculate the optimal fertilizer mix. Here's how the flow works with these components:
- Mapper: This component converts the high-level nutrient targets and the list of available fertilizers into a structured optimization problem. It translates real-world data about fertilizers and nutrient requirements into mathematical variables, constraints, and objectives suitable for optimization. After the solver processes these, the mapper also translates the optimized mathematical solution back into practical terms, specifying exact quantities of each fertilizer to use.
- Solver: Once the optimization problem is defined, this component uses mathematical algorithms to find the best combination of fertilizers. The solver processes the variables and constraints set by the mapper to minimize or maximize the objective function, typically focusing on cost efficiency or nutrient precision.
- Adapter: Acts as the bridge between the domain-specific data (fertilizers and nutrient targets) and the optimization tools. It takes the problem as defined by the mapper, passes it to the solver for optimization, and then interprets the output from the solver back into a practical solution, detailing the specific amounts of each fertilizer to use to achieve the nutrient targets.
- Together, these components ensure that the FertilizerOptimizer effectively integrates and processes the inputs to produce an optimized fertilizer mix that meets the specified NPK goals with the required precision.
## Developers
This tool was developed by **Anatoliy Yermakov**.
- **LinkedIn**: [Anatoliy Yermakov](https://www.linkedin.com/in/anatoliyyermakov)
- **GitHub**: [i7aket](https://github.com/i7aket)

Special thanks to **Artem Frolov** for his invaluable assistance and guidance in the development of this project.
- **LinkedIn**: [Artem Frolov](https://www.linkedin.com/in/artfrolov/)
- **GitHub**: [AqueGen](https://github.com/AqueGen)

## License:
This project is licensed under the MIT License.

## Installation and Configuration

This section guides you through setting up and configuring the necessary components for the project using Dependency Injection (DI).

### Direct Instantiation

To manually instantiate the components without using DI, use the following example:

```csharp
// Creating instances manually without DI
    IOptimizationProblemSolver solver = new GoogleOrToolsOptimizationSolver();
    IOptimizationProblemMapper mapper = new OptimizationProblemMapper();
    IFertilizerOptimizer optimizer = new FertilizerOptimizationAdapter(solver, mapper);
```
Using Dependency Injection
For integrating these components into a project that supports Dependency Injection, such as an ASP.NET Core application, configure your services in the Startup.cs or a similar configuration file as follows:
```csharp
public void ConfigureServices(IServiceCollection services)
{
// Registering services in the DI container
    services.AddSingleton<IOptimizationProblemSolver, GoogleOrToolsOptimizationSolver>();
    services.AddSingleton<IOptimizationProblemMapper, OptimizationProblemMapper>();
    services.AddSingleton<FertilizerOptimizationAdapter>();
}
```

## Example usage
```csharp
// Setup the optimizer
IOptimizationProblemSolver solver = new GoogleOrToolsOptimizationSolver();
IOptimizationProblemMapper mapper = new OptimizationProblemMapper();
IFertilizerOptimizer optimizer = new FertilizerOptimizationAdapter(solver, mapper);

// Define your PPM target for optimization
PpmTarget target = new PpmTargetBuilder()
    .AddN(150)
    .AddP(50)
    .AddK(200)
    .AddMg(60)
    .AddCa(60)
    .AddS(80)
    .Build();

// Configure the solution finder settings
SolutionFinderSettings settings = new SolutionFinderSettingsBuilder()
    .AddN(1)
    .AddP(1)
    .AddK(1)
    .AddCa(1)
    .AddMg(1)
    .AddS(1)
    .AddCl(1)
    .Build();

// Create a list of fertilizers for optimization
IList<FertilizerOptimizationModel> collection = new List<FertilizerOptimizationModel>()
{
    new FertilizerBuilder().AddNo3(11.863).AddCaNonChelated(16.972).Build(),
    new FertilizerBuilder().AddNo3(13.854).AddK(38.672).Build(),
    new FertilizerBuilder().AddNo3(17.499).AddNh4(17.499).Build(),
    new FertilizerBuilder().AddMgNonChelated(9.861).AddS(13.008).Build(),
    new FertilizerBuilder().AddP(22.761).AddK(28.731).Build(),
    new FertilizerBuilder().AddS(18.401).AddK(44.874).Build(),
    new FertilizerBuilder().AddMgNonChelated(9.479).AddNo3(10.925).Build(),
    new FertilizerBuilder().AddS(18.969).AddMnNonChelated(32.506).Build()
};

// Optimize the solution
Solution solution = optimizer.Optimize(target, collection, settings);
```


## Dependencies
### NPKOptimizer
- [**Google OR-Tools**](https://developers.google.com/optimization): Used for performing optimization calculations.
### NPKOptimizer.Tests
- [**xUnit**](https://xunit.net/): Framework for unit testing.
- [**AutoFixture**](https://github.com/AutoFixture/AutoFixture): Generates test data.
- [**FluentAssertions**](https://fluentassertions.com/): Enhanced assertions for tests.
- [**Microsoft.AspNetCore.Mvc.Testing**](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0): Testing for ASP.NET MVC applications.
- [**NSubstitute**](https://nsubstitute.github.io/): Library for creating mock and stub objects.
- [**Microsoft.NET.Test.Sdk**](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/): Test SDK for .NET.

## Contact Information:
- **LinkedIn**: [Anatoliy Yermakov](https://www.linkedin.com/in/anatoliyyermakov)
