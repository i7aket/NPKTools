# NPKTools.Optimizer.Preset

Part of the [NPKTools](https://github.com/i7aket/NPKTools) suite. A preconfigured
[`NPKTools.Optimizer`](https://www.nuget.org/packages/NPKTools.Optimizer/) that ships its own
fertilizer catalogue, so you can go from a nutrient target to a set of workable mixes without
describing any fertilizers yourself.

Targets **.NET 10**.

## What it contains

- **17 macronutrient fertilizers** combined into **18 bundles** — calcium nitrate, potassium
  nitrate, magnesium sulfate, calcium chloride, MKP, MAG, SOP, DKP, MOP, ammonium nitrate,
  ammonium chloride, ammonium sulfate, urea, urea phosphate, MAP, phosphoric acid and calcium
  monobasic phosphate.
- **17 micronutrient fertilizers** in **4 sets** — basic (boric acid, sodium borate, molybdate,
  silicate, selenate), sulfate, nitrate and chelated (EDTA) forms of iron, copper, manganese and
  zinc.
- Macronutrient searches run **twice**: once honouring the sulfur target, and once with sulfur
  unconstrained to widen the set of feasible mixes. Results that prescribe the same fertilizers at
  the same weights are collapsed.

Bundles are built lazily and cached, so the repository should be a singleton — the DI helper below
already registers it as one.

## Setup

```csharp
using Microsoft.Extensions.DependencyInjection;
using NPKTools.Optimizer.Preset;

ServiceProvider provider = new ServiceCollection()
    .AddNpkToolsPreset()   // also registers the underlying optimizer
    .BuildServiceProvider();

IFertilizerOptimizationService service = provider.GetRequiredService<IFertilizerOptimizationService>();
```

Or construct it directly:

```csharp
IFertilizerOptimizer optimizer = new FertilizerOptimizationAdapter(
    new GoogleOrToolsOptimizationSolver(),
    new OptimizationProblemMapper());

IFertilizerOptimizationService service =
    new FertilizerOptimizationService(optimizer, new FertilizerBundleRepository());
```

## Example

```csharp
using NPKTools.Core.Domain.Collections;
using NPKTools.Core.Domain.Fertilizers;
using NPKTools.Core.Domain.PpmTarget;
using NPKTools.Core.Domain.PpmTarget.Builder;

PpmTarget target = new PpmTargetBuilder()
    .AddN(150).AddP(50).AddK(200).AddCa(100).AddMg(50)
    .AddLiters(100)
    .Build();

(Solutions Macro, Solutions Micro) result = service.FindSolutions(target);

Console.WriteLine($"{result.Macro.Count} macro and {result.Micro.Count} micro mixes found.");

foreach (Solution solution in result.Macro)
{
    foreach (Fertilizer fertilizer in solution)
    {
        Console.WriteLine($"  {fertilizer.Name.Value}: {fertilizer.Weight.Value:F3} g");
    }
}
```

`FindMacroSolutions`, `FindMicroSolutions` and `FindSolutions` return `Solutions.Empty` when nothing
satisfies the target, so you never need a null check before enumerating. A target that only mentions
micronutrients will legitimately produce an empty macro set, and vice versa.

## Cancellation

A macro search solves 18 linear programs in sequence, which takes noticeable time. Pass a token to
abandon it:

```csharp
using CancellationTokenSource cts = new(TimeSpan.FromSeconds(5));

try
{
    Solutions solutions = service.FindMacroSolutions(target, cts.Token);
}
catch (OperationCanceledException)
{
    // The search was abandoned between bundles.
}
```

## Breaking changes in 2.0.0

`IFertilizerBundleRepository.Marco()` is now correctly spelled `Macro()`, the `Find*` methods return
`Solutions` rather than `Solutions?`, and they accept an optional `CancellationToken`. See the
[changelog](https://github.com/i7aket/NPKTools/blob/main/CHANGELOG.md).

## Developers

Developed by **Anatoliy Yermakov** ([LinkedIn](https://www.linkedin.com/in/anatoliyyermakov),
[GitHub](https://github.com/i7aket)).

Special thanks to **Artem Frolov** ([LinkedIn](https://www.linkedin.com/in/artfrolov/),
[GitHub](https://github.com/AqueGen)) for his invaluable assistance and guidance.

## License

MIT.
