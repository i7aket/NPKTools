# NPKTools.Optimizer.OrTools

Optional [Google OR-Tools](https://developers.google.com/optimization) (GLOP) backend for
[`NPKTools.Optimizer`](https://www.nuget.org/packages/NPKTools.Optimizer/), part of the
[NPKTools](https://github.com/i7aket/NPKTools) suite.

Targets **.NET 10**.

## You probably do not need this package

`NPKTools.Optimizer` ships a fully managed `SimplexOptimizationSolver` as its default. It has no
native dependencies, produces the same answers as GLOP on these problems, runs everywhere .NET runs
— including WebAssembly — and on a full preset search it is measurably faster (0.67 ms against
3.67 ms), because GLOP's per-solve setup dominates at this problem size.

Installing this package brings OR-Tools' native binaries with it, which restricts you to
**linux-x64**, **linux-arm64**, **osx-x64**, **osx-arm64** and **win-x64**. There is no
`browser-wasm` build, so a project depending on this package cannot run client-side.

Reasonable reasons to install it anyway:

- You already trust GLOP and want it in production.
- You want to cross-check the managed solver's results against a reference implementation.
- You are solving much larger custom catalogues than the preset ones and want a production-grade
  solver's numerics.

## Usage

Register it **before** `AddNpkToolsOptimizer()` or `AddNpkToolsPreset()`. Those register the managed
solver with `TryAdd`, so whichever solver is registered first wins:

`ServiceCollection` and `BuildServiceProvider()` come from
**Microsoft.Extensions.DependencyInjection** — this package references only the abstractions, so in a
console app add the implementation too (`dotnet add package Microsoft.Extensions.DependencyInjection`).

```csharp
using Microsoft.Extensions.DependencyInjection;
using NPKTools.Optimizer.OrTools;
using NPKTools.Optimizer.Preset;

ServiceProvider provider = new ServiceCollection()
    .AddNpkToolsOrToolsSolver()   // must come first
    .AddNpkToolsPreset()
    .BuildServiceProvider();
```

Or construct it directly:

```csharp
using NPKTools.Optimizer.Components;
using NPKTools.Optimizer.Contracts;
using NPKTools.Optimizer.OrTools;

IFertilizerOptimizer optimizer = new FertilizerOptimizationAdapter(
    new GoogleOrToolsOptimizationSolver(),
    new OptimizationProblemMapper());
```

`GoogleOrToolsOptimizationSolver` implements `IOptimizationProblemSolver` and is a drop-in
replacement for the managed default: same contract, and it returns `null` on anything other than an
optimal result.

## Equivalence with the managed solver

The repository asserts the two backends agree, with 60 tests covering the 22 curated preset bundles
across 11 target profiles plus 40 randomized synthetic catalogues. They compare feasibility
verdicts, optimal objective values, non-negativity and constraint satisfaction.

Where a linear program has several optimal vertices the two solvers may report different fertilizer
weights of identical total cost, so do not expect byte-identical mixes when switching backends.

## When this backend is worth it

For the problems this library generates the two are equivalent, and the managed solver is faster —
differential testing over 17,760 mapper-generated problems found no disagreement, with a worst
constraint violation of 1.4e-14. GLOP earns its place on badly scaled input: it scales and
equilibrates the problem, whereas the managed solver is a dense tableau with a fixed tolerance and
will stop at a suboptimal vertex, or report no solution, once coefficients span ten or more orders of
magnitude in a single problem. If you feed the optimizer your own catalogue with extreme prices or
nutrient percentages, use this backend.

## New in 2.0.0

This package is new. `GoogleOrToolsOptimizationSolver` previously lived in `NPKTools.Optimizer`,
which meant every consumer inherited the native dependency. See the
[changelog](https://github.com/i7aket/NPKTools/blob/main/CHANGELOG.md) for the migration note.

## Developers

Developed by **Anatoliy Yermakov** ([LinkedIn](https://www.linkedin.com/in/anatoliyyermakov),
[GitHub](https://github.com/i7aket)).

Special thanks to **Artem Frolov** ([LinkedIn](https://www.linkedin.com/in/artfrolov/),
[GitHub](https://github.com/AqueGen)) for his invaluable assistance and guidance.

## License

MIT.
