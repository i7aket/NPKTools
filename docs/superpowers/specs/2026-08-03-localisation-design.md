# Eight languages, and the terminology to go with them

Date: 2026-08-03
Status: approved

## Why

The calculator is English only, and its audience is not. The markets that grow hydroponically are
the Netherlands, Spain, Poland, Turkey, Germany and the Russian-speaking and Ukrainian-speaking
world — and the vocabulary those growers use is technical enough that a machine translation is worse
than leaving it in English. "Carbonate hardness" becomes "carbon firmness"; "chelated iron" becomes
something about shells.

So this is two jobs that must not be confused: **the machinery** for showing a translated interface,
and **the terminology** that goes into it. They ship as two pull requests for the same reason: one is
reviewed by reading code, the other by reading words.

## Languages

`en ru uk nl de es pl tr`

Chosen by where the industry is rather than by population. The Netherlands is the centre of
greenhouse hydroponics; Almería and Antalya are the two largest greenhouse concentrations in Europe;
Poland has the largest sector in Central Europe. Russian and Ukrainian are the maintainer's own
markets.

Ukrainian is not Russian with adjustments. `жорсткість` not `жёсткость`, `лужність` not `щёлочность`,
`добриво` not `удобрение`, `розчин` not `раствор`. It gets its own glossary column and its own
reviewer, and that reviewer checks it against Ukrainian sources rather than against the Russian
column.

None of the eight is right-to-left, so there is no bidirectional work.

## The constraint that shapes everything

The app is built with `InvariantGlobalization=true`, deliberately. The csproj records why: the ICU
data is 2.5 MB on disk and 1.1 MB of first load, and — the sharper reason — "1,5 grams read as 15
would be a tenfold weighing error".

So the standard .NET route (resx, satellite assemblies, `CultureInfo`) is out. It would bring ICU
back and reopen the number question in a tool where the output gets weighed on a scale.

### Numbers

- **Input accepts what a grower types**: `1,5`, `1.5`, `1 500`, and `1 500` with a non-breaking
  space. The comma is read as a decimal separator explicitly, which is safe precisely because it is
  explicit — the old danger was a culture-aware parser treating it as a thousands separator.
- **Display is always a dot**, in every language. A recipe is a set of weights that may be read
  aloud, photographed, or pasted into a message crossing a border; one unambiguous form is worth more
  than local familiarity.

## The machinery

### Keys are symbolic

`water.mode.osmosis`, not `"Osmosis"`. Rewording the English then does not orphan seven other
languages, and a missing key is detectable rather than invisible.

### One JSON file per language, fetched when needed

`wwwroot/i18n/{lang}.json`, about 10 KB each. Languages nobody selects are never downloaded, so the
first load does not grow. A correction is a change to a text file and a pull request — no rebuild,
which matters for translations arriving from people who do not build the app.

A missing key falls back to English. **A test asserts every language carries every key**; that is the
test that matters, because the failure it prevents — adding a string and forgetting seven files — is
the one that actually happens.

### Plural forms are part of the design, not an afterthought

"8 recipes" needs **three** forms in Russian, Ukrainian and Polish: 1 рецепт, 2 рецепта, 5 рецептов.
A dictionary lookup alone produces "5 рецепт", which is the single most recognisable mark of a
localisation done carelessly.

```json
"recipes.count": { "one": "рецепт", "few": "рецепта", "many": "рецептов" }
```

Selector per language, for integers, following CLDR:

| Language | Rule |
|---|---|
| en, de, nl, es, tr | `one` when n = 1, else `other` |
| ru, uk | `one` when n%10 = 1 and n%100 ≠ 11; `few` when n%10 ∈ 2–4 and n%100 ∉ 12–14; else `many` |
| pl | `one` when n = 1; `few` when n%10 ∈ 2–4 and n%100 ∉ 12–14; else `many` |

Around thirty lines of code, and the difference between localisation and a word swap.

### What is never translated

Element symbols — `N`, `P`, `K`, `Ca` — and chemical formulas. They are the same on a bag in every
one of these countries, and translating them would make the app harder to use, not easier. Unit
labels *are* translated: `мС/см`, `мэкв/л`. `°dH` stays as it is written everywhere.

### Library messages stop reaching the screen

`CalculatorModel` currently does `Error = ex.Message`, so 242 English exception strings from the
NuGet package can appear in the interface. Localising them inside the library would be wrong: they
are an API for developers, and a consumer catching an exception should not get Ukrainian.

Instead the app maps the few real failure modes to its own translated strings. This is better design
independently of translation — the messages become ones a grower can act on rather than ones a
developer wrote for themselves.

## Choosing a language

Detected from `navigator.language`, matched to the nearest supported tag, overridable by a picker in
the header, and remembered in local storage.

**Not carried in the link.** A link sent to another country should not impose the sender's language on
the reader; the recipe travels, the interface stays theirs.

## Terminology

A glossary of about sixty terms comes first, each with an English definition saying what it means —
not just what it is called. The interface is then translated *against* the glossary, so one concept
cannot become three different words in three places.

The glossary is shown to the maintainer before any interface translation begins. Sixty lines is where
a wrong term is still cheap; the same mistake spread across 135 strings in eight languages is not.

Then one reviewer per language, checking the terms against how they are actually written on fertilizer
bags and in agronomic sources for that market — not against the English, and not against each other.

## Verification

- Every language carries every key, asserted by a test.
- Plural selectors produce the right form for 1, 2, 5, 11, 21, 22, 101 in each language.
- Numbers: `1,5`, `1.5`, `1 500` all parse; every displayed figure uses a dot.
- The screenshot harness runs each language at phone, tablet and desktop widths. German and Russian
  run 20–35% longer than English, and a clipped button is exactly what that produces — better found
  by measurement than by a user.

## Order of work

1. **Machinery** — keys, loader, plural rules, picker. The interface stays English and behaves
   exactly as it does now. First pull request.
2. **Glossary** — around sixty terms, shown for review.
3. **Translation** of the interface, through the glossary.
4. **Per-language review**, one reviewer each.
5. **Layout audit** across all eight languages.

Steps 2–5 are the second pull request: code in the first, words in the second, so neither review has
to wade through the other.
