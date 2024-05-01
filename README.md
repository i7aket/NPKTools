# NPKTools
<img src="assets/logo.png" alt="Logo" width="200"/>

NPKTools is developed on the .NET platform and helps adjust the ratios of various fertilizers to achieve the desired PPM (parts per million) profile. Additionally, it is capable of calculating the PPM of a mixture of fertilizers.
## Features:
### NPKTools.Optimizer
- The Optimizer is composed of three main components: an adapter, a mapper, and a solver. It takes a nutrient target in NPK values, a list of available fertilizers, and specific precision settings as inputs to calculate the optimal fertilizer mix. Here's how the flow works with these components:
- Mapper: This component converts the high-level nutrient targets and the list of available fertilizers into a structured optimization problem. It translates real-world data about fertilizers and nutrient requirements into mathematical variables, constraints, and objectives suitable for optimization. After the solver processes these, the mapper also translates the optimized mathematical solution back into practical terms, specifying exact quantities of each fertilizer to use.
- Solver: Once the optimization problem is defined, this component uses mathematical algorithms to find the best combination of fertilizers. The solver processes the variables and constraints set by the mapper to minimize or maximize the objective function, typically focusing on cost efficiency or nutrient precision.
- Adapter: Acts as the bridge between the domain-specific data (fertilizers and nutrient targets) and the optimization tools. It takes the problem as defined by the mapper, passes it to the solver for optimization, and then interprets the output from the solver back into a practical solution, detailing the specific amounts of each fertilizer to use to achieve the nutrient targets.
- Together, these components ensure that the FertilizerOptimizer effectively integrates and processes the inputs to produce an optimized fertilizer mix that meets the specified NPK goals with the required precision.
### NPKTools.PpmCalc
- The PpmCalculationService is a component in the NPKOptimizer software that calculates the concentration of various nutrients in parts per million (ppm) when a specified collection of fertilizers is diluted in a given volume of water. This service is essential for users who need to understand the nutrient composition of their fertilizer solutions to ensure optimal plant growth.
- Key Functions:
Nutrient Calculation: It computes the ppm concentrations of major and minor plant nutrients including nitrogen (in forms of nitrate, ammonium, and amine), phosphorus, potassium, magnesium, sulfur, calcium, and trace elements such as iron, copper, manganese, zinc, boron, molybdenum, chlorine, silicon, selenium, and sodium.
Flexible Dilution: Users can specify the volume of water in liters for the dilution process. This flexibility allows for accurate adjustment of nutrient concentrations based on different watering needs or systems.
### NPKTools.Optimizer.Preset
- FertilizerOptimizationService is an preconfigured version of the FertilizerOptimizer. It includes:
- 17 basic types of macronutrient fertilizers combined into 18 different sets to optimize the selection process.
- 17 types of micronutrient fertilizers grouped into four main sets: basic, sulfate, nitrate, and chelated.
- During the selection of macronutrient fertilizers, the service conducts two searches: in the first, sulfur is accounted for as specified, while the second search excludes sulfur coefficients to expand the possible options.
### NPKTools.Optimizer.PpmTargetParser
- Project primarily focuses on parsing strings that represent the concentration of various elements in parts per million (ppm) and converting them into structured PpmTarget objects which can be further processed or analyzed within the system. The NPKTools.Optimizer.PpmTargetParser project is designed as a specialized component within the NPKTools suite, aimed at interpreting and transforming user input into actionable data models.
## Developers
This tool was developed by **Anatoliy Yermakov**.
- **LinkedIn**: [Anatoliy Yermakov](https://www.linkedin.com/in/anatoliyyermakov)
- **GitHub**: [i7aket](https://github.com/i7aket)

Special thanks to **Artem Frolov** for his invaluable assistance and guidance in the development of this project.
- **LinkedIn**: [Artem Frolov](https://www.linkedin.com/in/artfrolov/)
- **GitHub**: [AqueGen](https://github.com/AqueGen)
## Installation:
The modules of the NPKTools suite, including NPKTools.Optimizer, NPKTools.PpmCalc, NPKTools.Optimizer.Preset, and NPKTools.Optimizer.PpmTargetParser, are available as NuGet packages. You can install these packages via NuGet Package Manager in Visual Studio or via the command line interface (CLI) as follows:
Using Visual Studio

Open your solution in Visual Studio.
Go to Solution Explorer, right-click on your project, and select Manage NuGet Packages.
Search for the following packages:
- NPKTools.Optimizer
- NPKTools.PpmCalc
- NPKTools.Optimizer.Preset
- NPKTools.Optimizer.PpmTargetParser

Install needed package by selecting it from the list and clicking Install.

## License:
This project is licensed under the MIT License.

## Code Coverage:
The project maintains a high standard of code quality with 99% unit test coverage, ensuring that the features perform as expected and are reliable under various scenarios.

## Roadmap:
### NPKTools.Optimizer ✔ Completed.
Description: This feature calculates the best combination of fertilizers from a provided collection based on fertilizer attributes and the desired ppm (parts per million) profile. This process optimizes nutrient delivery for specific agricultural needs.
### - NPKTools.PpmCalc ✔ Completed.
Description: This function computes the PPM (parts per million) of nutrients in a given mixture of fertilizers, taking into account the weights of the fertilizers and the volume of liquid in which they will be dissolved. This is crucial for ensuring the accuracy of nutrient delivery according to the specified agricultural requirements.
### NPKTools.Optimizer.Preset: ✔ Completed.
Description: A preconfigured version featuring over 40 fertilizers, built using Fertilizer Composition, and more than 20 fertilizer blending scenarios.
### NPKTools.Optimizer.PpmTargetParser ✔ Completed.

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