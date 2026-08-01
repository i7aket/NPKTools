# Changelog

All notable changes to this project are documented here.
This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0-preview.1] - 2026-08-01

**A new package line.** `SYT.NPKTools` replaces the six `NPKTools.*` packages, which receive no
further updates. The version resets to 1.0.0 because the package identity is new; in terms of code
this is a direct continuation, carrying every fix listed under *Inherited from the NPKTools line*
below.

The find-and-replace table for moving off the old packages is in
[README.md](README.md#migrating-from-npktools-1x).

### One package instead of six

`NPKTools.Core`, `NPKTools.Optimizer`, `NPKTools.Optimizer.Preset`, `NPKTools.Optimizer.PPMCalc` and
`NPKTools.Optimizer.PpmTargetParser` are now a single `SYT.NPKTools`.

The split cost consumers something and bought them nothing. Five of the six projects had identical
external dependencies — `Microsoft.Extensions.DependencyInjection.Abstractions` alone — and together
came to 161 KB of IL, so nobody was avoiding a meaningful download by taking a subset. Meanwhile a
first-time user had to know which three of six packages made a working setup, and every release
multiplied version-matrix combinations that were never tested apart.

- **Namespaces are flattened** from 24 to 4: `SYT.NPKTools`, `.Fertilizers`, `.Nutrients`,
  `.Optimization`. The old tree had namespaces named after the type inside them
  (`NPKTools.Core.Domain.PpmTarget` containing `PpmTarget`), which forced awkward qualification.
- **One `AddNpkTools()`** replaces `AddNpkToolsOptimizer()`, `AddNpkToolsPreset()`,
  `AddNpkToolsPpmCalc()` and `AddNpkToolsPpmTargetParser()`. Registration order between the four was
  load-bearing and undocumented; now there is no order to get wrong.
- **`ThrowIf`, `ReportFormatter`, `Labels`, `Names` and `OptimizationSettings` are `internal`.** They
  were public only because they had to cross package boundaries. `ElementFieldBase` and the
  `*BuilderBase<T>` types stay public — the public value objects and builders derive from them.
- **`FindSolutions` returns `FertilizerSolutions`** instead of a
  `(Solutions Macro, Solutions Micro)` tuple, so the shape is named, documented and extensible.
  It carries `Empty` and `IsEmpty`.

### Google OR-Tools is no longer shipped

`NPKTools.Optimizer.OrTools` is gone, and with it `AddNpkToolsOrToolsSolver()`. The GLOP solver now
lives at `tests/SYT.NPKTools.OrToolsOracle`, which is not published.

This is what makes the browser story real rather than conditional. OR-Tools distributes native
binaries only for linux-x64/arm64, osx-x64/arm64 and win-x64 — there is no `browser-wasm` build — so
while a shipped OR-Tools package existed, "runs in the browser" depended on which package you
installed. Now every published byte is managed code.

Nothing is lost in validation terms: the oracle is exactly how the managed solver's correctness is
established, and it is also the benchmark baseline, which is why it is its own project rather than a
file inside a test assembly. `IOptimizationProblemSolver` remains public and the default is registered
with `TryAdd`, so a consumer who wants GLOP at runtime registers their own implementation first — the
oracle is a complete worked example of doing that.

### Packaging

- Package id `SYT.NPKTools`, title `SYT NPKTools`, matching the other `SYT.*` packages.
- The licence ships as a file inside the package rather than as an SPDX expression, so the exact text
  is in what consumers downloaded.
- One README, packed from the repository root, so the NuGet page and the GitHub landing page cannot
  drift apart. Its images use absolute URLs because NuGet does not resolve relative paths.
- Test projects consolidated from six to three — `SYT.NPKTools.Tests`,
  `SYT.NPKTools.IntegrationTests`, `SYT.NPKTools.OrTools.Tests` — because the old split mirrored
  package boundaries that no longer exist. **405 tests**, unchanged in coverage.

---

## Inherited from the NPKTools line

Everything below shipped in the `NPKTools.*` packages and is present in `SYT.NPKTools 1.0.0-preview.1`.
It is kept here because it is the actual history of this code, not of a package id.

### Fixed

- **Reports were locale-dependent.** `ReportFormatter.AppendLineIfNonZero` formatted its decimal
  branch with the ambient culture while the integer branch used the invariant culture, so the same
  fertilizer rendered as `Weight: 100,000` on one machine and `Weight: 100.000` on another. All
  report output — `Fertilizer.Report()`, `Ppm.Report()` and `Fertilizer.GetNutrientSummary()` — is
  now invariant. `GetNutrientSummary` was affected separately through its `:N2` format specifier.
- **Sodium could not be parsed.** The target parser omitted `Na` from its set of accepted elements
  even though it read a sodium target back out, so `"Na=5"` threw `FormatException` and the sodium
  target was always zero.
- **`FertilizerPrice.Value` and `RangeFactorSettings.Value` were public mutable fields**, letting
  callers overwrite a validated value and bypass the positive/range checks. Both are now get-only
  properties, matching every other value object.
- **`ArgumentNullException.ThrowIfNull` was called on `ConcentrateType`**, an enum, where it is a
  no-op.
- **The mapper threw a bare `ArgumentOutOfRangeException`** with no parameter name or message from
  its unreachable default branch.
- **One-sided constraints always came back infeasible from the managed solver.** A non-finite bound
  is the only way `OptimizationConstraint` expresses "≥ only" or "≤ only", and it went straight into
  the right-hand side, leaving an artificial variable basic at infinity. `2 ≤ x + y ≤ +∞` returned
  null while OR-Tools solved it. A non-finite bound now contributes no row on that side.
- **The managed solver could report a wrong answer as a success.** On badly scaled problems it
  returned points violating `x ≥ 0` by as much as 0.88. Every result is now verified against the
  original constraints in one O(mn) pass and downgraded to null if it does not hold. Found by
  differential testing against GLOP over ~16,200 adversarial random LPs.
- **A `NaN` bound produced a `NaN` "solution" reported as success.** NaN bounds and coefficients are
  now rejected with `ArgumentException`.
- **The managed solver silently ignored coefficients naming an undeclared variable**, so a typo
  became a zero. It now throws `KeyNotFoundException`, as the OR-Tools backend already did.

### Added

- **A fully managed solver.** `SimplexOptimizationSolver` is a two-phase primal simplex using Bland's
  rule, with no native dependencies, and it is the default behind `IOptimizationProblemSolver`.

  The problem size justifies a dense tableau: a macronutrient bundle is at most 16 variables and 7
  range constraints, and a full preset search is 40 such problems. Equivalence with GLOP is asserted
  by 60 tests — 20 over the curated preset bundles across 11 target profiles, and 40 randomized
  synthetic catalogues (11 feasible, 29 infeasible). Differential testing over 17,760
  mapper-generated problems found no disagreement: worst constraint violation 1.4e-14, worst relative
  cost gap 5.8e-16.

  It is not a general-purpose LP solver, and the class documentation says so. On a problem whose
  coefficients span ten or more orders of magnitude, round-off can make it stop at a suboptimal vertex
  or report no solution where one exists — but never return an infeasible mix, because every answer is
  verified first.

  Verified in the browser: the full pipeline was published to `browser-wasm` and executed, producing
  11 macro solutions landing exactly on an `N=150 P=50 K=200 Ca=100 Mg=50 S=60` target. (Plain
  150/50/200 N/P/K yields 4, in both backends.)
- **The test suite actually runs.** No test project referenced `xunit.runner.visualstudio`, so
  `dotnet test` reported "No test is available" for all six assemblies and 283 tests never executed —
  in CI or locally. That is why two culture-dependent failures sat unnoticed in the repository.
- **Direct unit tests for the default solver.** Its coverage had been entirely differential against
  OR-Tools, which only runs where the OR-Tools native binaries exist, so on an unsupported RID the
  shipped solver had no coverage at all. There are now 22 tests with hand-computed optima, including
  the maximization path — previously exercised by zero tests in the suite, so a sign error would have
  shipped green — the unbounded and infeasible paths, and every documented guard clause.
- **Regression tests for both culture and sodium bugs**, verified to fail when the fix is reverted.
  Report output is asserted identical across de-DE, ru-RU, fr-FR and en-US.
- **`CancellationToken` support** on `IFertilizerOptimizationService`. A macro search solves 36 linear
  programs in sequence — the 18 bundles once with the sulfur target and once without — and previously
  could not be interrupted.
- XML documentation shipped in the package, plus SourceLink, deterministic builds and `.snupkg`
  symbol packages.
- A `CHANGELOG.md`, `SECURITY.md`, `.editorconfig`, `global.json`, Dependabot configuration,
  issue/PR templates, and a BenchmarkDotNet project.

### Changed

- **Target framework `net10.0`** (was `net8.0`).
- **Test dependencies:** xunit 2.7.1 → 2.9.3, NSubstitute 5.1.0 → 6.0.0,
  Microsoft.NET.Test.Sdk 17.9.0 → 18.8.1, Google.OrTools 9.9.3963 → 9.15.6755.
  `FluentAssertions` 6.12.0 is replaced by **AwesomeAssertions 9.4.0**, the Apache-2.0 community fork
  of FluentAssertions 7, because FluentAssertions 8+ requires a paid commercial licence.
- **Removed** the dead `Microsoft.AspNetCore.Mvc.Testing` reference from every test project (the
  repository contains no ASP.NET code), the redundant `xunit.assert` reference, and duplicate
  `ProjectReference` entries.
- NuGet versions moved into `Directory.Packages.props` (Central Package Management) and shared package
  metadata into `src/Directory.Build.props`.
- The build treats warnings as errors with `AnalysisLevel: latest-recommended`. All 105 pre-existing
  warnings were resolved rather than suppressed, apart from documented, narrowly scoped exceptions.
- **API cleanup.** The parameterless constructors and property setters on `Fertilizer`,
  `FertilizerAttributes`, `Ppm`, `PpmTarget` and `SolutionFinderSettings` are gone — they left
  non-nullable properties null and produced 144 `CS8618` warnings, and nothing used them. `Solution`
  and `Solutions` are `IReadOnlyList<T>` rather than deriving from `List<T>`, so a result cannot be
  mutated after the optimizer produced it. Search methods return `Solutions.Empty` instead of `null`.
  `IFertilizerBundleRepository.Marco()` → `Macro()` and `PpmTargetBuilder.AddLitters` → `AddLiters`
  fix misspellings in the public API. 13 leaf types were sealed.
- `ThrowIf.NullOrEmpty` no longer enumerates to decide emptiness. It read the count via `Any()`, which
  starts enumerating; it now reads `Count` from `IReadOnlyCollection<T>` or `ICollection<T>`.
  `Enumerable.TryGetNonEnumeratedCount` alone was not sufficient — it does not recognise
  `IReadOnlyCollection<T>`, which is what `Solution` and `Solutions` are.
- **Documentation that was wrong rather than merely thin was corrected.** The tolerance rule was
  inverted: the allowed deviation is `target × (1 − min(rangeFactor, elementPrecision))`, so the looser
  of the two precisions decides and raising the range factor *tightens* the search. `FertilizerWeight`
  was documented as "kilograms or pounds" when the ppm arithmetic requires grams. The OR-Tools solver
  declared an `InvalidOperationException` it never throws. Only iron tracks four chelate forms, not
  every chelatable element. Quick-start snippets were missing usings and never mentioned that
  `ServiceCollection` needs `Microsoft.Extensions.DependencyInjection` rather than the abstractions
  package the library references — so the examples on the package pages did not compile as shown. Every
  snippet is now compiled and run as part of preparing a release.
- CI runs build, test and pack on Linux, Windows and macOS for every push and pull request. Publishing
  is triggered by a version tag, which is checked for valid SemVer and against the committed
  `<Version>` before anything is built — a version pushed to NuGet.org can be unlisted but never
  replaced. CodeQL scanning runs through the repository's GitHub default setup.

## NPKTools 1.1.6 and earlier

See the [commit history](https://github.com/i7aket/NPKTools/commits/main) for releases before this
changelog was introduced.
