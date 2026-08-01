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

## Account for your source water

Tap and well water is rarely blank, and whatever it carries is added on top of everything the
fertilizers contribute. Deduct it before optimizing:

```csharp
WaterProfile tap = new WaterProfileBuilder()
    .AddCa(45).AddMg(15).AddS(20).AddNa(25).AddCl(30).AddNitrate(6)
    .Build();

WaterAdjustedTarget adjusted = target.AdjustFor(tap);
Solutions solutions = optimizer.FindMacroSolutions(adjusted.Target);
```

This is not a refinement. On the water above, a mix calculated against the raw target overshoots
**calcium by 45%**, magnesium by 30% and sulfur by 33% — because the water supplies that much again on
top. Deducting first lands every element exactly on target:

| Element | Target | From mix | From water | Total | Deviation |
| --- | --- | --- | --- | --- | --- |
| Ca | 100 | 55 | 45 | 100 | 0% |
| Mg | 50 | 35 | 15 | 50 | 0% |
| S | 60 | 40 | 20 | 60 | 0% |

Use `WaterProfile.Pure` for reverse osmosis, distilled or rain water; it leaves the target untouched.

If the water already supplies more of something than you asked for, that element is reported rather
than silently truncated — fertilizer only adds, so no mix can bring it down:

```csharp
foreach (NutrientExcess excess in adjusted.Excesses)
{
    Console.WriteLine($"{excess.Element}: water has {excess.InWater}, target {excess.Target}");
}
```

The remedies are outside the calculation — raise the target, or dilute the source water — which is why
this surfaces instead of being hidden. `adjusted.Target` is an ordinary `PpmTarget`, so nothing
downstream needs to know water was involved.

## Judge the mix, not just compute it

Absolute ppm figures say how strong a solution is; the ratios say what it will do.

```csharp
NutrientRatios ratios = calculator.CalculatePpm(mix, mix.WaterLiters).Ratios();

Console.WriteLine($"NO3:NH4 {ratios.NitrateToAmmonium}");   // pH drift at the root
Console.WriteLine($"K:Ca    {ratios.PotassiumToCalcium}");  // these compete for uptake
Console.WriteLine($"Ca:Mg   {ratios.CalciumToMagnesium}");
```

`NitrateToAmmonium` is the one to watch most closely, because it predicts pH movement rather than
nutrition: ammonium uptake acidifies the root zone, nitrate uptake alkalizes it. A ratio whose
denominator is zero is `null` rather than zero or infinity — a nitrate-only mix is the everyday case,
and reporting `0` or `∞` for it would mislead.

To see which salt supplied what:

```csharp
foreach (FertilizerContribution part in mix.Breakdown(calculator))
{
    Console.WriteLine($"{part.Fertilizer.Name.Value}: S {part.Contribution.Sulfur.Value:F1} ppm");
}
```

This answers what a bare recipe cannot — which salt is responsible for the sulfur you did not ask for:

```
salt                                     g      N      P      K     Ca     Mg      S
Calcium Nitrate Tetrahydrate          58.9   69.9    0.0    0.0  100.0    0.0    0.0
Potassium Nitrate                     35.4   49.0    0.0  136.9    0.0    0.0    0.0
Magnesium Sulfate Heptahydrate        23.4    0.0    0.0    0.0    0.0   23.0   30.4
Potassium Dihydrogen Phosphate        22.0    0.0   50.0   63.1    0.0    0.0    0.0
Magnesium Nitrate Hexahydrate         28.4   31.1    0.0    0.0    0.0   27.0    0.0
```

Every salt brings a counter-ion along with the nutrient you wanted, so an unrequested element is rarely
a mistake — it is the price of the element next to it. Contributions are measured through the same
calculator as the whole mix, so the parts always sum to the total.

## mM and meq/L, and what the charge balance tells you

Ppm is a mass unit, so it flatters the light elements. 40 ppm of calcium and 40 ppm of magnesium look
alike and are not: 1.00 mM against 1.65 mM, two-thirds again as many magnesium ions. Every published
formulation — Steiner, Hoagland, the Dutch advisory tables — is stated in mM for that reason, so working
from a paper recipe means converting.

```csharp
MolarProfile mM = ppm.AsMillimolar();     // all 16 elements, plus the three nitrogen forms
IonBalance ions = ppm.IonBalance();       // charge, in milliequivalents per litre
```

```
element    ppm      mM     meq/L
NO3-N    150.0   10.71   10.71
P         50.0    1.61    1.61
K        210.0    5.37    5.37
Ca       160.0    3.99    7.98
Mg        50.0    2.06    4.11
S         65.0    2.03    4.05

cations 17.47  anions 16.38  total 33.85 meq/L
```

**The gap between the two sides is not an error.** Every salt is electrically neutral, so a recipe of
nothing but salts balances exactly — and where it does not, the gap is the acid or base the recipe itself
contributes. `AcidEquivalents` is that number, in meq/L of H⁺:

| salt | AcidEquivalents | why |
| --- | --- | --- |
| monopotassium phosphate | 0.00 | K⁺ against H₂PO₄⁻, one charge each |
| phosphoric acid | **+10.20** | supplies H₂PO₄⁻ with a proton, not a metal |
| dipotassium phosphate | **−5.74** | two K⁺ against HPO₄²⁻, which takes up a proton at working pH |

Positive means the recipe pulls pH down by itself, so less pH-down is needed than the water's alkalinity
alone suggests. Negative means it pushes pH up. Across the built-in catalogue, fourteen of the seventeen
macro salts come out at exactly zero; the three that do not are precisely the three with acid-base
character.

Micronutrients are left out of the charge figures on purpose. Iron may be Fe²⁺ or Fe³⁺ and in chelated
form the complex is an anion rather than a cation, so a charge for it would be a guess — and at under
5 ppm in total they would move the balance by less than 0.1 meq/L against a typical 25–30. Boron and
silicon are excluded for a firmer reason: as boric and silicic acid they are undissociated at
nutrient-solution pH and carry no charge at all. Urea likewise contributes nitrogen in mM and nothing in
meq, because it is a neutral molecule.

Phosphorus is counted as H₂PO₄⁻ at one charge, the dominant species between pH 5.5 and 6.5 where these
solutions are run. Above pH 7.2 half of it is HPO₄²⁻. This library does not model pH, so the assumption
is stated rather than computed — and it is what makes the acid-base figures above come out in whole
protons.

## Use the salts you already have

The preset catalogue offers eighteen macro bundles and therefore a dozen recipes to compare. Someone
working from their own shelf used to get one, because one list of salts is one bundle. Now the bundles
are generated:

```csharp
Fertilizer[] shelf = [calciumNitrate, potassiumNitrate, magnesiumSulfate, mkp, sop, mag];

IFertilizerOptimizationService service = NpkTools.CreateOptimizationService(shelf);
Solutions recipes = service.FindMacroSolutions(target);   // several, not one
```

The first bundle holds everything on the shelf; each of the others leaves out one salt. That is the
whole rule, and it is the one that measured best — alternatives can only come from taking something
away, because handing the optimizer more salts than it needs yields the same best mix again. Removing
one forces the linear program to route around it, which is both a different recipe and the question a
grower actually asks: *what does this look like without the MKP?*

Measured against the hand-written catalogue on three macro targets, counting distinct recipes returned:

| bundle strategy | distinct recipes |
| --- | --- |
| one source per element, cross product | 5 |
| hand-written catalogue (18 bundles) | 19 |
| **hold one out** | **21** |
| hold out pairs and triples as well | 21 |

Holding out pairs adds nothing, because a smaller subset either reproduces a recipe already found or
solves nothing at all. Building bundles up from per-element choices does badly for a reason worth
knowing: six simultaneous element targets need at least six salts to satisfy at non-negative weights, and
a bundle assembled as one-source-per-element rarely has that many once the overlaps collapse.

To see what generation could not do, ask the repository rather than the service:

```csharp
CustomFertilizerBundleRepository bundles = NpkTools.CreateBundleRepository(shelf);

if (!bundles.MacroGeneration.IsComplete)
{
    // ["Mg"] — no salt on the shelf supplies magnesium, so no bundle can meet a magnesium target
    Console.WriteLine(string.Join(", ", bundles.MacroGeneration.UncoveredElements));
}

// Salts that carry nothing any target can ask for — table salt is the honest example
Console.WriteLine(string.Join(", ", bundles.UnusableSalts));
```

This matters more than it looks. Without it a missing magnesium source shows up as "no solutions", which
sends someone hunting through their shelf for a mistake that is not there.

Macro and micro are split the way the preset catalogue splits them: carrying any micronutrient makes a
salt a micro salt, even when it also carries a macro element. Iron sulfate's sulfur is incidental —
dosing it to meet a sulfur target would mean iron at a hundred times the intended rate.

Labelling a recipe is a set difference against the first bundle:

```csharp
string omitted = bundles.Macro()[0].Except(chosenBundle).Single().Name.Value;   // "without the SOP"
```

## Mix once a month, not every watering

Weighing six salts every time you water is what stops people using a calculated recipe. A concentrate
solves that, and the two tanks exist for a chemical reason: calcium sulfate and the higher calcium
phosphates are barely soluble, so what stays dissolved at working strength falls out of solution at
100×.

```csharp
ConcentratePlan plan = mix.AsConcentrate(concentrateLiters: 1);

Console.WriteLine($"1:{plan.DilutionRatio:F0}");          // 1:100
Console.WriteLine($"{plan.MillilitresPerLiter:F0} ml");   // 10 ml of each tank per liter

foreach (ConcentrateComponent c in plan.TankA.Components)
{
    Console.WriteLine($"{c.Fertilizer.Name.Value}: {c.Grams:F1} g ({c.GramsPerLiter:F1} g/L)");
}
```

The whole recipe's salt goes into the smaller volume, so a 100-liter recipe concentrated into 1 liter is
1:100. Concentrating changes how much water the salt goes into, never how much salt is needed — the
weights are the working recipe's weights, which is why the finished solution still lands on target:

```
=== working mix, 100 L ===
  Calcium Nitrate Tetrahydrate                  67.76 g  tank A
  Potassium Nitrate                             37.98 g  tank A
  Magnesium Sulfate Heptahydrate (MGS)          36.13 g  tank B
  Ammonium Nitrate                               4.08 g  tank A
  Potassium Dihydrogen Phosphate (MKP)          21.97 g  tank B
  Magnesium Nitrate Hexahydrate (MAG)            2.50 g  tank A

=== concentrate, 1 L per tank -> 1:100 ===
dose: 10.0 ml of A + 10.0 ml of B per liter

TANK A  (112.3 g/L total)
TANK B  (58.1 g/L total)
```

Each salt's tank comes from its own `ConcentrateType`, which the preset catalogue already carries. A
custom salt has none unless you set one, so a tank is inferred from its composition and the inference is
reported in `Warnings` — guessing silently is how someone's own sulfate ends up next to calcium.

`GramsPerLiter` is the figure to check against a salt's solubility. A recipe that dissolves happily at
working strength can be impossible at 100×, and that limit binds per salt long before the tank total
does.

**On the precipitation check.** `HasPrecipitationRisk` flags calcium meeting sulfate or phosphate in one
tank *from different salts*. It is a rule of thumb, not a solubility calculation. A single salt carrying
both internally is not flagged — monocalcium phosphate is a soluble compound, not two reagents that
happen to be adjacent, and a check that fired on it would teach you to ignore the check. Predicting
actual precipitation needs solubility products, pH and temperature, none of which this library models.

### Will it actually dissolve?

The other way a concentrate fails is simpler: there is more salt than the water can hold.

```csharp
ConcentratePlan plan = mix.AsConcentrate(concentrateLiters: 1);

if (plan.ExceedsSolubility)
{
    Console.WriteLine($"too strong — go no further than 1:{plan.MaxDilutionRatio:F0}");
}
```

Two checks run, and they catch different mistakes:

- **One salt past its own limit.** Certain — arithmetic against a published figure. Monocalcium phosphate
  at 18 g/L is the one that bites first in practice, against potassium nitrate's 316 and calcium nitrate's
  1290.
- **The tank saturated as a whole.** `SaturationFraction` adds each salt's share of its own limit; above 1
  the tank cannot dissolve. Without this, four salts each at 40% of their limit pass individually and fail
  in the bucket, because they compete for the same water. It is a first-order screen — exact for a single
  salt, slightly optimistic for a mixture, since salts sharing an ion crowd each other out more than the
  sum suggests.

`MaxDilutionRatio` is the answer to "then how strong *can* I make it": a 1:1000 concentrate that cannot
dissolve may be fine at 1:600.

**A salt with no published figure is reported, not assumed.** `SolubilityTable.Default` covers 21 of the
catalogue's 34 salts. The rest — calcium chloride hexahydrate, urea phosphate, the EDTA chelates, sodium
silicate and selenate, the nitrate micro salts — carry no entry, because the figures in circulation for
them disagree by more than the check would be worth. A chelate's solubility depends on the formulation; a
deliquescent hydrate's on which hydrate it actually is. Guessing would be worse than declining: a wrong
limit either blocks a tank that would have mixed or passes one that will not.

```csharp
foreach (string salt in plan.UnknownSolubility)
{
    Console.WriteLine($"{salt}: no figure, nothing checked");
}
```

For a salt of your own, take the figure off the bag:

```csharp
SolubilityTable table = SolubilityTable.Default.With("My Own Salt", gramsPerLitreAt20C: 320);
ConcentratePlan plan = mix.AsConcentrate(concentrateLiters: 1, table);
```

The figures are for 20 °C in pure water. Solubility rises steeply with temperature, so a cold garage is
the pessimistic case and a warm room the forgiving one.

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
| `SYT.NPKTools.Nutrients` | `Ppm`, `PpmTarget`, `WaterProfile`, `NutrientRatios`, `MolarProfile`, `IonBalance`, the ppm calculator and the target parser |
| `SYT.NPKTools.Concentrates` | `ConcentratePlan` and the A/B tank split |
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
solutions landing exactly on an `N=150 P=50 K=200 Ca=100 Mg=50 S=60` target.

The library's own contribution to a download is **128 KB, or about 46 KB gzipped**. Everything else is
the .NET runtime: a minimal WebAssembly app came to roughly 1.1 MB gzipped in total, and a full Blazor
app with UI assets is naturally larger than that.

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

**It is not a general-purpose LP solver.** There is no scaling, equilibration or iterative refinement,
and the pivoting tolerance is absolute. Differential testing puts the safe band at roughly **1e-6 to
1e5** in coefficient and right-hand-side magnitude, where agreement with GLOP is exact; this library's
own problems — nutrient percentages and ppm/10 right-hand sides — sit comfortably inside it.

Outside that band it can stop at a suboptimal vertex or report no solution where one exists. This does
*not* require an ill-conditioned problem: because the tolerance is absolute, a perfectly conditioned
problem that is merely uniformly large fails too — at a scale of 1e9 a genuine improving direction can
have a reduced cost below the tolerance and be mistaken for optimality. Every answer is verified against
the original constraints before being returned, to a relative tolerance of 1e-6, so a returned mix
satisfies its constraints to about six significant digits; the failure mode is a missed or suboptimal
solution rather than a badly violated one.

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

418 tests run on Linux, Windows and macOS in CI, with coverage collected via coverlet and formatting
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

Authentication to NuGet.org uses **trusted publishing** rather than a stored API key: the job requests
an OIDC token from GitHub, NuGet.org validates it against a policy and issues a key that expires in an
hour. There is no long-lived secret in this repository to leak or rotate. The policy must match the
workflow exactly — repository owner, repository name, workflow file `release.yml`, and environment
`nuget` — so renaming this file or changing that environment breaks publishing until the policy is
updated to match.

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
