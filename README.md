# NPKTools

<img src="assets/logo.png" alt="Logo" width="200"/>

[![CI](https://github.com/i7aket/NPKTools/actions/workflows/ci.yml/badge.svg)](https://github.com/i7aket/NPKTools/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/NPKTools.Core.svg)](https://www.nuget.org/packages/NPKTools.Core/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

NPKTools is a set of .NET libraries for fertilizer nutrient management. It adjusts the ratios of
available fertilizers to hit a desired PPM (parts per million) profile, and calculates the PPM of a
given fertilizer mixture.

Targets **.NET 10**. The current release is **2.0.0**, which contains breaking API changes — see
[CHANGELOG.md](CHANGELOG.md) for the migration table.

## Packages

| Package | Purpose |
| --- | --- |
| [`NPKTools.Core`](https://www.nuget.org/packages/NPKTools.Core/) | Domain model shared by everything else: fertilizers, ppm values, ppm targets, settings, builders |
| [`NPKTools.Optimizer`](https://www.nuget.org/packages/NPKTools.Optimizer/) | Solves the fertilizer mix as a linear program using Google OR-Tools |
| [`NPKTools.Optimizer.Preset`](https://www.nuget.org/packages/NPKTools.Optimizer.Preset/) | The optimizer preloaded with 34 fertilizers and 22 blending scenarios |
| [`NPKTools.PPMCalc`](https://www.nuget.org/packages/NPKTools.PPMCalc/) | Calculates the ppm of a fertilizer mixture |
| [`NPKTools.Optimizer.PpmTargetParser`](https://www.nuget.org/packages/NPKTools.Optimizer.PpmTargetParser/) | Parses `"N=150 P=50 K=200"` into a `PpmTarget` |

> **Renamed in 2.0.0:** the ppm calculator used to ship as `NPKTools.Optimizer.PPMCalc`. It is now
> `NPKTools.PPMCalc`, matching its assembly and namespace.

## Installation

```bash
dotnet add package NPKTools.Optimizer.Preset
dotnet add package NPKTools.PPMCalc
dotnet add package NPKTools.Optimizer.PpmTargetParser
```

`NPKTools.Core` and `NPKTools.Optimizer` come in as transitive dependencies of the preset package.

## Quick start

Register the services with your DI container:

```csharp
using Microsoft.Extensions.DependencyInjection;
using NPKTools.Optimizer.Preset;
using NPKTools.Optimizer.PpmTargetParser;
using NPKTools.PPMCalc;

ServiceProvider provider = new ServiceCollection()
    .AddNpkToolsPreset()          // IFertilizerOptimizationService + the optimizer beneath it
    .AddNpkToolsPpmCalc()         // IPpmCalculationService
    .AddNpkToolsPpmTargetParser() // IPpmTargetParser
    .BuildServiceProvider();
```

Find fertilizer mixes for a nutrient target:

```csharp
IPpmTargetParser parser = provider.GetRequiredService<IPpmTargetParser>();
IFertilizerOptimizationService optimizer = provider.GetRequiredService<IFertilizerOptimizationService>();
IPpmCalculationService calculator = provider.GetRequiredService<IPpmCalculationService>();

// Elements are separated by spaces or commas; L is the water volume in liters.
PpmTarget target = parser.Parse("N=150 P=50 K=200 Ca=100 Mg=50 L=100");

(Solutions Macro, Solutions Micro) result = optimizer.FindSolutions(target);

foreach (Solution solution in result.Macro)
{
    foreach (Fertilizer fertilizer in solution)
    {
        Console.WriteLine($"{fertilizer.Name.Value}: {fertilizer.Weight.Value:F3} g");
    }

    // A Solution is an IReadOnlyList<Fertilizer>, so it can be measured directly.
    Console.WriteLine(calculator.CalculatePpm(solution, solution.WaterLiters).Report());
}
```

`FindMacroSolutions`, `FindMicroSolutions` and `FindSolutions` return `Solutions.Empty` rather than
`null` when nothing satisfies the target, so the loops above are safe without a null check.

A macro search solves 18 linear programs in sequence, so pass a token if you need to bail out:

```csharp
using CancellationTokenSource cts = new(TimeSpan.FromSeconds(5));
Solutions solutions = optimizer.FindMacroSolutions(target, cts.Token);
```

Without a DI container, construct the pieces directly:

```csharp
IFertilizerOptimizer adapter = new FertilizerOptimizationAdapter(
    new GoogleOrToolsOptimizationSolver(),
    new OptimizationProblemMapper());

IFertilizerOptimizationService service =
    new FertilizerOptimizationService(adapter, new FertilizerBundleRepository());
```

## How it works

### NPKTools.Optimizer

Three components in a pipeline:

- **Mapper** turns the nutrient target and the available fertilizers into a linear program —
  variables, constraints and an objective — and turns the solver's answer back into concrete
  fertilizer weights.
- **Solver** minimises the objective (by default, cost) subject to the constraints. The default
  implementation uses Google OR-Tools' GLOP. Swap it by registering your own
  `IOptimizationProblemSolver` before calling `AddNpkToolsOptimizer()`.
- **Adapter** drives the two and is what `IFertilizerOptimizer` exposes.

Each element's tolerance is the smaller of its own precision setting and the global
`RangeFactorSettings`; an element with a precision of `0` is left unconstrained.

### NPKTools.Optimizer.Preset

Wraps the optimizer with a curated fertilizer catalogue:

- 17 macronutrient fertilizers combined into 18 bundles.
- 17 micronutrient fertilizers in four sets: basic, sulfate, nitrate and chelated.
- Macronutrient searches run twice — once honouring the sulfur target, once ignoring sulfur — to
  widen the set of feasible mixes. Duplicate results are collapsed.

Bundles are built lazily and cached, so register the repository as a singleton (the DI helper
already does).

### NPKTools.PPMCalc

Computes ppm for nitrogen (split into nitrate, ammonium and amine), phosphorus, potassium,
magnesium, sulfur, calcium, and the trace elements iron, copper, manganese, zinc, boron,
molybdenum, chlorine, silicon, selenium and sodium, for any water volume.

### NPKTools.Optimizer.PpmTargetParser

Parses `element=value` pairs separated by spaces or commas into a `PpmTarget`. Element names are
case-insensitive. Accepted elements: `N P K Ca Mg S Fe Cu Mn Zn B Mo Cl Si Se Na` plus `L` for
water volume in liters (default `1`). Unknown elements, malformed pairs and duplicates all raise
`FormatException`. Values are always parsed with the invariant culture, so `1.5` means one and a
half regardless of the machine's regional settings.

## Building from source

```bash
git clone https://github.com/i7aket/NPKTools.git
cd NPKTools
dotnet build
dotnet test
```

Requires the .NET 10 SDK (pinned in `global.json`). The build treats warnings as errors, so a clean
`dotnet build` is part of the contract.

Dependency versions are centrally managed: NuGet versions live in `Directory.Packages.props` and
shared package metadata in `src/Directory.Build.props`. Add references without a `Version`
attribute.

## Testing

308 tests across six projects, run on Linux, Windows and macOS in CI. Coverage is collected on
every run via coverlet and uploaded as a build artifact.

## Releasing

Publishing is triggered by a version tag, not by pushes to `main`:

```bash
git tag v2.0.1
git push origin v2.0.1
```

The tag name (minus the `v`) becomes the package version. The workflow builds, runs the full test
suite, then pushes to NuGet.org and GitHub Packages.

## Dependencies

- [**Google OR-Tools**](https://developers.google.com/optimization) — linear programming solver.
- [**Microsoft.Extensions.DependencyInjection.Abstractions**](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection.Abstractions/) — for the DI helpers.

Test-only: [xUnit](https://xunit.net/),
[AwesomeAssertions](https://github.com/AwesomeAssertions/AwesomeAssertions),
[AutoFixture](https://github.com/AutoFixture/AutoFixture),
[NSubstitute](https://nsubstitute.github.io/), [coverlet](https://github.com/coverlet-coverage/coverlet).

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md).

## Developers

This tool was developed by **Anatoliy Yermakov**.

- **LinkedIn**: [Anatoliy Yermakov](https://www.linkedin.com/in/anatoliyyermakov)
- **GitHub**: [i7aket](https://github.com/i7aket)

Special thanks to **Artem Frolov** for his invaluable assistance and guidance in the development of
this project.

- **LinkedIn**: [Artem Frolov](https://www.linkedin.com/in/artfrolov/)
- **GitHub**: [AqueGen](https://github.com/AqueGen)

## License

Licensed under the [MIT License](LICENSE).
