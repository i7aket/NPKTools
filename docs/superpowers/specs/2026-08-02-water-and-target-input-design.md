# Target input, macro/micro separation, and knowing your water

Date: 2026-08-02
Status: approved

## Why

Three complaints, one root cause: the calculator asks for what it needs internally rather than
for what a grower actually has.

1. **The target is one text field.** `N=150 P=50 K=210 Ca=160 Mg=50 S=65 L=100` is a good transport
   format and a poor input control. A typo fails the whole calculation.
2. **Macro and micro are one undifferentiated block.** Sixteen fields in one grid, in an order that
   means nothing to anyone reading it. The salt picker already groups them; the inputs do not.
3. **Source water can only be entered as a full laboratory analysis.** Almost nobody has one. What
   growers do have is an EC meter, and often a drop-test kit for hardness. Right now that knowledge
   has nowhere to go, so the honest options are "pretend it is reverse osmosis" or "make up numbers".

The third is the one worth solving properly. HydroBuddy and the store calculators offer a water
composition field and nothing else — no estimation, no presets. A calculator that turns a meter
reading into a defensible composition is doing something none of them do.

## What this is not

- Not a temperature correction for EC. Meters compensate to 25 °C and report as if at 25 °C, which
  is what the library already models.
- Not carbonate (CO₃²⁻) speciation. It matters above pH 8.3; source water is below that.
- Not iron or manganese in well water. Both oxidise and precipitate on aeration, so they do not
  reach the plant and should not be subtracted from a target.

---

## 1. Target: two cards, table input

### Layout

**Card "Target — macro"** — reservoir volume in litres, then N, P, K, Ca, Mg, S.
**Card "Target — micro"** — Fe, Mn, Zn, B, Cu, Mo, Cl, Si, Se, Na.

Below the macro card, a collapsed `<details>` labelled "As a string" holds the existing text input.
It is bidirectional: typing or pasting a string rewrites the table, editing the table rewrites the
string.

### Source of truth moves

Today `CalculatorModel.TargetText` is the state and is reparsed on every recalculation. It becomes
the other way round: the fields are the state, and the string is projected from them.

- `TargetFields` — `Dictionary<string, double>` keyed by element symbol, plus `Liters`.
- `TargetText` — a computed property. The getter formats the fields in catalogue order, omitting
  zeros, always appending `L=`. The setter parses through the existing `IPpmTargetParser` and, on
  success, replaces the fields; on failure it sets `Error` and leaves the fields untouched.

This keeps `CalculatorState.Target` a string, so **links and files written by the current version
keep working unchanged**. It also removes a class of failure: a number input cannot hold a malformed
value, so the only way to reach a parse error is the collapsed string field, and an error there no
longer destroys what is in the table.

### Round-trip requirement

`Parse(Format(fields)) == fields` for every field set the UI can produce. Formatting uses invariant
culture and trims trailing zeros.

---

## 2. Macro and micro, everywhere

One component, `ElementGrid`, renders a labelled grid of numeric ppm fields. It is used four times:
target macro, target micro, water macro, water micro. Two implementations of the same grid would
drift apart within a month.

```
ElementGrid
  Elements   IReadOnlyList<string>   symbols, in display order
  Values     IDictionary<string, double>
  Step       double                  0.1 for macro, 0.01 for micro
  Changed    EventCallback<string>   fires with the symbol that changed
```

Element groups are defined once, in the library, so the UI grouping and the salt grouping cannot
disagree. There are **three** groups, not two — a distinction the existing code already makes and
the UI currently loses:

- `Macro` — N, P, K, Ca, Mg, S
- `Micro` — Fe, Cu, Mn, Zn, B, Mo, Si, Se — the micronutrients that are actually dosed
- `CounterIons` — Cl, Na — arrive with other salts rather than being dosed for

`FertilizerBundleGenerator.MicroElements` deliberately omits Cl and Na, because reporting them as
uncovered would be noise. But a target and a water analysis both accept all sixteen, so the input
grid must show them. Folding them into `Micro` would contradict the generator; leaving them out
would make sodium unenterable — which matters most for exactly the softened water this change is
about. So they are their own group, shown in the micro card below a divider.

A test asserts the three groups are disjoint and together equal the sixteen symbols
`PpmTargetParser` accepts, so adding an element to the library without placing it in a group breaks
the build rather than silently disappearing from the interface.

Water in "analysis" mode renders the same two groups as two bordered sub-blocks inside the water
card, matching the two target cards visually.

---

## 3. Source water: a ladder of precision

A segmented control with four rungs. Each rung removes one assumption.

| Mode | Inputs | Assumed |
|---|---|---|
| Reverse osmosis | none | nothing; water is zero |
| EC | water type, EC | the whole composition — preset shape, scaled to EC |
| EC + tests | water type, EC, GH, KH | Ca:Mg split, and the Na:Cl:SO₄ split |
| Analysis | 16 ppm fields | nothing |

Default is reverse osmosis, as today.

### EC units

The EC field carries a unit selector: **mS/cm · ppm-500 · ppm-700**. `ppm-500 = mS/cm × 500`,
`ppm-700 = mS/cm × 700`. Two scales because Hanna and Truncheon meters print different numbers for
the same water, and growers quote whichever their meter shows. This is what answers "I only know
the ppm".

Hardness fields take **°dH** with a `ppm CaCO₃` alternative. `1 °dH = 0.3567 meq/L = 17.85 ppm CaCO₃`.

### The presets

A preset is a *shape* — the proportions between ions — not a fixed composition. Values below are the
nominal scale; the derived columns are computed with this library's own conductivity model, and land
in the standard textbook ranges for each water class.

| Id | UI label | Ca | Mg | Na | S | Cl | N | EC µS/cm | HCO₃ ppm | GH °dH | KH °dKH |
|---|---|--:|--:|--:|--:|--:|--:|--:|--:|--:|--:|
| `SoftLowAlkalinity` | Soft, low alkalinity | 18 | 4 | 8 | 4 | 9 | 1 | 173 | 61 | 3.4 | 2.8 |
| `CalciumBicarbonateModerate` | Calcium bicarbonate, moderately hard | 55 | 11 | 16 | 13 | 21 | 3 | 471 | 166 | 10.2 | 7.6 |
| `CalciumBicarbonateHard` | Calcium bicarbonate, hard | 105 | 22 | 28 | 32 | 38 | 5 | 892 | 295 | 19.8 | 13.6 |
| `SodiumExchangeSoftened` | Softened by sodium exchange | 4 | 1 | 120 | 14 | 30 | 2 | 559 | 222 | 0.8 | 10.2 |

The interface stays English, as every existing string in it is. Mixing languages in one card would
read as a bug, and translating the whole app is a separate piece of work.

The fourth is not there for completeness. A domestic softener swaps calcium for sodium: EC stays
high while calcium is gone. Estimated as "hard", such water yields a recipe that underfeeds calcium
into an already sodium-loaded solution — the worst of the common mistakes, and one worth being able
to name.

Nitrogen is nitrate. HCO₃ is never entered: the library derives it from the cation surplus
(`WaterProfileExtensions.EstimatedAlkalinity`), which is what makes a six-number shape sufficient.

### The estimator

`WaterEstimator.Estimate(preset, targetEc, gh?, kh?) -> WaterEstimate`

```
pinnedCaMg  = gh is null ? null : gh * 0.3567 meq, split by the preset's Ca:Mg meq ratio
pinnedAlk   = kh is null ? null : kh * 0.3567 meq

profile(k):
    Ca, Mg          = pinnedCaMg ?? k * preset(Ca, Mg)
    N, S, Cl, Na    = k * preset(N, S, Cl, Na)
    if pinnedAlk is not null:
        delta = pinnedAlk - (cationMeq - anionMeq)
        delta > 0  ->  Na += delta * 22.990     # close the balance on sodium
        delta < 0  ->  Cl += -delta * 35.450    # close the balance on chloride
    return profile

solve k by bisection over [0, 100] so that EstimateConductivity(profile(k)) == targetEc
```

Three things make this work:

- **EC is monotone in k.** Every term of the conductivity sum grows with k, and so does whichever
  ion closes the balance. Bisection is therefore exact to any tolerance; 60 iterations is far more
  than needed and still free.
- **Closing on Na or Cl is what a real analysis does.** A laboratory report is closed on the ion it
  measured least well. Sodium and chloride are precisely the two this app knows least about, so the
  slack belongs there rather than smeared over calcium.
- **Over-determination resolves itself.** With GH and KH pinned, the free part may be driven to zero
  and the EC still overshoot. That means the entered numbers disagree, and the estimator says so —
  `Feasible = false`, k clamped to 0 — instead of quietly picking one of them.

Validation, in the UI: if the recomputed EC differs from the entered EC by more than 20%, warn.
Never silently adjust.

The preset also self-checks. Softened water needs a cation surplus of `KH − GH = +3.36 meq`; the
sodium shape supplies +3.5 meq per unit of scale, and the calcium shapes supply a negative surplus.
Choose the wrong preset for softened water and the estimate is visibly wrong rather than plausibly
wrong.

### Output

The estimate is always displayed element by element, labelled as an estimate, alongside the derived
alkalinity and the recomputed EC. It feeds `AdjustFor` exactly as a typed-in analysis does.

---

## 4. Acidification

Shown whenever alkalinity > 0.

### Inputs

- Acid: nitric 60% / nitric 38% / phosphoric 85% / phosphoric 75% / sulfuric 98% / sulfuric 37% /
  custom (%w/w + density).
- Target pH, default 5.8.
- Water pH, optional, default 7.6.

Water pH is the one place where a pH reading genuinely carries information: it locates the water on
its titration curve. It says nothing about composition, which is why it is not an input anywhere
else.

### The calculation

Carbonate equilibrium, pKa₁ = 6.35, pKa₂ = 10.33:

```
α₁(pH)  = 1 / (1 + 10^(6.35 - pH) + 10^(pH - 10.33))
C_T     = Alk / α₁(pH_water)                       # total carbonate, conserved
acid    = Alk - C_T · α₁(pH_target) + [H⁺]_target  # meq/L
```

Worked example, moderately hard water (HCO₃ 166 ppm → Alk 2.72 meq/L), pH 7.6 → 5.8:
`α₁(7.6) = 0.945`, `C_T = 2.878 mM`, `α₁(5.8) = 0.220`, **acid = 2.09 meq/L** — 77% of alkalinity,
not the 100% most guides quote.

### Acid table

Equivalent weight assumes the protons actually available at pH 5.8: one for nitric, one for
phosphoric (pKa₂ = 7.20 is out of reach), two for sulfuric (pKa₂ = 1.99 is fully dissociated).

| Acid | %w/w | density g/mL | eq/L | contributes per meq |
|---|--:|--:|--:|---|
| Nitric | 60 | 1.367 | 13.02 | 14.007 mg N |
| Nitric | 38 | 1.234 | 7.44 | 14.007 mg N |
| Phosphoric | 85 | 1.685 | 14.62 | 30.974 mg P |
| Phosphoric | 75 | 1.579 | 12.08 | 30.974 mg P |
| Sulfuric | 98 | 1.836 | 36.70 | 16.030 mg S |
| Sulfuric | 37 | 1.276 | 9.63 | 16.030 mg S |

`mL for the tank = acid_meq_per_L × litres / (eq/L) / 1000 × 1000`

For the worked example in 100 L with 60% nitric: 16.1 mL, contributing **29.3 ppm N**.

### Integration

The nutrients an acid carries are subtracted from the target alongside the water. Missing them is a
29 ppm error in nitrogen on a 150 ppm target — a fifth of the nitrogen, from a step most calculators
treat as unrelated to feeding.

There is no circular dependency: the dose follows from alkalinity alone, never from the recipe. The
chain is `water → alkalinity → acid dose → acid nutrients → combined deduction → optimizer`.

**Warn when the acid overshoots the target it feeds.** The same 2.09 meq/L delivered as phosphoric
acid is 64.7 ppm of phosphorus, against a typical target of 50 — the acid alone overshoots the whole
phosphorus target before a single salt is weighed, and no recipe can bring it back down. Sulfuric
gives 33.5 ppm S against a 65 ppm target, which is merely large. Nitric's 29.3 ppm N against 150 is
comfortable. So the check is per element and against the entered target, not a fixed rule: if an
acid's contribution exceeds its element's target, say so and name the acid that would not.

### The caveat, stated in the UI

The model is for a closed vessel. An open, aerated reservoir loses CO₂, the pH drifts back up, and
the practical dose approaches the full alkalinity. This is a real limit of the chemistry, not of the
implementation, and it is better said than hidden.

---

## Components

Domain logic goes in the library, not the UI. It is chemistry, it needs tests, and it is useful to
anyone consuming the package.

| File | Responsibility |
|---|---|
| `src/SYT.NPKTools/Nutrients/WaterPreset.cs` | the four shapes, as data |
| `src/SYT.NPKTools/Nutrients/WaterEstimator.cs` | shape + EC + GH/KH → `WaterProfile` |
| `src/SYT.NPKTools/Nutrients/WaterEstimate.cs` | the result: profile, recomputed EC, feasibility |
| `src/SYT.NPKTools/Nutrients/AcidDose.cs` | carbonate equilibrium, volume, nutrient contribution |
| `src/SYT.NPKTools/Nutrients/Acid.cs` | the acid table, and a custom acid |
| `src/SYT.NPKTools/Nutrients/ElementGroups.cs` | macro/micro symbol lists, one definition |
| `web/.../Components/ElementGrid.razor` | one grid, four uses |
| `web/.../Components/WaterPanel.razor` | the four modes; lifted out of `Home.razor` |
| `web/.../Components/AcidPanel.razor` | acid inputs and dose |
| `web/.../CalculatorModel.cs` | fields as truth; water by mode; acid in the deduction chain |
| `web/.../CalculatorState.cs` | `v=2`; `v=1` still readable |
| `web/.../Pages/Home.razor` | composition only, no calculation |
| `tests/SYT.NPKTools.Calculator.Tests/` | new project; the app has none, and target round-tripping and state versioning need one. A plain `net10.0` test project can reference the Blazor WebAssembly project — verified. |

`Home.razor` is 237 lines and holds the water grid, the salt picker, the concentrate field and all
their handlers. Lifting the water block out is not incidental tidying — the water block roughly
triples in this change and cannot stay inline.

## Persistence

`CalculatorState` gains: `waterMode`, `waterPreset`, `waterEc`, `waterEcUnit`, `waterGh`,
`waterKh`, `acid` (`{ type, percent, density, targetPh, waterPh }`). Link fragment keys stay short:
`wm wp we wu wg wk ay ap aw`.

The fragment version goes to `v=2`. A `v=1` link or an old file is read as before, with the mode
inferred: **analysis** if any water value is non-zero, **reverse osmosis** otherwise; acid off. A
`v=2` reader must tolerate missing keys, as `v=1` already does.

## Tests

- Each preset produces its documented EC, HCO₃, GH and KH, to the precision printed in the table.
- `Estimate` reproduces the entered EC within 0.5% whenever feasible.
- GH and KH, when given, come back out of the estimated profile unchanged.
- Softened-water shape resolves the positive cation surplus; a calcium shape given the same GH/KH
  reports infeasible or is visibly driven to a different composition.
- GH/KH implying more EC than entered → `Feasible == false`, nothing silently adjusted.
- Acid dose reproduces the worked example: 2.09 meq/L, 16.1 mL of 60% nitric per 100 L, 29.3 ppm N.
- Every acid in the table: eq/L matches %w/w × density ÷ equivalent weight.
- Target string round-trips through the field table.
- `v=1` state loads with the inferred mode; `v=2` round-trips.
