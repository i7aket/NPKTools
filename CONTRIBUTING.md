# Contributing to SYT.NPKTools

This is a fertilizer calculator. Somebody weighs out what it says and puts it in a tank that a crop
depends on for months. That is the whole reason the conventions below are strict: a plausible-looking
number is worse here than an error message, because an error message gets read.

The repository is `i7aket/NPKTools`; the packages are `SYT.NPKTools` and
`SYT.NPKTools.DependencyInjection`. Older documents and issues call it *NPKOptimizer* — that name is
from 1.x and is only kept in [the migration section of the README](README.md#migrating-from-npktools-1x).

## What you need

- **.NET 10 SDK.** `global.json` pins `10.0.100` with `rollForward: latestFeature`, so a newer 10.x
  feature band is fine and 9.x is not.
- Nothing else. The library has no dependencies at all, which is deliberate — it is what lets it run in
  a browser. Please keep it that way; a `PackageReference` in `src/SYT.NPKTools` needs a very good
  argument.

The solution file is **`SYT.NPKTools.slnx`**, not a `.sln`. Bare commands find it:

```bash
dotnet build                       # Debug
dotnet test                        # about 820 tests, a couple of seconds after a warm build
dotnet format                      # before every commit
dotnet format --verify-no-changes  # what CI runs
```

## Three things that will fail your build

1. **Warnings are errors.** `TreatWarningsAsErrors` with `AnalysisLevel=latest-recommended`. An
   analyzer suggestion you disagree with is a conversation, not a `#pragma`.
2. **Public members in `src/` need XML documentation.** CS1591 is enforced there and switched off in
   `.editorconfig` only for value objects and constants tables, where the undocumented members are
   `Value` accessors and string constants whose value is their meaning. A new public type without
   `<summary>` breaks the build.
3. **Formatting is not a review topic.** CI runs `dotnet format --verify-no-changes` on Ubuntu. Run
   `dotnet format` locally and the subject never comes up.

Code-style rules (IDExxxx) stay editor suggestions rather than build errors — `.editorconfig` documents
the conventions without making a stray blank line fail CI.

## Tests

xUnit, and **AwesomeAssertions** — not FluentAssertions. The using is `using AwesomeAssertions;` and the
API is the one you expect. `AutoFixture` and `NSubstitute` are available. All of them come from
`tests/Directory.Build.props`, so **do not add `PackageReference` lines to a test project**; a project
gets the harness by being named `*Tests`.

```csharp
/// <summary>
/// Says what this covers, and — where it is not obvious — why it is worth covering.
/// </summary>
[Fact]
[Trait("Category", "Unit")]
public void Method_Scenario_Expectation()
{
    // Arrange, Act, Assert. The comments are optional; the order is not.
}
```

- `Method_Scenario_Expectation` naming. `CA1707` is suppressed in test projects for exactly this.
- `[Trait("Category", "Unit")]` on everything, so a category can be filtered.
- A test that documents *why* a case matters earns its place. `Select_ForPolish_TreatsTwentyOneAsMany`
  is a better name than `Select_Works`, and the remark above it should say that 21 behaves like 1 in
  Russian and not in Polish — otherwise the next person "simplifies" the rule and the test looks wrong.
- Not everything under `tests/` is a test project: `SYT.NPKTools.OrToolsOracle` is shared support code
  for the differential tests and the benchmarks, and is marked `IsTestProject=false` so `dotnet test`
  does not try to run it.

**A number that comes from the physical world needs a source.** The EC model is checked against
certified KCl conductivity standards, the solubility figures against published handbook values, the
atomic masses against IUPAC. If you add a figure you cannot source, say so in the XML documentation and
keep it out of the warnings a grower acts on. Reporting a figure as unchecked is the practice here;
assuming it is safe is not.

## The browser app

`web/SYT.NPKTools.Calculator` is Blazor WebAssembly, and two of its constraints are easy to break by
accident.

**`InvariantGlobalization=true`.** No ICU, no culture-aware formatting. This is not a size
optimisation: a culture-aware parser can read `1,5` grams as `15`, which is a tenfold weighing error.
Parsing is tolerant and explicit — `Localisation/Numbers.cs` accepts a comma as a decimal point — and
display is always invariant with a dot. Do not introduce `CultureInfo`-aware formatting, resx or
satellite assemblies.

**Every user-visible string lives behind a key** in `Resources/*.json`, in eight languages. A test
fails if the eight files do not carry identical key sets, so adding a string means adding it to all
eight. Two rules make the difference between a translatable interface and one that only looks
translated:

- **One key per sentence, never per fragment.** Do not assemble a sentence around a value in markup.
  Word order differs across these eight languages, and a sentence built from pieces cannot be fixed by
  a translator. `Translations.Format(key, values)` exists for this.
- **Counts go through `Translations.Plural`**, never string concatenation. Russian, Ukrainian and
  Polish take three forms, and Polish disagrees with Russian about 21.

Terminology comes from [`docs/superpowers/glossary-hydroponics.md`](docs/superpowers/glossary-hydroponics.md),
which was checked term by term against extension services, fertilizer registration certificates and
labelling law — and which also records what could **not** be verified. Do not translate a new string
from English alone: check whether the glossary fixes the term, and add it there if it does not.

## Two traps that have cost real time

- **`dotnet publish -o <dir>` does not clean the directory.** Stale assemblies accumulate and you end up
  debugging a build that is not the one you think it is. Remove the directory first.
- **Blazor does not re-render a child component whose parameters have not changed.** Every panel in the
  calculator takes one parameter — an `EventCallback` to the same method on the page — which compares
  equal on every pass, so a panel only ever redrew when it handled the event itself. The acidification
  card was invisible for several releases because of this. `CalculatorModel.Changed` and
  `Translations.Changed` exist so a panel can learn that the calculation or the language moved. A new
  component that displays either should subscribe to both.

## Verifying a change to the interface

"Nothing visible changed" is a claim that has to be measured, and reading the diff does not measure it.
Razor drops literal whitespace between an expression and the block that follows it, which is how two
sentences once ran together in a change whose diff looked obviously correct.

```bash
# 1. Build the branch and a build of main, and serve both
rm -rf /tmp/after && dotnet publish web/SYT.NPKTools.Calculator -c Release -o /tmp/after
python3 scripts/serve.py /tmp/after/wwwroot 8082 &

# 2. Capture the rendered text from each and diff it
node scripts/rendered-text.mjs http://127.0.0.1:8081/ before.txt
node scripts/rendered-text.mjs http://127.0.0.1:8082/ after.txt
diff before.txt after.txt

# 3. For anything touching layout or wording, measure every label
node scripts/layout-audit.mjs http://127.0.0.1:8082/
```

`scripts/README.md` has the Chrome command the harnesses need. Both capture with `innerText`, which is
what the layout actually shows: a text-node walk invents a space where two nodes touch, and
`textContent` keeps the insignificant whitespace a wrapped line of markup leaves inside a block
element. Both of those report differences that are not on the screen.

One warning about the layout audit, learned the hard way: **it reported "no overflow anywhere" while
silently measuring English eight times**, because a language change did not redraw the page. If you
audit across languages, check that the pages differ in length before believing the result.

## Commits and pull requests

- Branch from `main`: `feat/…`, `fix/…`, `docs/…`, `chore/…`.
- Conventional-commit subjects — `feat(calculator): …`, `fix(optimizer): …`. The body is where the
  reasoning goes, and it is worth writing: it is the only place a future reader learns *why*.
- One squash commit per pull request into `main`.
- Fill in the [pull request template](.github/pull_request_template.md). Its checklist is the contract,
  not a formality.
- `CHANGELOG.md` gets an entry for anything a user would notice. A breaking change to a public API says
  so in the entry, because the packages are consumed.
- CI runs the build and the tests on Ubuntu, Windows and macOS, plus CodeQL. All of it has to be green.

## Reporting something rather than fixing it

A good bug report here carries the target string, the salts that were ticked and the water — or, far
better, **the link the app puts in the address bar**, which carries the whole setup. Paste that and the
problem is reproducible exactly.

Open an issue before starting anything large, so nobody writes the same thing twice.
