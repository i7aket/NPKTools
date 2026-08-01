# NPKTools.Optimizer

Part of the [NPKTools](https://github.com/i7aket/NPKTools) suite. Given a nutrient target, a list of
available fertilizers and precision settings, it works out how much of each fertilizer to use by
solving the mix as a linear program.

Targets **.NET 10**. Fully managed with no native dependencies, so it runs anywhere .NET runs —
including WebAssembly, where a native solver cannot go.

If you want a ready-made fertilizer catalogue instead of supplying your own, use
[`NPKTools.Optimizer.Preset`](https://www.nuget.org/packages/NPKTools.Optimizer.Preset/).

## How it works

Three components in a pipeline:

- **Mapper** (`IOptimizationProblemMapper`) turns the nutrient target and the available fertilizers
  into a linear program — variables, constraints, objective — and turns the solver's answer back
  into concrete fertilizer weights.
- **Solver** (`IOptimizationProblemSolver`) minimises the objective subject to the constraints. The
  default objective is total cost, taken from each fertilizer's `Price`. The default implementation
  is `SimplexOptimizationSolver`, a managed two-phase primal simplex. Install
  [`NPKTools.Optimizer.OrTools`](https://www.nuget.org/packages/NPKTools.Optimizer.OrTools/) for a
  Google OR-Tools (GLOP) backend instead — note it brings native binaries and cannot run on
  WebAssembly.
- **Adapter** (`IFertilizerOptimizer`) drives the two and is the public entry point.

Each element is constrained to `target ± target × (1 − p)`, where `p` is the **smaller** of that
element's precision setting and the global `RangeFactorSettings`. The looser of the two therefore
decides, and raising `RangeFactorSettings` tightens the search rather than widening it. An element
with a precision of `0` is left unconstrained entirely.

The default solver is a dense tableau with a fixed tolerance and no scaling. That suits this problem
— nutrient percentages and ppm/10 right-hand sides span about three orders of magnitude — but it is
not a general-purpose LP solver: on a problem whose coefficients span ten or more orders of
magnitude, round-off can make it stop at a suboptimal vertex or report no solution where one exists.
Every answer is verified against the original constraints before it is returned, so it will not hand
back a mix that violates them. Use the OR-Tools backend if you need robustness across arbitrary
scaling.

## Setup

With dependency injection. `ServiceCollection` and `BuildServiceProvider()` come from
**Microsoft.Extensions.DependencyInjection** — this package references only the abstractions, so add
the implementation too in a console app:

```csharp
using Microsoft.Extensions.DependencyInjection;
using NPKTools.Optimizer;
using NPKTools.Optimizer.Contracts;

ServiceProvider provider = new ServiceCollection()
    .AddNpkToolsOptimizer()
    .BuildServiceProvider();

IFertilizerOptimizer optimizer = provider.GetRequiredService<IFertilizerOptimizer>();
```

Registrations use `TryAdd`, so registering a solver first replaces the managed default:

```csharp
new ServiceCollection()
    .AddSingleton<IOptimizationProblemSolver, MyOwnSolver>()
    .AddNpkToolsOptimizer();
```

Or construct the components directly:

```csharp
using NPKTools.Optimizer.Components;
using NPKTools.Optimizer.Contracts;

IFertilizerOptimizer optimizer = new FertilizerOptimizationAdapter(
    new SimplexOptimizationSolver(),
    new OptimizationProblemMapper());
```

## Example

```csharp
using NPKTools.Core.Domain.Collections;
using NPKTools.Core.Domain.Fertilizers;
using NPKTools.Core.Domain.Fertilizers.Builders;
using NPKTools.Core.Domain.PpmTarget;
using NPKTools.Core.Domain.PpmTarget.Builder;
using NPKTools.Core.Domain.SolutionsFinderSettings;
using NPKTools.Core.Domain.SolutionsFinderSettings.Builder;

// What the finished solution should contain, and in how much water.
PpmTarget target = new PpmTargetBuilder()
    .AddN(150).AddP(50).AddK(200).AddMg(60).AddCa(60).AddS(80)
    .AddLiters(100)
    .Build();

// How tightly each element must be matched. 1 = exact, 0 = unconstrained.
SolutionFinderSettings settings = new SolutionFinderSettingsBuilder()
    .AddN(1).AddP(1).AddK(1).AddCa(1).AddMg(1).AddS(1).AddCl(1)
    .Build();

// The fertilizers you actually have. Values are percentages by weight.
IReadOnlyList<Fertilizer> available =
[
    new FertilizerBuilder().AddName("Calcium Nitrate").AddNo3(11.863).AddCaNonChelated(16.972).Build(),
    new FertilizerBuilder().AddName("Potassium Nitrate").AddNo3(13.854).AddK(38.672).Build(),
    new FertilizerBuilder().AddName("Ammonium Nitrate").AddNo3(17.499).AddNh4(17.499).Build(),
    new FertilizerBuilder().AddName("Magnesium Sulfate").AddMgNonChelated(9.861).AddS(13.008).Build(),
    new FertilizerBuilder().AddName("MKP").AddP(22.761).AddK(28.731).Build(),
    new FertilizerBuilder().AddName("SOP").AddS(18.401).AddK(44.874).Build()
];

Solution? solution = optimizer.Optimize(target, available, settings);

if (solution is null)
{
    Console.WriteLine("No mix of these fertilizers can hit that target.");
    return;
}

foreach (Fertilizer fertilizer in solution)
{
    Console.WriteLine($"{fertilizer.Name.Value}: {fertilizer.Weight.Value:F3} g");
}
```

`Optimize` returns `null` when the linear program is infeasible — that is, no combination of the
supplied fertilizers can reach the target within the given precision. Every fertilizer in
`available` must have a distinct `RefId` and a distinct nutrient composition; duplicates throw.

## Breaking changes in 2.0.0

`Optimize` now takes `IReadOnlyList<Fertilizer>` rather than `IList<Fertilizer>`, and `Solution` is
an `IReadOnlyList<Fertilizer>` instead of deriving from `List<Fertilizer>`.

`GoogleOrToolsOptimizationSolver` moved out of this package into
[`NPKTools.Optimizer.OrTools`](https://www.nuget.org/packages/NPKTools.Optimizer.OrTools/), so this
package no longer carries native binaries. Add that package and call `AddNpkToolsOrToolsSolver()`
before `AddNpkToolsOptimizer()` to keep using GLOP. See the
[changelog](https://github.com/i7aket/NPKTools/blob/main/CHANGELOG.md).

## Developers

Developed by **Anatoliy Yermakov** ([LinkedIn](https://www.linkedin.com/in/anatoliyyermakov),
[GitHub](https://github.com/i7aket)).

Special thanks to **Artem Frolov** ([LinkedIn](https://www.linkedin.com/in/artfrolov/),
[GitHub](https://github.com/AqueGen)) for his invaluable assistance and guidance.

## License

MIT.
