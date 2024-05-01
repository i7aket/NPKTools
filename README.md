# Fertilizer Optimizer
<img src="assets/logo.png" alt="Logo" width="200"/>

Fertilizer Optimizer is a tool developed on the .NET platform that helps adjust the ratio of each fertilizer to achieve the desired PPM (parts per million) profile. Additionally, it is capable of calculating the PPM of a mixture of fertilizers.
## Features:
### Fertilizer Optimizer
- The FertilizerOptimizer is composed of three main components: an adapter, a mapper, and a solver. It takes a nutrient target in NPK values, a list of available fertilizers, and specific precision settings as inputs to calculate the optimal fertilizer mix. Here's how the flow works with these components:
- Mapper: This component converts the high-level nutrient targets and the list of available fertilizers into a structured optimization problem. It translates real-world data about fertilizers and nutrient requirements into mathematical variables, constraints, and objectives suitable for optimization. After the solver processes these, the mapper also translates the optimized mathematical solution back into practical terms, specifying exact quantities of each fertilizer to use.
- Solver: Once the optimization problem is defined, this component uses mathematical algorithms to find the best combination of fertilizers. The solver processes the variables and constraints set by the mapper to minimize or maximize the objective function, typically focusing on cost efficiency or nutrient precision.
- Adapter: Acts as the bridge between the domain-specific data (fertilizers and nutrient targets) and the optimization tools. It takes the problem as defined by the mapper, passes it to the solver for optimization, and then interprets the output from the solver back into a practical solution, detailing the specific amounts of each fertilizer to use to achieve the nutrient targets.
- Together, these components ensure that the FertilizerOptimizer effectively integrates and processes the inputs to produce an optimized fertilizer mix that meets the specified NPK goals with the required precision.
### Ppm Analizerer
- The PpmCalculationService is a component in the NPKOptimizer software that calculates the concentration of various nutrients in parts per million (ppm) when a specified collection of fertilizers is diluted in a given volume of water. This service is essential for users who need to understand the nutrient composition of their fertilizer solutions to ensure optimal plant growth.
- Key Functions:
Nutrient Calculation: It computes the ppm concentrations of major and minor plant nutrients including nitrogen (in forms of nitrate, ammonium, and amine), phosphorus, potassium, magnesium, sulfur, calcium, and trace elements such as iron, copper, manganese, zinc, boron, molybdenum, chlorine, silicon, selenium, and sodium.
Flexible Dilution: Users can specify the volume of water in liters for the dilution process. This flexibility allows for accurate adjustment of nutrient concentrations based on different watering needs or systems.
### Fertilizer Optimizer Preconfigured
- FertilizerOptimizationService is an preconfigured version of the FertilizerOptimizer. It includes:
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
## Installation:
To set up the Fertilizer Optimizer tool, you need to clone the repository and set up the environment. Follow these steps:
1. **Clone the repository**
    - For the stable version from the main branch:
        ```
        git clone https://github.com/i7aket/NPKOptimizer.git
        ```
    - For the latest developments from the develop branch:
        ```
        git clone -b develop https://github.com/i7aket/NPKOptimizer.git
        ```
2. **Set up the environment**
    Navigate to the project directory and install the necessary dependencies:
    ```
    cd NPKOptimizer
    dotnet restore
    ```
3. **Build the project**
Compile the project to make sure everything is set up correctly:
    ```
    dotnet build
    ```
## License:
This project is licensed under the MIT License - see the [License](LICENSE) file for details.

## Code Coverage:
The project maintains a high standard of code quality with 99% unit test coverage, ensuring that the features perform as expected and are reliable under various scenarios.

## Roadmap:
### Optimal Fertilizer Ratio Finder: ✔ Completed.
Description: This feature calculates the best combination of fertilizers from a provided collection based on fertilizer attributes and the desired ppm (parts per million) profile. This process optimizes nutrient delivery for specific agricultural needs.
### PPM Calculation for Fertilizer Mixtures: ✔ Completed.
Description: This function computes the PPM (parts per million) of nutrients in a given mixture of fertilizers, taking into account the weights of the fertilizers and the volume of liquid in which they will be dissolved. This is crucial for ensuring the accuracy of nutrient delivery according to the specified agricultural requirements.
### NPKOptimizer Preconfigured: ✔ Completed.
Description: A preconfigured version featuring over 40 fertilizers, built using Fertilizer Composition, and more than 20 fertilizer blending scenarios.
### GreenSecrets Telegram Bot: ✔ In final stages of development.
Description: A demo version of the final NPKOptimizer Preconfigured as a Telegram bot.

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
## Usage:
### Example of fertilizer ratio optimizer usage
    ```
    // Create the base optimizer
    BaseMacroOptimizer optimizer = new BaseMacroOptimizer();

    // Create the target profile for optimization
    PpmTarget target = new PpmTargetBuilder()
        .AddN(150)
        .AddP(50)
        .AddK(200)
        .AddMg(60)
        .AddCa(60)
        .AddS(80)
        .Build();

    // Obtain the optimized solution
    Solution solutionResult = optimizer.Optimize(target);

    // Print the solution to the console
    PrintSolutionResults(solutionResult);

    // Create the fertilizer mix analyzer
    IPpmCalculationService calc = new PpmCalculationService();

    // Analyze the mix generated by the optimizer
    Ppm ppmResult = calc.CalculatePpm(solutionResult);

    // Print the analysis results to the console
    PrintPpmResults(ppmResult);

    static void PrintSolutionResults(Solution solution)
    {
        StringBuilder outputBuilder = new StringBuilder();
        foreach (Fertilizer fertilizer in solution)
        {
            outputBuilder.AppendLine($"Fertilizer ID: {fertilizer.RefId.Value}, ");
            if (fertilizer.Nitrogen.Value != 0)
                outputBuilder.AppendLine($"Nitrogen: {fertilizer.Nitrogen.Value}, ");
            if (fertilizer.Phosphorus.Value != 0)
                outputBuilder.AppendLine($"Phosphorus: {fertilizer.Phosphorus.Value}, ");
            if (fertilizer.Potassium.Value != 0)
                outputBuilder.AppendLine($"Potassium: {fertilizer.Potassium.Value}, ");
            if (fertilizer.Calcium.Value != 0)
                outputBuilder.AppendLine($"Calcium: {fertilizer.Calcium.Value}, ");
            if (fertilizer.Magnesium.Value != 0)
                outputBuilder.AppendLine($"Magnesium: {fertilizer.Magnesium.Value}, ");
            if (fertilizer.Sulfur.Value != 0)
                outputBuilder.AppendLine($"Sulfur: {fertilizer.Sulfur.Value}, ");
            if (fertilizer.Weight.Value != 0)
                outputBuilder.AppendLine($"Weight: {fertilizer.Weight.Value} gr");
            outputBuilder.AppendLine($"----------------------------------------");
        }

        Console.WriteLine(outputBuilder.ToString());
    }

    static void PrintPpmResults(Ppm ppmResult)
    {
        StringBuilder outputBuilder = new StringBuilder();
        outputBuilder.AppendLine($"Nitrogen ppm: {ppmResult.Nitrogen?.Value ?? 0}");
        outputBuilder.AppendLine($"Phosphorus ppm: {ppmResult.Phosphorus?.Value ?? 0}");
        outputBuilder.AppendLine($"Potassium ppm: {ppmResult.Potassium?.Value ?? 0}");
        outputBuilder.AppendLine($"Calcium ppm: {ppmResult.Calcium?.Value ?? 0}");
        outputBuilder.AppendLine($"Magnesium ppm: {ppmResult.Magnesium?.Value ?? 0}");
        outputBuilder.AppendLine($"Sulfur ppm: {ppmResult.Sulfur?.Value ?? 0}");
        outputBuilder.AppendLine($"Liters: {ppmResult.Liters?.Value ?? 0}");
        Console.WriteLine(outputBuilder.ToString());
    }

    // Base class for the macro optimizer
    public class BaseMacroOptimizer
    {
        // Instantiate the optimizer with a mapper and a solver adapter
        IFertilizerOptimizer _optimizer =
            new FertilizerOptimizationAdapter(new GoogleOrToolsOptimizationSolver(), new OptimizationProblemMapper());

        // Method to optimize based on the desired profile
        public Solution Optimize(PpmTarget ppmTarget)
        {
            // Define the basic set of fertilizers for the recipe
            IList<FertilizerOptimizationModel> collection = new List<FertilizerOptimizationModel>()
            {
                // CalciumNitrate
                new FertilizerBuilder()
                    .AddNo3(11.863)
                    .AddCaNonChelated(16.972)
                    .Build(),

                // PotassiumNitrate
                new FertilizerBuilder()
                    .AddNo3(13.854)
                    .AddK(38.672)
                    .Build(),

                // AmmoniumNitrate
                new FertilizerBuilder()
                    .AddNo3(17.499)
                    .AddNh4(17.499)
                    .Build(),

                // MagnesiumSulfate
                new FertilizerBuilder()
                    .AddMgNonChelated(9.861)
                    .AddS(13.008)
                    .Build(),

                // PotassiumDihydrogenPhosphate
                new FertilizerBuilder()
                    .AddP(22.761)
                    .AddK(28.731)
                    .Build(),

                // PotassiumSulfate
                new FertilizerBuilder()
                    .AddS(18.401)
                    .AddK(44.874)
                    .Build(),

                // MagnesiumNitrate
                new FertilizerBuilder()
                    .AddMgNonChelated(9.479)
                    .AddNo3(10.925)
                    .Build(),

                // ManganeseSulfate
                new FertilizerBuilder()
                    .AddS(18.969)
                    .AddMnNonChelated(32.506)
                    .Build()
            };

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

            // Perform the optimization and return the result
            return _optimizer.Optimize(ppmTarget, collection, settings);
        }
    }
    ```

