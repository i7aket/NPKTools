# API reference

**The XML documentation is the authority**, and it ships inside the package, so your IDE already knows more
about a signature than this page can. CS1591 is enforced in `src/`, which means a new public member without
a `<summary>` breaks the build — with carve-outs worth knowing about, because they are exactly the members
you will look up and not find. `.editorconfig` switches it off for `ValueObjects/`, `Builders/` and
`Internal/`, so every `FertilizerBuilder.Add…` method and every value object's `.Value` is undocumented by
design.

What this page gives you is the shape: which of the public types you actually touch, in what order, and
which of them you can substitute.

For working code see [examples](examples.md); every snippet there is a test. For why it is arranged this
way, [architecture](architecture.md).

## The entry point

`NpkTools` is a static factory in namespace `SYT.NPKTools`, and it is the whole entry point. Nothing else
needs constructing by hand.

| Method | Returns | Use it when |
|---|---|---|
| `CreateTargetParser()` | `IPpmTargetParser` | you have a target as a string |
| `CreateOptimizationService(IOptimizationProblemSolver? solver = null)` | `IFertilizerOptimizationService` | you want recipes from the curated catalogue |
| `CreateOptimizationService(IEnumerable<Fertilizer> salts, BundleGenerationSettings? settings = null, IOptimizationProblemSolver? solver = null)` | `IFertilizerOptimizationService` | you want recipes from your own shelf |
| `CreateOptimizer(IOptimizationProblemSolver? solver = null)` | `IFertilizerOptimizer` | you want one linear program over one list of salts, not a search |
| `CreatePpmCalculator()` | `IPpmCalculationService` | you have weights and want to know what is in the tank |
| `CreateBundleRepository()` | `IFertilizerBundleRepository` | you want the catalogue's own bundles |
| `CreateBundleRepository(IEnumerable<Fertilizer> salts, BundleGenerationSettings? settings = null)` | `CustomFertilizerBundleRepository` | you want bundles from your own shelf |

The `solver` parameter defaults to `SimplexOptimizationSolver`, which is fully managed and therefore works
everywhere .NET runs, WebAssembly included. That default is why the calculator is a static site.

## The interfaces

Seven, all public, spread over three namespaces — `SYT.NPKTools.Nutrients`, `SYT.NPKTools` and
`SYT.NPKTools.Optimization`.

```csharp
// SYT.NPKTools.Nutrients
PpmTarget Parse(string input);                                                    // IPpmTargetParser
Ppm CalculatePpm(IReadOnlyList<Fertilizer> collection, double waterLiters = 1);   // IPpmCalculationService

// SYT.NPKTools — the search
Solutions FindMacroSolutions(PpmTarget target, CancellationToken cancellationToken = default);   // IFertilizerOptimizationService
Solutions FindMicroSolutions(PpmTarget target, CancellationToken cancellationToken = default);
FertilizerSolutions FindSolutions(PpmTarget target, CancellationToken cancellationToken = default);
IReadOnlyList<IReadOnlyList<Fertilizer>> Macro();                                        // IFertilizerBundleRepository
IReadOnlyList<IReadOnlyList<Fertilizer>> Micro();

// SYT.NPKTools.Optimization — the seams
Solution? Optimize(PpmTarget target, IReadOnlyList<Fertilizer> sourceCollection, SolutionFinderSettings settings);   // IFertilizerOptimizer
OptimizationProblem CreateOptimizationProblem(PpmTarget target, IReadOnlyList<Fertilizer> sourceCollection, SolutionFinderSettings settings);   // IOptimizationProblemMapper
Solution? CreateSolution(Dictionary<string, double> solutionValues, IReadOnlyList<Fertilizer> originalSourceCollection, double waterLiters = 1);
Dictionary<string, double>? Solve(OptimizationProblem problem);                                            // IOptimizationProblemSolver
```

**Every `Find…` takes a `CancellationToken`, and you should pass one.** Each call runs a linear program per
bundle, so a large shelf takes a noticeable amount of time.

**Only one of the seven is substitutable through `NpkTools`:** `IOptimizationProblemSolver`, which every
factory method takes as an optional parameter. The mapper, the bundle repository and the optimizer are all
constructed internally with no overload to pass your own, so replacing any of those means building
`FertilizerOptimizationService` or `FertilizerOptimizationAdapter` yourself. That is a deliberate
narrowness — the solver is the seam anyone actually needs — but it is worth knowing before you plan around
an interface you cannot inject.

`IOptimizationProblemSolver` is the seam that matters. The managed simplex is the default; the repository's
differential tests substitute OR-Tools and check that the two agree, which is how the managed one earns
being trusted.

## What you get back

**`Solutions`** is `IReadOnlyList<Solution>` with a `Solutions.Empty`. Several recipes are returned because
several reach the target; they differ in which salts they use. Nothing is "best" — that depends on what is
on your shelf and what you would rather spend.

**`Solution`** is `IReadOnlyList<Fertilizer>` plus `WaterLiters`. Each `Fertilizer` in it carries the
`Weight` to measure out.

**`FertilizerSolutions(Solutions Macro, Solutions Micro)`** — what `FindSolutions` returns, with
`IsEmpty` and `FertilizerSolutions.Empty`.

**`Ppm`** — what is actually in the tank, one property per element (`Nitrogen`, `Phosphorus`, … `Sodium`),
each a value object with a `.Value`, plus `Liters` and `Report()`. Six extension methods turn it into the
other things a grower reads:

```csharp
// PpmExtensions
ConductivityEstimate EstimateConductivity(this Ppm ppm, double bicarbonateMeqPerLitre = 0);
MolarProfile AsMillimolar(this Ppm ppm);        // mM, how a feed chart is written
NutrientRatios Ratios(this Ppm ppm);           // NO₃:NH₄, K:Ca, Ca:Mg …
IonBalance IonBalance(this Ppm ppm);           // the same solution as charges, in meq/L
Ppm Plus(this Ppm ppm, Ppm other);             // what the water and the salts come to together
string Breakdown(this Ppm ppm);                // one line per element, for a log
```

The bicarbonate parameter on the first is not decoration: a water's bicarbonate conducts, so leaving it at
zero understates the EC of anything mixed into real water.

**`ConductivityEstimate`** — `MilliSiemensPerCm`, `MicroSiemensPerCm`, `IdealMicroSiemensPerCm`,
`AsTdsPpm(double scale = 500)`, a property per ion's contribution, `IonicStrength`, `Correction`,
`ValidatedIonicStrength` and **`IsWithinValidatedRange`**. Check the last one: the model is validated
against certified KCl standards up to the ionic strength of a normal feed, and a concentrate is an order of
magnitude past it.

**`NutrientRatios`** — `NitrateToAmmonium`, `NitrogenToPotassium`, `PotassiumToCalcium`,
`CalciumToMagnesium`, `PotassiumToMagnesium`, `NitrogenToSulfur`, `NitrogenToPhosphorus`, `TotalPpm`. Each
ratio is `double?`, null when the denominator is zero — a null is not a zero, and displaying it as one
would be a lie about the mix.

**`IonBalance`** — the same solution as charges in meq/L per ion, plus `Total`, `AcidEquivalents`,
`RelativeDifference` and `IsChargeNeutral`. This is what makes a charge imbalance visible, and
`AcidEquivalents` is how the app reports what the acid contributed.

## The target and the water

**`PpmTarget`** — one property per element named by symbol (`N`, `P`, `K`, `Ca`, `Mg`, `S`, `Fe`, `Cu`,
`Mn`, `Zn`, `B`, `Mo`, `Cl`, `Si`, `Se`, `Na`) plus `Liters`. `IPpmTargetParser.Parse` reads
`"N=150 P=50 K=210 L=100"`; `L` is the reservoir volume.

**`WaterProfile`** — built with `WaterProfileBuilder`, and the piece other calculators do not have:

```csharp
WaterAdjustedTarget AdjustFor(this PpmTarget target, WaterProfile water);
double EstimatedAlkalinity(this WaterProfile water);     // meq/L, from the cation surplus
double GeneralHardness(this WaterProfile water);          // °dH
double CarbonateHardness(this WaterProfile water);        // °dH
ConductivityEstimate EstimateConductivity(this WaterProfile water);
Ppm AsPpm(this WaterProfile water);                        // the water as a Ppm, to add to a recipe
```

**`WaterAdjustedTarget(PpmTarget Target, IReadOnlyList<NutrientExcess> Excesses)`**, with `HasExcesses`.
Pass `Target` to the optimizer. Read `Excesses` — each is
`NutrientExcess(string Element, double InWater, double Target)` with `Overshoot` — because fertilizer only
adds, and an element the water oversupplies is a thing no recipe can fix.

**`WaterPreset`** — four shapes of real water (`SoftLowAlkalinity`, `CalciumBicarbonateModerate`,
`CalciumBicarbonateHard`, `SodiumExchangeSoftened`, and `All`), each with `Id`, `Label`, its per-element
proportions, `NominalMicroSiemensPerCm` and `ToProfile(scale)`.
**`WaterEstimator.Estimate(WaterPreset preset, double microSiemensPerCm, double? generalHardness = null,
double? carbonateHardness = null)`** scales a preset until its computed conductivity matches a meter
reading. It returns a `WaterEstimate` — `Profile`, `MicroSiemensPerCm`, `RequestedMicroSiemensPerCm`,
`RelativeError`, `Feasible` — which you read rather than construct; its constructor is internal.

`Feasible` is false in either direction: when a hardness already accounts for more conductivity than the
meter saw, and when the reading is higher than the preset reaches at full scale. Both mean the readings do
not describe one water, which is worth saying rather than returning arithmetic.

## Salts

**`Fertilizer`** is built with `FertilizerBuilder`: `AddName`, `AddType`, then one `Add…` per nutrient
form — `AddNo3`, `AddNh4`, `AddNh2`, `AddP`, `AddK`, `AddCaNonChelated`, `AddCaEdta`, `AddMgNonChelated`,
`AddS`, the micronutrients and their chelates — then `AddWeight` and `Build()`. The chelated and
non-chelated forms are separate members on purpose: a chelate behaves differently and the library will not
guess which you meant.

**`ChemicalFormula`** derives the percentages instead of trusting typed ones:

```csharp
static bool TryParse(string? text, out ChemicalFormula? formula, out FormulaProblem? problem);
double MolarMass { get; }
double PercentOf(string symbol);
double NitratePercent, AmmoniumPercent, AmidePercent { get; }
IReadOnlyDictionary<string, int> Atoms { get; }
```

Accepts plain and Unicode subscripts, bracketed groups, and hydrates joined by `*`, `·`, `×` or `.`.
**`FormulaProblem(FormulaProblemKind Kind, string Message, string? Value, int? Position)`** names the
failure rather than only describing it, so a caller can write its own sentence in its own language.

**`FormulaComposition`** — `TryCreate(name, formula, type, out Fertilizer?, out FormulaProblem?)`,
`SuggestTank(formula)`, and `LooksChelated(formula)`. The last one matters: iron EDTA parses fine and
yields 7.6% nitrogen that is holding the metal rather than feeding the plant, and a formula cannot tell the
two apart.

## Acid

```csharp
static AcidPlan Calculate(
    double alkalinityMilliequivalentsPerLitre, double waterPh, double targetPh, Acid acid, double litres);
static double BicarbonateFraction(double ph);
```

`Acid.All` holds six — nitric, phosphoric and sulfuric at two strengths each — each with `Id`, `Label`,
`Kind` (an `AcidKind`), `PercentByWeight`, `DensityGramsPerMillilitre`, `EquivalentWeight`,
`EquivalentsPerLitre`, `NutrientSymbol` and `MilligramsOfNutrientPerMilliequivalent`. **`AcidPlan`** gives `MilliequivalentsPerLitre`,
`Millilitres`, `NutrientSymbol` and `NutrientPpm`.

The dose is less than the alkalinity, and that is the point: worked from the carbonate equilibrium, at pH
5.8 about three quarters needs neutralising, where the rule of thumb says all of it. Subtract
`NutrientPpm` from the target as you would the water — an acid's nitrogen is in the reservoir exactly as
the water's is.

## Concentrates

```csharp
static ConcentratePlan AsConcentrate(this Solution solution, double concentrateLiters, SolubilityTable? solubility = null);
```

**`ConcentratePlan`** — `TankA`, `TankB`, `Tanks`, `ConcentrateLiters`, `WorkingLiters`,
`DilutionRatio`, `MillilitresPerLiter`, `MaxDilutionRatio`, `Warnings`, `HasWarnings`,
`HasPrecipitationRisk`, `ExceedsSolubility`, and `UnknownSolubility`.

That last one is the honest part: a salt with no published figure is named there rather than assumed to be
fine, so `MaxDilutionRatio` excludes it and the real ceiling may be lower than it looks.

**`ConcentrateTank(ConcentrateType Tank, IReadOnlyList<ConcentrateComponent> Components)`** —
`TotalGrams`, `TotalGramsPerLiter`, `SaturationFraction`, `IsSaturated`, `IsEmpty`. Each
**`ConcentrateComponent`** carries its `Fertilizer`, `Grams`, `GramsPerLiter`, `SolubilityLimit` and
`ExceedsSolubility`.

**`ConcentrateWarning(ConcentrateWarningKind Kind, ConcentrateType Tank, IReadOnlyList<string> Fertilizers, string Message, double? Actual, double? Allowed)`**.
Four kinds: `PrecipitationRisk`, `SolubilityExceeded`, `TankSaturated`, `TankInferred`. `Message` is prose
for a log; `Actual` and `Allowed` carry the two figures so an application can write the sentence in the
reader's language.

## Grouping and settings

**`ElementGroups`** — `Macro` (six), `Micro` (eight), `CounterIons` (Cl and Na), `All` (sixteen). Use these
rather than writing the lists out; they are what the app's grids iterate.

**`SolutionFinderSettings`** controls the search, and one field explains most surprises: `RangeFactor`
defaults to **1, which is the tightest it goes** — every element the target names becomes an exact
equality, and a value above 1 is rejected. So a short shelf that "cannot" solve is usually
over-constrained rather than short of an element, and the way to loosen it is to *lower* the factor.

One nuance worth having: an element the target does not mention at all is unconstrained, but an element
you explicitly set to zero is pinned to zero. `Ca=0` is a requirement, not a shrug.
`BundleGenerationSettings.MaxBundles` caps how many subsets of your shelf are tried.

## What is not API

`SYT.NPKTools.Internal` is not public API whatever its accessibility says. And **folder names are not
namespaces here**: the files under `Presets/` and `Optimization/Contracts/` declare `namespace
SYT.NPKTools` and `namespace SYT.NPKTools.Optimization`, so a `<see cref="..."/>` written from the folder
path will not compile.
