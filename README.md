# SYT.NPKTools

<img src="https://raw.githubusercontent.com/i7aket/NPKTools/main/assets/logo.png" alt="NPKTools logo" width="200"/>

[![CI](https://github.com/i7aket/NPKTools/actions/workflows/ci.yml/badge.svg)](https://github.com/i7aket/NPKTools/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/vpre/SYT.NPKTools.svg)](https://www.nuget.org/packages/SYT.NPKTools/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/i7aket/NPKTools/blob/main/LICENSE)

Fertilizer nutrient management for .NET. Give it a target nutrient profile in ppm and the fertilizers
you have, and it works out how much of each to use by solving the mix as a linear program. It also
calculates the ppm of a mixture you already have.

Targets **.NET 10**. No dependencies and nothing native, so the whole pipeline runs server-side or in
the browser under WebAssembly.

```bash
dotnet add package SYT.NPKTools
```

> **This is `1.0.0-preview.2`.** It supersedes the `NPKTools.*` packages, which are no longer
> updated. If you are coming from those, see [Migrating from NPKTools 1.x](#migrating-from-npktools-1x).

## Quick start

Nothing to configure and nothing else to install — `NpkTools` builds the services for you.

```csharp
using SYT.NPKTools;
using SYT.NPKTools.Fertilizers;
using SYT.NPKTools.Nutrients;

IPpmTargetParser parser = NpkTools.CreateTargetParser();
IFertilizerOptimizationService optimizer = NpkTools.CreateOptimizationService();
IPpmCalculationService calculator = NpkTools.CreatePpmCalculator();

// Elements are separated by spaces or commas; L is the water volume in liters.
PpmTarget target = parser.Parse("N=150 P=50 K=200 Ca=100 Mg=50 L=100");

FertilizerSolutions result = optimizer.FindSolutions(target);

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

`FindMacroSolutions`, `FindMicroSolutions` and `FindSolutions` return empty sets rather than `null`
when nothing satisfies the target, so the loops above need no null check.

A macro search solves 36 linear programs in sequence — the 18 bundles once honouring the sulfur target
and once ignoring it — so pass a token if you need to bail out:

```csharp
using CancellationTokenSource cts = new(TimeSpan.FromSeconds(5));
Solutions solutions = optimizer.FindMacroSolutions(target, cts.Token);
```

## Dependency injection

Two ways, both supported. Pick by whether you would rather have one line or zero dependencies.

**Register the factory results yourself.** No extra package — `IServiceCollection` belongs to your
application, so this needs nothing from us:

```csharp
services.AddSingleton(NpkTools.CreateOptimizationService());
services.AddSingleton(NpkTools.CreatePpmCalculator());
services.AddSingleton(NpkTools.CreateTargetParser());
```

**Or take the optional extension package** and do it in one line:

```bash
dotnet add package SYT.NPKTools.DependencyInjection
```

```csharp
services.AddNpkTools();
```

That is the entire reason the second package exists: an `IServiceCollection` extension method requires
`Microsoft.Extensions.DependencyInjection.Abstractions`, and keeping it out of `SYT.NPKTools` is what
makes the main package dependency-free. The extension lives in the
`Microsoft.Extensions.DependencyInjection` namespace, so a typical `Program.cs` needs no extra `using`.

It also takes the solver, so substitution stays an argument rather than a registration order:

```csharp
services.AddNpkTools(new MyGlopSolver());
```

Singletons are correct either way: everything here is stateless, apart from the bundle repository's
immutable lazy cache.

The factory is a convenience over public constructors, not a requirement — you can always assemble the
graph yourself:

```csharp
using SYT.NPKTools;
using SYT.NPKTools.Optimization;

IFertilizerOptimizer adapter = new FertilizerOptimizationAdapter(
    new SimplexOptimizationSolver(),
    new OptimizationProblemMapper());

IFertilizerOptimizationService service =
    new FertilizerOptimizationService(adapter, new FertilizerBundleRepository());
```

## What's in the box

| Package | Dependencies | Take it if |
| --- | --- | --- |
| [`SYT.NPKTools`](https://www.nuget.org/packages/SYT.NPKTools/) | **none** | always |
| [`SYT.NPKTools.DependencyInjection`](https://www.nuget.org/packages/SYT.NPKTools.DependencyInjection/) | the above + `Microsoft.Extensions.DependencyInjection.Abstractions` | you want `services.AddNpkTools()` in one line |

`SYT.NPKTools` has **no dependencies at all**. Not "few" — none. There is nothing in it to conflict
with whatever versions your application already resolves, and nothing native, so it runs wherever .NET
runs.

| Namespace | Contents |
| --- | --- |
| `SYT.NPKTools` | `Solution`, `Solutions`, `FertilizerSolutions`, the preset catalogue and the `NpkTools` factory |
| `SYT.NPKTools.Fertilizers` | `Fertilizer`, its nutrient value objects and builders |
| `SYT.NPKTools.Nutrients` | `Ppm`, `PpmTarget`, the ppm calculator and the target parser |
| `SYT.NPKTools.Optimization` | The linear program, the solver, the mapper and the settings |

## Runs in the browser

Every part of this package is managed code, so parsing a target, optimizing the mix and reporting the
resulting ppm all work client-side under WebAssembly with no server round trip.

That is why the default solver is a managed simplex rather than Google OR-Tools. OR-Tools ships native
binaries only for linux-x64/arm64, osx-x64/arm64 and win-x64 — there is no `browser-wasm` build — so
anything depending on it is server-only. The problem is small enough that this costs nothing: a
macronutrient bundle is at most 16 variables and 7 range constraints, and a full preset search is 40
such problems.

Verified end to end: the pipeline was published to `browser-wasm` and executed, producing 11 macro
solutions landing exactly on an `N=150 P=50 K=200 Ca=100 Mg=50 S=60` target. Transfer cost is roughly
1.1 MB gzipped, almost entirely the .NET runtime — the library itself is about 51 KB.

## About the solver

`SimplexOptimizationSolver` is a two-phase primal simplex using Bland's rule, on a dense tableau. It
is validated against Google OR-Tools' GLOP, which the repository keeps as a test-only oracle rather
than shipping: 60 equivalence tests plus differential testing over 17,760 problems generated by the
mapper, in which the two never disagreed — worst constraint violation 1.4e-14, worst relative cost gap
5.8e-16.

At this problem size the managed solver is also the faster of the two, because GLOP's per-solve setup
dominates. Measured on a full macro-plus-micro search:

| Solver | Full search | Allocated |
| --- | --- | --- |
| Managed simplex | **0.67 ms** | 939 KB |
| OR-Tools GLOP | 3.67 ms | 913 KB |

Reproduce with `dotnet run -c Release --project benchmarks/SYT.NPKTools.Benchmarks`.

**It is not a general-purpose LP solver.** There is no scaling, equilibration or iterative refinement.
Within this library's regime — nutrient percentages and ppm/10 right-hand sides, about three orders of
magnitude apart — it is exact. Given a problem whose coefficients span ten or more orders of magnitude,
round-off can make it stop at a suboptimal vertex or report no solution where one exists. Every answer
it returns is verified against the original constraints before being handed back, so it will never
return a mix that violates them.

If you need robustness across arbitrary scaling, `IOptimizationProblemSolver` is public — pass your own
implementation and it is used for every solve:

```csharp
IFertilizerOptimizationService service = NpkTools.CreateOptimizationService(new MyGlopSolver());
```

An explicit argument rather than a registration order, so it cannot be silently ineffective. The
repository's oracle at `tests/SYT.NPKTools.OrToolsOracle` is a complete worked example, backed by GLOP.

## How it works

Three components in a pipeline:

- **Mapper** turns the nutrient target and the available fertilizers into a linear program —
  variables, constraints and an objective — and turns the solver's answer back into concrete
  fertilizer weights.
- **Solver** minimises the objective (by default, cost) subject to the constraints.
- **Adapter** drives the two and is what `IFertilizerOptimizer` exposes.

Each element is constrained to `target ± target × (1 − p)`, where `p` is the **smaller** of that
element's precision setting and the global `RangeFactorSettings`. The looser of the two therefore
decides, and raising `RangeFactorSettings` tightens the search rather than widening it. At the default
of `1`, an element with a precision of `1` becomes an exact equality; an element with a precision of
`0` is left unconstrained entirely.

### The preset catalogue

- 17 macronutrient fertilizers combined into 18 bundles.
- 17 micronutrient fertilizers in four sets: basic, sulfate, nitrate and chelated.
- Macronutrient searches run twice — once honouring the sulfur target, once ignoring it — to widen the
  set of feasible mixes, so 18 bundles mean 36 solves. Duplicate results are collapsed.

Bundles are built lazily and cached, so the repository is registered as a singleton.

### Nutrients covered

Nitrogen (split into nitrate, ammonium and amine), phosphorus, potassium, magnesium, sulfur, calcium,
and the trace elements iron, copper, manganese, zinc, boron, molybdenum, chlorine, silicon, selenium
and sodium. Chelated and non-chelated fractions are tracked separately: iron carries all four chelate
forms (EDTA, DTPA, EDDHA, HBED), while copper, manganese, zinc, calcium and magnesium carry EDTA only.

### Target strings

`element=value` pairs separated by spaces or commas. Element names are case-insensitive. Accepted:
`N P K Ca Mg S Fe Cu Mn Zn B Mo Cl Si Se Na`, plus `L` for water volume in liters (default `1`).
Unknown elements, malformed pairs and duplicates all raise `FormatException`. Values are always parsed
with the invariant culture, so `1.5` means one and a half regardless of regional settings.

Weights are in **grams** and ppm is `percent × grams ÷ liters × 10`.

## Migrating from NPKTools 1.x

The six `NPKTools.*` packages are replaced by this single one. Namespaces are flattened; the mechanical
part of the move is a find-and-replace:

| NPKTools 1.x | SYT.NPKTools |
| --- | --- |
| packages `NPKTools.Core`, `.Optimizer`, `.Optimizer.Preset`, `.Optimizer.PPMCalc`, `.Optimizer.PpmTargetParser` | one package, `SYT.NPKTools` |
| `NPKTools.Core.Domain.Fertilizers*` | `SYT.NPKTools.Fertilizers` |
| `NPKTools.Core.Domain.PartsPerMillion*`, `.PpmTarget*`, `NPKTools.PPMCalc`, `NPKTools.Optimizer.PpmTargetParser` | `SYT.NPKTools.Nutrients` |
| `NPKTools.Core.Domain.SolutionsFinderSettings*`, `NPKTools.Optimizer.Components`, `.Contracts` | `SYT.NPKTools.Optimization` |
| `NPKTools.Core.Domain.Collections`, `NPKTools.Optimizer.Preset` | `SYT.NPKTools` |
| `AddNpkToolsOptimizer()`, `AddNpkToolsPreset()`, `AddNpkToolsPpmCalc()`, `AddNpkToolsPpmTargetParser()` | one `AddNpkTools()` from `SYT.NPKTools.DependencyInjection`, or register the `NpkTools` factory results yourself — see [Dependency injection](#dependency-injection) |
| `AddNpkToolsOrToolsSolver()` | pass the solver: `AddNpkTools(mySolver)` or `NpkTools.CreateOptimizationService(mySolver)` |
| `Microsoft.Extensions.DependencyInjection.Abstractions` was a dependency of the library | only of the optional DI package; `SYT.NPKTools` has no dependencies |
| `IFertilizerBundleRepository.Marco()` | `.Macro()` |
| `PpmTargetBuilder.AddLitters(...)` | `.AddLiters(...)` |
| `FindSolutions` returned `(Solutions Macro, Solutions Micro)` | returns `FertilizerSolutions` |
| `Solution : List<Fertilizer>`, `Solutions : List<Solution>` | `IReadOnlyList<…>`; construct with `new Solution(fertilizers, waterLiters)` |
| `Solutions?` returns, `null` for "not found" | `Solutions`, `Solutions.Empty` |
| parameterless constructors + settable properties on the domain types | constructors only; use the builders |
| `NPKTools.Core.Common`, `NPKTools.Core.Const`/`.Constants` | internal — `ThrowIf`, `ReportFormatter`, `Labels`, `Names` are no longer public |

The full history is in [CHANGELOG.md](https://github.com/i7aket/NPKTools/blob/main/CHANGELOG.md).

## Building from source

```bash
git clone https://github.com/i7aket/NPKTools.git
cd NPKTools
dotnet build
dotnet test
```

Requires the .NET 10 SDK (pinned in `global.json`). The build treats warnings as errors, so a clean
`dotnet build` is part of the contract. NuGet versions live in `Directory.Packages.props` and shared
package metadata in `src/Directory.Build.props`; add references without a `Version` attribute.

405 tests run on Linux, Windows and macOS in CI, with coverage collected via coverlet and formatting
enforced by `dotnet format --verify-no-changes`. CodeQL scanning runs through GitHub's default setup.

## Releasing

Publishing is triggered by a version tag, not by pushes to `main`. Bump `<Version>` in
`src/Directory.Build.props`, commit it, then tag:

```bash
git tag v1.0.0-preview.2
git push origin v1.0.0-preview.2
```

The workflow refuses to publish if the tag is not valid SemVer or does not match the committed
`<Version>`, because a version pushed to NuGet.org can be unlisted but never replaced. It then builds,
runs the full test suite, and pushes to NuGet.org and GitHub Packages.

Always release through the workflow. `dotnet pack` records the current branch in the nuspec's
`<repository>` element, so a package built locally on a feature branch carries that branch name in its
public metadata; packing from a tag ref records the tag.

## Developers

Developed by **Anatoliy Yermakov**.

- **LinkedIn**: [Anatoliy Yermakov](https://www.linkedin.com/in/anatoliyyermakov)
- **GitHub**: [i7aket](https://github.com/i7aket)

Special thanks to **Artem Frolov** for his invaluable assistance and guidance in the development of
this project.

- **LinkedIn**: [Artem Frolov](https://www.linkedin.com/in/artfrolov/)
- **GitHub**: [AqueGen](https://github.com/AqueGen)

## Contributing

See [CONTRIBUTING.md](https://github.com/i7aket/NPKTools/blob/main/CONTRIBUTING.md).

## License

Licensed under the [MIT License](https://github.com/i7aket/NPKTools/blob/main/LICENSE).
