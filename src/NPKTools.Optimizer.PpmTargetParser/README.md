# NPKTools.Optimizer.PpmTargetParser
The NPKTools.Optimizer.PpmTargetParser project is designed as a specialized component within the NPKTools suite, aimed at interpreting and transforming user input into actionable data models. This project primarily focuses on parsing strings that represent the concentration of various elements in parts per million (ppm) and converting them into structured PpmTarget objects which can be further processed or analyzed within the system.


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
using NPKTools.Optimizer.PpmTargetParser;
using NPKTools.Core.Domain.PpmTarget;

// Manually creating an instance of PpmTargetParser without DI
PpmTargetParser ppmTargetParser = new PpmTargetParser();
```
Using Dependency Injection
For integrating these components into a project that supports Dependency Injection, such as an ASP.NET Core application, configure your services in the Startup.cs or a similar configuration file as follows:
```csharp
public void ConfigureServices(IServiceCollection services)
{
// Registering services in the DI container
    services.AddSingleton<IPpmTargetParser, PpmTargetParser>();
}
```

## Example usage

```csharp
 string input = "N=150, P=50, K=200, Ca=40, Mg=30";
 try
 {
    PpmTarget target = ppmTargetParser.Parse(input);
    Console.WriteLine("Parsing successful! Target values are set.");
 }
 catch (Exception ex)
 {
    Console.WriteLine($"Error parsing input: {ex.Message}");
 }
```



## Dependencies
### NPKTools.Optimizer.PpmTargetParser.Tests
- [**xUnit**](https://xunit.net/): Framework for unit testing.
- [**AutoFixture**](https://github.com/AutoFixture/AutoFixture): Generates test data.
- [**FluentAssertions**](https://fluentassertions.com/): Enhanced assertions for tests.
- [**Microsoft.AspNetCore.Mvc.Testing**](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0): Testing for ASP.NET MVC applications.
- [**NSubstitute**](https://nsubstitute.github.io/): Library for creating mock and stub objects.
- [**Microsoft.NET.Test.Sdk**](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/): Test SDK for .NET.

## Contact Information:
- **LinkedIn**: [Anatoliy Yermakov](https://www.linkedin.com/in/anatoliyyermakov)
