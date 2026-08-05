# Worked examples

**Every example on this page is a test.** They live in
[`tests/SYT.NPKTools.IntegrationTests/DocumentedExamplesTests.cs`](../tests/SYT.NPKTools.IntegrationTests/DocumentedExamplesTests.cs)
and run in CI, so if an API moves the build fails rather than the reader. Copying from here is safe in a
way that copying from prose is not.

```bash
dotnet test tests/SYT.NPKTools.IntegrationTests --filter "FullyQualifiedName~DocumentedExamples"
```

## The shortest thing that works

```csharp
IPpmTargetParser parser = NpkTools.CreateTargetParser();
IFertilizerOptimizationService optimizer = NpkTools.CreateOptimizationService();

PpmTarget target = parser.Parse("N=150 P=50 K=210 Ca=160 Mg=50 S=65 L=100");
Solutions recipes = optimizer.FindMacroSolutions(target);

Solution recipe = recipes[0];          // several are returned; they differ in which salts they use
double liters = recipe.WaterLiters;     // 100
foreach (Fertilizer salt in recipe)
{
    Console.WriteLine($"{salt.Name.Value}: {salt.Weight.Value:F2} g");
}
```

`L=100` in the target string is the reservoir volume in litres. `Solutions` is a list — every entry hits
the target, and they differ in which salts they use, so the choice between them is about what you have
and what you would rather spend.

## What the water already supplies comes off the target

This is the step most calculators skip, and it is the difference between a recipe and a guess.

```csharp
PpmTarget target = NpkTools.CreateTargetParser().Parse("N=150 K=210 Ca=160 Mg=50 S=65 L=100");
WaterProfile water = new WaterProfileBuilder().AddCa(60).AddMg(12).Build();

WaterAdjustedTarget adjusted = target.AdjustFor(water);

adjusted.Target.Ca.Value;   // 100 — the water brought 60 of the 160
adjusted.Target.Mg.Value;   // 38
adjusted.Excesses;          // empty
```

Pass `adjusted.Target` to the optimizer, not the original. On hard water the difference is the whole
point: ignore it and you overdose calcium by exactly what the water carried.

## Water that oversupplies is reported, not ignored

```csharp
PpmTarget target = NpkTools.CreateTargetParser().Parse("N=150 K=210 Ca=100 L=100");
WaterProfile hardWater = new WaterProfileBuilder().AddCa(160).Build();

WaterAdjustedTarget adjusted = target.AdjustFor(hardWater);

NutrientExcess excess = adjusted.Excesses[0];
excess.Element;     // "Ca"
excess.InWater;     // 160
excess.Target;      // 100
excess.Overshoot;   // 60
```

**Fertilizer only adds.** There is no salt that removes calcium from water, so once the water carries
more than the target no recipe can reach it. Saying which element and by how much is the only useful
answer; silently clamping to zero would produce a recipe that looks fine and is not.

## Your own shelf instead of the catalogue

```csharp
Fertilizer[] shelf =
[
    new FertilizerBuilder().AddName("Calcium Nitrate Tetrahydrate").AddType(ConcentrateType.A)
        .AddNo3(11.86).AddCaNonChelated(16.97).Build(),
    new FertilizerBuilder().AddName("Potassium Nitrate").AddType(ConcentrateType.A)
        .AddNo3(13.85).AddK(38.67).Build(),
    new FertilizerBuilder().AddName("Magnesium Sulfate Heptahydrate").AddType(ConcentrateType.B)
        .AddMgNonChelated(9.86).AddS(13.01).Build(),
    new FertilizerBuilder().AddName("Monopotassium Phosphate").AddType(ConcentrateType.B)
        .AddP(22.76).AddK(28.73).Build(),
    new FertilizerBuilder().AddName("Potassium Sulfate").AddType(ConcentrateType.B)
        .AddK(44.87).AddS(18.4).Build(),
    new FertilizerBuilder().AddName("Ammonium Nitrate").AddType(ConcentrateType.A)
        .AddNo3(17.5).AddNh4(17.5).Build(),
];

IFertilizerOptimizationService optimizer = NpkTools.CreateOptimizationService(shelf);
Solutions recipes = optimizer.FindMacroSolutions(target);
```

**Six salts, and that is not an accident.** The first four — calcium nitrate, potassium nitrate,
magnesium sulfate and MKP — cannot hit this six-element target at all, and writing this example is how
that was found out. Adding potassium sulfate as the fifth gives one recipe; ammonium nitrate as the sixth
gives two. By default every element the target names is solved as an *exact* equality, so six numbers need
about six independent knobs, and four salts whose elements are pairwise tied leave the problem
over-constrained rather than short of an ingredient.

If your own shelf "cannot" solve, that is usually why. Two ways out: add a salt that supplies one element
more independently — potassium sulfate is the classic, because it frees potassium from nitrogen — or
**lower** `RangeFactor` in `SolutionFinderSettings` below its default of 1, which turns the equalities
into ranges. Note the direction: 1 is the tightest it goes, and a value above 1 is rejected.

`AddType` decides which concentrate tank a salt belongs in — `A` for calcium, `B` for sulfates and
phosphates. It matters only if you make a concentrate, and then it matters a lot.

## A salt from its chemical formula

The safe way to add a salt, because the percentages are derived instead of typed.

```csharp
if (ChemicalFormula.TryParse("KNO3", out ChemicalFormula? formula, out FormulaProblem? problem))
{
    formula!.MolarMass;              // 101.10
    formula.PercentOf("K");          // 38.67
    formula.NitratePercent;          // 13.85
}

FormulaComposition.TryCreate("Shop KNO3", "KNO3", ConcentrateType.A, out Fertilizer? salt, out _);
```

Hydrates are written with `*`, `·`, `×` or `.` — `Ca(NO3)2*4H2O` — and both plain and subscript digits are
accepted, because the catalogue mixes them within a single formula.

**Why this beats typing percentages:** a bag quotes P₂O₅ and K₂O, and in the EU, Spain, Poland and
Turkey that is required by law rather than being a convention. A figure copied straight off the label
overstates phosphorus by 2.29× — and it is not only P and K, since the same rules quote calcium and
magnesium as CaO and MgO. A formula cannot make that mistake. See
[the FAQ](faq.md#my-bag-says-46-ko-what-do-i-type) for the divisors when you have no formula.

**Do not describe a chelate by formula.** Iron EDTA parses fine and yields 7.6% nitrogen — nitrogen that
is holding the iron, not feeding the plant. `FormulaComposition.LooksChelated` exists to warn about
exactly this; describe a chelate by percentages, where the agent is named.

## A formula that cannot be read says which failure it is

```csharp
ChemicalFormula.TryParse("KNO3!", out _, out FormulaProblem? problem);   // false

problem!.Kind;       // FormulaProblemKind.UnexpectedCharacter
problem.Value;       // "!"
problem.Position;    // 5
```

`Message` carries English prose for a log. `Kind`, `Value` and `Position` are there so an application can
write its own sentence, in its own language — which is what the browser app does, because this message
reaches somebody who has just made a typo.

## Judging the mix, not just computing it

```csharp
Ppm inTank = NpkTools.CreatePpmCalculator().CalculatePpm(recipe, recipe.WaterLiters);
inTank.Nitrogen.Value;                       // 150, within 1%

ConductivityEstimate ec = inTank.EstimateConductivity();
ec.MilliSiemensPerCm;                        // what a meter should read
ec.IsWithinValidatedRange;                   // true for a feed, false for a concentrate
ec.AsTdsPpm();                               // conductivity × 0.5, the "ppm 500" convention

MolarProfile molar = inTank.AsMillimolar();   // mM, which is how a feed chart is written
NutrientRatios ratios = inTank.Ratios();      // NO₃:NH₄, K:Ca, Ca:Mg …
```

`IsWithinValidatedRange` is worth checking rather than ignoring. The EC model is validated against
certified KCl standards up to the ionic strength of a normal feed; a concentrate at 1:100 is an order of
magnitude past that, and outside the range the figure is an ordering rather than a measurement.

## How much acid a water needs

```csharp
AcidPlan plan = AcidDose.Calculate(
    alkalinityMilliequivalentsPerLitre: 3,
    waterPh: 7.6,
    targetPh: 5.8,
    acid: Acid.Nitric60,
    litres: 100);

plan.MilliequivalentsPerLitre;   // ~2.3 — less than the alkalinity, and that is correct
plan.Millilitres;                // what to measure out
plan.NutrientSymbol;             // "N" — nitric acid brings nitrogen with it
plan.NutrientPpm;                // and that nitrogen is in the reservoir
```

Worked from the carbonate equilibrium rather than a rule of thumb. Neutralising the *whole* alkalinity
would take the water past the target pH; the usual rule overstates the dose by about a quarter.

Subtract `NutrientPpm` from the target the same way you subtract the water, or the tank will carry more
of that element than you asked for. `Acid.All` lists the six built in — nitric, phosphoric and sulfuric
at two strengths each — and the useful trick is that the same neutralisation delivered by a different
acid lands on a different element, so when one overshoots another usually fits.

## Storing it as an A/B concentrate

```csharp
ConcentratePlan plan = recipe.AsConcentrate(concentrateLiters: 2);

plan.DilutionRatio;        // 50 — a 100 L recipe in 2 L
plan.MillilitresPerLiter;  // 20 mL of each tank per litre of water
plan.TankA.Components;     // calcium here
plan.TankB.Components;     // sulfates and phosphates here
plan.MaxDilutionRatio;     // how far solubility allows you to go
plan.Warnings;             // and whether this one will actually dissolve
```

Calcium is never stored beside sulfate or phosphate: at working strength they stay in solution, at
concentrate strength they make gypsum and a phosphate precipitate. A single salt containing both
internally — monocalcium phosphate — is deliberately *not* flagged, because it is a soluble compound
rather than two reagents that happen to be adjacent.

## A concentrate that will not dissolve says so, with numbers

```csharp
// 20 g of a salt that dissolves to 18 g/L, in half a litre.
Fertilizer phosphate = new FertilizerBuilder()
    .AddName("Calcium Monobasic Phosphate").AddType(ConcentrateType.B)
    .AddP(23.5).AddCaNonChelated(15.9)
    .AddWeight(20)
    .Build();

ConcentratePlan tight = new Solution([phosphate], waterLiters: 100).AsConcentrate(concentrateLiters: 0.5);

ConcentrateWarning warning = tight.Warnings
    .First(w => w.Kind == ConcentrateWarningKind.SolubilityExceeded);

warning.Tank;        // ConcentrateType.B
warning.Actual;      // 40 — g/L this salt would need
warning.Allowed;     // 18 — g/L it dissolves to at 20 °C
warning.Fertilizers; // which salt
```

The ordinary recipe above raises no warnings at all, which is why this example builds its own solution
rather than reusing it — `Warnings.First(...)` on a plan that is fine throws.

`Message` is prose for a log; `Actual` and `Allowed` are there so an application can write the sentence
itself. The four kinds are a precipitation risk, a salt over its own solubility, a tank saturated by its
salts competing for the same water, and a tank the library had to infer.

Where no published solubility figure exists the plan says so in `UnknownSolubility` rather than assuming
the salt is fine — so a ceiling computed without it may be lower than it looks.

## Dependency injection

```csharp
services.AddNpkTools();   // from SYT.NPKTools.DependencyInjection
```

Registers all five services as singletons. The extension sits in the
`Microsoft.Extensions.DependencyInjection` namespace by convention, so a typical `Program.cs` needs no
extra `using`. This is the only reason a second package exists: `SYT.NPKTools` itself has no
dependencies at all, which is what lets it run in a browser.

## Where to read next

- [README](../README.md) — the same ground at more length, with the reasoning
- [`docs/api-reference.md`](api-reference.md) — the entry points, with signatures
- [`docs/architecture.md`](architecture.md) — how the pieces fit
- [`docs/faq.md`](faq.md) — when a number surprises you
