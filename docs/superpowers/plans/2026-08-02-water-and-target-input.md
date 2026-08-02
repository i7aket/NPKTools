# Water estimation and table target input — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Let a grower enter the target as a table, see macro and micro separated, and describe their
water with whatever they actually have — nothing, an EC meter, a drop-test kit, or a lab analysis —
including the acid needed to neutralise it.

**Architecture:** All chemistry goes into `SYT.NPKTools` as pure, testable static classes
(`WaterPreset`, `WaterEstimator`, `AcidDose`). The Blazor app gains no domain logic: `CalculatorModel`
grows fields and a mode switch, and the `.razor` files only render and raise events. A new test
project covers the app's own model, which has never had one.

**Tech Stack:** .NET 10, C# 13, Blazor WebAssembly, xUnit 2.9.3, AwesomeAssertions.

Spec: `docs/superpowers/specs/2026-08-02-water-and-target-input-design.md`

## Global Constraints

- Target framework `net10.0`. Solution file is `SYT.NPKTools.slnx` (not a `.sln`).
- `TreatWarningsAsErrors=true` and `AnalysisLevel=latest-recommended`. A warning fails the build.
- `GenerateDocumentationFile=true` and CS1591 is enforced in `src/`: **every public type and member
  needs an XML doc comment**, including every `<param>` on a positional record.
- `Nullable=enable`, `ImplicitUsings=enable`.
- CI runs `dotnet format --verify-no-changes`. Run `dotnet format` before every commit.
- Tests: xUnit, AwesomeAssertions (`using AwesomeAssertions;` — **not** FluentAssertions), naming
  `Method_Scenario_Expectation`, and `[Trait("Category", "Unit")]` on every test.
- The test harness packages come from `tests/Directory.Build.props` automatically. A new test
  project's `.csproj` needs nothing but a `ProjectReference`.
- The app's UI text is English. Do not introduce Russian strings.
- Numbers in the app are parsed and formatted with `CultureInfo.InvariantCulture`; the app is built
  with `InvariantGlobalization=true`, so a comma decimal separator cannot be interpreted.
- Element symbol strings inside `src/SYT.NPKTools` come from the internal `Names` class; atomic
  masses from internal `AtomicMasses`. Do not retype literals.

**Build:** `dotnet build SYT.NPKTools.slnx -c Release` (~35 s from cold)
**Test:** `dotnet test SYT.NPKTools.slnx -c Release`

---

### Task 1: Element groups

The input grid needs to know which symbols are macro, which are micro and which are counter-ions.
`FertilizerBundleGenerator` already makes this distinction privately and omits Cl and Na from its
micro list on purpose. Publishing one definition stops the UI from inventing a second.

**Files:**
- Create: `src/SYT.NPKTools/Nutrients/ElementGroups.cs`
- Test: `tests/SYT.NPKTools.Tests/ElementGroupsTests.cs`

**Interfaces:**
- Consumes: internal `SYT.NPKTools.Internal.Names`.
- Produces: `ElementGroups.Macro`, `.Micro`, `.CounterIons`, `.All` — all
  `IReadOnlyList<string>`.

- [ ] **Step 1: Write the failing test**

```csharp
using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers the element groups an input form is built from.
/// </summary>
public class ElementGroupsTests
{
    /// <summary>
    /// An element in two groups would appear twice on screen and be entered twice.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Groups_Overlap_IsEmpty()
    {
        string[] all = [.. ElementGroups.Macro, .. ElementGroups.Micro, .. ElementGroups.CounterIons];

        all.Should().OnlyHaveUniqueItems();
    }

    /// <summary>
    /// The groups together are the whole input form. An element the parser accepts but no group
    /// carries would be silently unenterable, which is how sodium would have gone missing.
    /// </summary>
    [Theory]
    [InlineData("N")]
    [InlineData("P")]
    [InlineData("K")]
    [InlineData("Ca")]
    [InlineData("Mg")]
    [InlineData("S")]
    [InlineData("Fe")]
    [InlineData("Cu")]
    [InlineData("Mn")]
    [InlineData("Zn")]
    [InlineData("B")]
    [InlineData("Mo")]
    [InlineData("Cl")]
    [InlineData("Si")]
    [InlineData("Se")]
    [InlineData("Na")]
    [Trait("Category", "Unit")]
    public void All_ContainsEverySymbolTheParserAccepts(string symbol)
    {
        ElementGroups.All.Should().Contain(symbol);
    }

    /// <summary>
    /// Sixteen elements and no seventeenth: a symbol added to the library without a group would
    /// break here rather than vanish from the interface.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void All_HasSixteenElements()
    {
        ElementGroups.All.Should().HaveCount(16);
    }

    /// <summary>
    /// Chlorine and sodium are not dosed for, and the bundle generator leaves them out of its micro
    /// list for that reason. They are still entered, so they are their own group rather than being
    /// folded into the micronutrients.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void CounterIons_AreChlorineAndSodium()
    {
        ElementGroups.CounterIons.Should().Equal("Cl", "Na");
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test tests/SYT.NPKTools.Tests -c Release --filter "FullyQualifiedName~ElementGroupsTests"`
Expected: build failure — `The name 'ElementGroups' does not exist`.

- [ ] **Step 3: Write the implementation**

```csharp
using SYT.NPKTools.Internal;

namespace SYT.NPKTools.Nutrients;

/// <summary>
/// The element symbols an input form is grouped by, in display order.
/// </summary>
/// <remarks>
/// <para>
/// Three groups rather than two, because the library already draws the finer distinction and an
/// interface that draws only the coarse one loses information. <see cref="Micro"/> is what
/// <see cref="Presets.FertilizerBundleGenerator"/> doses for. <see cref="CounterIons"/> is chlorine
/// and sodium: they arrive with other salts rather than being dosed for, which is why the generator
/// leaves them out of its own micro list — reporting them as uncovered would be noise.
/// </para>
/// <para>
/// They are still entered, though. A water analysis reports both, and sodium is the whole story in
/// water from an ion-exchange softener, so leaving them off a form would hide the case that most
/// needs seeing.
/// </para>
/// </remarks>
public static class ElementGroups
{
    /// <summary>The macronutrients, in the order a feed chart lists them.</summary>
    public static IReadOnlyList<string> Macro { get; } =
        [Names.N, Names.P, Names.K, Names.Ca, Names.Mg, Names.S];

    /// <summary>The micronutrients that are dosed for.</summary>
    public static IReadOnlyList<string> Micro { get; } =
        [Names.Fe, Names.Cu, Names.Mn, Names.Zn, Names.B, Names.Mo, Names.Si, Names.Se];

    /// <summary>Ions that arrive with other salts rather than being dosed for.</summary>
    public static IReadOnlyList<string> CounterIons { get; } = [Names.Cl, Names.Na];

    /// <summary>Every symbol an analysis or a target can carry, macro first.</summary>
    public static IReadOnlyList<string> All { get; } = [.. Macro, .. Micro, .. CounterIons];
}
```

- [ ] **Step 4: Run the tests to verify they pass**

Run: `dotnet test tests/SYT.NPKTools.Tests -c Release --filter "FullyQualifiedName~ElementGroupsTests"`
Expected: PASS, 19 tests.

- [ ] **Step 5: Format and commit**

```bash
dotnet format
git add src/SYT.NPKTools/Nutrients/ElementGroups.cs tests/SYT.NPKTools.Tests/ElementGroupsTests.cs
git commit -m "feat(nutrients): publish the element groups an input form is built from"
```

---

### Task 2: Hardness readings

GH and KH are what a drop-test kit reports, and the estimator takes them as input while the tests
need them as output. One conversion, defined once.

**Files:**
- Modify: `src/SYT.NPKTools/Nutrients/Extensions/WaterProfileExtensions.cs`
- Test: `tests/SYT.NPKTools.Tests/HardnessTests.cs`

**Interfaces:**
- Consumes: existing `WaterProfileExtensions.EstimatedAlkalinity`, `AsPpm`, `PpmExtensions.IonBalance`.
- Produces: `WaterProfileExtensions.GeneralHardness(this WaterProfile) -> double` (°dH),
  `WaterProfileExtensions.CarbonateHardness(this WaterProfile) -> double` (°dKH),
  `WaterProfileExtensions.MilliequivalentsPerGermanDegree` — `public const double` = `0.3567`.

- [ ] **Step 1: Write the failing test**

```csharp
using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers hardness expressed in German degrees, the unit every drop-test kit prints.
/// </summary>
/// <remarks>
/// One German degree is 17.85 mg/L as CaCO₃, which is 0.3567 meq/L. General hardness counts calcium
/// and magnesium; carbonate hardness counts the bicarbonate, which this library infers from the
/// cation surplus rather than taking as an input.
/// </remarks>
public class HardnessTests
{
    /// <summary>
    /// 100 ppm of calcium is 4.99 meq/L, which is 14.0 °dH. Checked against the definition rather
    /// than against the implementation.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GeneralHardness_ForCalciumOnly_MatchesTheDefinition()
    {
        WaterProfile water = new WaterProfileBuilder().AddCa(100).Build();

        water.GeneralHardness().Should().BeApproximately(13.99, 0.05);
    }

    /// <summary>
    /// Magnesium counts the same, per equivalent rather than per milligram: 24.3 ppm of magnesium is
    /// 2 meq and so twice the hardness of 23.0 ppm of sodium, which is not hardness at all.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GeneralHardness_CountsMagnesiumAndIgnoresSodium()
    {
        WaterProfile hard = new WaterProfileBuilder().AddMg(24.305).Build();
        WaterProfile salty = new WaterProfileBuilder().AddNa(22.990).Build();

        hard.GeneralHardness().Should().BeApproximately(5.61, 0.05);
        salty.GeneralHardness().Should().Be(0);
    }

    /// <summary>
    /// Carbonate hardness follows the inferred alkalinity, so calcium with no matching anion reads
    /// as bicarbonate — which is what calcium bicarbonate water is.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void CarbonateHardness_FollowsTheInferredAlkalinity()
    {
        WaterProfile water = new WaterProfileBuilder().AddCa(100).Build();

        water.CarbonateHardness().Should().BeApproximately(
            water.EstimatedAlkalinity() / WaterProfileExtensions.MilliequivalentsPerGermanDegree,
            1e-9);
        water.CarbonateHardness().Should().BeApproximately(13.99, 0.05);
    }

    /// <summary>
    /// Water whose anions already match its cations has no alkalinity, and so no carbonate hardness,
    /// however hard it is: calcium sulfate water is the textbook case.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void CarbonateHardness_ForCalciumSulfateWater_IsZero()
    {
        WaterProfile water = new WaterProfileBuilder().AddCa(100).AddS(80).Build();

        water.CarbonateHardness().Should().Be(0);
        water.GeneralHardness().Should().BeApproximately(13.99, 0.05);
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test tests/SYT.NPKTools.Tests -c Release --filter "FullyQualifiedName~HardnessTests"`
Expected: build failure — `GeneralHardness` is not defined.

- [ ] **Step 3: Write the implementation**

Append these three members inside the existing `WaterProfileExtensions` class, after
`EstimatedAlkalinity`:

```csharp
    /// <summary>
    /// One German degree of hardness, in milliequivalents per litre.
    /// </summary>
    /// <remarks>
    /// 1 °dH is 17.85 mg/L as CaCO₃, and CaCO₃ has an equivalent weight of 50.04, so one degree is
    /// 0.3567 meq/L. The same figure converts carbonate hardness, because both scales count
    /// equivalents and differ only in which ions they count.
    /// </remarks>
    public const double MilliequivalentsPerGermanDegree = 0.3567;

    /// <summary>
    /// The water's general hardness — calcium and magnesium — in German degrees.
    /// </summary>
    /// <param name="water">The source water's analysis.</param>
    /// <returns>The hardness in °dH.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="water"/> is null.</exception>
    /// <remarks>
    /// Counted per equivalent, not per milligram: magnesium is lighter than calcium and so contributes
    /// more hardness per milligram. Sodium contributes none at all, which is exactly why softened
    /// water reads as soft while its conductivity says otherwise.
    /// </remarks>
    public static double GeneralHardness(this WaterProfile water)
    {
        ArgumentNullException.ThrowIfNull(water);

        IonBalance balance = water.AsPpm().IonBalance();

        return (balance.Calcium + balance.Magnesium) / MilliequivalentsPerGermanDegree;
    }

    /// <summary>
    /// The water's carbonate hardness — its alkalinity — in German degrees.
    /// </summary>
    /// <param name="water">The source water's analysis.</param>
    /// <returns>The carbonate hardness in °dKH.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="water"/> is null.</exception>
    /// <remarks>
    /// The same quantity as <see cref="EstimatedAlkalinity"/>, in the unit a drop-test kit prints. It
    /// is inferred from the cation surplus rather than measured, so water whose anions already balance
    /// its cations reads as zero however hard it is.
    /// </remarks>
    public static double CarbonateHardness(this WaterProfile water)
    {
        ArgumentNullException.ThrowIfNull(water);

        return water.EstimatedAlkalinity() / MilliequivalentsPerGermanDegree;
    }
```

- [ ] **Step 4: Run the tests to verify they pass**

Run: `dotnet test tests/SYT.NPKTools.Tests -c Release --filter "FullyQualifiedName~HardnessTests"`
Expected: PASS, 4 tests.

If `IonBalance.Calcium` turns out not to be exposed in meq, read
`src/SYT.NPKTools/Nutrients/IonBalance.cs` and use the properties it does expose — the class holds
per-ion milliequivalents, and the property names follow the ion, not the element.

- [ ] **Step 5: Format and commit**

```bash
dotnet format
git add src/SYT.NPKTools/Nutrients/Extensions/WaterProfileExtensions.cs tests/SYT.NPKTools.Tests/HardnessTests.cs
git commit -m "feat(nutrients): report general and carbonate hardness in German degrees"
```

---

### Task 3: Water presets

Four shapes of ordinary water. A preset is proportions, not a composition — the estimator scales it.

**Files:**
- Create: `src/SYT.NPKTools/Nutrients/WaterPreset.cs`
- Test: `tests/SYT.NPKTools.Tests/WaterPresetTests.cs`

**Interfaces:**
- Consumes: `WaterProfileBuilder`, `WaterProfileExtensions.GeneralHardness` / `.CarbonateHardness`
  (Task 2).
- Produces: `WaterPreset` with `Id`, `Label`, `Calcium`, `Magnesium`, `Sodium`, `Sulfur`,
  `Chlorine`, `Nitrogen` (all `double`, ppm at nominal scale), `ToProfile(double scale = 1)
  -> WaterProfile`, the four statics `SoftLowAlkalinity`, `CalciumBicarbonateModerate`,
  `CalciumBicarbonateHard`, `SodiumExchangeSoftened`, and `All -> IReadOnlyList<WaterPreset>`.

- [ ] **Step 1: Write the failing test**

```csharp
using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers the built-in shapes of ordinary source water.
/// </summary>
/// <remarks>
/// Each preset is asserted against the readings a grower can take themselves — conductivity,
/// general hardness, carbonate hardness — rather than against its own ppm figures. That is what
/// makes the table in the design document a claim about real water rather than a restatement of the
/// constants, and it is what would fail if the conductivity model were changed underneath.
/// </remarks>
public class WaterPresetTests
{
    /// <summary>
    /// The documented readings for every preset. Ranges are the textbook classification each preset
    /// is named for, so a preset drifting out of its own class fails here.
    /// </summary>
    [Theory]
    [InlineData("SoftLowAlkalinity", 173, 61, 3.4, 2.8)]
    [InlineData("CalciumBicarbonateModerate", 471, 166, 10.2, 7.6)]
    [InlineData("CalciumBicarbonateHard", 892, 295, 19.8, 13.6)]
    [InlineData("SodiumExchangeSoftened", 559, 222, 0.8, 10.2)]
    [Trait("Category", "Unit")]
    public void ToProfile_MatchesTheDocumentedReadings(
        string id,
        double microSiemens,
        double bicarbonatePpm,
        double generalHardness,
        double carbonateHardness)
    {
        WaterPreset preset = WaterPreset.All.Single(p => p.Id == id);

        WaterProfile water = preset.ToProfile();

        water.EstimateConductivity().MicroSiemensPerCm.Should().BeApproximately(microSiemens, 1.0);
        (water.EstimatedAlkalinity() * 61.016).Should().BeApproximately(bicarbonatePpm, 1.0);
        water.GeneralHardness().Should().BeApproximately(generalHardness, 0.1);
        water.CarbonateHardness().Should().BeApproximately(carbonateHardness, 0.1);
    }

    /// <summary>
    /// Conductivity rises with scale, which is the property the estimator's bisection depends on.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ToProfile_Conductivity_RisesWithScale()
    {
        WaterPreset preset = WaterPreset.CalciumBicarbonateModerate;

        double half = preset.ToProfile(0.5).EstimateConductivity().MicroSiemensPerCm;
        double whole = preset.ToProfile(1.0).EstimateConductivity().MicroSiemensPerCm;
        double doubled = preset.ToProfile(2.0).EstimateConductivity().MicroSiemensPerCm;

        half.Should().BeLessThan(whole);
        whole.Should().BeLessThan(doubled);
    }

    /// <summary>
    /// A scale of zero is pure water, so an estimator driven to its floor produces something the rest
    /// of the library already handles rather than a special case.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ToProfile_AtZeroScale_IsPureWater()
    {
        WaterProfile water = WaterPreset.CalciumBicarbonateHard.ToProfile(0);

        water.EstimateConductivity().MicroSiemensPerCm.Should().Be(0);
        water.EstimatedAlkalinity().Should().Be(0);
    }

    /// <summary>
    /// Softened water is the preset that earns its place: high conductivity, no hardness. Estimated
    /// as one of the calcium presets it would promise calcium that is not there.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void SodiumExchangeSoftened_IsConductiveButNotHard()
    {
        WaterProfile softened = WaterPreset.SodiumExchangeSoftened.ToProfile();
        WaterProfile moderate = WaterPreset.CalciumBicarbonateModerate.ToProfile();

        softened.EstimateConductivity().MicroSiemensPerCm
            .Should().BeGreaterThan(moderate.EstimateConductivity().MicroSiemensPerCm);
        softened.GeneralHardness().Should().BeLessThan(1);
    }

    /// <summary>
    /// A negative scale is a caller error, not a water.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ToProfile_WithNegativeScale_Throws()
    {
        Action act = () => WaterPreset.SoftLowAlkalinity.ToProfile(-1);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test tests/SYT.NPKTools.Tests -c Release --filter "FullyQualifiedName~WaterPresetTests"`
Expected: build failure — `WaterPreset` does not exist.

- [ ] **Step 3: Write the implementation**

```csharp
namespace SYT.NPKTools.Nutrients;

/// <summary>
/// The shape of an ordinary source water — the proportions between its ions, not a composition.
/// </summary>
/// <remarks>
/// <para>
/// A grower with an EC meter knows how much is dissolved in their water and nothing about what. A
/// preset supplies the missing half: which ions, in what ratio. <see cref="WaterEstimator"/> then
/// scales the shape until its computed conductivity matches the meter, so the numbers that come out
/// are consistent with a real reading rather than invented.
/// </para>
/// <para>
/// Bicarbonate is not a field. The library infers it from the cation surplus, which is what makes
/// six numbers enough to describe water that is mostly calcium bicarbonate.
/// </para>
/// <para>
/// These are shapes of water classes, not of particular supplies. A grower with a laboratory
/// analysis should enter it; a preset is for the grower who has none.
/// </para>
/// </remarks>
public sealed record WaterPreset
{
    private WaterPreset(
        string id,
        string label,
        double calcium,
        double magnesium,
        double sodium,
        double sulfur,
        double chlorine,
        double nitrogen)
    {
        Id = id;
        Label = label;
        Calcium = calcium;
        Magnesium = magnesium;
        Sodium = sodium;
        Sulfur = sulfur;
        Chlorine = chlorine;
        Nitrogen = nitrogen;
    }

    /// <summary>Gets the stable identifier, safe to persist in a link or a file.</summary>
    public string Id { get; }

    /// <summary>Gets the name to show.</summary>
    public string Label { get; }

    /// <summary>Gets the calcium at nominal scale, in ppm.</summary>
    public double Calcium { get; }

    /// <summary>Gets the magnesium at nominal scale, in ppm.</summary>
    public double Magnesium { get; }

    /// <summary>Gets the sodium at nominal scale, in ppm.</summary>
    public double Sodium { get; }

    /// <summary>Gets the sulfur at nominal scale, in ppm, as elemental S.</summary>
    public double Sulfur { get; }

    /// <summary>Gets the chlorine at nominal scale, in ppm.</summary>
    public double Chlorine { get; }

    /// <summary>Gets the nitrogen at nominal scale, in ppm, as nitrate.</summary>
    public double Nitrogen { get; }

    /// <summary>
    /// Surface water from granite country, and rain-fed reservoirs. Little of anything.
    /// </summary>
    public static WaterPreset SoftLowAlkalinity { get; } =
        new("SoftLowAlkalinity", "Soft, low alkalinity", 18, 4, 8, 4, 9, 1);

    /// <summary>
    /// The commonest municipal supply: hardness and alkalinity both from limestone.
    /// </summary>
    public static WaterPreset CalciumBicarbonateModerate { get; } =
        new("CalciumBicarbonateModerate", "Calcium bicarbonate, moderately hard", 55, 11, 16, 13, 21, 3);

    /// <summary>
    /// Groundwater from chalk or limestone aquifers. Enough alkalinity to move root-zone pH all season.
    /// </summary>
    public static WaterPreset CalciumBicarbonateHard { get; } =
        new("CalciumBicarbonateHard", "Calcium bicarbonate, hard", 105, 22, 28, 32, 38, 5);

    /// <summary>
    /// Water from a domestic ion-exchange softener, which trades calcium for sodium.
    /// </summary>
    /// <remarks>
    /// Conductivity stays high while the calcium is gone, so a meter alone cannot tell this apart from
    /// hard water. Estimated as hard, it would promise calcium that is not there while understating
    /// sodium that is — the most expensive of the ordinary mistakes, and the reason this preset exists.
    /// </remarks>
    public static WaterPreset SodiumExchangeSoftened { get; } =
        new("SodiumExchangeSoftened", "Softened by sodium exchange", 4, 1, 120, 14, 30, 2);

    /// <summary>Gets every preset, in rising order of dissolved content.</summary>
    public static IReadOnlyList<WaterPreset> All { get; } =
    [
        SoftLowAlkalinity,
        CalciumBicarbonateModerate,
        CalciumBicarbonateHard,
        SodiumExchangeSoftened,
    ];

    /// <summary>
    /// Builds the water this shape describes, at a given scale.
    /// </summary>
    /// <param name="scale">
    /// A multiplier on every ion. One is the nominal composition; the estimator solves for the scale
    /// that reproduces a measured conductivity.
    /// </param>
    /// <returns>The scaled analysis.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="scale"/> is negative.</exception>
    public WaterProfile ToProfile(double scale = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(scale);

        return new WaterProfileBuilder()
            .AddCa(Calcium * scale)
            .AddMg(Magnesium * scale)
            .AddNa(Sodium * scale)
            .AddS(Sulfur * scale)
            .AddCl(Chlorine * scale)
            .AddNitrate(Nitrogen * scale)
            .Build();
    }
}
```

- [ ] **Step 4: Run the tests to verify they pass**

Run: `dotnet test tests/SYT.NPKTools.Tests -c Release --filter "FullyQualifiedName~WaterPresetTests"`
Expected: PASS, 8 tests.

If a preset misses its documented reading, **change the preset's ppm figures, not the tolerance** —
the readings are the specification and the ppm figures are the free parameters.

- [ ] **Step 5: Format and commit**

```bash
dotnet format
git add src/SYT.NPKTools/Nutrients/WaterPreset.cs tests/SYT.NPKTools.Tests/WaterPresetTests.cs
git commit -m "feat(nutrients): add four shapes of ordinary source water"
```

---

### Task 4: The water estimator

Turn a meter reading, and optionally a drop test, into an analysis.

**Files:**
- Create: `src/SYT.NPKTools/Nutrients/WaterEstimate.cs`
- Create: `src/SYT.NPKTools/Nutrients/WaterEstimator.cs`
- Test: `tests/SYT.NPKTools.Tests/WaterEstimatorTests.cs`

**Interfaces:**
- Consumes: `WaterPreset.ToProfile` (Task 3), `WaterProfileExtensions.GeneralHardness`,
  `.CarbonateHardness`, `.MilliequivalentsPerGermanDegree` (Task 2), `PpmExtensions.IonBalance`,
  internal `AtomicMasses`.
- Produces:
  `WaterEstimator.Estimate(WaterPreset preset, double microSiemensPerCm, double? generalHardness = null, double? carbonateHardness = null) -> WaterEstimate`
  and `WaterEstimate` with `Profile`, `MicroSiemensPerCm`, `RequestedMicroSiemensPerCm`, `Feasible`,
  `RelativeError`.

- [ ] **Step 1: Write the failing test**

```csharp
using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers estimating a water analysis from the readings a grower can actually take.
/// </summary>
/// <remarks>
/// The estimator's contract is narrow and worth stating: it reproduces the conductivity it was given,
/// it honours a hardness reading exactly when one is supplied, and it says so rather than compromising
/// when the readings it was given cannot describe the same water.
/// </remarks>
public class WaterEstimatorTests
{
    /// <summary>
    /// The central claim: whatever the meter said, the estimate reads the same on the same model.
    /// </summary>
    [Theory]
    [InlineData(150)]
    [InlineData(300)]
    [InlineData(471)]
    [InlineData(900)]
    [Trait("Category", "Unit")]
    public void Estimate_FromConductivityAlone_ReproducesTheReading(double microSiemens)
    {
        WaterEstimate estimate = WaterEstimator.Estimate(
            WaterPreset.CalciumBicarbonateModerate,
            microSiemens);

        estimate.Feasible.Should().BeTrue();
        estimate.MicroSiemensPerCm.Should().BeApproximately(microSiemens, microSiemens * 0.005);
    }

    /// <summary>
    /// At its own nominal conductivity a preset comes back unscaled, so the estimator adds nothing
    /// of its own when it has nothing to solve for.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Estimate_AtThePresetsOwnConductivity_ReturnsThePresetUnchanged()
    {
        WaterPreset preset = WaterPreset.CalciumBicarbonateModerate;
        double nominal = preset.ToProfile().EstimateConductivity().MicroSiemensPerCm;

        WaterEstimate estimate = WaterEstimator.Estimate(preset, nominal);

        estimate.Profile.Calcium.Value.Should().BeApproximately(preset.Calcium, 0.5);
        estimate.Profile.Sodium.Value.Should().BeApproximately(preset.Sodium, 0.5);
    }

    /// <summary>
    /// A hardness reading is a measurement, not a hint: it comes back out of the estimate unchanged.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Estimate_WithHardnessReadings_HonoursThemExactly()
    {
        WaterEstimate estimate = WaterEstimator.Estimate(
            WaterPreset.CalciumBicarbonateModerate,
            microSiemensPerCm: 450,
            generalHardness: 9,
            carbonateHardness: 7);

        estimate.Feasible.Should().BeTrue();
        estimate.Profile.GeneralHardness().Should().BeApproximately(9, 0.05);
        estimate.Profile.CarbonateHardness().Should().BeApproximately(7, 0.05);
        estimate.MicroSiemensPerCm.Should().BeApproximately(450, 5);
    }

    /// <summary>
    /// Carbonate hardness above general hardness means a cation surplus that calcium cannot supply,
    /// which is what softened water is. The sodium shape resolves it; the estimate stays feasible and
    /// still reads back both drop tests.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Estimate_ForSoftenedWater_ResolvesTheSurplusWithSodium()
    {
        WaterEstimate estimate = WaterEstimator.Estimate(
            WaterPreset.SodiumExchangeSoftened,
            microSiemensPerCm: 560,
            generalHardness: 0.8,
            carbonateHardness: 10.2);

        estimate.Feasible.Should().BeTrue();
        estimate.Profile.CarbonateHardness().Should().BeApproximately(10.2, 0.05);
        estimate.Profile.Sodium.Value.Should().BeGreaterThan(50);
        estimate.Profile.Calcium.Value.Should().BeLessThan(10);
    }

    /// <summary>
    /// Drop tests that already imply more conductivity than the meter read are not reconcilable. The
    /// estimator reports that rather than quietly preferring one reading over the other.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Estimate_WhenHardnessExceedsTheReading_IsNotFeasible()
    {
        WaterEstimate estimate = WaterEstimator.Estimate(
            WaterPreset.CalciumBicarbonateHard,
            microSiemensPerCm: 100,
            generalHardness: 20,
            carbonateHardness: 14);

        estimate.Feasible.Should().BeFalse();
        estimate.MicroSiemensPerCm.Should().BeGreaterThan(100);
        estimate.RelativeError.Should().BeGreaterThan(0.2);
    }

    /// <summary>
    /// A meter reading of zero is reverse osmosis, and nothing is dissolved in it.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Estimate_AtZeroConductivity_IsPureWater()
    {
        WaterEstimate estimate = WaterEstimator.Estimate(WaterPreset.CalciumBicarbonateHard, 0);

        estimate.Feasible.Should().BeTrue();
        estimate.Profile.Calcium.Value.Should().BeApproximately(0, 1e-6);
        estimate.MicroSiemensPerCm.Should().BeApproximately(0, 1e-6);
    }

    /// <summary>
    /// Guard clauses: a null preset and a negative reading are caller errors.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Estimate_WithInvalidArguments_Throws()
    {
        Action nullPreset = () => WaterEstimator.Estimate(null!, 400);
        Action negative = () => WaterEstimator.Estimate(WaterPreset.SoftLowAlkalinity, -1);

        nullPreset.Should().Throw<ArgumentNullException>();
        negative.Should().Throw<ArgumentOutOfRangeException>();
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test tests/SYT.NPKTools.Tests -c Release --filter "FullyQualifiedName~WaterEstimatorTests"`
Expected: build failure — `WaterEstimator` does not exist.

- [ ] **Step 3: Write `WaterEstimate`**

```csharp
namespace SYT.NPKTools.Nutrients;

/// <summary>
/// An analysis inferred from a meter reading, with the evidence for judging it.
/// </summary>
/// <remarks>
/// An estimate that cannot be checked is a guess. This carries the conductivity the estimate actually
/// has alongside the one that was asked for, so a caller can show the agreement rather than assert it.
/// </remarks>
public sealed record WaterEstimate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WaterEstimate"/> record.
    /// </summary>
    /// <param name="profile">The inferred analysis.</param>
    /// <param name="microSiemensPerCm">The conductivity the inferred analysis has.</param>
    /// <param name="requestedMicroSiemensPerCm">The conductivity that was asked for.</param>
    /// <param name="feasible">Whether the readings could be reconciled.</param>
    internal WaterEstimate(
        WaterProfile profile,
        double microSiemensPerCm,
        double requestedMicroSiemensPerCm,
        bool feasible)
    {
        Profile = profile;
        MicroSiemensPerCm = microSiemensPerCm;
        RequestedMicroSiemensPerCm = requestedMicroSiemensPerCm;
        Feasible = feasible;
    }

    /// <summary>Gets the inferred analysis, ready to subtract from a target.</summary>
    public WaterProfile Profile { get; }

    /// <summary>Gets the conductivity the inferred analysis has, in µS/cm.</summary>
    public double MicroSiemensPerCm { get; }

    /// <summary>Gets the conductivity that was asked for, in µS/cm.</summary>
    public double RequestedMicroSiemensPerCm { get; }

    /// <summary>
    /// Gets a value indicating whether the readings describe the same water.
    /// </summary>
    /// <remarks>
    /// False when a hardness reading already accounts for more conductivity than the meter showed.
    /// The profile is still returned — it is the closest water consistent with the drop tests — but
    /// it does not match the meter, and presenting it without saying so would be dishonest.
    /// </remarks>
    public bool Feasible { get; }

    /// <summary>Gets how far the estimate lands from the reading, as a fraction of it.</summary>
    public double RelativeError => RequestedMicroSiemensPerCm > 0
        ? Math.Abs(MicroSiemensPerCm - RequestedMicroSiemensPerCm) / RequestedMicroSiemensPerCm
        : 0;
}
```

- [ ] **Step 4: Write `WaterEstimator`**

```csharp
using SYT.NPKTools.Internal;

namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Infers a water analysis from the readings a grower can take themselves.
/// </summary>
/// <remarks>
/// <para>
/// A conductivity meter measures how much is dissolved, never what. A <see cref="WaterPreset"/>
/// supplies the proportions, and this scales them until the computed conductivity matches the meter.
/// Because the scaling runs through this library's own ion-by-ion conductivity model, the result is
/// consistent with the reading rather than merely plausible.
/// </para>
/// <para>
/// A drop test, when there is one, is treated as a measurement and pinned: general hardness fixes
/// calcium and magnesium, carbonate hardness fixes the bicarbonate, and only what is left over is
/// scaled. Pinning the bicarbonate means the charge balance no longer closes by itself, so the
/// shortfall is taken up by sodium or chloride — the two ions this library knows least about, and the
/// two a laboratory closes a real analysis on for the same reason.
/// </para>
/// <para>
/// Three readings can describe no water at all. Drop tests implying more conductivity than the meter
/// showed leave nothing to scale, and the estimate says
/// <see cref="WaterEstimate.Feasible"/> is false rather than silently preferring one reading.
/// </para>
/// </remarks>
public static class WaterEstimator
{
    /// <summary>The widest scale the solver will consider — far beyond any drinkable water.</summary>
    private const double MaximumScale = 100;

    /// <summary>Enough halvings to pin the scale to the last bit of a double.</summary>
    private const int Iterations = 60;

    /// <summary>
    /// Infers an analysis from a conductivity reading and, when available, hardness drop tests.
    /// </summary>
    /// <param name="preset">The shape of the water.</param>
    /// <param name="microSiemensPerCm">The meter reading, in µS/cm.</param>
    /// <param name="generalHardness">Calcium and magnesium, in °dH, or null when not measured.</param>
    /// <param name="carbonateHardness">Alkalinity, in °dKH, or null when not measured.</param>
    /// <returns>The estimate, with the evidence for judging it.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="preset"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when any reading is negative.
    /// </exception>
    public static WaterEstimate Estimate(
        WaterPreset preset,
        double microSiemensPerCm,
        double? generalHardness = null,
        double? carbonateHardness = null)
    {
        ArgumentNullException.ThrowIfNull(preset);
        ArgumentOutOfRangeException.ThrowIfNegative(microSiemensPerCm);

        if (generalHardness is { } gh)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(gh, nameof(generalHardness));
        }

        if (carbonateHardness is { } kh)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(kh, nameof(carbonateHardness));
        }

        double floor = Conductivity(Build(preset, 0, generalHardness, carbonateHardness));
        if (floor > microSiemensPerCm)
        {
            WaterProfile clamped = Build(preset, 0, generalHardness, carbonateHardness);
            return new WaterEstimate(clamped, floor, microSiemensPerCm, feasible: false);
        }

        double ceiling = Conductivity(Build(preset, MaximumScale, generalHardness, carbonateHardness));
        if (ceiling < microSiemensPerCm)
        {
            WaterProfile clamped = Build(preset, MaximumScale, generalHardness, carbonateHardness);
            return new WaterEstimate(clamped, ceiling, microSiemensPerCm, feasible: false);
        }

        double low = 0;
        double high = MaximumScale;
        for (int i = 0; i < Iterations; i++)
        {
            double middle = (low + high) / 2;
            if (Conductivity(Build(preset, middle, generalHardness, carbonateHardness)) < microSiemensPerCm)
            {
                low = middle;
            }
            else
            {
                high = middle;
            }
        }

        WaterProfile solved = Build(preset, (low + high) / 2, generalHardness, carbonateHardness);
        return new WaterEstimate(solved, Conductivity(solved), microSiemensPerCm, feasible: true);
    }

    private static double Conductivity(WaterProfile water) =>
        water.EstimateConductivity().MicroSiemensPerCm;

    /// <summary>
    /// Builds the candidate water at a scale, pinning whatever was measured.
    /// </summary>
    /// <remarks>
    /// Monotone in <paramref name="scale"/>, which is what makes bisection valid: every unpinned ion
    /// rises with it, and so does whichever ion closes the charge balance.
    /// </remarks>
    private static WaterProfile Build(
        WaterPreset preset,
        double scale,
        double? generalHardness,
        double? carbonateHardness)
    {
        double calcium = preset.Calcium * scale;
        double magnesium = preset.Magnesium * scale;

        if (generalHardness is { } gh)
        {
            // Split the measured hardness the way the preset splits its own, per equivalent.
            double presetCalciumMeq = preset.Calcium / AtomicMasses.Ca * Charges.Calcium;
            double presetMagnesiumMeq = preset.Magnesium / AtomicMasses.Mg * Charges.Magnesium;
            double presetTotalMeq = presetCalciumMeq + presetMagnesiumMeq;

            double measuredMeq = gh * WaterProfileExtensions.MilliequivalentsPerGermanDegree;
            double calciumShare = presetTotalMeq > 0 ? presetCalciumMeq / presetTotalMeq : 1;

            calcium = measuredMeq * calciumShare / Charges.Calcium * AtomicMasses.Ca;
            magnesium = measuredMeq * (1 - calciumShare) / Charges.Magnesium * AtomicMasses.Mg;
        }

        double sodium = preset.Sodium * scale;
        double chlorine = preset.Chlorine * scale;
        double sulfur = preset.Sulfur * scale;
        double nitrogen = preset.Nitrogen * scale;

        if (carbonateHardness is { } kh)
        {
            WaterProfile unclosed = Compose(calcium, magnesium, sodium, sulfur, chlorine, nitrogen);
            double surplus = -unclosed.AsPpm().IonBalance().AcidEquivalents;
            double wanted = kh * WaterProfileExtensions.MilliequivalentsPerGermanDegree;
            double delta = wanted - surplus;

            if (delta > 0)
            {
                sodium += delta * AtomicMasses.Na / Charges.Sodium;
            }
            else
            {
                chlorine += -delta * AtomicMasses.Cl / Charges.Chloride;
            }
        }

        return Compose(calcium, magnesium, sodium, sulfur, chlorine, nitrogen);
    }

    private static WaterProfile Compose(
        double calcium,
        double magnesium,
        double sodium,
        double sulfur,
        double chlorine,
        double nitrogen) =>
        new WaterProfileBuilder()
            .AddCa(calcium)
            .AddMg(magnesium)
            .AddNa(sodium)
            .AddS(sulfur)
            .AddCl(chlorine)
            .AddNitrate(nitrogen)
            .Build();
}
```

- [ ] **Step 5: Run the tests to verify they pass**

Run: `dotnet test tests/SYT.NPKTools.Tests -c Release --filter "FullyQualifiedName~WaterEstimatorTests"`
Expected: PASS, 10 tests (four theory cases and six facts).

Two failures are plausible here and both have specific fixes:

- `IonBalance.AcidEquivalents` sign. The existing `EstimatedAlkalinity` reads
  `Math.Max(0, -balance.AcidEquivalents)`, so a cation surplus is a *negative* `AcidEquivalents`.
  If the softened-water test comes out with the surplus resolved by chloride instead of sodium, the
  sign is flipped; correct it here, not in `EstimatedAlkalinity`.
- Monotonicity. If a bisection result overshoots badly, check that pinning general hardness has not
  made conductivity *fall* with scale — it cannot, because pinned ions are constant and unpinned ones
  rise, but a sign error in the closure would break it.

- [ ] **Step 6: Format and commit**

```bash
dotnet format
git add src/SYT.NPKTools/Nutrients/WaterEstimate.cs src/SYT.NPKTools/Nutrients/WaterEstimator.cs tests/SYT.NPKTools.Tests/WaterEstimatorTests.cs
git commit -m "feat(nutrients): infer a water analysis from a meter reading and drop tests"
```

---

### Task 5: Acids and the dose that neutralises alkalinity

**Files:**
- Create: `src/SYT.NPKTools/Nutrients/Acid.cs`
- Create: `src/SYT.NPKTools/Nutrients/AcidPlan.cs`
- Create: `src/SYT.NPKTools/Nutrients/AcidDose.cs`
- Test: `tests/SYT.NPKTools.Tests/AcidDoseTests.cs`

**Interfaces:**
- Consumes: internal `AtomicMasses`, `Names`.
- Produces: `AcidKind` enum (`Nitric`, `Phosphoric`, `Sulfuric`); `Acid` with `Kind`,
  `PercentByWeight`, `DensityGramsPerMillilitre`, `Label`, `EquivalentsPerLitre`,
  `NutrientSymbol`, `MilligramsOfNutrientPerMilliequivalent`, the six statics and `All`;
  `AcidPlan` with `MilliequivalentsPerLitre`, `Millilitres`, `NutrientSymbol`, `NutrientPpm`;
  `AcidDose.BicarbonateFraction(double ph) -> double` and
  `AcidDose.Calculate(double alkalinityMeqPerLitre, double waterPh, double targetPh, Acid acid, double litres) -> AcidPlan`.

- [ ] **Step 1: Write the failing test**

```csharp
using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers the acid needed to bring source water down to a working pH.
/// </summary>
/// <remarks>
/// The worked case throughout is the moderately hard preset — 166 ppm of bicarbonate, 2.72 meq/L of
/// alkalinity — taken from pH 7.6 to 5.8, which is the ordinary situation this feature exists for.
/// </remarks>
public class AcidDoseTests
{
    private const double ModerateAlkalinity = 2.721;

    /// <summary>
    /// At the first pKa the carbonate is half bicarbonate by definition, which is the one point on
    /// the curve that can be checked without trusting the implementation.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void BicarbonateFraction_AtTheFirstPka_IsAHalf()
    {
        AcidDose.BicarbonateFraction(6.35).Should().BeApproximately(0.5, 0.001);
    }

    /// <summary>
    /// Below the pKa the carbonate is mostly carbonic acid; above it, mostly bicarbonate.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void BicarbonateFraction_RisesWithPh()
    {
        AcidDose.BicarbonateFraction(5.8).Should().BeApproximately(0.2199, 0.001);
        AcidDose.BicarbonateFraction(7.6).Should().BeApproximately(0.9451, 0.001);
    }

    /// <summary>
    /// The headline figure: neutralising to pH 5.8 takes about three quarters of the alkalinity, not
    /// all of it. Most guides quote the full figure, and the difference is a quarter of the acid.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Calculate_ForModeratelyHardWater_TakesAboutThreeQuartersOfTheAlkalinity()
    {
        AcidPlan plan = AcidDose.Calculate(ModerateAlkalinity, 7.6, 5.8, Acid.Nitric60, 100);

        plan.MilliequivalentsPerLitre.Should().BeApproximately(2.089, 0.005);
        (plan.MilliequivalentsPerLitre / ModerateAlkalinity).Should().BeApproximately(0.768, 0.005);
    }

    /// <summary>
    /// The volume a grower actually measures out, and the nitrogen it brings with it.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Calculate_WithNitricAcid_GivesTheVolumeAndTheNitrogen()
    {
        AcidPlan plan = AcidDose.Calculate(ModerateAlkalinity, 7.6, 5.8, Acid.Nitric60, 100);

        plan.Millilitres.Should().BeApproximately(16.05, 0.1);
        plan.NutrientSymbol.Should().Be("N");
        plan.NutrientPpm.Should().BeApproximately(29.3, 0.1);
    }

    /// <summary>
    /// Phosphoric acid is the case that justifies reporting the nutrient at all: neutralising this
    /// water with it delivers 65 ppm of phosphorus, past a typical 50 ppm target before any salt is
    /// weighed, and no recipe can take it back out.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Calculate_WithPhosphoricAcid_OvershootsATypicalPhosphorusTarget()
    {
        AcidPlan plan = AcidDose.Calculate(ModerateAlkalinity, 7.6, 5.8, Acid.Phosphoric85, 100);

        plan.NutrientSymbol.Should().Be("P");
        plan.NutrientPpm.Should().BeApproximately(64.7, 0.5);
        plan.Millilitres.Should().BeApproximately(14.3, 0.1);
    }

    /// <summary>
    /// Sulfuric acid gives up two protons, so it takes least volume and brings sulfur rather than a
    /// nutrient that is usually already at target.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Calculate_WithSulfuricAcid_UsesTwoProtonsPerMolecule()
    {
        AcidPlan plan = AcidDose.Calculate(ModerateAlkalinity, 7.6, 5.8, Acid.Sulfuric98, 100);

        plan.NutrientSymbol.Should().Be("S");
        plan.Millilitres.Should().BeApproximately(5.69, 0.1);
        plan.NutrientPpm.Should().BeApproximately(33.5, 0.5);
    }

    /// <summary>
    /// Every acid in the table: strength follows from its own percentage, density and the protons it
    /// can give up at working pH, so a typo in the table cannot pass unnoticed.
    /// </summary>
    [Theory]
    [InlineData("Nitric60", 13.02)]
    [InlineData("Nitric38", 7.44)]
    [InlineData("Phosphoric85", 14.62)]
    [InlineData("Phosphoric75", 12.09)]
    [InlineData("Sulfuric98", 36.69)]
    [InlineData("Sulfuric37", 9.63)]
    [Trait("Category", "Unit")]
    public void EquivalentsPerLitre_MatchTheTable(string id, double expected)
    {
        Acid acid = Acid.All.Single(a => a.Id == id);

        acid.EquivalentsPerLitre.Should().BeApproximately(expected, 0.02);
    }

    /// <summary>
    /// Reverse osmosis needs no acid, and neither does water already at the target pH.
    /// </summary>
    [Theory]
    [InlineData(0, 7.6, 5.8)]
    [InlineData(2.721, 5.8, 5.8)]
    [InlineData(2.721, 5.5, 5.8)]
    [Trait("Category", "Unit")]
    public void Calculate_WhenNothingNeedsNeutralising_IsZero(
        double alkalinity,
        double waterPh,
        double targetPh)
    {
        AcidPlan plan = AcidDose.Calculate(alkalinity, waterPh, targetPh, Acid.Nitric60, 100);

        plan.MilliequivalentsPerLitre.Should().Be(0);
        plan.Millilitres.Should().Be(0);
        plan.NutrientPpm.Should().Be(0);
    }

    /// <summary>
    /// Volume scales with the tank, and concentration does not.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Calculate_ScalesVolumeWithTheTankButNotConcentration()
    {
        AcidPlan small = AcidDose.Calculate(ModerateAlkalinity, 7.6, 5.8, Acid.Nitric60, 10);
        AcidPlan large = AcidDose.Calculate(ModerateAlkalinity, 7.6, 5.8, Acid.Nitric60, 1000);

        large.Millilitres.Should().BeApproximately(small.Millilitres * 100, 0.01);
        large.NutrientPpm.Should().BeApproximately(small.NutrientPpm, 1e-9);
    }

    /// <summary>
    /// A custom acid is described the same way the built-in ones are, and behaves the same.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Custom_MatchesABuiltInAcidGivenTheSameFigures()
    {
        Acid custom = new(AcidKind.Nitric, 60, 1.367);

        custom.EquivalentsPerLitre.Should().BeApproximately(Acid.Nitric60.EquivalentsPerLitre, 1e-9);
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test tests/SYT.NPKTools.Tests -c Release --filter "FullyQualifiedName~AcidDoseTests"`
Expected: build failure — `Acid` does not exist.

- [ ] **Step 3: Write `Acid`**

```csharp
using SYT.NPKTools.Internal;

namespace SYT.NPKTools.Nutrients;

/// <summary>
/// The acids used to neutralise the alkalinity of source water.
/// </summary>
public enum AcidKind
{
    /// <summary>Nitric acid, HNO₃. Contributes nitrate.</summary>
    Nitric,

    /// <summary>Phosphoric acid, H₃PO₄. Contributes phosphorus.</summary>
    Phosphoric,

    /// <summary>Sulfuric acid, H₂SO₄. Contributes sulfur.</summary>
    Sulfuric,
}

/// <summary>
/// A bottle of acid: what it is, how strong, and what it brings to the solution besides acidity.
/// </summary>
/// <remarks>
/// <para>
/// Strength is derived, never stored. Equivalents per litre follow from the percentage by weight, the
/// density and the protons the molecule can actually give up at working pH, so a mistyped density
/// changes the dose rather than quietly disagreeing with it.
/// </para>
/// <para>
/// The protons that count are the ones available above pH 5.5. Nitric gives one. Phosphoric gives one:
/// its second dissociation is at pKa 7.20, out of reach of a solution run at 5.8. Sulfuric gives two,
/// its second at pKa 1.99 being fully dissociated. Counting phosphoric as three-protic — an easy
/// mistake — would understate the dose threefold.
/// </para>
/// </remarks>
public sealed record Acid
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Acid"/> record.
    /// </summary>
    /// <param name="kind">Which acid it is.</param>
    /// <param name="percentByWeight">Concentration, as a percentage by weight.</param>
    /// <param name="densityGramsPerMillilitre">Density of the liquid, in g/mL.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the percentage is outside 0–100, or the density is not positive.
    /// </exception>
    public Acid(AcidKind kind, double percentByWeight, double densityGramsPerMillilitre)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(percentByWeight);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(percentByWeight, 100);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(densityGramsPerMillilitre);

        Kind = kind;
        PercentByWeight = percentByWeight;
        DensityGramsPerMillilitre = densityGramsPerMillilitre;
        Id = $"{kind}{percentByWeight:0.##}";
        Label = $"{kind} {percentByWeight:0.##}%";
    }

    private Acid(AcidKind kind, double percentByWeight, double density, string id, string label)
        : this(kind, percentByWeight, density)
    {
        Id = id;
        Label = label;
    }

    /// <summary>Gets a stable identifier, safe to persist in a link or a file.</summary>
    public string Id { get; }

    /// <summary>Gets the name to show.</summary>
    public string Label { get; }

    /// <summary>Gets which acid it is.</summary>
    public AcidKind Kind { get; }

    /// <summary>Gets the concentration, as a percentage by weight.</summary>
    public double PercentByWeight { get; }

    /// <summary>Gets the density of the liquid, in g/mL.</summary>
    public double DensityGramsPerMillilitre { get; }

    /// <summary>Gets the weight of acid that supplies one mole of usable protons, in g/mol.</summary>
    public double EquivalentWeight => Kind switch
    {
        AcidKind.Nitric => 63.012,
        AcidKind.Phosphoric => 97.994,
        AcidKind.Sulfuric => 98.079 / 2,
        _ => throw new InvalidOperationException($"Unknown acid kind '{Kind}'."),
    };

    /// <summary>Gets the element symbol the acid contributes to the solution.</summary>
    public string NutrientSymbol => Kind switch
    {
        AcidKind.Nitric => Names.N,
        AcidKind.Phosphoric => Names.P,
        AcidKind.Sulfuric => Names.S,
        _ => throw new InvalidOperationException($"Unknown acid kind '{Kind}'."),
    };

    /// <summary>Gets the nutrient delivered per milliequivalent of acid, in milligrams.</summary>
    public double MilligramsOfNutrientPerMilliequivalent => Kind switch
    {
        AcidKind.Nitric => AtomicMasses.N,
        AcidKind.Phosphoric => AtomicMasses.P,
        AcidKind.Sulfuric => AtomicMasses.S / 2,
        _ => throw new InvalidOperationException($"Unknown acid kind '{Kind}'."),
    };

    /// <summary>Gets the usable protons per litre of the liquid, in equivalents.</summary>
    public double EquivalentsPerLitre =>
        PercentByWeight / 100 * DensityGramsPerMillilitre * 1000 / EquivalentWeight;

    /// <summary>Technical-grade nitric acid, 60%.</summary>
    public static Acid Nitric60 { get; } = new(AcidKind.Nitric, 60, 1.367, "Nitric60", "Nitric acid 60%");

    /// <summary>Dilute nitric acid, 38%.</summary>
    public static Acid Nitric38 { get; } = new(AcidKind.Nitric, 38, 1.234, "Nitric38", "Nitric acid 38%");

    /// <summary>Concentrated orthophosphoric acid, 85%.</summary>
    public static Acid Phosphoric85 { get; } =
        new(AcidKind.Phosphoric, 85, 1.685, "Phosphoric85", "Phosphoric acid 85%");

    /// <summary>Orthophosphoric acid, 75%.</summary>
    public static Acid Phosphoric75 { get; } =
        new(AcidKind.Phosphoric, 75, 1.579, "Phosphoric75", "Phosphoric acid 75%");

    /// <summary>Concentrated sulfuric acid, 98%.</summary>
    public static Acid Sulfuric98 { get; } =
        new(AcidKind.Sulfuric, 98, 1.836, "Sulfuric98", "Sulfuric acid 98%");

    /// <summary>Battery-strength sulfuric acid, 37%.</summary>
    public static Acid Sulfuric37 { get; } =
        new(AcidKind.Sulfuric, 37, 1.276, "Sulfuric37", "Sulfuric acid 37%");

    /// <summary>Gets every built-in acid.</summary>
    public static IReadOnlyList<Acid> All { get; } =
        [Nitric60, Nitric38, Phosphoric85, Phosphoric75, Sulfuric98, Sulfuric37];
}
```

- [ ] **Step 4: Write `AcidPlan` and `AcidDose`**

`AcidPlan.cs`:

```csharp
namespace SYT.NPKTools.Nutrients;

/// <summary>
/// How much acid a tank needs, and what the acid brings with it.
/// </summary>
public sealed record AcidPlan
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AcidPlan"/> record.
    /// </summary>
    /// <param name="milliequivalentsPerLitre">Acidity to add, in meq/L.</param>
    /// <param name="millilitres">Volume of the liquid acid, for the whole tank.</param>
    /// <param name="nutrientSymbol">The element the acid contributes.</param>
    /// <param name="nutrientPpm">How much of that element it contributes, in ppm.</param>
    internal AcidPlan(
        double milliequivalentsPerLitre,
        double millilitres,
        string nutrientSymbol,
        double nutrientPpm)
    {
        MilliequivalentsPerLitre = milliequivalentsPerLitre;
        Millilitres = millilitres;
        NutrientSymbol = nutrientSymbol;
        NutrientPpm = nutrientPpm;
    }

    /// <summary>Gets the acidity to add, in milliequivalents per litre.</summary>
    public double MilliequivalentsPerLitre { get; }

    /// <summary>Gets the volume of liquid acid for the whole tank, in millilitres.</summary>
    public double Millilitres { get; }

    /// <summary>Gets the element symbol the acid contributes.</summary>
    public string NutrientSymbol { get; }

    /// <summary>
    /// Gets how much of that element the acid contributes, in ppm.
    /// </summary>
    /// <remarks>
    /// Subtract this from the target alongside the water. Nitric acid on moderately hard water carries
    /// around 29 ppm of nitrogen, a fifth of an ordinary target, and phosphoric acid can carry more
    /// phosphorus than the target asks for in total.
    /// </remarks>
    public double NutrientPpm { get; }
}
```

`AcidDose.cs`:

```csharp
namespace SYT.NPKTools.Nutrients;

/// <summary>
/// The acid needed to bring source water to a working pH.
/// </summary>
/// <remarks>
/// <para>
/// Worked from the carbonate equilibrium rather than from a rule of thumb. Alkalinity at ordinary
/// source-water pH is very nearly all bicarbonate, so the total carbonate follows from the alkalinity
/// and the water's own pH; adding strong acid converts bicarbonate to carbonic acid without changing
/// that total, and the fraction left at the target pH is what does not need neutralising.
/// </para>
/// <para>
/// The usual rule — neutralise the whole alkalinity — overstates the dose by about a quarter at
/// pH 5.8, because a fifth of the carbonate is still bicarbonate there.
/// </para>
/// <para>
/// <b>The model is for a closed vessel.</b> An open, aerated reservoir loses carbon dioxide, the
/// equilibrium shifts back, and pH climbs again over hours; the dose a grower ends up adding
/// approaches the full alkalinity. That is chemistry, not an error here, and it is worth stating
/// wherever this figure is shown.
/// </para>
/// </remarks>
public static class AcidDose
{
    /// <summary>The first dissociation of carbonic acid, at 25 °C.</summary>
    private const double FirstPka = 6.35;

    /// <summary>The second dissociation, bicarbonate to carbonate.</summary>
    private const double SecondPka = 10.33;

    /// <summary>
    /// The share of a water's total carbonate that is bicarbonate at a given pH.
    /// </summary>
    /// <param name="ph">The pH.</param>
    /// <returns>A fraction between zero and one.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the pH is outside 0–14.</exception>
    public static double BicarbonateFraction(double ph)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(ph);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(ph, 14);

        return 1 / (1 + Math.Pow(10, FirstPka - ph) + Math.Pow(10, ph - SecondPka));
    }

    /// <summary>
    /// Works out the acid a tank needs, and what that acid contributes.
    /// </summary>
    /// <param name="alkalinityMilliequivalentsPerLitre">The water's alkalinity, in meq/L.</param>
    /// <param name="waterPh">The pH of the untreated water.</param>
    /// <param name="targetPh">The pH to reach.</param>
    /// <param name="acid">The acid to use.</param>
    /// <param name="litres">The volume of the tank.</param>
    /// <returns>The plan. Zero throughout when nothing needs neutralising.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="acid"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the alkalinity is negative, a pH is outside 0–14, or the volume is not positive.
    /// </exception>
    public static AcidPlan Calculate(
        double alkalinityMilliequivalentsPerLitre,
        double waterPh,
        double targetPh,
        Acid acid,
        double litres)
    {
        ArgumentNullException.ThrowIfNull(acid);
        ArgumentOutOfRangeException.ThrowIfNegative(alkalinityMilliequivalentsPerLitre);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(litres);

        double residualFraction = BicarbonateFraction(targetPh);
        double waterFraction = BicarbonateFraction(waterPh);

        if (alkalinityMilliequivalentsPerLitre <= 0 || targetPh >= waterPh)
        {
            return new AcidPlan(0, 0, acid.NutrientSymbol, 0);
        }

        double totalCarbonate = alkalinityMilliequivalentsPerLitre / waterFraction;
        double freeProtons = Math.Pow(10, -targetPh) * 1000;
        double residual = totalCarbonate * residualFraction - freeProtons;
        double needed = Math.Max(0, alkalinityMilliequivalentsPerLitre - residual);

        return new AcidPlan(
            needed,
            needed * litres / acid.EquivalentsPerLitre,
            acid.NutrientSymbol,
            needed * acid.MilligramsOfNutrientPerMilliequivalent);
    }
}
```

- [ ] **Step 5: Run the tests to verify they pass**

Run: `dotnet test tests/SYT.NPKTools.Tests -c Release --filter "FullyQualifiedName~AcidDoseTests"`
Expected: PASS, 17 tests.

- [ ] **Step 6: Run the whole library suite**

Run: `dotnet test tests/SYT.NPKTools.Tests -c Release`
Expected: PASS. Nothing in Tasks 1–5 changes existing behaviour, so any existing failure is a
regression to fix before moving on.

- [ ] **Step 7: Format and commit**

```bash
dotnet format
git add src/SYT.NPKTools/Nutrients/Acid.cs src/SYT.NPKTools/Nutrients/AcidPlan.cs src/SYT.NPKTools/Nutrients/AcidDose.cs tests/SYT.NPKTools.Tests/AcidDoseTests.cs
git commit -m "feat(nutrients): work out the acid that neutralises a water's alkalinity"
```

---

### Task 6: A test project for the app

The calculator has no tests. The next three tasks change how its state is held, parsed and persisted,
which is precisely the kind of change that needs them.

**Files:**
- Create: `tests/SYT.NPKTools.Calculator.Tests/SYT.NPKTools.Calculator.Tests.csproj`
- Create: `tests/SYT.NPKTools.Calculator.Tests/CalculatorStateTests.cs`
- Modify: `SYT.NPKTools.slnx`

**Interfaces:**
- Consumes: the existing `CalculatorState` and `CalculatorModel`.
- Produces: a test project the following tasks add to.

A plain `net10.0` test project referencing the Blazor WebAssembly project builds and runs — verified
before this plan was written. `tests/Directory.Build.props` supplies the whole harness, so the
`.csproj` carries nothing else.

- [ ] **Step 1: Create the project file**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\..\web\SYT.NPKTools.Calculator\SYT.NPKTools.Calculator.csproj" />
  </ItemGroup>

</Project>
```

- [ ] **Step 2: Add it to the solution**

In `SYT.NPKTools.slnx`, inside the `/tests/` folder element, add:

```xml
    <Project Path="tests/SYT.NPKTools.Calculator.Tests/SYT.NPKTools.Calculator.Tests.csproj" />
```

- [ ] **Step 3: Write a test that pins today's behaviour**

```csharp
using AwesomeAssertions;
using SYT.NPKTools.Calculator;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers the three ways a setup is carried between sessions: local storage, a link, and a file.
/// </summary>
/// <remarks>
/// These are compatibility tests before they are anything else. A link someone saved is a promise, and
/// the version-2 format added later in this change must not break it.
/// </remarks>
public class CalculatorStateTests
{
    private static readonly string[] Catalogue =
        ["Calcium nitrate", "Potassium nitrate", "Magnesium sulfate"];

    /// <summary>
    /// A file written now reads back identically.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ToJson_RoundTrips()
    {
        CalculatorState state = new()
        {
            Target = "N=150 P=50 K=210 L=100",
            Water = new Dictionary<string, double> { ["Ca"] = 40 },
            Salts = ["Calcium nitrate"],
            ConcentrateLiters = 2,
        };

        CalculatorState? read = CalculatorState.FromJson(state.ToJson());

        read.Should().NotBeNull();
        read!.Target.Should().Be(state.Target);
        read.Water.Should().ContainKey("Ca").WhoseValue.Should().Be(40);
        read.Salts.Should().Equal("Calcium nitrate");
        read.ConcentrateLiters.Should().Be(2);
    }

    /// <summary>
    /// A link written now reads back identically, salts included.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ToFragment_RoundTrips()
    {
        CalculatorState state = new()
        {
            Target = "N=150 K=210 L=50",
            Water = new Dictionary<string, double> { ["Ca"] = 30 },
            Salts = ["Potassium nitrate"],
            ConcentrateLiters = 1,
        };

        (CalculatorState? read, bool saltsUsable) =
            CalculatorState.FromFragment(state.ToFragment(Catalogue), Catalogue);

        read.Should().NotBeNull();
        read!.Target.Should().Be(state.Target);
        read.Salts.Should().Equal("Potassium nitrate");
        saltsUsable.Should().BeTrue();
    }

    /// <summary>
    /// Text that is not a state file is an everyday event, not an error.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void FromJson_ForUnreadableText_ReturnsNull()
    {
        CalculatorState.FromJson("not json at all").Should().BeNull();
    }
}
```

`FromFragment` returns `(CalculatorState? State, bool SaltsUsable)`, not a bare state — the flag says
whether the catalogue has changed size since the link was written, which would make its salt indices
mean something else. Deconstruct it, as above.

- [ ] **Step 4: Run the tests**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release`
Expected: PASS, 3 tests.

- [ ] **Step 5: Confirm the solution picks the project up**

Run: `dotnet test SYT.NPKTools.slnx -c Release`
Expected: PASS, and the calculator tests appear in the run.

- [ ] **Step 6: Format and commit**

```bash
dotnet format
git add tests/SYT.NPKTools.Calculator.Tests SYT.NPKTools.slnx
git commit -m "test(calculator): give the app a test project and pin its state round-trips"
```

---

### Task 7: The target as fields

Make the fields the state and the string a projection of them.

**Files:**
- Modify: `web/SYT.NPKTools.Calculator/CalculatorModel.cs`
- Test: `tests/SYT.NPKTools.Calculator.Tests/TargetFieldsTests.cs`

**Interfaces:**
- Consumes: `ElementGroups.All` (Task 1), the existing `IPpmTargetParser`.
- Produces: on `CalculatorModel` — `IDictionary<string, double> TargetFields` (every symbol in
  `ElementGroups.All`, defaulting to 0), `double Liters`, and `string TargetText { get; set; }`
  now computed from them.

- [ ] **Step 1: Write the failing test**

```csharp
using AwesomeAssertions;
using SYT.NPKTools.Calculator;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers the target held as fields, with the string as a projection of them.
/// </summary>
/// <remarks>
/// The string stays the transport format — it is what a link and a file carry — so the two
/// representations have to agree in both directions. A field set that does not survive the trip
/// through a string would be silently lost the next time someone opened their own link.
/// </remarks>
public class TargetFieldsTests
{
    /// <summary>
    /// The default target is the one the app has always opened with.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TargetText_ByDefault_IsTheUsualStarterTarget()
    {
        CalculatorModel model = new();

        model.TargetFields["N"].Should().Be(150);
        model.TargetFields["K"].Should().Be(210);
        model.Liters.Should().Be(100);
    }

    /// <summary>
    /// Editing a field rewrites the string.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TargetText_FollowsTheFields()
    {
        CalculatorModel model = new();

        model.TargetFields["N"] = 175;

        model.TargetText.Should().Contain("N=175").And.NotContain("N=150");
    }

    /// <summary>
    /// Pasting a string rewrites the fields — the reason the string field is kept at all.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TargetText_WhenSet_RewritesTheFields()
    {
        CalculatorModel model = new();

        model.TargetText = "N=100 P=30 K=140 L=25";

        model.TargetFields["N"].Should().Be(100);
        model.TargetFields["P"].Should().Be(30);
        model.TargetFields["K"].Should().Be(140);
        model.Liters.Should().Be(25);
    }

    /// <summary>
    /// An element left out of the string is zero, not left at its previous value. Otherwise pasting a
    /// shorter target would silently keep parts of the old one.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TargetText_WhenSet_ClearsElementsItOmits()
    {
        CalculatorModel model = new();

        model.TargetText = "N=100 L=10";

        model.TargetFields["Ca"].Should().Be(0);
        model.TargetFields["S"].Should().Be(0);
    }

    /// <summary>
    /// Every field set survives the round trip through the string.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TargetText_RoundTripsEveryField()
    {
        CalculatorModel written = new();
        foreach ((string symbol, int index) in written.TargetFields.Keys.ToList().Select((s, i) => (s, i)))
        {
            written.TargetFields[symbol] = (index + 1) * 1.5;
        }

        written.Liters = 37.5;

        CalculatorModel read = new()
        {
            TargetText = written.TargetText,
        };

        read.Liters.Should().Be(37.5);
        foreach (string symbol in written.TargetFields.Keys)
        {
            read.TargetFields[symbol].Should().BeApproximately(written.TargetFields[symbol], 1e-9);
        }
    }

    /// <summary>
    /// A malformed string reports an error and leaves the table alone. Before this change a typo
    /// destroyed the whole target; now the table is the state and only the paste box can be wrong.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TargetText_WhenMalformed_ReportsAnErrorAndKeepsTheFields()
    {
        CalculatorModel model = new();
        double before = model.TargetFields["N"];

        model.TargetText = "N=oops K=210";

        model.Error.Should().NotBeNull();
        model.TargetFields["N"].Should().Be(before);
    }

    /// <summary>
    /// Zeros are left out of the string, so a link stays short and a file stays readable.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TargetText_OmitsZeroes()
    {
        CalculatorModel model = new();

        model.TargetText = "N=100 L=10";

        model.TargetText.Should().Be("N=100 L=10");
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release --filter "FullyQualifiedName~TargetFieldsTests"`
Expected: build failure — `TargetFields` does not exist.

- [ ] **Step 3: Change `CalculatorModel`**

Replace the `TargetText` auto-property with fields plus a projection. The rest of the class is
untouched in this task.

```csharp
    /// <summary>The target, element by element, in ppm. Keyed by symbol.</summary>
    /// <remarks>
    /// The state, rather than a view of it. A number field cannot hold a malformed value, so the only
    /// way to reach a parse error is the string box — and an error there no longer destroys what is
    /// already entered.
    /// </remarks>
    public Dictionary<string, double> TargetFields { get; } =
        ElementGroups.All.ToDictionary(symbol => symbol, _ => 0d, StringComparer.Ordinal);

    /// <summary>The reservoir volume, in litres.</summary>
    public double Liters { get; set; } = 100;

    /// <summary>
    /// The target as the parser accepts it — a projection of <see cref="TargetFields"/>.
    /// </summary>
    /// <remarks>
    /// Kept because it is the transport format: a link and a file both carry it, so a setup saved by
    /// any version stays readable. Setting it parses, and on failure records the error and leaves the
    /// fields as they were.
    /// </remarks>
    public string TargetText
    {
        get
        {
            IEnumerable<string> pairs = ElementGroups.All
                .Where(symbol => TargetFields[symbol] > 0)
                .Select(symbol => $"{symbol}={Format(TargetFields[symbol])}");

            return string.Join(' ', pairs.Append($"L={Format(Liters)}"));
        }

        set
        {
            PpmTarget parsed;
            try
            {
                parsed = _parser.Parse(value);
            }
            catch (Exception ex) when (ex is ArgumentException or FormatException or InvalidOperationException)
            {
                Error = ex.Message;
                return;
            }

            Error = null;
            Liters = parsed.Liters.Value;
            foreach (string symbol in ElementGroups.All)
            {
                TargetFields[symbol] = ValueOf(parsed, symbol);
            }
        }
    }

    private static string Format(double value) =>
        value.ToString("0.####", System.Globalization.CultureInfo.InvariantCulture);

    // PpmTarget names its properties by symbol, so the symbol is the property name.
    private static double ValueOf(PpmTarget target, string symbol) => symbol switch
    {
        "N" => target.N.Value,
        "P" => target.P.Value,
        "K" => target.K.Value,
        "Ca" => target.Ca.Value,
        "Mg" => target.Mg.Value,
        "S" => target.S.Value,
        "Fe" => target.Fe.Value,
        "Cu" => target.Cu.Value,
        "Mn" => target.Mn.Value,
        "Zn" => target.Zn.Value,
        "B" => target.B.Value,
        "Mo" => target.Mo.Value,
        "Cl" => target.Cl.Value,
        "Si" => target.Si.Value,
        "Se" => target.Se.Value,
        "Na" => target.Na.Value,
        _ => 0,
    };
```

Note that `WaterProfile` is the opposite — it names the same quantities `Nitrogen`, `Phosphorus`,
`Calcium` and so on. The two are not interchangeable.

Then, in the constructor, seed the starter target through the setter so there is one definition of it:

```csharp
        TargetText = "N=150 P=50 K=210 Ca=160 Mg=50 S=65 L=100";
```

And in `Recalculate`, replace the parse with a build from the fields — the string is no longer the
input:

```csharp
        PpmTarget target;
        try
        {
            target = _parser.Parse(TargetText);
        }
        catch (Exception ex) when (ex is ArgumentException or FormatException or InvalidOperationException)
        {
            Error = ex.Message;
            Target = null;
            return;
        }
```

stays as it is — `TargetText` is now generated, so it always parses, and the `catch` becomes a
guard rather than a live path. Leave it: a generated string that fails to parse is a bug worth
surfacing rather than crashing on.

Check the exact property names on `PpmTarget` before writing `ValueOf` — read
`src/SYT.NPKTools/Nutrients/PpmTarget.cs`. If a property is named differently, follow the source.

- [ ] **Step 4: Run the tests to verify they pass**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release --filter "FullyQualifiedName~TargetFieldsTests"`
Expected: PASS, 7 tests.

- [ ] **Step 5: Format and commit**

```bash
dotnet format
git add web/SYT.NPKTools.Calculator/CalculatorModel.cs tests/SYT.NPKTools.Calculator.Tests/TargetFieldsTests.cs
git commit -m "refactor(calculator): hold the target as fields and project the string from them"
```

---

### Task 8: The element grid, and two target cards

**Files:**
- Create: `web/SYT.NPKTools.Calculator/Components/ElementGrid.razor`
- Modify: `web/SYT.NPKTools.Calculator/Pages/Home.razor:11-25`
- Modify: `web/SYT.NPKTools.Calculator/wwwroot/css/app.css`

**Interfaces:**
- Consumes: `ElementGroups` (Task 1), `CalculatorModel.TargetFields` and `.Liters` (Task 7).
- Produces: `<ElementGrid Elements Values Step Changed />`, reused by the water panel in Task 9.

- [ ] **Step 1: Write the component**

```razor
@*
    One grid of ppm fields. Used four times — target macro, target micro, water macro, water micro —
    because two implementations of the same grid drift apart within a month.
*@

<div class="field-grid">
    @foreach (string element in Elements)
    {
        <div class="field">
            <label for="@($"{Prefix}-{element}")">@element</label>
            <input id="@($"{Prefix}-{element}")" type="number" min="0" step="@Step"
                   value="@Values[element]"
                   @onchange="e => OnChanged(element, e)" />
        </div>
    }
</div>

@code {
    /// <summary>The symbols to show, in order.</summary>
    [Parameter, EditorRequired]
    public IReadOnlyList<string> Elements { get; set; } = [];

    /// <summary>The values being edited, keyed by symbol.</summary>
    [Parameter, EditorRequired]
    public IDictionary<string, double> Values { get; set; } = new Dictionary<string, double>();

    /// <summary>Distinguishes this grid's input ids from the others on the page.</summary>
    [Parameter, EditorRequired]
    public string Prefix { get; set; } = string.Empty;

    /// <summary>The increment for the spinner. Micronutrients want a finer one.</summary>
    [Parameter]
    public string Step { get; set; } = "0.1";

    /// <summary>Raised with the symbol whose value changed.</summary>
    [Parameter]
    public EventCallback<string> Changed { get; set; }

    private async Task OnChanged(string element, ChangeEventArgs e)
    {
        Values[element] = Parse(e.Value);
        await Changed.InvokeAsync(element);
    }

    // Half-typed input should read as empty rather than throw, and the culture is invariant on
    // purpose: a comma decimal separator read as a thousands separator is a tenfold weighing error.
    private static double Parse(object? value) =>
        double.TryParse(
            value?.ToString(),
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out double parsed) && parsed >= 0
            ? parsed
            : 0;
}
```

- [ ] **Step 2: Replace the target card in `Home.razor`**

Replace the whole `<section class="card">` holding `<h2>Target</h2>` with:

```razor
        <section class="card">
            <h2>Target — macro</h2>
            <div class="field" style="margin-bottom:var(--grid-gap)">
                <label for="liters">Reservoir, litres</label>
                <input id="liters" type="number" min="0" step="1" value="@Model.Liters"
                       @onchange="OnLitersChanged" />
            </div>
            <ElementGrid Elements="ElementGroups.Macro" Values="Model.TargetFields"
                         Prefix="t" Step="1" Changed="_ => Edited()" />

            <details class="extra">
                <summary>As a string</summary>
                <input class="target-input" type="text" value="@Model.TargetText"
                       @onchange="OnTargetChanged" aria-label="Target nutrient profile in ppm" />
                <p class="card-note">
                    Element=ppm, separated by spaces. <code>L</code> is the reservoir volume in litres.
                    Paste one from a feed chart, or copy this one out.
                </p>
            </details>

            @if (Model.Error is not null)
            {
                <div class="notice error" role="alert">
                    <Icon Kind="IconKind.Error" />
                    <span><strong>Cannot solve.</strong> @Model.Error</span>
                </div>
            }
        </section>

        <section class="card">
            <h2>Target — micro</h2>
            <ElementGrid Elements="ElementGroups.Micro" Values="Model.TargetFields"
                         Prefix="t" Step="0.01" Changed="_ => Edited()" />
            <p class="card-note">Counter-ions — not dosed for, but a target may still name them.</p>
            <ElementGrid Elements="ElementGroups.CounterIons" Values="Model.TargetFields"
                         Prefix="t" Step="0.1" Changed="_ => Edited()" />
        </section>
```

Add `@using SYT.NPKTools.Nutrients` to `_Imports.razor` if it is not already there, and add the
litres handler to the `@code` block:

```csharp
    private Task OnLitersChanged(ChangeEventArgs e)
    {
        Model.Liters = Parse(e.Value);
        return Edited();
    }
```

- [ ] **Step 3: Add the CSS for the collapsed section**

Append to `app.css`, after the forms section:

```css
/* A collapsed escape hatch: the string format, kept out of the way of the table. */

.extra { margin-top: var(--grid-gap); }

.extra > summary {
  font-size: var(--text-xs);
  font-weight: 500;
  color: var(--text-muted);
  cursor: pointer;
  list-style: none;
}

.extra > summary::before { content: "▸ "; }
.extra[open] > summary::before { content: "▾ "; }
.extra > summary::-webkit-details-marker { display: none; }
.extra > * + * { margin-top: 6px; }
```

- [ ] **Step 4: Build and look at it**

```bash
dotnet build SYT.NPKTools.slnx -c Release
dotnet run --project web/SYT.NPKTools.Calculator
```

Open the address it prints. Check: the target is two cards; editing a macro field recalculates;
opening "As a string" shows the current target; pasting `N=100 P=30 K=140 L=25` into it updates the
table; the recipes on the right change with every edit.

- [ ] **Step 5: Format and commit**

```bash
dotnet format
git add web/SYT.NPKTools.Calculator
git commit -m "feat(calculator): enter the target as a table, macro and micro apart"
```

---

### Task 9: The water panel

**Files:**
- Create: `web/SYT.NPKTools.Calculator/Components/WaterPanel.razor`
- Modify: `web/SYT.NPKTools.Calculator/CalculatorModel.cs`
- Modify: `web/SYT.NPKTools.Calculator/Pages/Home.razor` — remove the water card, place `<WaterPanel />`
- Modify: `web/SYT.NPKTools.Calculator/wwwroot/css/app.css`
- Test: `tests/SYT.NPKTools.Calculator.Tests/WaterModeTests.cs`

**Interfaces:**
- Consumes: `WaterPreset`, `WaterEstimator`, `WaterEstimate` (Tasks 3–4), `ElementGrid` (Task 8).
- Produces: on `CalculatorModel` — `WaterInputMode Mode { get; set; }` (enum
  `Osmosis`, `Conductivity`, `ConductivityWithTests`, `Analysis`), `string WaterPresetId`,
  `double WaterEc`, `EcUnit WaterEcUnit` (enum `MilliSiemensPerCm`, `Ppm500`, `Ppm700`),
  `double? WaterGh`, `double? WaterKh`, and `WaterEstimate? WaterEstimate { get; private set; }`
  set by `Recalculate`.

- [ ] **Step 1: Write the failing test**

```csharp
using AwesomeAssertions;
using SYT.NPKTools.Calculator;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers the four ways a grower can describe their water.
/// </summary>
public class WaterModeTests
{
    /// <summary>
    /// Reverse osmosis is the default, and it deducts nothing.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_InOsmosisMode_LeavesTheTargetAlone()
    {
        CalculatorModel model = new();

        model.Recalculate();

        model.Mode.Should().Be(WaterInputMode.Osmosis);
        model.WaterProfile.Calcium.Value.Should().Be(0);
        model.Alkalinity.Should().Be(0);
    }

    /// <summary>
    /// A meter reading produces an analysis whose conductivity matches it.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_InConductivityMode_MatchesTheReading()
    {
        CalculatorModel model = new()
        {
            Mode = WaterInputMode.Conductivity,
            WaterPresetId = WaterPreset.CalciumBicarbonateModerate.Id,
            WaterEc = 0.45,
            WaterEcUnit = EcUnit.MilliSiemensPerCm,
        };

        model.Recalculate();

        model.WaterEstimate.Should().NotBeNull();
        model.WaterEstimate!.Feasible.Should().BeTrue();
        model.WaterProfile.EstimateConductivity().MicroSiemensPerCm
            .Should().BeApproximately(450, 5);
        model.WaterProfile.Calcium.Value.Should().BeGreaterThan(20);
    }

    /// <summary>
    /// A ppm meter says the same thing on a different scale. 0.45 mS/cm is 225 on the 500 scale and
    /// 315 on the 700 scale, and all three must land on the same water.
    /// </summary>
    [Theory]
    [InlineData(EcUnit.MilliSiemensPerCm, 0.45)]
    [InlineData(EcUnit.Ppm500, 225)]
    [InlineData(EcUnit.Ppm700, 315)]
    [Trait("Category", "Unit")]
    public void Recalculate_AcrossEcUnits_DescribesTheSameWater(EcUnit unit, double reading)
    {
        CalculatorModel model = new()
        {
            Mode = WaterInputMode.Conductivity,
            WaterPresetId = WaterPreset.CalciumBicarbonateModerate.Id,
            WaterEc = reading,
            WaterEcUnit = unit,
        };

        model.Recalculate();

        model.WaterProfile.EstimateConductivity().MicroSiemensPerCm
            .Should().BeApproximately(450, 5);
    }

    /// <summary>
    /// Drop tests are honoured exactly, and the mode is what decides whether they are read at all —
    /// values left over from a previous mode must not leak into this one.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_InConductivityMode_IgnoresHardnessEnteredForTheOtherMode()
    {
        CalculatorModel model = new()
        {
            Mode = WaterInputMode.Conductivity,
            WaterPresetId = WaterPreset.CalciumBicarbonateModerate.Id,
            WaterEc = 0.45,
            WaterGh = 2,
            WaterKh = 1,
        };

        model.Recalculate();

        model.WaterProfile.GeneralHardness().Should().BeGreaterThan(5);
    }

    /// <summary>
    /// With the tests read, both come back out of the estimate unchanged.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_WithTests_HonoursThem()
    {
        CalculatorModel model = new()
        {
            Mode = WaterInputMode.ConductivityWithTests,
            WaterPresetId = WaterPreset.CalciumBicarbonateModerate.Id,
            WaterEc = 0.45,
            WaterGh = 9,
            WaterKh = 7,
        };

        model.Recalculate();

        model.WaterProfile.GeneralHardness().Should().BeApproximately(9, 0.1);
        model.WaterProfile.CarbonateHardness().Should().BeApproximately(7, 0.1);
    }

    /// <summary>
    /// A typed-in analysis is used as typed, with no estimate involved.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_InAnalysisMode_UsesTheTypedFigures()
    {
        CalculatorModel model = new() { Mode = WaterInputMode.Analysis };
        model.Water["Ca"] = 42;

        model.Recalculate();

        model.WaterProfile.Calcium.Value.Should().Be(42);
        model.WaterEstimate.Should().BeNull();
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release --filter "FullyQualifiedName~WaterModeTests"`
Expected: build failure — `WaterInputMode` does not exist.

- [ ] **Step 3: Add the modes to `CalculatorModel`**

Add two enums in their own file, `web/SYT.NPKTools.Calculator/WaterInput.cs`:

```csharp
namespace SYT.NPKTools.Calculator;

/// <summary>How much a grower knows about their source water.</summary>
public enum WaterInputMode
{
    /// <summary>Reverse osmosis, distilled or rain. Nothing dissolved.</summary>
    Osmosis,

    /// <summary>A meter reading and a water type.</summary>
    Conductivity,

    /// <summary>A meter reading, a water type, and hardness drop tests.</summary>
    ConductivityWithTests,

    /// <summary>A full laboratory analysis.</summary>
    Analysis,
}

/// <summary>The scale a conductivity meter prints in.</summary>
/// <remarks>
/// A "ppm" meter shows conductivity multiplied by a fixed factor, and manufacturers do not agree on
/// the factor. Hanna uses 500, Truncheon 700, so the same water reads 225 on one and 315 on the other.
/// </remarks>
public enum EcUnit
{
    /// <summary>Millisiemens per centimetre.</summary>
    MilliSiemensPerCm,

    /// <summary>Parts per million on the 500 scale.</summary>
    Ppm500,

    /// <summary>Parts per million on the 700 scale.</summary>
    Ppm700,
}
```

Then in `CalculatorModel`, add the properties and rewrite `BuildWater`:

```csharp
    /// <summary>How the source water is being described.</summary>
    public WaterInputMode Mode { get; set; } = WaterInputMode.Osmosis;

    /// <summary>The chosen water shape, by <see cref="WaterPreset.Id"/>.</summary>
    public string WaterPresetId { get; set; } = WaterPreset.CalciumBicarbonateModerate.Id;

    /// <summary>The meter reading, in <see cref="WaterEcUnit"/>.</summary>
    public double WaterEc { get; set; }

    /// <summary>The scale the meter reading is in.</summary>
    public EcUnit WaterEcUnit { get; set; } = EcUnit.MilliSiemensPerCm;

    /// <summary>General hardness in °dH, or null when not measured.</summary>
    public double? WaterGh { get; set; }

    /// <summary>Carbonate hardness in °dKH, or null when not measured.</summary>
    public double? WaterKh { get; set; }

    /// <summary>The inferred analysis, or null when the water was not estimated.</summary>
    public WaterEstimate? WaterEstimate { get; private set; }

    /// <summary>The meter reading converted to µS/cm, whatever scale it was entered in.</summary>
    public double WaterMicroSiemensPerCm => WaterEcUnit switch
    {
        EcUnit.MilliSiemensPerCm => WaterEc * 1000,
        EcUnit.Ppm500 => WaterEc / 500 * 1000,
        EcUnit.Ppm700 => WaterEc / 700 * 1000,
        _ => 0,
    };

    private WaterProfile BuildWater()
    {
        WaterEstimate = null;

        switch (Mode)
        {
            case WaterInputMode.Osmosis:
                return WaterProfile.Pure;

            case WaterInputMode.Conductivity:
            case WaterInputMode.ConductivityWithTests:
                bool withTests = Mode == WaterInputMode.ConductivityWithTests;
                WaterPreset preset = WaterPreset.All.FirstOrDefault(p => p.Id == WaterPresetId)
                    ?? WaterPreset.CalciumBicarbonateModerate;

                WaterEstimate = WaterEstimator.Estimate(
                    preset,
                    WaterMicroSiemensPerCm,
                    withTests ? WaterGh : null,
                    withTests ? WaterKh : null);

                return WaterEstimate.Profile;

            default:
                WaterProfileBuilder builder = new();
                builder.AddNitrate(Water["N"]).AddP(Water["P"]).AddK(Water["K"])
                    .AddCa(Water["Ca"]).AddMg(Water["Mg"]).AddS(Water["S"])
                    .AddFe(Water["Fe"]).AddCu(Water["Cu"]).AddMn(Water["Mn"]).AddZn(Water["Zn"])
                    .AddB(Water["B"]).AddMo(Water["Mo"]).AddCl(Water["Cl"]).AddSi(Water["Si"])
                    .AddSe(Water["Se"]).AddNa(Water["Na"]);
                return builder.Build();
        }
    }
```

- [ ] **Step 4: Run the tests to verify they pass**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release --filter "FullyQualifiedName~WaterModeTests"`
Expected: PASS, 8 tests.

- [ ] **Step 5: Write `WaterPanel.razor`**

```razor
@inject CalculatorModel Model
@using SYT.NPKTools.Nutrients

<section class="card">
    <h2>Source water</h2>

    <div class="segmented" role="group" aria-label="How the water is described">
        @foreach ((WaterInputMode mode, string label) in Modes)
        {
            <button type="button" class="@(Model.Mode == mode ? "on" : null)"
                    @onclick="() => Choose(mode)">@label</button>
        }
    </div>

    @if (Model.Mode == WaterInputMode.Osmosis)
    {
        <p class="card-note">
            Reverse osmosis, distilled or rain. Nothing is deducted from the target.
        </p>
    }
    else if (Model.Mode == WaterInputMode.Analysis)
    {
        <p class="card-note">A laboratory analysis, in ppm. Macro first.</p>
        <ElementGrid Elements="ElementGroups.Macro" Values="Model.Water" Prefix="w"
                     Step="0.1" Changed="_ => Edited()" />
        <div class="group-divider">Micro</div>
        <ElementGrid Elements="ElementGroups.Micro" Values="Model.Water" Prefix="w"
                     Step="0.01" Changed="_ => Edited()" />
        <div class="group-divider">Counter-ions</div>
        <ElementGrid Elements="ElementGroups.CounterIons" Values="Model.Water" Prefix="w"
                     Step="0.1" Changed="_ => Edited()" />
    }
    else
    {
        <div class="field">
            <label for="w-preset">Water type</label>
            <select id="w-preset" value="@Model.WaterPresetId" @onchange="OnPresetChanged">
                @foreach (WaterPreset preset in WaterPreset.All)
                {
                    <option value="@preset.Id">@preset.Label</option>
                }
            </select>
        </div>

        <div class="field-grid" style="margin-top:var(--grid-gap)">
            <div class="field">
                <label for="w-ec">Meter reading</label>
                <input id="w-ec" type="number" min="0" step="0.01" value="@Model.WaterEc"
                       @onchange="OnEcChanged" />
            </div>
            <div class="field">
                <label for="w-unit">Scale</label>
                <select id="w-unit" value="@Model.WaterEcUnit" @onchange="OnUnitChanged">
                    <option value="@EcUnit.MilliSiemensPerCm">mS/cm</option>
                    <option value="@EcUnit.Ppm500">ppm (500)</option>
                    <option value="@EcUnit.Ppm700">ppm (700)</option>
                </select>
            </div>
        </div>

        @if (Model.Mode == WaterInputMode.ConductivityWithTests)
        {
            <div class="field-grid" style="margin-top:var(--grid-gap)">
                <div class="field">
                    <label for="w-gh">GH, °dH</label>
                    <input id="w-gh" type="number" min="0" step="0.5" value="@Model.WaterGh"
                           @onchange="OnGhChanged" />
                </div>
                <div class="field">
                    <label for="w-kh">KH, °dKH</label>
                    <input id="w-kh" type="number" min="0" step="0.5" value="@Model.WaterKh"
                           @onchange="OnKhChanged" />
                </div>
            </div>
            <p class="card-note">
                From a drop-test kit. Both optional — each one entered replaces a guess with a
                measurement.
            </p>
        }

        @if (Model.WaterEstimate is { } estimate)
        {
            <div class="group-divider">Estimated analysis — not an analysis</div>
            <div class="field-grid">
                @foreach ((string symbol, double value) in Estimated(estimate))
                {
                    <div class="field">
                        <label>@symbol</label>
                        <div class="readout">@value.ToString("F1")</div>
                    </div>
                }
            </div>

            @if (!estimate.Feasible)
            {
                <div class="notice warn" role="status">
                    <Icon Kind="IconKind.Warning" />
                    <span>
                        <strong>These readings describe no water.</strong>
                        The hardness entered already accounts for
                        @estimate.MicroSiemensPerCm.ToString("F0") µS/cm, more than the
                        @estimate.RequestedMicroSiemensPerCm.ToString("F0") µS/cm the meter read.
                        Check both, and the water type.
                    </span>
                </div>
            }
            else if (estimate.RelativeError > 0.2)
            {
                <div class="notice warn" role="status">
                    <Icon Kind="IconKind.Warning" />
                    <span>
                        <strong>The estimate is @((estimate.RelativeError * 100).ToString("F0"))% off
                        the reading.</strong> A different water type may fit better.
                    </span>
                </div>
            }
        }
    }
</section>

@code {
    /// <summary>Raised after any edit, so the page can recalculate and persist.</summary>
    [Parameter]
    public EventCallback Changed { get; set; }

    private static readonly (WaterInputMode Mode, string Label)[] Modes =
    [
        (WaterInputMode.Osmosis, "Osmosis"),
        (WaterInputMode.Conductivity, "EC"),
        (WaterInputMode.ConductivityWithTests, "EC + tests"),
        (WaterInputMode.Analysis, "Analysis"),
    ];

    private static IEnumerable<(string Symbol, double Value)> Estimated(WaterEstimate estimate) =>
    [
        ("Ca", estimate.Profile.Calcium.Value),
        ("Mg", estimate.Profile.Magnesium.Value),
        ("Na", estimate.Profile.Sodium.Value),
        ("S", estimate.Profile.Sulfur.Value),
        ("Cl", estimate.Profile.Chlorine.Value),
        ("N", estimate.Profile.Nitrogen.Nitrate),
    ];

    private Task Choose(WaterInputMode mode)
    {
        Model.Mode = mode;
        return Edited();
    }

    private Task OnPresetChanged(ChangeEventArgs e)
    {
        Model.WaterPresetId = e.Value?.ToString() ?? Model.WaterPresetId;
        return Edited();
    }

    private Task OnEcChanged(ChangeEventArgs e)
    {
        Model.WaterEc = Parse(e.Value) ?? 0;
        return Edited();
    }

    private Task OnUnitChanged(ChangeEventArgs e)
    {
        Model.WaterEcUnit = Enum.TryParse(e.Value?.ToString(), out EcUnit unit)
            ? unit
            : EcUnit.MilliSiemensPerCm;
        return Edited();
    }

    private Task OnGhChanged(ChangeEventArgs e)
    {
        Model.WaterGh = Parse(e.Value);
        return Edited();
    }

    private Task OnKhChanged(ChangeEventArgs e)
    {
        Model.WaterKh = Parse(e.Value);
        return Edited();
    }

    private Task Edited() => Changed.InvokeAsync();

    // Null rather than zero for a cleared field: an unmeasured drop test is not a measurement of zero,
    // and the estimator treats the two differently.
    private static double? Parse(object? value) =>
        double.TryParse(
            value?.ToString(),
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out double parsed) && parsed >= 0
            ? parsed
            : null;
}
```

- [ ] **Step 6: Wire it into `Home.razor`**

Delete the whole `<section class="card">` holding `<h2>Source water</h2>` and its handlers
`OnWaterChanged`, and put in its place:

```razor
        <WaterPanel Changed="Edited" />
```

Keep the water EC / alkalinity metrics and the "Water oversupplies" notice — move them into
`WaterPanel.razor` below the mode-specific inputs so all the water reporting is in one component.

- [ ] **Step 7: Add the CSS**

```css
/* ---------------------------------------------------------------- segmented control */

.segmented {
  display: flex;
  gap: 0;
  margin-bottom: var(--grid-gap);
  border: 1px solid var(--border);
  border-radius: 4px;
  overflow: hidden;
}

.segmented > button {
  flex: 1;
  min-height: 32px;
  padding: 0 8px;
  font-size: var(--text-xs);
  color: var(--text-muted);
  background: transparent;
  border: 0;
  border-radius: 0;
}

.segmented > button + button { border-left: 1px solid var(--border); }
.segmented > button.on { color: var(--on-primary); background: var(--primary); }

.group-divider {
  margin: var(--grid-gap) 0 6px;
  padding-top: 6px;
  font-size: var(--text-xs);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.06em;
  color: var(--text-muted);
  border-top: 1px solid var(--border);
}

.readout {
  min-height: 32px;
  padding: 4px 6px;
  font-family: var(--font-data);
  font-size: var(--text-sm);
  font-variant-numeric: tabular-nums;
  color: var(--text-muted);
  background: var(--muted);
  border: 1px solid var(--border);
  border-radius: 4px;
}

select {
  width: 100%;
  min-height: 32px;
  padding: 4px 6px;
  font-family: inherit;
  font-size: var(--text-sm);
  color: var(--text);
  background: var(--background);
  border: 1px solid var(--border);
  border-radius: 4px;
}

@media (pointer: coarse) {
  .segmented > button,
  select { min-height: 44px; }
}
```

- [ ] **Step 8: Build, run and check each mode**

```bash
dotnet build SYT.NPKTools.slnx -c Release
dotnet run --project web/SYT.NPKTools.Calculator
```

Check: switching to EC shows the type and reading; entering 0.45 mS/cm on the moderate preset shows
about Ca 52, Mg 10, Na 15; switching the scale to ppm (500) and entering 225 shows the same water;
"EC + tests" with GH 20, KH 14 and EC 0.1 shows the infeasible warning; "Analysis" shows three
grids and behaves as before.

- [ ] **Step 9: Format and commit**

```bash
dotnet format
git add web/SYT.NPKTools.Calculator tests/SYT.NPKTools.Calculator.Tests
git commit -m "feat(calculator): describe water by osmosis, meter, drop tests or analysis"
```

---

### Task 10: Acidification

**Files:**
- Create: `web/SYT.NPKTools.Calculator/Components/AcidPanel.razor`
- Modify: `web/SYT.NPKTools.Calculator/CalculatorModel.cs`
- Modify: `web/SYT.NPKTools.Calculator/Pages/Home.razor`
- Test: `tests/SYT.NPKTools.Calculator.Tests/AcidIntegrationTests.cs`

**Interfaces:**
- Consumes: `Acid`, `AcidDose`, `AcidPlan` (Task 5).
- Produces: on `CalculatorModel` — `bool AcidEnabled`, `string AcidId`, `double TargetPh`,
  `double WaterPh`, `AcidPlan? Acid { get; private set; }`, and the acid's nutrient folded into the
  deduction before the optimizer runs.

- [ ] **Step 1: Write the failing test**

```csharp
using AwesomeAssertions;
using SYT.NPKTools.Calculator;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers the acid dose and, more importantly, the nutrients it brings with it.
/// </summary>
/// <remarks>
/// The dose itself is covered in the library. What is covered here is that the calculator subtracts
/// what the acid contributes — a step every competing calculator treats as unrelated to feeding, and
/// the reason a nitric-acid grower's nitrogen runs a fifth high.
/// </remarks>
public class AcidIntegrationTests
{
    private static CalculatorModel HardWater() => new()
    {
        Mode = WaterInputMode.Conductivity,
        WaterPresetId = WaterPreset.CalciumBicarbonateModerate.Id,
        WaterEc = 0.471,
        AcidEnabled = true,
        AcidId = Acid.Nitric60.Id,
        WaterPh = 7.6,
        TargetPh = 5.8,
    };

    /// <summary>
    /// Water with no alkalinity needs no acid, whatever the setting says.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_OnOsmosisWater_PlansNoAcid()
    {
        CalculatorModel model = new() { AcidEnabled = true, AcidId = Acid.Nitric60.Id };

        model.Recalculate();

        model.Acid.Should().BeNull();
    }

    /// <summary>
    /// On alkaline water the plan appears, with the volume for the reservoir as entered.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_OnAlkalineWater_PlansTheDose()
    {
        CalculatorModel model = HardWater();

        model.Recalculate();

        model.Acid.Should().NotBeNull();
        model.Acid!.MilliequivalentsPerLitre.Should().BeApproximately(2.09, 0.05);
        model.Acid.Millilitres.Should().BeApproximately(16.05, 0.5);
        model.Acid.NutrientPpm.Should().BeApproximately(29.3, 0.5);
    }

    /// <summary>
    /// The nitrogen the acid carries is deducted from the target, so the salts supply the remainder
    /// rather than the whole. This is the point of the feature.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_DeductsWhatTheAcidContributes()
    {
        CalculatorModel with = HardWater();
        with.Recalculate();

        CalculatorModel without = HardWater();
        without.AcidEnabled = false;
        without.Recalculate();

        with.Recipes.Should().NotBeEmpty();
        without.Recipes.Should().NotBeEmpty();

        double nitrogenWith = with.Recipes[0].InTank.Nitrogen.Value;
        double nitrogenWithout = without.Recipes[0].InTank.Nitrogen.Value;

        // Both land on the 150 ppm target. The acidified one gets there with ~29 ppm less nitrogen
        // from salts, because the acid already supplied it — and the reported figure has to say so.
        with.Acid!.NutrientPpm.Should().BeGreaterThan(20);
        nitrogenWithout.Should().BeApproximately(150, 5);
        nitrogenWith.Should().BeApproximately(150, 5);
    }

    /// <summary>
    /// Turning it off removes the plan and the deduction together.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_WhenDisabled_PlansNothing()
    {
        CalculatorModel model = HardWater();
        model.AcidEnabled = false;

        model.Recalculate();

        model.Acid.Should().BeNull();
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release --filter "FullyQualifiedName~AcidIntegrationTests"`
Expected: build failure — `AcidEnabled` does not exist.

- [ ] **Step 3: Add the acid to `CalculatorModel`**

Properties:

```csharp
    /// <summary>Whether the water is being acidified.</summary>
    public bool AcidEnabled { get; set; }

    /// <summary>The chosen acid, by <see cref="Nutrients.Acid.Id"/>.</summary>
    public string AcidId { get; set; } = Nutrients.Acid.Nitric60.Id;

    /// <summary>The pH to bring the solution to.</summary>
    public double TargetPh { get; set; } = 5.8;

    /// <summary>The pH of the untreated water.</summary>
    public double WaterPh { get; set; } = 7.6;

    /// <summary>The acid plan, or null when no acid is needed or wanted.</summary>
    public AcidPlan? Acid { get; private set; }
```

Add a private field for the water the recipe is actually solved against:

```csharp
    /// <summary>
    /// The source water plus whatever the acid contributes.
    /// </summary>
    /// <remarks>
    /// Distinct from <see cref="WaterProfile"/>, which stays the water as it comes out of the tap:
    /// the alkalinity and the water EC shown on screen describe the untreated supply, and folding the
    /// acid into them would make both wrong. This is what the recipe is solved against and what the
    /// reported tank contents are measured from — an acid's nitrogen is in the reservoir exactly as
    /// the water's is.
    /// </remarks>
    private WaterProfile _effectiveWater = WaterProfile.Pure;
```

In `Recalculate`, after `Alkalinity` is computed and before `AdjustFor`:

```csharp
        Acid = null;
        _effectiveWater = WaterProfile;

        if (AcidEnabled && Alkalinity > 0)
        {
            Nutrients.Acid acid = Nutrients.Acid.All.FirstOrDefault(a => a.Id == AcidId)
                ?? Nutrients.Acid.Nitric60;

            Acid = AcidDose.Calculate(Alkalinity, WaterPh, TargetPh, acid, Liters);
            _effectiveWater = WithNutrient(WaterProfile, Acid.NutrientSymbol, Acid.NutrientPpm);
        }
```

and use it where `WaterProfile` was passed to `AdjustFor`:

```csharp
        WaterAdjustedTarget adjusted = target.AdjustFor(_effectiveWater);
```

**And in `Describe`**, which currently reads `Ppm inTank = fromSalts.Plus(WaterProfile);` — change it
to `_effectiveWater` too:

```csharp
        Ppm inTank = fromSalts.Plus(_effectiveWater);
```

Missing this second change is the subtle half of the bug. The optimizer would solve for a target
reduced by 29 ppm of nitrogen while the reported tank contents added back only the water, so the
recipe card would show 121 ppm of nitrogen for a solution that actually holds 150 — understating the
figure by exactly the amount this feature exists to account for.

Add the helper. It rebuilds rather than mutates, because `WaterProfile` is immutable:

```csharp
    /// <summary>
    /// The same water with one element raised — how an acid's own nutrient enters the deduction.
    /// </summary>
    /// <remarks>
    /// Rebuilt rather than mutated: a <see cref="WaterProfile"/> is immutable, and an acid contributes
    /// to the reservoir exactly as the water does, so it belongs in the same profile rather than in a
    /// separate adjustment the optimizer would have to be told about.
    /// </remarks>
    private static WaterProfile WithNutrient(WaterProfile water, string symbol, double ppm)
    {
        WaterProfileBuilder builder = new();
        builder
            .AddNitrate(water.Nitrogen.Nitrate + (symbol == "N" ? ppm : 0))
            .AddAmmonium(water.Nitrogen.Ammonium)
            .AddAmine(water.Nitrogen.Amine)
            .AddP(water.Phosphorus.Value + (symbol == "P" ? ppm : 0))
            .AddK(water.Potassium.Value)
            .AddCa(water.Calcium.Value)
            .AddMg(water.Magnesium.Value)
            .AddS(water.Sulfur.Value + (symbol == "S" ? ppm : 0))
            .AddFe(water.Iron.Value).AddCu(water.Copper.Value).AddMn(water.Manganese.Value)
            .AddZn(water.Zinc.Value).AddB(water.Boron.Value).AddMo(water.Molybdenum.Value)
            .AddCl(water.Chlorine.Value).AddSi(water.Silicon.Value).AddSe(water.Selenium.Value)
            .AddNa(water.Sodium.Value);

        return builder.Build();
    }
```

- [ ] **Step 4: Run the tests to verify they pass**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release --filter "FullyQualifiedName~AcidIntegrationTests"`
Expected: PASS, 4 tests.

- [ ] **Step 5: Write `AcidPanel.razor`**

```razor
@inject CalculatorModel Model
@using SYT.NPKTools.Nutrients

@if (Model.Alkalinity > 0)
{
    <section class="card">
        <h2>Acidification</h2>

        <label class="salt-row" style="padding-left:0">
            <input type="checkbox" checked="@Model.AcidEnabled" @onchange="OnEnabledChanged" />
            <span>Neutralise the alkalinity — @Model.Alkalinity.ToString("F2") meq/L</span>
        </label>

        @if (Model.AcidEnabled)
        {
            <div class="field" style="margin-top:var(--grid-gap)">
                <label for="acid">Acid</label>
                <select id="acid" value="@Model.AcidId" @onchange="OnAcidChanged">
                    @foreach (Acid acid in Acid.All)
                    {
                        <option value="@acid.Id">@acid.Label</option>
                    }
                </select>
            </div>

            <div class="field-grid" style="margin-top:var(--grid-gap)">
                <div class="field">
                    <label for="ph-water">Water pH</label>
                    <input id="ph-water" type="number" min="0" max="14" step="0.1"
                           value="@Model.WaterPh" @onchange="OnWaterPhChanged" />
                </div>
                <div class="field">
                    <label for="ph-target">Target pH</label>
                    <input id="ph-target" type="number" min="0" max="14" step="0.1"
                           value="@Model.TargetPh" @onchange="OnTargetPhChanged" />
                </div>
            </div>

            @if (Model.Acid is { } plan)
            {
                <div class="metrics" style="margin-top:8px">
                    <div class="metric">
                        <div class="label">Acid</div>
                        <div class="value">@plan.Millilitres.ToString("F1") <span class="unit">mL</span></div>
                    </div>
                    <div class="metric">
                        <div class="label">Strength</div>
                        <div class="value">@plan.MilliequivalentsPerLitre.ToString("F2") <span class="unit">meq/L</span></div>
                    </div>
                    <div class="metric">
                        <div class="label">Adds @plan.NutrientSymbol</div>
                        <div class="value">@plan.NutrientPpm.ToString("F0") <span class="unit">ppm</span></div>
                    </div>
                </div>

                @if (Overshoots(plan))
                {
                    <div class="notice warn" role="status">
                        <Icon Kind="IconKind.Warning" />
                        <span>
                            <strong>This acid overshoots the @plan.NutrientSymbol target on its own.</strong>
                            @plan.NutrientPpm.ToString("F0") ppm against a target of
                            @Model.TargetFields[plan.NutrientSymbol].ToString("F0"). Fertilizer only adds,
                            so no recipe can bring it back down.
                            @if (Fits() is { } better)
                            {
                                <text>@better.Label would fit.</text>
                            }
                        </span>
                    </div>
                }

                <p class="card-note">
                    The @plan.NutrientSymbol above is deducted from the target, like the water.
                    Worked for a closed vessel: an open reservoir loses CO₂ and drifts back up, so the
                    dose you settle on will be nearer the full @Model.Alkalinity.ToString("F2") meq/L.
                    The EC on each recipe still counts this water's bicarbonate, which the acid removes,
                    so it reads high once you have acidified — by roughly
                    @((Model.Alkalinity * 44.5).ToString("F0")) µS/cm.
                </p>
            }
        }
    </section>
}

@code {
    /// <summary>Raised after any edit, so the page can recalculate and persist.</summary>
    [Parameter]
    public EventCallback Changed { get; set; }

    private bool Overshoots(AcidPlan plan) =>
        Model.TargetFields.TryGetValue(plan.NutrientSymbol, out double target)
        && target > 0
        && plan.NutrientPpm > target;

    // The same neutralisation delivered by a different acid lands on a different element, so there is
    // usually one that fits. Naming it is more use than telling somebody to go and look.
    private Acid? Fits() =>
        Model.Acid is not { } plan
            ? null
            : Acid.All.FirstOrDefault(candidate =>
                Model.TargetFields.TryGetValue(candidate.NutrientSymbol, out double target)
                && target > 0
                && plan.MilliequivalentsPerLitre * candidate.MilligramsOfNutrientPerMilliequivalent
                    <= target);

    private Task OnEnabledChanged(ChangeEventArgs e)
    {
        Model.AcidEnabled = e.Value is true;
        return Changed.InvokeAsync();
    }

    private Task OnAcidChanged(ChangeEventArgs e)
    {
        Model.AcidId = e.Value?.ToString() ?? Model.AcidId;
        return Changed.InvokeAsync();
    }

    private Task OnWaterPhChanged(ChangeEventArgs e)
    {
        Model.WaterPh = Parse(e.Value, Model.WaterPh);
        return Changed.InvokeAsync();
    }

    private Task OnTargetPhChanged(ChangeEventArgs e)
    {
        Model.TargetPh = Parse(e.Value, Model.TargetPh);
        return Changed.InvokeAsync();
    }

    private static double Parse(object? value, double fallback) =>
        double.TryParse(
            value?.ToString(),
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out double parsed) && parsed is >= 0 and <= 14
            ? parsed
            : fallback;
}
```

- [ ] **Step 6: Place it in `Home.razor`**

Directly after `<WaterPanel Changed="Edited" />`:

```razor
        <AcidPanel Changed="Edited" />
```

- [ ] **Step 7: Build, run and check**

```bash
dotnet build SYT.NPKTools.slnx -c Release
dotnet run --project web/SYT.NPKTools.Calculator
```

Check: on osmosis water there is no acid card; on EC 0.45 moderate water it appears; ticking it shows
about 16 mL of 60% nitric and +29 ppm N; choosing phosphoric 85% raises the overshoot warning against
the default P target of 50.

- [ ] **Step 8: Format and commit**

```bash
dotnet format
git add web/SYT.NPKTools.Calculator tests/SYT.NPKTools.Calculator.Tests
git commit -m "feat(calculator): plan the acid for alkaline water and deduct what it adds"
```

---

### Task 11: Persist the new state

**Files:**
- Modify: `web/SYT.NPKTools.Calculator/CalculatorState.cs`
- Modify: `web/SYT.NPKTools.Calculator/CalculatorModel.cs` — `Capture` and `Apply`
- Test: `tests/SYT.NPKTools.Calculator.Tests/StateVersionTests.cs`

**Interfaces:**
- Consumes: everything added in Tasks 7, 9 and 10.
- Produces: `CalculatorState` fields `WaterMode`, `WaterPresetId`, `WaterEc`, `WaterEcUnit`,
  `WaterGh`, `WaterKh`, `AcidEnabled`, `AcidId`, `TargetPh`, `WaterPh`; fragment `v=2`.

- [ ] **Step 1: Write the failing test**

```csharp
using AwesomeAssertions;
using SYT.NPKTools.Calculator;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers carrying the new settings between sessions, and the old ones still arriving.
/// </summary>
/// <remarks>
/// A link someone saved is a promise. Version 1 links carry no water mode, so one has to be inferred,
/// and the only honest inference is what the link's own numbers say: water values mean an analysis,
/// no water values mean reverse osmosis.
/// </remarks>
public class StateVersionTests
{
    private static readonly string[] Catalogue = ["Calcium nitrate", "Potassium nitrate"];

    /// <summary>
    /// Everything entered survives a trip through a file.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Capture_ThenApply_RoundTripsTheWaterAndAcidSettings()
    {
        CalculatorModel written = new()
        {
            Mode = WaterInputMode.ConductivityWithTests,
            WaterPresetId = WaterPreset.SodiumExchangeSoftened.Id,
            WaterEc = 320,
            WaterEcUnit = EcUnit.Ppm500,
            WaterGh = 1.5,
            WaterKh = 9,
            AcidEnabled = true,
            AcidId = Acid.Phosphoric85.Id,
            WaterPh = 7.9,
            TargetPh = 6.1,
        };

        CalculatorModel read = new();
        read.Apply(CalculatorState.FromJson(written.Capture().ToJson())!);

        read.Mode.Should().Be(WaterInputMode.ConductivityWithTests);
        read.WaterPresetId.Should().Be(WaterPreset.SodiumExchangeSoftened.Id);
        read.WaterEc.Should().Be(320);
        read.WaterEcUnit.Should().Be(EcUnit.Ppm500);
        read.WaterGh.Should().Be(1.5);
        read.WaterKh.Should().Be(9);
        read.AcidEnabled.Should().BeTrue();
        read.AcidId.Should().Be(Acid.Phosphoric85.Id);
        read.WaterPh.Should().Be(7.9);
        read.TargetPh.Should().Be(6.1);
    }

    /// <summary>
    /// And through a link.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ToFragment_RoundTripsTheWaterSettings()
    {
        CalculatorModel written = new()
        {
            Mode = WaterInputMode.Conductivity,
            WaterPresetId = WaterPreset.CalciumBicarbonateHard.Id,
            WaterEc = 0.82,
            WaterEcUnit = EcUnit.MilliSiemensPerCm,
        };

        string fragment = written.Capture().ToFragment(Catalogue);
        (CalculatorState? carried, _) = CalculatorState.FromFragment(fragment, Catalogue);
        CalculatorModel read = new();
        read.Apply(carried!);

        read.Mode.Should().Be(WaterInputMode.Conductivity);
        read.WaterPresetId.Should().Be(WaterPreset.CalciumBicarbonateHard.Id);
        read.WaterEc.Should().BeApproximately(0.82, 1e-9);
    }

    /// <summary>
    /// A version 1 link with water values means someone typed an analysis in.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void FromFragment_ForAVersionOneLinkWithWater_InfersAnalysisMode()
    {
        CalculatorState state = new()
        {
            Target = "N=150 L=100",
            Water = new Dictionary<string, double> { ["Ca"] = 40 },
        };

        CalculatorModel read = new();
        read.Apply(state);

        read.Mode.Should().Be(WaterInputMode.Analysis);
        read.Water["Ca"].Should().Be(40);
    }

    /// <summary>
    /// A version 1 link with no water values means reverse osmosis, which is what it always meant.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void FromFragment_ForAVersionOneLinkWithoutWater_InfersOsmosis()
    {
        CalculatorState state = new() { Target = "N=150 L=100" };

        CalculatorModel read = new();
        read.Apply(state);

        read.Mode.Should().Be(WaterInputMode.Osmosis);
        read.AcidEnabled.Should().BeFalse();
    }

    /// <summary>
    /// An old link still opens: the salts and the target arrive as they always did.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void FromFragment_ReadsAVersionOneFragment()
    {
        (CalculatorState? read, _) = CalculatorState.FromFragment("v=1&t=N%3D150%20L%3D50", Catalogue);

        read.Should().NotBeNull();
        read!.Target.Should().Be("N=150 L=50");
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release --filter "FullyQualifiedName~StateVersionTests"`
Expected: FAIL — the new settings do not survive.

- [ ] **Step 3: Add the fields to `CalculatorState`**

```csharp
    /// <summary>How the water was described. Absent in version 1, where it is inferred.</summary>
    [JsonPropertyName("waterMode")]
    public string? WaterMode { get; set; }

    /// <summary>The chosen water shape.</summary>
    [JsonPropertyName("waterPreset")]
    public string? WaterPresetId { get; set; }

    /// <summary>The meter reading, in whatever scale <see cref="WaterEcUnit"/> names.</summary>
    [JsonPropertyName("waterEc")]
    public double? WaterEc { get; set; }

    /// <summary>The scale the meter reading is in.</summary>
    [JsonPropertyName("waterEcUnit")]
    public string? WaterEcUnit { get; set; }

    /// <summary>General hardness in °dH, when measured.</summary>
    [JsonPropertyName("waterGh")]
    public double? WaterGh { get; set; }

    /// <summary>Carbonate hardness in °dKH, when measured.</summary>
    [JsonPropertyName("waterKh")]
    public double? WaterKh { get; set; }

    /// <summary>Whether the water is being acidified.</summary>
    [JsonPropertyName("acidEnabled")]
    public bool? AcidEnabled { get; set; }

    /// <summary>The chosen acid.</summary>
    [JsonPropertyName("acidId")]
    public string? AcidId { get; set; }

    /// <summary>The pH to reach.</summary>
    [JsonPropertyName("targetPh")]
    public double? TargetPh { get; set; }

    /// <summary>The pH of the untreated water.</summary>
    [JsonPropertyName("waterPh")]
    public double? WaterPh { get; set; }
```

In `ToFragment`, change `v=1` to `v=2` and append the new keys, each only when it is not at its
default — the same rule the existing keys follow, so a link stays short:

```csharp
        builder.Append("v=2");
```

then, alongside the existing appends:

```csharp
        if (WaterMode is not null) { builder.Append("&wm=").Append(Uri.EscapeDataString(WaterMode)); }
        if (WaterPresetId is not null) { builder.Append("&wp=").Append(Uri.EscapeDataString(WaterPresetId)); }
        if (WaterEc is { } ec) { builder.Append("&we=").Append(ec.ToString("R", CultureInfo.InvariantCulture)); }
        if (WaterEcUnit is not null) { builder.Append("&wu=").Append(Uri.EscapeDataString(WaterEcUnit)); }
        if (WaterGh is { } gh) { builder.Append("&wg=").Append(gh.ToString("R", CultureInfo.InvariantCulture)); }
        if (WaterKh is { } kh) { builder.Append("&wk=").Append(kh.ToString("R", CultureInfo.InvariantCulture)); }
        if (AcidEnabled is true) { builder.Append("&ae=1"); }
        if (AcidId is not null) { builder.Append("&ay=").Append(Uri.EscapeDataString(AcidId)); }
        if (TargetPh is { } tph) { builder.Append("&ap=").Append(tph.ToString("R", CultureInfo.InvariantCulture)); }
        if (WaterPh is { } wph) { builder.Append("&aw=").Append(wph.ToString("R", CultureInfo.InvariantCulture)); }
```

In `FromFragment`, read the same keys, tolerating their absence exactly as the existing code does for
`t`, `w`, `s` and `c`. Accept both `v=1` and `v=2`; the version guard must not reject a version it
does not know, because the keys are already optional.

- [ ] **Step 4: Extend `Capture` and `Apply` in `CalculatorModel`**

`Capture` gains:

```csharp
        WaterMode = Mode.ToString(),
        WaterPresetId = WaterPresetId,
        WaterEc = WaterEc,
        WaterEcUnit = WaterEcUnit.ToString(),
        WaterGh = WaterGh,
        WaterKh = WaterKh,
        AcidEnabled = AcidEnabled,
        AcidId = AcidId,
        TargetPh = TargetPh,
        WaterPh = WaterPh,
```

`Apply` gains, after the existing water loop:

```csharp
        // Version 1 carried no mode. What the link itself says is the only honest inference: water
        // values mean somebody typed an analysis, and no water values mean they used osmosis.
        Mode = Enum.TryParse(state.WaterMode, out WaterInputMode mode)
            ? mode
            : Water.Values.Any(v => v > 0) ? WaterInputMode.Analysis : WaterInputMode.Osmosis;

        if (state.WaterPresetId is not null
            && WaterPreset.All.Any(p => p.Id == state.WaterPresetId))
        {
            WaterPresetId = state.WaterPresetId;
        }

        WaterEc = state.WaterEc ?? 0;
        WaterEcUnit = Enum.TryParse(state.WaterEcUnit, out EcUnit unit) ? unit : EcUnit.MilliSiemensPerCm;
        WaterGh = state.WaterGh;
        WaterKh = state.WaterKh;

        AcidEnabled = state.AcidEnabled ?? false;
        if (state.AcidId is not null && Nutrients.Acid.All.Any(a => a.Id == state.AcidId))
        {
            AcidId = state.AcidId;
        }

        TargetPh = state.TargetPh ?? 5.8;
        WaterPh = state.WaterPh ?? 7.6;
```

An unknown preset or acid id is dropped rather than accepted, matching how unknown salt names are
already handled.

- [ ] **Step 5: Run the tests to verify they pass**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release`
Expected: PASS, all tests including the ones from Task 6.

- [ ] **Step 6: Format and commit**

```bash
dotnet format
git add web/SYT.NPKTools.Calculator tests/SYT.NPKTools.Calculator.Tests
git commit -m "feat(calculator): carry the water and acid settings in links and files"
```

---

### Task 12: Whole-solution verification and the changelog

**Files:**
- Modify: `CHANGELOG.md`

- [ ] **Step 1: Format, build and test everything**

```bash
dotnet format --verify-no-changes
dotnet build SYT.NPKTools.slnx -c Release
dotnet test SYT.NPKTools.slnx -c Release
```

Expected: all three clean. `--verify-no-changes` failing means a commit went out unformatted; run
`dotnet format` and amend.

- [ ] **Step 2: Verify the published site builds**

The Pages workflow publishes rather than builds, and publishing relinks the runtime, which is a step
CI never exercises.

```bash
dotnet workload install wasm-tools
dotnet publish web/SYT.NPKTools.Calculator -c Release -o /tmp/npk-publish
```

Expected: succeeds, and `/tmp/npk-publish/wwwroot/index.html` exists.

- [ ] **Step 3: Add the changelog entry**

`CHANGELOG.md` has no unreleased heading — its top entry is `## [1.0.0-preview.3] - 2026-08-01`.
Create one above it, then add the entry, following the format already there:

```markdown
## [Unreleased]

### Added

- Source water can be described by a meter reading and an optional hardness drop test, not only by a
  full laboratory analysis. Four presets cover ordinary water classes, including softened water,
  where high conductivity and absent calcium would otherwise be read as hardness.
- The acid needed to neutralise a water's alkalinity, from the carbonate equilibrium rather than a
  rule of thumb, with the nitrogen, phosphorus or sulfur it contributes deducted from the target.
- `WaterPreset`, `WaterEstimator`, `AcidDose` and `ElementGroups` in `SYT.NPKTools`, and
  `GeneralHardness` / `CarbonateHardness` on `WaterProfile`.

### Changed

- The target is entered as a table, macro and micro in separate cards. The string format is kept,
  collapsed, and still what links and files carry.
```

- [ ] **Step 4: Commit**

```bash
git add CHANGELOG.md
git commit -m "docs: record the water estimation and table input changes"
```

---

## Self-review

**Spec coverage.** Target as a table → Task 7 (state) and Task 8 (interface). Macro/micro apart →
Tasks 1, 8, 9. Four water modes → Task 9. EC unit scales → Task 9. Presets → Task 3. Estimator with
GH/KH pinning and the feasibility flag → Task 4. Acid dose, the acid table, the nutrient deduction
and the overshoot warning → Tasks 5 and 10. Persistence with `v=1` compatibility → Task 11. The
"what this is not" section needs no task by definition.

**Type consistency.** `WaterEstimate.Profile/MicroSiemensPerCm/RequestedMicroSiemensPerCm/Feasible/RelativeError`
are used in Tasks 4 and 9 with the same names. `AcidPlan.MilliequivalentsPerLitre/Millilitres/NutrientSymbol/NutrientPpm`
likewise in Tasks 5, 10 and 11. `Acid.Id` is used as the persisted key in Tasks 5, 10 and 11.
`ElementGroups.Macro/Micro/CounterIons/All` in Tasks 1, 7, 8 and 9.
`CalculatorModel.Mode/WaterPresetId/WaterEc/WaterEcUnit/WaterGh/WaterKh` in Tasks 9 and 11.

**Known soft spots**, called out rather than hidden:

- `CalculatorModel.Acid` is a property named the same as the `Acid` type, so inside the model the
  type needs qualifying as `Nutrients.Acid`. The plan does this; do not "tidy" it away.
- `PpmTarget`'s property names are assumed in `ValueOf` (Task 7). Read the source rather than trust
  the list.
- `IonBalance`'s per-ion properties are assumed to be milliequivalents in Task 2. Read the source.
- No component-level tests: there is no bUnit in this repo and adding one is a larger decision than
  this change should make. Everything testable was pushed into the model and the library, and each UI
  task ends with a specific list of things to look at in the running app.
