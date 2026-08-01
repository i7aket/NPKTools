# Changelog

All notable changes to this project are documented here.
This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- **Source water is deducted from the target.** `WaterProfile` describes what the tap or well water
  already contains, and `target.AdjustFor(water)` returns the target the fertilizers actually have to
  meet.

  This is a correctness fix disguised as a feature. Whatever the water carries is added on top of
  everything the fertilizers contribute, so on an ordinary municipal analysis — Ca 45, Mg 15, S 20 — a
  mix calculated against the raw target overshoots calcium by **45%**, magnesium by 30% and sulfur by
  33%. Deducting first lands every element on target exactly. It is also the feature every serious
  competitor has and this library did not.

  The deduction happens ahead of the optimizer rather than inside it: the result is an ordinary
  `PpmTarget`, so the solver, mapper and bundles are untouched, and the arithmetic stays inspectable.
  `WaterProfile.Pure` is the all-zero profile for reverse osmosis and leaves a target unchanged, which
  is asserted as a regression guard for callers who do not use the feature.

  Where the water already oversupplies an element, that element is clamped to zero and reported in
  `WaterAdjustedTarget.Excesses` rather than silently truncated. Fertilizer only adds, so no mix can
  bring it down — the remedies are to raise the target or dilute the water, both outside the
  calculation. Hard water against a low calcium target is the everyday case.

  `WaterProfile` carries all 16 elements, symmetric with `PpmTarget`, and reuses the existing `*Ppm`
  value objects rather than introducing sixteen more. It has no water volume, because a water analysis
  is a concentration and holds regardless of how much is used.

- **Nutrient ratios and a per-fertilizer breakdown.** `ppm.Ratios()` returns the ratios a profile is
  actually judged by — NO₃:NH₄, N:K, K:Ca, Ca:Mg, K:Mg, N:S, N:P — and `solution.Breakdown(calculator)`
  attributes every element to the salt that supplied it.

  Absolute ppm figures say how strong a solution is; the ratios say what it will do. NO₃:NH₄ is the one
  worth watching most closely, because it predicts pH movement at the root rather than nutrition:
  ammonium uptake acidifies the root zone, nitrate uptake alkalizes it.

  A ratio whose denominator is zero is `null`, not zero and not infinity. A nitrate-only mix is the
  everyday case and reporting "0" or "∞" for its NO₃:NH₄ would be actively misleading.

  The breakdown answers what a bare recipe cannot: which salt is responsible for the sulfur nobody
  asked for. Every salt brings a counter-ion along with the nutrient you wanted, so an unrequested
  element is rarely a mistake — it is the price of the element next to it, and seeing which salt carries
  it is what makes a recipe adjustable. Contributions are measured through the same calculator as the
  whole mix, so the parts cannot drift from the total; a test asserts they sum.

  Together these close the standing complaint about comparable tools: that they compute a recipe but
  say nothing about whether it is a sensible one.

- **A source-water analysis reads its own alkalinity.** `water.AsPpm()` runs a `WaterProfile` through the
  solution analyses, and `water.EstimatedAlkalinity()` reports the bicarbonate its cation-anion gap implies.

  This fell out of the charge balance rather than being designed. A finished recipe's cations equal its
  anions; a water analysis's do not, and that is not an error in the analysis — bicarbonate is what balances
  real water, it is not a plant nutrient, and so it has nowhere to be entered. The gap left over is a
  measurement of it. On an ordinary municipal analysis (Ca 45, Mg 12, S 18, Na 20, Cl 25) the surplus is
  2.27 meq/L, which is 139 ppm of HCO₃⁻ or 114 as CaCO₃ — and also the acid needed per litre to neutralise
  the water, which is the number that decides whether hard water pushes root-zone pH up all season.

  The water's own EC comes with it, and is the quickest check that an analysis was typed in correctly:
  compare it against the meter reading the grower already has. The same tap water above reads 358 µS/cm.

  An estimate from what the analysis contains rather than a substitute for a measured alkalinity figure — it
  cannot distinguish bicarbonate from carbonate or from an ion nobody entered, and it clamps at zero, since
  negative alkalinity is not a thing.

- **EC estimation from the ions.** `ppm.EstimateConductivity()` predicts what a conductivity meter will
  read, and `AsTdsPpm(scale)` converts that to the "ppm" a TDS meter shows.

  Computed from each ion's molar conductivity rather than by scaling total dissolved solids by a fitted
  factor, which is the distinction that makes it worth having: a mole of sulfate conducts more than twice
  what a mole of dihydrogen phosphate does, so two solutions of identical ppm read differently and a single
  factor cannot know which it was given. The per-ion shares come out of the same calculation, so a caller can
  see that nitrate is usually about a third of the reading.

  Checked against the certified KCl conductivity standards at 25 °C — the reference solutions meters
  themselves are calibrated against. The estimate is **+0.1%** at 0.001 M and **+0.3%** at 0.01 M, which
  bracket the ionic strength of a real feed, and −3.9% at 0.1 M, ten times stronger than anything anyone
  grows in. A test asserts all three, including the last, so the boundary of usefulness is a fact in the
  suite rather than a claim in a comment.

  `IdealMicroSiemensPerCm` is the raw sum of limiting molar conductivities, exact at infinite dilution and
  increasingly high above it as ions interfere with each other. `MicroSiemensPerCm` applies a Kohlrausch-form
  correction, `Correction` exposes the factor used (≈0.91 for a normal feed) and `IonicStrength` the
  exactly-computable quantity that sets it. The single coefficient in that correction is anchored on the
  certified standards, not fitted to fertilizer recipes — which was the point of doing this ionically rather
  than by regression.

  Urea gives an EC of exactly zero while carrying a great deal of nitrogen, because an uncharged molecule
  carries no current. Asserted as a test, because it is the reason EC is a poor proxy for feed strength
  whenever urea is involved rather than an edge case to be smoothed over.

- **mM and meq/L, and the charge balance.** `ppm.AsMillimolar()` converts a profile to millimoles per
  litre; `ppm.IonBalance()` expresses it as charge in milliequivalents and reports how the two sides
  compare.

  Ppm is a mass unit and so flatters the light elements: 40 ppm of calcium and 40 ppm of magnesium are
  1.00 mM and 1.65 mM. Every published formulation — Steiner, Hoagland, the Dutch advisory tables — is
  stated in mM, so working from a paper recipe meant converting sixteen numbers by hand.

  The charge balance turned out to be more interesting than expected, and the first version of this entry
  described it wrongly as an error check. It is not: every salt is electrically neutral, so a recipe of
  nothing but salts balances exactly, and where it does not the gap is the acid or base the recipe itself
  contributes. `AcidEquivalents` reports that in meq/L of H⁺ — positive pulls pH down, negative pushes it
  up. Measured across the catalogue, fourteen of the seventeen macro salts come out at exactly zero;
  phosphoric acid at +1 proton per phosphorus, urea phosphate likewise, and dipotassium phosphate at −1,
  which are exactly the three salts with acid-base character. So the figure says whether a recipe needs
  more or less pH-down than the water alone would suggest.

  Micronutrients are excluded from the charge figures deliberately. Iron may be Fe²⁺ or Fe³⁺ and in
  chelated form the complex is an anion rather than a cation, so any charge for it is a guess, and at under
  5 ppm in total they would move the balance by less than 0.1 meq/L against a typical 25–30 — uncertainty
  added to a figure whose worth is that it is exact. Boron and silicon are excluded for a firmer reason: as
  boric and silicic acid they are undissociated at nutrient-solution pH. Urea contributes nitrogen in mM
  and nothing in meq, being a neutral molecule.

  Phosphorus is counted as H₂PO₄⁻ at one charge, the dominant species between pH 5.5 and 6.5. Above pH 7.2
  half of it is HPO₄²⁻; the library does not model pH, so the assumption is documented rather than
  computed. It is also what makes the acid-base figures come out in whole protons.

- **Bundles are generated from your own salts.** `NpkTools.CreateOptimizationService(shelf)` takes a list
  of salts and searches it the way the service searches the preset catalogue.

  This is the answer to "I want to use what I have". The catalogue offers eighteen macro bundles and so a
  dozen recipes to compare; a custom shelf used to be one bundle and therefore one recipe. Everything
  downstream is unchanged, because `CustomFertilizerBundleRepository` is an ordinary
  `IFertilizerBundleRepository` — the same solver, the same ppm calculator, the same concentrate split.

  The generation rule is hold-one-out: the first bundle holds every salt, and each of the others leaves
  out exactly one. It was chosen by measurement, not taste. Counting distinct recipes returned across
  three macro targets: one-source-per-element cross product scored 5, the hand-written catalogue 19,
  hold-one-out **21**, and holding out pairs and triples as well also 21. Depth beyond one buys nothing —
  a smaller subset either reproduces a recipe already found or solves nothing — so it is not implemented.

  The reason building bundles up scores so badly is worth recording: six simultaneous element targets need
  at least six salts to satisfy at non-negative weights, and a bundle assembled as one source per element
  rarely has six once overlapping salts collapse. The first version of this feature did exactly that and
  scored 5 against the catalogue's 19; it was replaced rather than tuned.

  `GeneratedBundles` reports what generation could not do: `UncoveredElements` names elements no supplied
  salt contains, and `CustomFertilizerBundleRepository.UnusableSalts` names salts carrying nothing any
  target can ask for. Without the first, a missing magnesium source appears as "no solutions" and sends
  someone hunting through their shelf for a mistake that is not there.

  Macro and micro split as the catalogue splits them — any micronutrient makes a salt a micro salt, so
  iron sulfate's incidental sulfur cannot be recruited to meet a sulfur target.

- **A/B concentrates.** `mix.AsConcentrate(concentrateLiters)` splits a working-strength recipe into two
  stock tanks and reports the dilution ratio and the dose per liter.

  This is what makes a calculated recipe usable more than once. Weighing six salts every watering is
  what stops people using one; mixing two tanks a month and dosing 10 ml of each is not. The whole
  recipe's salt goes into the smaller volume, so a 100-liter recipe in a 1-liter tank is 1:100 —
  concentrating changes how much water the salt goes into, never how much salt is needed, which is why
  the finished solution still lands on the same target the optimizer solved for.

  Two tanks rather than one for a chemical reason: calcium sulfate and the higher calcium phosphates are
  barely soluble, so what stays dissolved at working strength falls out of solution at 100×. Each salt's
  tank comes from its own `ConcentrateType`, which the preset catalogue already carries, so the split is
  data rather than re-derived chemistry. A custom salt has none unless its author sets one, so a tank is
  inferred from composition and the inference is reported — guessing silently is how someone's own
  sulfate ends up beside calcium.

  `HasPrecipitationRisk` flags calcium meeting sulfate or phosphate in one tank **from different salts**,
  and is documented as a rule of thumb rather than a solubility calculation. A single salt carrying both
  internally is deliberately not flagged: an audit of the catalogue found `Calcium Monobasic Phosphate`
  (Ca 15.9%, P 24.6% in one compound), which the naive rule would have false-positived. A check that
  fires on a legitimately soluble salt teaches users to ignore the check, so the guard is asserted in
  both directions — the false positive must not fire, and an internally-mixed salt must not suppress a
  genuine collision beside it.

- **Concentrates are checked against solubility.** `SolubilityTable` holds how much of each salt a litre of
  water takes at 20 °C, and `AsConcentrate` now reports whether the tanks can physically be mixed.

  Two checks, catching different mistakes. A single salt past its own limit is certain — arithmetic against
  a published figure — and monocalcium phosphate at 18 g/L is what binds first in practice, against
  potassium nitrate's 316 and calcium nitrate's 1290. The second is the tank as a whole:
  `ConcentrateTank.SaturationFraction` adds each salt's share of its own limit, and above 1 the tank cannot
  dissolve. That one was added after measurement, not by design: a 1:500 concentrate reaching 610 g/L in one
  tank passed the per-salt check with every salt comfortably inside its own limit, which is too permissive
  to be useful. Salts in a tank compete for the same water. The sum of fractions is the standard first-order
  screen — exact for a single salt, slightly optimistic for a mixture, since salts sharing an ion crowd each
  other out more than the sum suggests.

  `ConcentratePlan.MaxDilutionRatio` answers the question the warning raises: a 1:1000 concentrate that
  cannot dissolve may be fine at 1:600.

  **A salt with no published figure is reported rather than assumed safe.** The table covers 21 of the
  catalogue's 34 salts; the rest — calcium chloride hexahydrate, urea phosphate, the EDTA chelates, sodium
  silicate and selenate, the nitrate micro salts — carry no entry, because the figures in circulation for
  them disagree by more than the check would be worth. A chelate's solubility depends on the formulation, a
  deliquescent hydrate's on which hydrate it is. Guessing would be worse than declining: a wrong limit
  either blocks a tank that would have mixed or passes one that will not. Those salts come back in
  `ConcentratePlan.UnknownSolubility`, `SaturationFraction` is null rather than a partial sum, and no
  ceiling is computed — the missing salt could be the one that binds. `SolubilityTable.With` takes a figure
  off a bag label for a salt of your own.

  A test asserts that every name in the table resolves to a real catalogue salt. It exists because the
  first version of the table misspelled five micro-salt names, and a name that matches nothing fails
  silently: the salt reports as unchecked and the check appears to work.

## [1.0.0-preview.2] - 2026-08-01

**A new package line.** `SYT.NPKTools` replaces the six `NPKTools.*` packages, which receive no
further updates. The version resets to 1.0.0 because the package identity is new; in terms of code
this is a direct continuation, carrying every fix listed under *Inherited from the NPKTools line*
below.

The find-and-replace table for moving off the old packages is in
[README.md](README.md#migrating-from-npktools-1x).

### One package instead of six

`NPKTools.Core`, `NPKTools.Optimizer`, `NPKTools.Optimizer.Preset`, `NPKTools.Optimizer.PPMCalc` and
`NPKTools.Optimizer.PpmTargetParser` are now a single `SYT.NPKTools`, plus one optional add-on for the
DI one-liner (see below).

The split cost consumers something and bought them nothing. Five of the six projects had the same
single external dependency and came to 161 KB of IL together, so nobody was avoiding a meaningful
download by taking a subset. Meanwhile a first-time user had to know which three of six packages made
a working setup, and every release multiplied version-matrix combinations that were never tested
apart.

- **Namespaces are flattened** from 24 to 4: `SYT.NPKTools`, `.Fertilizers`, `.Nutrients`,
  `.Optimization`. The old tree had namespaces named after the type inside them
  (`NPKTools.Core.Domain.PpmTarget` containing `PpmTarget`), which forced awkward qualification.
- **`ThrowIf`, `ReportFormatter`, `Labels`, `Names` and `OptimizationSettings` are `internal`.** They
  were public only because they had to cross package boundaries. `ElementFieldBase` and the
  `*BuilderBase<T>` types stay public — the public value objects and builders derive from them.
- **`FindSolutions` returns `FertilizerSolutions`** instead of a
  `(Solutions Macro, Solutions Micro)` tuple, so the shape is named, documented and extensible.
  It carries `Empty` and `IsEmpty`.

### `SYT.NPKTools` has no dependencies

The four `AddNpkTools*()` extension methods are replaced by a `NpkTools` factory —
`CreateOptimizationService()`, `CreateOptimizer()`, `CreatePpmCalculator()`, `CreateTargetParser()`,
`CreateBundleRepository()` — and `Microsoft.Extensions.DependencyInjection.Abstractions` is no longer
a dependency of the library. It depends on nothing at all.

An `IServiceCollection` extension method requires that package, and it was the only thing in the
library that did. It was never necessary for the library to carry it: `IServiceCollection` belongs to
the consuming application, which already has it, so registration is one line per service against the
factory.

Two things improve as a side effect:

- **Substituting a solver is an explicit argument**, `CreateOptimizationService(mySolver)`, rather than
  "register yours before `AddNpkTools()` or `TryAdd` silently keeps the default". That ordering rule
  needed two tests to pin down and could fail quietly; an argument cannot.
- **Nothing in the package can conflict with a consumer's version graph**, because there is nothing in
  it to conflict.

For the record the removed dependency was cheap — 66 KB, no transitive dependencies of its own under
`net10.0`, and already present in the `Microsoft.AspNetCore.App` shared framework and in any Blazor
application. This change is about the library not imposing a hosting concern on callers who did not ask
for one, not about download size.

### A second, optional package for the one-liner

**New: `SYT.NPKTools.DependencyInjection`.** It contains one extension method:

```csharp
services.AddNpkTools();          // or AddNpkTools(mySolver)
```

This is the only legitimate reason to split a package — isolating a dependency — and it is why the
main package can be dependency-free while a one-line registration still exists. Take it if you want
the extension method; skip it and register the factory results yourself, losing nothing.

Notes on its design:

- It lives in the `Microsoft.Extensions.DependencyInjection` namespace, the convention for
  `IServiceCollection` extensions, so a typical `Program.cs` needs no extra `using`.
- The solver is a parameter, not a registration-order rule, so the footgun the old
  `AddNpkToolsOrToolsSolver()` mechanism had does not come back. `TryAdd` is used only to make calling
  the method twice idempotent.
- Registrations are factory delegates, so nothing is constructed for a service the application never
  resolves.

### Google OR-Tools is no longer shipped

`NPKTools.Optimizer.OrTools` is gone, and with it `AddNpkToolsOrToolsSolver()`. The GLOP solver now
lives at `tests/SYT.NPKTools.OrToolsOracle`, which is not published.

This is what makes the browser story real rather than conditional. OR-Tools distributes native
binaries only for linux-x64/arm64, osx-x64/arm64 and win-x64 — there is no `browser-wasm` build — so
while a shipped OR-Tools package existed, "runs in the browser" depended on which package you
installed. Now every published byte is managed code.

Nothing is lost in validation terms: the oracle is exactly how the managed solver's correctness is
established, and it is also the benchmark baseline, which is why it is its own project rather than a
file inside a test assembly. `IOptimizationProblemSolver` remains public, so a consumer who wants GLOP
at runtime passes their own implementation to `NpkTools.CreateOptimizationService(...)` — the oracle is
a complete worked example of doing that.

### Packaging

- Package ids `SYT.NPKTools` and `SYT.NPKTools.DependencyInjection`, titled `SYT NPKTools` and
  `SYT NPKTools Dependency Injection`, matching the other `SYT.*` packages.
- The licence ships as a file inside the package rather than as an SPDX expression, so the exact text
  is in what consumers downloaded.
- One README, packed from the repository root, so the NuGet page and the GitHub landing page cannot
  drift apart. Its images use absolute URLs because NuGet does not resolve relative paths.
- Test projects consolidated from six to three — `SYT.NPKTools.Tests`,
  `SYT.NPKTools.IntegrationTests`, `SYT.NPKTools.OrTools.Tests` — because the old split mirrored
  package boundaries that no longer exist. **418 tests**, unchanged in coverage.

---

## Inherited from the NPKTools line

Everything below shipped in the `NPKTools.*` packages and is present in `SYT.NPKTools 1.0.0-preview.2`.
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
- **An infinite coefficient produced a wrong answer reported as success.** `1 ≤ ∞·x ≤ 2` returned
  `{x = 0}` while GLOP correctly reported infeasible: the guard checked only for NaN, and verification
  could not catch the result either, because `∞ × 0` is NaN and every comparison against NaN is false,
  so the row looked satisfied. Non-finite coefficients — in constraints and in the objective — are now
  rejected with `ArgumentException`.
- **A contradictory infinite bound was accepted as feasible.** A *lower* bound of `+Infinity` means
  `Ax ≥ +Infinity`, which nothing satisfies, but it was treated as "unbounded on this side" and the row
  was silently dropped; the same for an upper bound of `-Infinity`. Both now throw. Only the
  outward-facing directions express a one-sided constraint.
- **Verification was vacuous for very small rows.** The allowance was
  `1e-6 × max(1, term magnitude)`, so a row bounded near `1e-7` was permitted a deviation larger than
  the entire constraint. It is now purely relative to each row's own scale, taken as the larger of the
  term magnitudes and the finite bounds. Re-checked for false negatives afterwards over 7,200
  differential problems spanning `1e-5` to `1e5`: none, and the one apparent disagreement turned out to
  be GLOP returning a point that violated a constraint by 6% relative.

### Added

- **A fully managed solver.** `SimplexOptimizationSolver` is a two-phase primal simplex using Bland's
  rule, with no native dependencies, and it is the default behind `IOptimizationProblemSolver`.

  The problem size justifies a dense tableau: a macronutrient bundle is at most 16 variables and 7
  range constraints, and a full preset search is 40 such problems. Equivalence with GLOP is asserted
  by 60 tests — 20 over the curated preset bundles across 11 target profiles, and 40 randomized
  synthetic catalogues (11 feasible, 29 infeasible). Differential testing over 17,760
  mapper-generated problems found no disagreement: worst constraint violation 1.4e-14, worst relative
  cost gap 5.8e-16.

  It is not a general-purpose LP solver, and the class documentation says so. Differential testing puts
  the safe band at roughly 1e-6 to 1e5 in coefficient magnitude. Outside it the solver can stop at a
  suboptimal vertex or report no solution where one exists — and note this does not require an
  ill-conditioned problem, because the pivoting tolerance is absolute, so a uniformly large but
  perfectly conditioned problem fails too. Every answer is verified against the original constraints to
  a relative 1e-6 before being returned.

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
