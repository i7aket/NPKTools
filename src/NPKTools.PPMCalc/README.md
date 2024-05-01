# NPKTools.Optimizer.PPMCalc
- The PpmCalculationService is a component in the NPKOptimizer software that calculates the concentration of various nutrients in parts per million (ppm) when a specified collection of fertilizers is diluted in a given volume of water. This service is essential for users who need to understand the nutrient composition of their fertilizer solutions to ensure optimal plant growth.
- Key Functions:
  Nutrient Calculation: It computes the ppm concentrations of major and minor plant nutrients including nitrogen (in forms of nitrate, ammonium, and amine), phosphorus, potassium, magnesium, sulfur, calcium, and trace elements such as iron, copper, manganese, zinc, boron, molybdenum, chlorine, silicon, selenium, and sodium.
  Flexible Dilution: Users can specify the volume of water in liters for the dilution process. This flexibility allows for accurate adjustment of nutrient concentrations based on different watering needs or systems.

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

### Direct Instantiation
For direct instantiation without using Dependency Injection, follow this example:

```csharp
// Manual instantiation of PpmCalculationService
IPpmCalculationService ppmCalculationService = new PpmCalculationService();
```

Using Dependency Injection
For projects that support Dependency Injection, such as ASP.NET Core applications, configure your services in the Startup.cs or a similar configuration file:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Registering PpmCalculationService in the DI container
    services.AddSingleton<IPpmCalculationService, PpmCalculationService>();
}
```
## Example Usage
Here's how you can use the PpmCalculationService to calculate nutrient concentrations:

```csharp
IPpmCalculationService ppmCalculationService = new PpmCalculationService();

IList<Fertilizer> fertilizers = new List<Fertilizer>
{
    new FertilizerBuilder()
        .AddWeight(0.589)
        .AddNo3(11.863)
        .AddCaNonChelated(16.972)
        .Build(),

    new FertilizerBuilder()
        .AddWeight(0.137)
        .AddNo3(17.499)
        .AddNh4(17.499)
        .Build(),

    new FertilizerBuilder()
        .AddWeight(0.325)
        .AddMgNonChelated(9.861)
        .AddS(13.008)
        .Build(),

    new FertilizerBuilder()
        .AddWeight(0.22)
        .AddP(22.761)
        .AddK(28.731)
        .Build(),

    new FertilizerBuilder()
        .AddWeight(0.305)
        .AddS(18.401)
        .AddK(44.874)
        .Build(),

    new FertilizerBuilder()
        .AddWeight(0.295)
        .AddMgNonChelated(9.479)
        .AddNo3(10.925)
        .Build(),

    new FertilizerBuilder()
        .AddFeNonChelated(20.088)
        .AddS(11.532)
        .AddWeight(0.01)
        .Build(),

    new FertilizerBuilder()
        .AddS(12.841)
        .AddCuNonChelated(25.451)
        .AddWeight(0.0002)
        .Build(),

    new FertilizerBuilder()
        .AddS(18.969)
        .AddMnNonChelated(32.506)
        .AddWeight(0.0017)
        .Build(),

    new FertilizerBuilder()
        .AddS(17.866)
        .AddZnNonChelated(36.433)
        .AddWeight(0.0009)
        .Build(),

    new FertilizerBuilder()
        .AddB(17.483)
        .AddWeight(0.0016)
        .Build(),

    new FertilizerBuilder()
        .AddNa(19.003)
        .AddMo(39.656)
        .AddWeight(0.000126)
        .Build(),

    new FertilizerBuilder()
        .AddCaNonChelated(18.295)
        .AddCl(32.364)
        .AddWeight(3.09E-05)
        .Build(),

    new FertilizerBuilder()
        .AddSi(23.009)
        .AddNa(37.669)
        .AddWeight(4.346E-05)
        .Build(),

    new FertilizerBuilder()
        .AddSe(41.795)
        .AddNa(24.335)
        .AddWeight(2.393E-05)
        .Build()
};

Ppm actualPpm = ppmCalculationService.CalculatePpm(fertilizers, 2);  
```
## Dependencies
### NPKTools.Optimizer.PPMCalc.Tests
- [**xUnit**](https://xunit.net/): Framework for unit testing.
- [**AutoFixture**](https://github.com/AutoFixture/AutoFixture): Generates test data.
- [**FluentAssertions**](https://fluentassertions.com/): Enhanced assertions for tests.
- [**Microsoft.AspNetCore.Mvc.Testing**](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0): Testing for ASP.NET MVC applications.
- [**NSubstitute**](https://nsubstitute.github.io/): Library for creating mock and stub objects.
- [**Microsoft.NET.Test.Sdk**](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/): Test SDK for .NET.

## Contact Information:
- **LinkedIn**: [Anatoliy Yermakov](https://www.linkedin.com/in/anatoliyyermakov)
