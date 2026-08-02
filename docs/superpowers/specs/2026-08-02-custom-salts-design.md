# Salts the grower owns but the catalogue does not

Date: 2026-08-02
Status: approved

## Why

The salt picker lists 34 built-in fertilizers and offers no way to add a thirty-fifth. A grower
whose shelf holds anything else — a local brand, a shop blend, a chelate the catalogue omits — can
tick boxes but cannot describe what they actually have. The calculator then solves for a shelf that
is not theirs.

## The shape of the problem

A custom salt must become the same `Fertilizer` the library already uses. That is the whole design
constraint, and satisfying it means **nothing downstream changes**: the bundle generator, the
optimizer, the ppm calculation and the A/B concentrate split all take `Fertilizer` and neither know
nor care where it came from.

What a `Fertilizer` needs: a name, a formula string, a `ConcentrateType`, and percentages by weight
split **by form** — nitrogen as nitrate, ammonium or amide; calcium, magnesium and the micros as
chelated or not. The form matters: it drives the ion balance, the acid-base character and the
conductivity estimate, so collapsing it would quietly degrade three separate readings.

## Two ways in

### Formula (the default)

Name, formula, concentrate type. The formula is parsed, percentages computed from atomic masses,
and forms assigned by what the formula contains:

| Group in the formula | Form it becomes |
|---|---|
| NO₃ | nitrate nitrogen |
| NH₄ | ammonium nitrogen |
| CO(NH₂)₂ | amide nitrogen |
| a metal, plainly bound | non-chelated |

The breakdown is shown before saving and can be corrected by hand.

### Percentages (the fallback)

The library's own fields. This is for blends — where there is no single formula — and for chelates.

**Chelates are the honest limit of the formula path.** Fe-EDTA has a formula, but writing it out
gains nothing: what the library needs is which chelating agent, and the builder already distinguishes
`AddFeEdta`, `AddFeDtpa`, `AddFeEddha`, `AddFeHbed` and `AddFeOrthoPart`. So chelated micros are
entered on the percentages tab, where the agent is chosen explicitly. Most of a shelf is simple
salts; most of its micronutrients are not.

## The formula parser

`SYT.NPKTools/Fertilizers/ChemicalFormula.cs`.

Understands element symbols, plain and Unicode subscripts — the catalogue mixes them, as in
`Ca(NO₃)2*4H₂O` — bracketed groups with multipliers, and hydrates joined by `*` or `·`. Anything
else is rejected with a message naming the offending position rather than a generic failure.

**It arrives with a test set nobody had to write.** The catalogue's declared percentages were
themselves computed from its formulas: Ca(NO₃)₂·4H₂O gives M = 236.146, Ca = 16.972%, N = 11.863%,
which is what `FertilizerCollectionBuilder` declares to the third decimal. Checked across the whole
catalogue before this spec was written: **43 declared percentages, every one matching within 0.02
percentage points, zero mismatches.** The parser is therefore validated against 43 real answers that
already live in the repository. Chelated salts are outside this set, since their formulas are not
written in the same plain form.

## Unique names are a correctness requirement, not a nicety

`CalculatorModel.Selected` is a `HashSet<string>` of names, and the shelf is assembled with
`Catalogue.Where(f => Selected.Contains(f.Name.Value))`. Two salts sharing a name would be selected
and deselected together, and one of them would silently enter every recipe the other was ticked for.

So a name that collides with a built-in or with another custom salt is refused at the point of
saving, with the clash named.

## Storage

```
CustomSalt
  Name              string
  Formula           string?   // null when entered as percentages
  ConcentrateType   A | B
  Percentages       map?      // only when entered by hand
  SolubilityGramsPerLitre  double?
```

- **Local storage and file** — a `customSalts` array, spelled out, readable and hand-editable.
- **Link** — `cs=` entries. A formula-defined salt is three short fields; a percentage-defined one
  carries only its non-zero forms.
- **Built-in indices do not move.** Custom salts travel in their own list rather than extending the
  catalogue, so the `n=` size check and the excluded-index list keep their current meaning and
  **every existing link still opens.**

A salt arriving in a link becomes part of the state like anything else, and persists on the next
edit. No "add this to your shelf?" prompt: it would be a dialogue in front of behaviour that is
already predictable.

## Concentrate type is a suggestion

Defaulted from composition — calcium or a plain nitrate suggests tank A, sulfate or phosphate
suggests B. A salt carrying both is defaulted to A with a warning that it cannot share a tank with
sulfates whatever the setting says. Always overridable.

## Solubility

One optional field, in g/L. Left blank, the concentrate plan reports unknown solubility, which is
exactly what it already does for anything absent from `SolubilityTable`. Nothing is invented to fill
the gap.

## Macro or micro is not a decision

`FertilizerBundleGenerator.IsMicro` classifies by composition: carrying any micronutrient makes a
salt micro. A custom iron salt joins the micro bundles and a custom nitrate joins the macro ones,
with no flag to set and no way for the grower to get it wrong. The picker's existing grouping picks
it up by the same call. The 64-bundle cap is unchanged.

## Interface

An "Add salt" button in the "Salts you have" card opens a two-tab form. Custom rows carry a badge
and offer edit and delete. The header count includes them.

## Components

| File | Responsibility |
|---|---|
| `src/SYT.NPKTools/Fertilizers/ChemicalFormula.cs` | formula → element mass fractions |
| `src/SYT.NPKTools/Fertilizers/FormulaComposition.cs` | mass fractions + groups → a `Fertilizer` |
| `web/.../CustomSalt.cs` | the stored definition, and materialising it |
| `web/.../Components/CustomSaltForm.razor` | the two-tab entry form |
| `web/.../Components/SaltPicker.razor` | lifted out of `Home.razor`, which is long again |
| `web/.../CalculatorModel.cs` | custom salts join the catalogue |
| `web/.../CalculatorState.cs` | `customSalts` in the file, `cs=` in the link |

## Tests

- Parser against the catalogue: every plain built-in salt's formula reproduces its declared
  percentages within 0.02 pp.
- Parser handles Unicode and plain subscripts, nested brackets, both hydrate separators.
- Malformed formulas are rejected, and the message names the position.
- A formula-defined custom salt reaches a recipe.
- A custom micronutrient salt lands in the micro bundles, not the macro ones.
- A name colliding with a built-in is refused.
- Link and file round-trip both kinds of custom salt.
- A `v=1` link, and a `v=2` link with no custom salts, still open unchanged.
