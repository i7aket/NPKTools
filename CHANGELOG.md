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

### Added

- **The test suite actually runs.** No test project referenced `xunit.runner.visualstudio`, so
  `dotnet test` reported "No test is available" for all six assemblies and 283 tests never
  executed — in CI or locally. The runner is now present and the suite is wired into CI.
- Regression tests for both bugs above: report output is asserted identical across de-DE, ru-RU,
  fr-FR and en-US, and sodium parsing is covered for mixed casing, combination with other
  elements, absence, and duplicates.
- **Dependency injection helpers**, so consumers no longer wire four objects together by hand:
  `AddNpkToolsOptimizer()`, `AddNpkToolsPreset()`, `AddNpkToolsPpmCalc()` and
  `AddNpkToolsPpmTargetParser()`. All use `TryAdd`, so registering your own
  `IOptimizationProblemSolver` first replaces the OR-Tools default.
- **`CancellationToken` support** on `IFertilizerOptimizationService`. A macro search solves 18
  linear programs in sequence and previously could not be interrupted.
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
| `Solution : List<Fertilizer>` | `Solution : IReadOnlyList<Fertilizer>` | A solution was mutable after the optimizer produced it. Construct with `new Solution(fertilizers, waterLiters)`; `WaterLiters` is now get-only. |
| `Solutions : List<Solution>` | `Solutions : IReadOnlyList<Solution>` | Same reason. |
| `Solutions?` returns, `null` for "not found" | `Solutions`, `Solutions.Empty` | Callers no longer need a null check before enumerating. |
| `Fertilizer`, `Ppm`, `PpmTarget`, `SolutionFinderSettings`, `FertilizerAttributes`: parameterless constructor + settable properties | constructor only, get-only properties | The parameterless constructors left non-nullable properties null and produced 144 `CS8618` warnings. Nothing in the repository used them. Use the existing builders (`FertilizerBuilder`, `PpmBuilder`, `PpmTargetBuilder`, `SolutionFinderSettingsBuilder`). |
| `IList<Fertilizer>` parameters on `IFertilizerOptimizer`, `IOptimizationProblemMapper`, `IPpmCalculationService` | `IReadOnlyList<Fertilizer>` | These APIs only enumerate their input, and it lets `Solution` be passed straight to `CalculatePpm`. |
| `FertilizerCollectionBuilder.Build()` returned `IList<Fertilizer>` | returns `IReadOnlyList<Fertilizer>` | Consistency with the above. |
| namespace `NPKTools.Core.Const` | `NPKTools.Core.Constants` | `Const` collides with a reserved keyword in some .NET languages (CA1716). |
| `OptimizationConstraint.Name` was a `required` field | `required` property | Consistency with the rest of the record. |
| package `NPKTools.Optimizer.PPMCalc` | **`NPKTools.PPMCalc`** | The id did not match the assembly or namespace, and the README documented a third name (`NPKTools.PpmCalc`) that never existed. |

## [1.1.6] and earlier

See the [commit history](https://github.com/i7aket/NPKTools/commits/main) for releases before
this changelog was introduced.
