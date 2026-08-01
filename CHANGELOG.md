# Changelog

All notable changes to this project are documented here.
This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2026-07-31

The whole suite moves to .NET 10. This release contains breaking API changes; see
**Migrating from 1.x** at the end of this entry.

### Fixed

- **Reports were locale-dependent.** `ReportFormatter.AppendLineIfNonZero` formatted its decimal
  branch with the ambient culture while the integer branch used the invariant culture, so the same
  fertilizer rendered as `Weight: 100,000` on one machine and `Weight: 100.000` on another. All
  report output — `Fertilizer.Report()`, `Ppm.Report()` and `Fertilizer.GetNutrientSummary()` — is
  now invariant. `GetNutrientSummary` was affected separately through its `:N2` format specifier.
- **Sodium could not be parsed.** `PpmTargetParser` omitted `Na` from its set of accepted elements
  even though it read a sodium target back out, so `"Na=5"` threw `FormatException` and the sodium
  target was always zero.
- **`FertilizerPrice.Value` and `RangeFactorSettings.Value` were public mutable fields**, letting
  callers overwrite a validated value and bypass the positive/range checks. Both are now
  get-only properties, matching every other value object.
- **`ArgumentNullException.ThrowIfNull` was called on `ConcentrateType`**, an enum, where it is a
  no-op.
- **The mapper threw a bare `ArgumentOutOfRangeException`** with no parameter name or message from
  its unreachable default branch.
- **One-sided constraints always came back infeasible from the managed solver.** A non-finite bound
  is the only way `OptimizationConstraint` expresses "≥ only" or "≤ only", and it went straight into
  the right-hand side, leaving an artificial variable basic at infinity. `2 ≤ x + y ≤ +∞` returned
  null while OR-Tools solved it. A non-finite bound now contributes no row on that side, so the two
  backends accept the same problems. This was a silent behavioral regression for any 1.x consumer of
  the raw `IOptimizationProblemSolver` API, since 2.0.0 also changes which solver is the default.
- **The managed solver could report a wrong answer as a success.** On badly scaled problems it
  returned points violating `x ≥ 0` by as much as 0.88 — reachable only through the raw solver API,
  since the mapper would have turned such a value into an `ArgumentOutOfRangeException` downstream.
  Every result is now verified against the original constraints in one O(mn) pass and downgraded to
  null if it does not hold. Found by differential testing against GLOP over ~16,200 adversarial
  random LPs.
- **A `NaN` bound produced a `NaN` "solution" reported as success.** NaN bounds and coefficients are
  now rejected with `ArgumentException`.
- **The managed solver silently ignored coefficients naming an undeclared variable**, so a typo
  became a zero. It now throws `KeyNotFoundException`, which is what the OR-Tools backend already
  did.

### Added

- **A fully managed solver, and the package split that makes it useful.**
  `SimplexOptimizationSolver` is a two-phase primal simplex with no native dependencies, and it is
  now the default behind `IOptimizationProblemSolver`. `NPKTools.Optimizer` therefore no longer
  depends on Google OR-Tools; the GLOP backend moved to the optional
  **`NPKTools.Optimizer.OrTools`** package, opted into with `AddNpkToolsOrToolsSolver()`.

  This is not a cosmetic change. OR-Tools ships native binaries only for linux-x64/arm64,
  osx-x64/arm64 and win-x64, so anything depending on it cannot run under WebAssembly. With the
  managed solver, `NPKTools.Optimizer`, `NPKTools.Optimizer.Preset`, `NPKTools.Optimizer.PPMCalc` and
  `NPKTools.Optimizer.PpmTargetParser` all run client-side — verified by publishing the full
  pipeline to `browser-wasm` and executing it, which produced 11 macro solutions landing exactly on
  an `N=150 P=50 K=200 Ca=100 Mg=50 S=60` target. (Plain 150/50/200 N/P/K yields 4, in both
  backends.)

  The problem size justifies it: a macronutrient bundle is at most 16 variables and 7 range
  constraints, and a full preset search is 40 such problems. Equivalence with OR-Tools is asserted
  by 60 tests — 20 over the curated preset bundles across 11 target profiles, and 40 randomized
  synthetic catalogues (11 feasible, 29 infeasible) comparing feasibility verdicts, optimal
  objective values, non-negativity and constraint satisfaction. Differential testing over 17,760
  mapper-generated problems found no disagreement with GLOP: the worst constraint violation was
  1.4e-14 and the worst relative cost gap 5.8e-16.

  It is not a general-purpose LP solver, and the class documentation now says so. It is a dense
  tableau with a fixed tolerance and no scaling or iterative refinement; on a problem whose
  coefficients span ten or more orders of magnitude, round-off can make it stop at a suboptimal
  vertex or report no solution where one exists. Every answer it returns is verified against the
  original constraints first, so it will not return a mix that violates them. `NPKTools.Optimizer.OrTools`
  remains available for input outside that envelope.
- **The test suite actually runs.** No test project referenced `xunit.runner.visualstudio`, so
  `dotnet test` reported "No test is available" for all six assemblies and 283 tests never
  executed — in CI or locally. The runner is now present and the suite is wired into CI. The suite is
  now 403 tests.
- **Direct unit tests for the default solver.** Its coverage was entirely differential against
  OR-Tools, which only runs where the OR-Tools native binaries exist — so on an unsupported RID the
  shipped default solver had no coverage at all. There are now 22 tests with hand-computed optima,
  including the maximization path (previously exercised by zero tests in the suite, so a sign error
  would have shipped green), the unbounded and infeasible paths, every documented guard clause, and
  the one-sided-bound and ill-scaling cases above.
- **A test for the OR-Tools substitution.** `AddNpkToolsOrToolsSolver().AddNpkToolsPreset()` is the
  flagship snippet in three READMEs and was never asserted; the whole mechanism rests on
  registration order, which is now pinned in both directions.
- `Ppm.Report()` is covered by the cross-culture tests alongside `Fertilizer.Report()`.
- Regression tests for both bugs above: report output is asserted identical across de-DE, ru-RU,
  fr-FR and en-US, and sodium parsing is covered for mixed casing, combination with other
  elements, absence, and duplicates.
- **Dependency injection helpers**, so consumers no longer wire four objects together by hand:
  `AddNpkToolsOptimizer()`, `AddNpkToolsPreset()`, `AddNpkToolsPpmCalc()` and
  `AddNpkToolsPpmTargetParser()`. All use `TryAdd`, so registering your own
  `IOptimizationProblemSolver` first replaces the managed default — which is how
  `AddNpkToolsOrToolsSolver()` substitutes the GLOP backend.
- **`CancellationToken` support** on `IFertilizerOptimizationService`. A macro search solves 36
  linear programs in sequence — the 18 bundles once with the sulfur target and once without — and
  previously could not be interrupted.
- XML documentation is now shipped in the packages (`GenerateDocumentationFile`), along with
  SourceLink, deterministic builds and `.snupkg` symbol packages.
- A `CHANGELOG.md`, `.editorconfig`, `global.json` and Dependabot configuration.

### Changed

- **Target framework is now `net10.0`** (was `net8.0`).
- **Google.OrTools 9.9.3963 → 9.15.6755.**
- **Test dependencies:** xunit 2.7.1 → 2.9.3, NSubstitute 5.1.0 → 6.0.0,
  Microsoft.NET.Test.Sdk 17.9.0 → 18.8.1. `FluentAssertions` 6.12.0 is replaced by
  **AwesomeAssertions 9.4.0**, the Apache-2.0 community fork of FluentAssertions 7, because
  FluentAssertions 8+ requires a paid commercial licence.
- Package metadata and versions moved into `src/Directory.Build.props`, and NuGet versions into
  `Directory.Packages.props` (Central Package Management). The eleven project files shrank from
  ~25 lines each to only what is specific to them.
- The build now treats warnings as errors with `AnalysisLevel: latest-recommended`. The 105
  pre-existing warnings are resolved rather than suppressed, apart from documented, narrowly
  scoped exceptions (`CA1051` on the builders' `ref`-assigned protected fields, xUnit naming and
  null-argument conventions in tests).
- `GeneratePackageOnBuild` is off; packing is an explicit `dotnet pack` step.
- CI runs build, test and pack on Linux, Windows and macOS for every push and pull request.
  Publishing is triggered by a `v*.*.*` tag rather than every push to `main`, and runs the test
  suite before pushing packages.

### Quality

- Sealed the 13 leaf types and interface-backed implementations that were never designed for
  inheritance (`Fertilizer`, `Ppm`, `PpmTarget`, `SolutionFinderSettings`, the services, the mapper,
  the adapter, the repository, the comparer). `FertilizerAttributes` and the `*BuilderBase<TBuilder>`
  types stay open because they exist to be inherited. Extend behaviour through the interfaces or
  composition.
- `ThrowIf.NullOrEmpty` no longer enumerates to decide emptiness. It read the count via `Any()`,
  which starts enumerating; it now reads `Count` from `IReadOnlyCollection<T>` or `ICollection<T>`
  and only falls back to enumeration for a genuinely lazy sequence. `Enumerable.TryGetNonEnumeratedCount`
  alone was not sufficient — it does not recognise `IReadOnlyCollection<T>`, which is what the
  library's own `Solution` and `Solutions` are.
- `dotnet format` applied across the repository (123 files: unused usings removed, missing final
  newlines added) and wired into CI as `--verify-no-changes`, so formatting cannot drift again.
- Added `benchmarks/NPKTools.Benchmarks` (BenchmarkDotNet). It records that the managed solver is
  about 5.5× faster than GLOP on a full preset search (0.67 ms against 3.67 ms), which keeps the
  choice of default solver an evidence-based one.
- **Corrected documentation that was wrong rather than merely thin.** The `RangeFactorSettings` XML
  doc and two READMEs had the tolerance rule inverted — the allowed deviation is
  `target × (1 − min(rangeFactor, elementPrecision))`, so the looser of the two precisions decides and
  raising the range factor *tightens* the search. `FertilizerWeight` was documented as "kilograms or
  pounds" when the ppm arithmetic requires grams. The `GoogleOrToolsOptimizationSolver` XML doc
  declared an `InvalidOperationException` it never throws. `NPKTools.Core`'s README claimed every
  chelatable element tracks four chelate forms; only iron does. Quick-start snippets were missing the
  usings they need and never mentioned that `ServiceCollection` requires
  `Microsoft.Extensions.DependencyInjection`, not just the abstractions package the libraries
  reference — so the examples on the package pages did not compile as shown. `CONTRIBUTING.md` still
  called the project NPKOptimizer.
- Added `SECURITY.md` describing the actual threat surface — these libraries perform no I/O, so it
  is essentially input handling and bounded solve time. CodeQL scanning is left to the repository's
  existing GitHub default setup; an advanced-setup workflow was tried and removed, because CodeQL
  rejects advanced configurations while default setup is enabled ("configuration error") and a second
  analysis would only duplicate the first.

### Removed

- The dead `Microsoft.AspNetCore.Mvc.Testing` reference from all six test projects. The repository
  contains no ASP.NET code.
- The redundant `xunit.assert` reference, which `xunit` already brings in.
- Duplicate `ProjectReference` entries in `NPKTools.Optimizer.PpmTargetParser.Tests` and
  `NPKTools.Optimizer.Preset.Tests`.

### Migrating from 1.x

| 1.x | 2.0.0 | Why |
| --- | --- | --- |
| `IFertilizerBundleRepository.Marco()` | `.Macro()` | Spelling fix in the public API. |
| `PpmTargetBuilderBase.AddLitters(...)` | `.AddLiters(...)` | Spelling fix. Compile-breaking for anyone building a `PpmTarget`. |
| `IFertilizerBundleRepository.Macro()`/`Micro()` returned `IList<IList<Fertilizer>>` | `IReadOnlyList<IReadOnlyList<Fertilizer>>` | The bundles are cached and shared; handing out a mutable list let a caller corrupt the catalogue for everyone. |
| `Solution : List<Fertilizer>` | `Solution : IReadOnlyList<Fertilizer>` | A solution was mutable after the optimizer produced it. Construct with `new Solution(fertilizers, waterLiters)`; `WaterLiters` is now get-only. |
| `Solutions : List<Solution>` | `Solutions : IReadOnlyList<Solution>` | Same reason. |
| `Solutions?` returns, `null` for "not found" | `Solutions`, `Solutions.Empty` | Callers no longer need a null check before enumerating. |
| `Fertilizer`, `Ppm`, `PpmTarget`, `SolutionFinderSettings`, `FertilizerAttributes`: parameterless constructor + settable properties | constructor only, get-only properties | The parameterless constructors left non-nullable properties null and produced 144 `CS8618` warnings. Nothing in the repository used them. Use the existing builders (`FertilizerBuilder`, `PpmBuilder`, `PpmTargetBuilder`, `SolutionFinderSettingsBuilder`). |
| `IList<Fertilizer>` parameters on `IFertilizerOptimizer`, `IOptimizationProblemMapper`, `IPpmCalculationService` | `IReadOnlyList<Fertilizer>` | These APIs only enumerate their input, and it lets `Solution` be passed straight to `CalculatePpm`. |
| `FertilizerCollectionBuilder.Build()` returned `IList<Fertilizer>` | returns `IReadOnlyList<Fertilizer>` | Consistency with the above. |
| namespace `NPKTools.Core.Const` | `NPKTools.Core.Constants` | `Const` collides with a reserved keyword in some .NET languages (CA1716). |
| `GoogleOrToolsOptimizationSolver` in `NPKTools.Optimizer` | package **`NPKTools.Optimizer.OrTools`**, namespace `NPKTools.Optimizer.OrTools` | Keeps native binaries out of the base package so it can run under WebAssembly. Add the package and call `AddNpkToolsOrToolsSolver()` before `AddNpkToolsOptimizer()`/`AddNpkToolsPreset()` to keep using GLOP. |
| OR-Tools was the only solver | `SimplexOptimizationSolver` is the default | Managed, dependency-free, and validated against OR-Tools. Results are equivalent; where an optimum is degenerate the two may report different vertices of equal cost. |
| `OptimizationConstraint.Name` was a `required` field | `required` property | Consistency with the rest of the record. |
| A coefficient naming an undeclared variable was ignored by `SimplexOptimizationSolver` | throws `KeyNotFoundException` | It is a typo, not a zero. OR-Tools already threw; the two backends now reject the same problems. Only affects code calling `IOptimizationProblemSolver` directly. |
| A `NaN` bound or coefficient produced a `NaN` result | throws `ArgumentException` | Returning NaN as a successful solve is the worst possible output. |

## [1.1.6] and earlier

See the [commit history](https://github.com/i7aket/NPKTools/commits/main) for releases before
this changelog was introduced.
