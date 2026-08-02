# Custom salts — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Let a grower add a fertilizer the catalogue does not have — by formula, or by percentages —
and have it take part in the recipe search exactly as a built-in does.

**Architecture:** A custom salt becomes the same `Fertilizer` the library already uses, so nothing
downstream changes. The library gains a formula parser and a formula→`Fertilizer` factory; the app
gains a stored `CustomSalt` definition, a form, and persistence.

**Tech Stack:** .NET 10, C# 13, Blazor WebAssembly, xUnit 2.9.3, AwesomeAssertions.

Spec: `docs/superpowers/specs/2026-08-02-custom-salts-design.md`

## Global Constraints

- Target framework `net10.0`. Solution file is `SYT.NPKTools.slnx`.
- `TreatWarningsAsErrors=true`, `AnalysisLevel=latest-recommended`. A warning fails the build.
- CS1591 enforced in `src/`: **every public type and member needs an XML doc comment**, including
  every `<param>`. Not enforced in `web/` or `tests/`.
- `Nullable=enable`, `ImplicitUsings=enable`.
- CI runs `dotnet format --verify-no-changes`. Run `dotnet format` before every commit.
- Tests: xUnit, `using AwesomeAssertions;` (**not** FluentAssertions), `Method_Scenario_Expectation`
  naming, `[Trait("Category", "Unit")]` on every test.
- UI text is English. Numbers parse and format with `CultureInfo.InvariantCulture`.
- Inside `src/SYT.NPKTools`, element symbols come from internal `Names`, atomic masses from internal
  `AtomicMasses`. Do not retype literals.
- `Fertilizer` is built with `FertilizerBuilder`. Its methods:
  `AddName AddFormula AddType AddId AddPrice AddNo3 AddNh4 AddNh2 AddP AddK AddCaNonChelated
  AddCaEdta AddMgNonChelated AddMgEdta AddS AddFeNonChelated AddFeEdta AddFeDtpa AddFeEddha
  AddFeHbed AddFeOrthoPart AddCuNonChelated AddCuEdta AddMnNonChelated AddMnEdta AddZnNonChelated
  AddZnEdta AddB AddMo AddCl AddSi AddSe AddNa AddWeight`
- `ConcentrateType` is `None | A | B`, in `SYT.NPKTools.Fertilizers.Enums`.

**Build:** `dotnet build SYT.NPKTools.slnx -c Release`
**Test:** `dotnet test SYT.NPKTools.slnx -c Release`

---

### Task 1: The formula parser

**Files:**
- Create: `src/SYT.NPKTools/Fertilizers/ChemicalFormula.cs`
- Test: `tests/SYT.NPKTools.Tests/ChemicalFormulaTests.cs`

**Interfaces:**
- Consumes: internal `AtomicMasses`.
- Produces: `ChemicalFormula.TryParse(string? text, out ChemicalFormula? formula, out string? error) -> bool`;
  instance members `Atoms` (`IReadOnlyDictionary<string,int>`), `MolarMass` (double),
  `PercentOf(string symbol) -> double`, `NitratePercent`, `AmmoniumPercent`, `AmidePercent` (all
  double, nitrogen expressed as elemental N).

- [ ] **Step 1: Write the failing test**

```csharp
using AwesomeAssertions;
using SYT.NPKTools;
using SYT.NPKTools.Fertilizers;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers reading a chemical formula into the composition a fertilizer is described by.
/// </summary>
/// <remarks>
/// The central test is not hand-written. The built-in catalogue declares both a formula and the
/// percentages that formula implies, so parsing each formula and comparing against the declared
/// figures checks the parser against dozens of real answers that already live in the repository.
/// </remarks>
public class ChemicalFormulaTests
{
    private static ChemicalFormula Parse(string text)
    {
        ChemicalFormula.TryParse(text, out ChemicalFormula? formula, out string? error)
            .Should().BeTrue(because: error);
        return formula!;
    }

    /// <summary>
    /// Potassium nitrate, the simplest case: three elements, no brackets, no water.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryParse_ForASimpleSalt_GivesTheMolarMassAndPercentages()
    {
        ChemicalFormula formula = Parse("KNO3");

        formula.MolarMass.Should().BeApproximately(101.102, 0.01);
        formula.PercentOf("K").Should().BeApproximately(38.672, 0.01);
        formula.PercentOf("N").Should().BeApproximately(13.854, 0.01);
    }

    /// <summary>
    /// Brackets with a multiplier, and water of crystallisation. The catalogue writes this one with
    /// a Unicode subscript inside the bracket and a plain digit outside, so both must work.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryParse_HandlesBracketsSubscriptsAndHydrates()
    {
        ChemicalFormula formula = Parse("Ca(NO₃)2*4H₂O");

        formula.MolarMass.Should().BeApproximately(236.146, 0.01);
        formula.PercentOf("Ca").Should().BeApproximately(16.972, 0.01);
        formula.PercentOf("N").Should().BeApproximately(11.863, 0.01);
    }

    /// <summary>
    /// Both hydrate separators mean the same thing.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryParse_AcceptsEitherHydrateSeparator()
    {
        Parse("MgSO4*7H2O").MolarMass.Should().BeApproximately(
            Parse("MgSO4·7H2O").MolarMass, 1e-9);
    }

    /// <summary>
    /// Nitrogen is split by the group it sits in, because the library treats the forms differently:
    /// they drive the ion balance, the acid-base reading and the conductivity estimate.
    /// </summary>
    [Theory]
    [InlineData("KNO3", 13.854, 0, 0)]
    [InlineData("NH4NO3", 17.499, 17.499, 0)]
    [InlineData("(NH4)2SO4", 0, 21.200, 0)]
    [InlineData("CO(NH2)2", 0, 0, 46.646)]
    [Trait("Category", "Unit")]
    public void TryParse_SplitsNitrogenByItsGroup(
        string text,
        double nitrate,
        double ammonium,
        double amide)
    {
        ChemicalFormula formula = Parse(text);

        formula.NitratePercent.Should().BeApproximately(nitrate, 0.02);
        formula.AmmoniumPercent.Should().BeApproximately(ammonium, 0.02);
        formula.AmidePercent.Should().BeApproximately(amide, 0.02);
    }

    /// <summary>
    /// Every plain salt in the catalogue reproduces its own declared percentages. This is the test
    /// worth having: the answers were not written for it.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryParse_ReproducesTheCataloguesOwnPercentages()
    {
        IFertilizerBundleRepository repository = NpkTools.CreateBundleRepository();
        Fertilizer[] catalogue =
        [
            .. repository.Macro().SelectMany(b => b)
                .Concat(repository.Micro().SelectMany(b => b))
                .DistinctBy(f => f.Name.Value)
        ];

        int checkedSalts = 0;
        foreach (Fertilizer salt in catalogue)
        {
            // Chelated salts name their chelating agent rather than spelling it out as atoms, so
            // their formulas do not parse. Skipping them is the point, not a workaround.
            if (!ChemicalFormula.TryParse(salt.Formula.Value, out ChemicalFormula? formula, out _))
            {
                continue;
            }

            checkedSalts++;
            formula!.PercentOf("K").Should().BeApproximately(salt.Potassium.Value, 0.02, salt.Name.Value);
            formula.PercentOf("P").Should().BeApproximately(salt.Phosphorus.Value, 0.02, salt.Name.Value);
            formula.PercentOf("S").Should().BeApproximately(salt.Sulfur.Value, 0.02, salt.Name.Value);
            formula.PercentOf("Ca").Should().BeApproximately(salt.Calcium.Value, 0.02, salt.Name.Value);
            formula.PercentOf("Mg").Should().BeApproximately(salt.Magnesium.Value, 0.02, salt.Name.Value);
            formula.PercentOf("N").Should().BeApproximately(salt.Nitrogen.Value, 0.02, salt.Name.Value);
        }

        checkedSalts.Should().BeGreaterThan(15);
    }

    /// <summary>
    /// A formula that cannot be read says where it gave up, rather than failing blankly.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Xx3")]
    [InlineData("Ca(NO3")]
    [InlineData("K2O)")]
    [InlineData("2KNO3")]
    [Trait("Category", "Unit")]
    public void TryParse_ForNonsense_FailsWithAMessage(string text)
    {
        bool parsed = ChemicalFormula.TryParse(text, out ChemicalFormula? formula, out string? error);

        parsed.Should().BeFalse();
        formula.Should().BeNull();
        error.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// An element the formula does not contain is zero, not an error.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void PercentOf_ForAnAbsentElement_IsZero()
    {
        Parse("KNO3").PercentOf("Fe").Should().Be(0);
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test tests/SYT.NPKTools.Tests -c Release --filter "FullyQualifiedName~ChemicalFormulaTests"`
Expected: build failure — `ChemicalFormula` does not exist.

- [ ] **Step 3: Write the implementation**

```csharp
using System.Globalization;
using System.Text;
using SYT.NPKTools.Internal;

namespace SYT.NPKTools.Fertilizers;

/// <summary>
/// A chemical formula read into the elements it contains.
/// </summary>
/// <remarks>
/// <para>
/// Exists so a grower can describe a fertilizer the catalogue does not carry by writing what is on
/// the bag rather than working out percentages by hand. Deriving them from the formula is both less
/// work and less error-prone: fertilizer labels quote phosphorus and potassium as their oxides,
/// P₂O₅ and K₂O, and a figure copied straight off a label overstates phosphorus by a factor of 2.3.
/// </para>
/// <para>
/// Nitrogen is reported by group as well as in total, because the library treats nitrate, ammonium
/// and amide differently — they carry different charges, different acid-base character, and urea
/// carries none at all.
/// </para>
/// </remarks>
public sealed class ChemicalFormula
{
    private static readonly IReadOnlyDictionary<string, double> Masses = new Dictionary<string, double>(StringComparer.Ordinal)
    {
        [Names.N] = AtomicMasses.N,
        [Names.P] = AtomicMasses.P,
        [Names.K] = AtomicMasses.K,
        [Names.Ca] = AtomicMasses.Ca,
        [Names.Mg] = AtomicMasses.Mg,
        [Names.S] = AtomicMasses.S,
        [Names.Fe] = AtomicMasses.Fe,
        [Names.Cu] = AtomicMasses.Cu,
        [Names.Mn] = AtomicMasses.Mn,
        [Names.Zn] = AtomicMasses.Zn,
        [Names.B] = AtomicMasses.B,
        [Names.Mo] = AtomicMasses.Mo,
        [Names.Cl] = AtomicMasses.Cl,
        [Names.Si] = AtomicMasses.Si,
        [Names.Se] = AtomicMasses.Se,
        [Names.Na] = AtomicMasses.Na,
        ["H"] = 1.008,
        ["O"] = 15.999,
        ["C"] = 12.011,
    };

    private ChemicalFormula(
        Dictionary<string, int> atoms,
        double nitrateNitrogen,
        double ammoniumNitrogen,
        double amideNitrogen)
    {
        Atoms = atoms;
        MolarMass = atoms.Sum(pair => Masses[pair.Key] * pair.Value);
        _nitrateAtoms = nitrateNitrogen;
        _ammoniumAtoms = ammoniumNitrogen;
        _amideAtoms = amideNitrogen;
    }

    private readonly double _nitrateAtoms;
    private readonly double _ammoniumAtoms;
    private readonly double _amideAtoms;

    /// <summary>Gets how many atoms of each element the formula contains.</summary>
    public IReadOnlyDictionary<string, int> Atoms { get; }

    /// <summary>Gets the molar mass, in grams per mole.</summary>
    public double MolarMass { get; }

    /// <summary>Gets the nitrate nitrogen, as a percentage of the whole by weight.</summary>
    public double NitratePercent => Share(_nitrateAtoms, Names.N);

    /// <summary>Gets the ammonium nitrogen, as a percentage of the whole by weight.</summary>
    public double AmmoniumPercent => Share(_ammoniumAtoms, Names.N);

    /// <summary>Gets the amide nitrogen, as a percentage of the whole by weight.</summary>
    public double AmidePercent => Share(_amideAtoms, Names.N);

    /// <summary>
    /// The share of the total weight one element accounts for.
    /// </summary>
    /// <param name="symbol">The element symbol.</param>
    /// <returns>A percentage, or zero when the formula does not contain the element.</returns>
    public double PercentOf(string symbol) =>
        Atoms.TryGetValue(symbol, out int count) ? Share(count, symbol) : 0;

    private double Share(double atoms, string symbol) =>
        MolarMass > 0 ? 100 * Masses[symbol] * atoms / MolarMass : 0;

    /// <summary>
    /// Reads a formula, reporting why rather than throwing when it cannot.
    /// </summary>
    /// <param name="text">The formula, for example <c>Ca(NO₃)2*4H₂O</c>.</param>
    /// <param name="formula">The parsed formula, or null when it could not be read.</param>
    /// <param name="error">What went wrong and where, or null on success.</param>
    /// <returns><see langword="true"/> when the formula was read.</returns>
    /// <remarks>
    /// Accepts element symbols, plain and Unicode subscripts — the catalogue mixes them within a
    /// single formula — bracketed groups with a multiplier, and hydrates joined by <c>*</c> or
    /// <c>·</c>. Anything else is refused; guessing at a formula would produce a fertilizer that is
    /// confidently wrong, which is worse than one that is rejected.
    /// </remarks>
    public static bool TryParse(string? text, out ChemicalFormula? formula, out string? error)
    {
        formula = null;
        error = null;

        if (string.IsNullOrWhiteSpace(text))
        {
            error = "The formula is empty.";
            return false;
        }

        string normalised = Normalise(text);
        Dictionary<string, int> atoms = new(StringComparer.Ordinal);
        double nitrate = 0;
        double ammonium = 0;
        double amide = 0;

        foreach (string part in normalised.Split('*', StringSplitOptions.RemoveEmptyEntries))
        {
            string body = part;
            int multiplier = 1;

            // A hydrate is written with its count in front: *4H2O. Only there does a leading digit
            // mean anything, which is why a formula starting with one is refused below.
            int digits = 0;
            while (digits < body.Length && char.IsAsciiDigit(body[digits]))
            {
                digits++;
            }

            if (digits > 0)
            {
                if (!ReferenceEquals(part, normalised) && part != normalised.Split('*')[0])
                {
                    multiplier = int.Parse(body[..digits], CultureInfo.InvariantCulture);
                    body = body[digits..];
                }
                else
                {
                    error = "A formula cannot start with a number.";
                    return false;
                }
            }

            if (!ParseGroup(body, multiplier, atoms, ref nitrate, ref ammonium, ref amide, out error))
            {
                return false;
            }
        }

        if (atoms.Count == 0)
        {
            error = "The formula names no elements.";
            return false;
        }

        formula = new ChemicalFormula(atoms, nitrate, ammonium, amide);
        return true;
    }

    private static string Normalise(string text)
    {
        StringBuilder builder = new(text.Length);
        foreach (char c in text.Trim())
        {
            builder.Append(c switch
            {
                '·' or '×' or '.' => '*',
                >= '₀' and <= '₉' => (char)('0' + (c - '₀')),
                '[' => '(',
                ']' => ')',
                _ => c,
            });
        }

        return builder.ToString();
    }

    private static bool ParseGroup(
        string body,
        int multiplier,
        Dictionary<string, int> atoms,
        ref double nitrate,
        ref double ammonium,
        ref double amide,
        out string? error)
    {
        error = null;
        int index = 0;

        while (index < body.Length)
        {
            if (body[index] == '(')
            {
                int depth = 1;
                int close = index + 1;
                while (close < body.Length && depth > 0)
                {
                    depth += body[close] switch { '(' => 1, ')' => -1, _ => 0 };
                    close++;
                }

                if (depth != 0)
                {
                    error = "A bracket is not closed.";
                    return false;
                }

                string inner = body[(index + 1)..(close - 1)];
                index = close;
                int count = ReadNumber(body, ref index);

                Dictionary<string, int> group = new(StringComparer.Ordinal);
                double innerNitrate = 0;
                double innerAmmonium = 0;
                double innerAmide = 0;
                if (!ParseGroup(inner, 1, group, ref innerNitrate, ref innerAmmonium, ref innerAmide, out error))
                {
                    return false;
                }

                Classify(group, count * multiplier, ref nitrate, ref ammonium, ref amide);

                foreach ((string symbol, int atomCount) in group)
                {
                    atoms[symbol] = atoms.GetValueOrDefault(symbol) + (atomCount * count * multiplier);
                }

                // Nitrogen inside the group has already been attributed; anything the group itself
                // did not explain is carried by the run-level classifier below.
                nitrate += innerNitrate * count * multiplier;
                ammonium += innerAmmonium * count * multiplier;
                amide += innerAmide * count * multiplier;
                continue;
            }

            if (!char.IsAsciiLetterUpper(body[index]))
            {
                error = $"Unexpected '{body[index]}' at position {index + 1}.";
                return false;
            }

            int start = index;
            index++;
            while (index < body.Length && char.IsAsciiLetterLower(body[index]))
            {
                index++;
            }

            string element = body[start..index];
            if (!Masses.ContainsKey(element))
            {
                error = $"'{element}' is not an element this calculator knows.";
                return false;
            }

            int howMany = ReadNumber(body, ref index);
            atoms[element] = atoms.GetValueOrDefault(element) + (howMany * multiplier);

            if (element == Names.N)
            {
                ClassifyRun(body, index, howMany * multiplier, ref nitrate, ref ammonium, ref amide);
            }
        }

        return true;
    }

    private static int ReadNumber(string body, ref int index)
    {
        int start = index;
        while (index < body.Length && char.IsAsciiDigit(body[index]))
        {
            index++;
        }

        return index == start ? 1 : int.Parse(body[start..index], CultureInfo.InvariantCulture);
    }

    /// <summary>Attributes a bracketed group's nitrogen, as in Ca(NO3)2 or (NH4)2SO4.</summary>
    private static void Classify(
        Dictionary<string, int> group,
        int repeats,
        ref double nitrate,
        ref double ammonium,
        ref double amide)
    {
        if (group.Count != 2 || !group.TryGetValue(Names.N, out int n) || n != 1)
        {
            return;
        }

        if (group.TryGetValue("O", out int oxygen) && oxygen == 3)
        {
            nitrate += repeats;
        }
        else if (group.TryGetValue("H", out int hydrogen))
        {
            if (hydrogen == 4)
            {
                ammonium += repeats;
            }
            else if (hydrogen == 2)
            {
                amide += repeats;
            }
        }
    }

    /// <summary>Attributes nitrogen written without brackets, as in KNO3 or NH4Cl.</summary>
    private static void ClassifyRun(
        string body,
        int after,
        int atoms,
        ref double nitrate,
        ref double ammonium,
        ref double amide)
    {
        string rest = body[after..];
        if (rest.StartsWith("O3", StringComparison.Ordinal))
        {
            nitrate += atoms;
        }
        else if (rest.StartsWith("H4", StringComparison.Ordinal))
        {
            ammonium += atoms;
        }
        else if (rest.StartsWith("H2", StringComparison.Ordinal))
        {
            amide += atoms;
        }
    }
}
```

- [ ] **Step 4: Run the tests**

Run: `dotnet test tests/SYT.NPKTools.Tests -c Release --filter "FullyQualifiedName~ChemicalFormulaTests"`
Expected: PASS.

Two things are likely to need adjustment and both are cheap:

- **Bracketed nitrogen counted twice.** `Classify` and the recursive `innerNitrate` both attribute
  `(NH4)2`. Whichever double-counts, drop the other — the `SplitsNitrogenByItsGroup` theory catches
  it immediately, since ammonium sulfate would read 42.4% instead of 21.2%.
- **`Nitrogen.Value`, `Calcium.Value` and friends** in the catalogue test: confirm these are
  percentages by weight and that `Nitrogen.Value` is the sum of the three forms. Read
  `src/SYT.NPKTools/Fertilizers/ValueObjects/` if a comparison fails by a suspiciously round factor.

- [ ] **Step 5: Format and commit**

```bash
dotnet format
git add src/SYT.NPKTools/Fertilizers/ChemicalFormula.cs tests/SYT.NPKTools.Tests/ChemicalFormulaTests.cs
git commit -m "feat(fertilizers): read a chemical formula into its composition"
```

---

### Task 2: A fertilizer from a formula

**Files:**
- Create: `src/SYT.NPKTools/Fertilizers/FormulaComposition.cs`
- Test: `tests/SYT.NPKTools.Tests/FormulaCompositionTests.cs`

**Interfaces:**
- Consumes: `ChemicalFormula` (Task 1), `FertilizerBuilder`, `ConcentrateType`.
- Produces:
  `FormulaComposition.TryCreate(string name, string formula, ConcentrateType type, out Fertilizer? fertilizer, out string? error) -> bool`
  and `FormulaComposition.SuggestTank(ChemicalFormula formula) -> ConcentrateType`.

- [ ] **Step 1: Write the failing test**

```csharp
using AwesomeAssertions;
using SYT.NPKTools.Fertilizers;
using SYT.NPKTools.Fertilizers.Enums;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers turning a formula into a fertilizer the optimizer can use.
/// </summary>
public class FormulaCompositionTests
{
    /// <summary>
    /// The result is an ordinary fertilizer, indistinguishable from a catalogue one.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryCreate_FromAFormula_BuildsAUsableFertilizer()
    {
        bool created = FormulaComposition.TryCreate(
            "My potassium nitrate", "KNO3", ConcentrateType.A,
            out Fertilizer? salt, out string? error);

        created.Should().BeTrue(because: error);
        salt!.Name.Value.Should().Be("My potassium nitrate");
        salt.Formula.Value.Should().Be("KNO3");
        salt.Potassium.Value.Should().BeApproximately(38.672, 0.02);
        salt.Nitrogen.Value.Should().BeApproximately(13.854, 0.02);
        salt.Type.Should().Be(ConcentrateType.A);
    }

    /// <summary>
    /// A micronutrient salt is recognised as micro by the same call the bundle generator uses, so it
    /// joins the micro bundles without anyone setting a flag.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryCreate_ForAMicronutrientSalt_IsClassifiedAsMicro()
    {
        FormulaComposition.TryCreate("My zinc sulfate", "ZnSO4*7H2O", ConcentrateType.B,
            out Fertilizer? zinc, out _).Should().BeTrue();
        FormulaComposition.TryCreate("My potassium nitrate", "KNO3", ConcentrateType.A,
            out Fertilizer? potassium, out _).Should().BeTrue();

        FertilizerBundleGenerator.IsMicro(zinc!).Should().BeTrue();
        FertilizerBundleGenerator.IsMicro(potassium!).Should().BeFalse();
    }

    /// <summary>
    /// Nitrogen keeps its forms, because the ion balance and the acid-base reading depend on them.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryCreate_KeepsTheNitrogenForms()
    {
        FormulaComposition.TryCreate("My ammonium nitrate", "NH4NO3", ConcentrateType.B,
            out Fertilizer? salt, out _).Should().BeTrue();

        salt!.Nitrogen.Nitrate.Should().BeApproximately(17.499, 0.02);
        salt.Nitrogen.Ammonium.Should().BeApproximately(17.499, 0.02);
    }

    /// <summary>
    /// A formula that cannot be read produces no fertilizer and says why.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryCreate_ForANonsenseFormula_Fails()
    {
        FormulaComposition.TryCreate("Nonsense", "Zz9", ConcentrateType.A,
            out Fertilizer? salt, out string? error).Should().BeFalse();

        salt.Should().BeNull();
        error.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// The tank suggestion follows the rule the concentrate split already enforces: calcium apart
    /// from sulfate and phosphate.
    /// </summary>
    [Theory]
    [InlineData("Ca(NO3)2*4H2O", ConcentrateType.A)]
    [InlineData("KNO3", ConcentrateType.A)]
    [InlineData("MgSO4*7H2O", ConcentrateType.B)]
    [InlineData("KH2PO4", ConcentrateType.B)]
    [Trait("Category", "Unit")]
    public void SuggestTank_FollowsTheConcentrateRule(string formula, ConcentrateType expected)
    {
        ChemicalFormula.TryParse(formula, out ChemicalFormula? parsed, out _).Should().BeTrue();

        FormulaComposition.SuggestTank(parsed!).Should().Be(expected);
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test tests/SYT.NPKTools.Tests -c Release --filter "FullyQualifiedName~FormulaCompositionTests"`
Expected: build failure — `FormulaComposition` does not exist.

- [ ] **Step 3: Write the implementation**

```csharp
using SYT.NPKTools.Fertilizers.Enums;
using SYT.NPKTools.Internal;

namespace SYT.NPKTools.Fertilizers;

/// <summary>
/// Builds a fertilizer from a chemical formula.
/// </summary>
/// <remarks>
/// Everything the optimizer needs is derivable from the formula, so a grower adding a salt the
/// catalogue lacks writes what is on the bag and the percentages follow. The result is an ordinary
/// <see cref="Fertilizer"/>: the bundle generator, the optimizer and the concentrate split cannot
/// tell it from a built-in one, and none of them need to.
/// </remarks>
public static class FormulaComposition
{
    /// <summary>
    /// Builds a fertilizer from a formula, reporting why rather than throwing when it cannot.
    /// </summary>
    /// <param name="name">The name to show. Must be unique among the salts on offer.</param>
    /// <param name="formula">The chemical formula.</param>
    /// <param name="type">Which concentrate tank the salt belongs in.</param>
    /// <param name="fertilizer">The result, or null when the formula could not be read.</param>
    /// <param name="error">What went wrong, or null on success.</param>
    /// <returns><see langword="true"/> when the fertilizer was built.</returns>
    /// <remarks>
    /// Metals are recorded as non-chelated: a formula spells out atoms, and a chelate is defined by
    /// its chelating agent rather than by its composition. Chelated products are described by their
    /// percentages instead.
    /// </remarks>
    public static bool TryCreate(
        string name,
        string formula,
        ConcentrateType type,
        out Fertilizer? fertilizer,
        out string? error)
    {
        fertilizer = null;

        if (string.IsNullOrWhiteSpace(name))
        {
            error = "The salt needs a name.";
            return false;
        }

        if (!ChemicalFormula.TryParse(formula, out ChemicalFormula? parsed, out error))
        {
            return false;
        }

        FertilizerBuilder builder = new FertilizerBuilder()
            .AddName(name.Trim())
            .AddFormula(formula.Trim())
            .AddType(type);

        Set(builder.AddNo3, parsed!.NitratePercent);
        Set(builder.AddNh4, parsed.AmmoniumPercent);
        Set(builder.AddNh2, parsed.AmidePercent);
        Set(builder.AddP, parsed.PercentOf(Names.P));
        Set(builder.AddK, parsed.PercentOf(Names.K));
        Set(builder.AddCaNonChelated, parsed.PercentOf(Names.Ca));
        Set(builder.AddMgNonChelated, parsed.PercentOf(Names.Mg));
        Set(builder.AddS, parsed.PercentOf(Names.S));
        Set(builder.AddFeNonChelated, parsed.PercentOf(Names.Fe));
        Set(builder.AddCuNonChelated, parsed.PercentOf(Names.Cu));
        Set(builder.AddMnNonChelated, parsed.PercentOf(Names.Mn));
        Set(builder.AddZnNonChelated, parsed.PercentOf(Names.Zn));
        Set(builder.AddB, parsed.PercentOf(Names.B));
        Set(builder.AddMo, parsed.PercentOf(Names.Mo));
        Set(builder.AddCl, parsed.PercentOf(Names.Cl));
        Set(builder.AddSi, parsed.PercentOf(Names.Si));
        Set(builder.AddSe, parsed.PercentOf(Names.Se));
        Set(builder.AddNa, parsed.PercentOf(Names.Na));

        fertilizer = builder.Build();
        error = null;
        return true;
    }

    /// <summary>
    /// Suggests which concentrate tank a salt belongs in.
    /// </summary>
    /// <param name="formula">The parsed formula.</param>
    /// <returns>Tank A or tank B.</returns>
    /// <remarks>
    /// The rule the concentrate split already enforces: calcium precipitates with sulfate and with
    /// phosphate, so the two must not share a tank. Calcium therefore suggests A and sulfur or
    /// phosphorus suggests B. A suggestion only — a salt carrying both is a real product, and the
    /// caller should be able to say where it goes.
    /// </remarks>
    public static ConcentrateType SuggestTank(ChemicalFormula formula)
    {
        ArgumentNullException.ThrowIfNull(formula);

        if (formula.PercentOf(Names.Ca) > 0)
        {
            return ConcentrateType.A;
        }

        bool precipitates = formula.PercentOf(Names.S) > 0 || formula.PercentOf(Names.P) > 0;
        return precipitates ? ConcentrateType.B : ConcentrateType.A;
    }

    // Builder setters throw when called twice, and several of them reject zero, so only real
    // content is passed through.
    private static void Set(Func<double, FertilizerBuilder> add, double percent)
    {
        if (percent > 0)
        {
            add(percent);
        }
    }
}
```

- [ ] **Step 4: Run the tests**

Run: `dotnet test tests/SYT.NPKTools.Tests -c Release --filter "FullyQualifiedName~FormulaCompositionTests"`
Expected: PASS.

If `FertilizerBuilder`'s methods return the base type rather than `FertilizerBuilder`, change the
`Set` delegate to match — read `FertilizerBuilderBase.cs` for the exact return type.

- [ ] **Step 5: Format and commit**

```bash
dotnet format
git add src/SYT.NPKTools/Fertilizers/FormulaComposition.cs tests/SYT.NPKTools.Tests/FormulaCompositionTests.cs
git commit -m "feat(fertilizers): build a fertilizer from a chemical formula"
```

---

### Task 3: The stored definition

**Files:**
- Create: `web/SYT.NPKTools.Calculator/CustomSalt.cs`
- Test: `tests/SYT.NPKTools.Calculator.Tests/CustomSaltTests.cs`

**Interfaces:**
- Consumes: `FormulaComposition` (Task 2), `ChemicalFormula` (Task 1).
- Produces: `CustomSalt` with settable `Name`, `Formula` (`string?`), `Tank` (`ConcentrateType`),
  `Percentages` (`Dictionary<string,double>`), `SolubilityGramsPerLitre` (`double?`); and
  `bool TryMaterialise(out Fertilizer? fertilizer, out string? error)`.

Percentage keys are the form names: `No3 Nh4 Nh2 P K Ca CaEdta Mg MgEdta S Fe FeEdta FeDtpa FeEddha
FeHbed Cu CuEdta Mn MnEdta Zn ZnEdta B Mo Cl Si Se Na`.

- [ ] **Step 1: Write the failing test**

```csharp
using AwesomeAssertions;
using SYT.NPKTools.Fertilizers;
using SYT.NPKTools.Fertilizers.Enums;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers a salt the grower described, in both the ways they can describe one.
/// </summary>
public class CustomSaltTests
{
    /// <summary>
    /// Described by formula, the percentages are worked out rather than entered.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryMaterialise_FromAFormula_DerivesThePercentages()
    {
        CustomSalt salt = new() { Name = "Shop KNO3", Formula = "KNO3", Tank = ConcentrateType.A };

        salt.TryMaterialise(out Fertilizer? built, out string? error).Should().BeTrue(because: error);

        built!.Potassium.Value.Should().BeApproximately(38.672, 0.02);
    }

    /// <summary>
    /// Described by percentages — the path for blends and for chelates, which a formula cannot
    /// usefully express.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryMaterialise_FromPercentages_UsesThemAsGiven()
    {
        CustomSalt salt = new()
        {
            Name = "Fe chelate 13%",
            Tank = ConcentrateType.B,
            Percentages = { ["FeEdta"] = 13 },
        };

        salt.TryMaterialise(out Fertilizer? built, out string? error).Should().BeTrue(because: error);

        built!.Iron.Value.Should().BeApproximately(13, 0.01);
        FertilizerBundleGenerator.IsMicro(built).Should().BeTrue();
    }

    /// <summary>
    /// A salt that carries nothing helps no target, and saying so at entry beats a silent absence
    /// from every recipe.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryMaterialise_WithNothingInIt_Fails()
    {
        CustomSalt salt = new() { Name = "Empty", Tank = ConcentrateType.A };

        salt.TryMaterialise(out Fertilizer? built, out string? error).Should().BeFalse();

        built.Should().BeNull();
        error.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// Percentages cannot add up to more than the whole.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryMaterialise_WhenPercentagesExceedTheWhole_Fails()
    {
        CustomSalt salt = new()
        {
            Name = "Impossible",
            Tank = ConcentrateType.A,
            Percentages = { ["K"] = 70, ["Ca"] = 45 },
        };

        salt.TryMaterialise(out _, out string? error).Should().BeFalse();
        error.Should().Contain("100");
    }

    /// <summary>
    /// The formula wins when both are present, because it is the more precise description and the
    /// one the form fills in automatically.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryMaterialise_WithBoth_PrefersTheFormula()
    {
        CustomSalt salt = new()
        {
            Name = "Both",
            Formula = "KNO3",
            Tank = ConcentrateType.A,
            Percentages = { ["K"] = 1 },
        };

        salt.TryMaterialise(out Fertilizer? built, out _).Should().BeTrue();

        built!.Potassium.Value.Should().BeApproximately(38.672, 0.02);
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release --filter "FullyQualifiedName~CustomSaltTests"`
Expected: build failure — `CustomSalt` does not exist.

- [ ] **Step 3: Write the implementation**

```csharp
using System.Text.Json.Serialization;
using SYT.NPKTools.Fertilizers;
using SYT.NPKTools.Fertilizers.Enums;

namespace SYT.NPKTools.Calculator;

/// <summary>
/// A fertilizer the grower owns and the catalogue does not carry.
/// </summary>
/// <remarks>
/// Stored as what was entered rather than as a built <see cref="Fertilizer"/>, so a saved file stays
/// readable and a formula can be re-derived if the library's atomic masses are ever corrected.
/// </remarks>
public sealed class CustomSalt
{
    /// <summary>The name to show. Unique among every salt on offer.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>The chemical formula, or null when the salt was described by percentages.</summary>
    [JsonPropertyName("formula")]
    public string? Formula { get; set; }

    /// <summary>Which concentrate tank the salt belongs in.</summary>
    [JsonPropertyName("tank")]
    public ConcentrateType Tank { get; set; } = ConcentrateType.A;

    /// <summary>Percentages by weight, keyed by nutrient form. Used when there is no formula.</summary>
    [JsonPropertyName("percentages")]
    public Dictionary<string, double> Percentages { get; init; } = new(StringComparer.Ordinal);

    /// <summary>How much dissolves in a litre, when known.</summary>
    [JsonPropertyName("solubility")]
    public double? SolubilityGramsPerLitre { get; set; }

    /// <summary>
    /// Builds the fertilizer this describes.
    /// </summary>
    /// <param name="fertilizer">The result, or null when the description is unusable.</param>
    /// <param name="error">What is wrong with the description, or null on success.</param>
    /// <returns><see langword="true"/> when a fertilizer was built.</returns>
    public bool TryMaterialise(out Fertilizer? fertilizer, out string? error)
    {
        fertilizer = null;

        if (string.IsNullOrWhiteSpace(Name))
        {
            error = "The salt needs a name.";
            return false;
        }

        if (!string.IsNullOrWhiteSpace(Formula))
        {
            return FormulaComposition.TryCreate(Name, Formula, Tank, out fertilizer, out error);
        }

        double total = Percentages.Values.Sum();
        if (total <= 0)
        {
            error = "The salt carries no nutrients, so no target can use it.";
            return false;
        }

        if (total > 100)
        {
            error = $"The percentages add up to {total:F1}, which is more than 100.";
            return false;
        }

        FertilizerBuilder builder = new FertilizerBuilder()
            .AddName(Name.Trim())
            .AddFormula(string.IsNullOrWhiteSpace(Formula) ? "—" : Formula)
            .AddType(Tank);

        foreach ((string form, double percent) in Percentages.Where(p => p.Value > 0))
        {
            if (!Apply(builder, form, percent))
            {
                error = $"'{form}' is not a nutrient form this calculator knows.";
                return false;
            }
        }

        fertilizer = builder.Build();
        error = null;
        return true;
    }

    private static bool Apply(FertilizerBuilder builder, string form, double percent)
    {
        switch (form)
        {
            case "No3": builder.AddNo3(percent); return true;
            case "Nh4": builder.AddNh4(percent); return true;
            case "Nh2": builder.AddNh2(percent); return true;
            case "P": builder.AddP(percent); return true;
            case "K": builder.AddK(percent); return true;
            case "Ca": builder.AddCaNonChelated(percent); return true;
            case "CaEdta": builder.AddCaEdta(percent); return true;
            case "Mg": builder.AddMgNonChelated(percent); return true;
            case "MgEdta": builder.AddMgEdta(percent); return true;
            case "S": builder.AddS(percent); return true;
            case "Fe": builder.AddFeNonChelated(percent); return true;
            case "FeEdta": builder.AddFeEdta(percent); return true;
            case "FeDtpa": builder.AddFeDtpa(percent); return true;
            case "FeEddha": builder.AddFeEddha(percent); return true;
            case "FeHbed": builder.AddFeHbed(percent); return true;
            case "Cu": builder.AddCuNonChelated(percent); return true;
            case "CuEdta": builder.AddCuEdta(percent); return true;
            case "Mn": builder.AddMnNonChelated(percent); return true;
            case "MnEdta": builder.AddMnEdta(percent); return true;
            case "Zn": builder.AddZnNonChelated(percent); return true;
            case "ZnEdta": builder.AddZnEdta(percent); return true;
            case "B": builder.AddB(percent); return true;
            case "Mo": builder.AddMo(percent); return true;
            case "Cl": builder.AddCl(percent); return true;
            case "Si": builder.AddSi(percent); return true;
            case "Se": builder.AddSe(percent); return true;
            case "Na": builder.AddNa(percent); return true;
            default: return false;
        }
    }
}
```

- [ ] **Step 4: Run the tests**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release --filter "FullyQualifiedName~CustomSaltTests"`
Expected: PASS.

- [ ] **Step 5: Format and commit**

```bash
dotnet format
git add web/SYT.NPKTools.Calculator/CustomSalt.cs tests/SYT.NPKTools.Calculator.Tests/CustomSaltTests.cs
git commit -m "feat(calculator): describe a salt the catalogue does not carry"
```

---

### Task 4: Custom salts join the shelf

**Files:**
- Modify: `web/SYT.NPKTools.Calculator/CalculatorModel.cs`
- Test: `tests/SYT.NPKTools.Calculator.Tests/CustomSaltShelfTests.cs`

**Interfaces:**
- Consumes: `CustomSalt` (Task 3).
- Produces: on `CalculatorModel` — `IReadOnlyList<CustomSalt> CustomSalts`,
  `bool TryAddCustomSalt(CustomSalt salt, out string? error)`, `void RemoveCustomSalt(string name)`,
  and `Catalogue` now returning built-ins followed by materialised custom salts.

- [ ] **Step 1: Write the failing test**

```csharp
using AwesomeAssertions;
using SYT.NPKTools.Fertilizers.Enums;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers a grower's own salts taking part in the recipe search.
/// </summary>
public class CustomSaltShelfTests
{
    private static CustomSalt Kno3() =>
        new() { Name = "Shop KNO3", Formula = "KNO3", Tank = ConcentrateType.A };

    /// <summary>
    /// An added salt appears on the shelf, ticked, and the count goes up by one.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryAddCustomSalt_PutsItOnTheShelfTicked()
    {
        CalculatorModel model = new();
        int before = model.Catalogue.Count;

        model.TryAddCustomSalt(Kno3(), out string? error).Should().BeTrue(because: error);

        model.Catalogue.Should().HaveCount(before + 1);
        model.Selected.Should().Contain("Shop KNO3");
    }

    /// <summary>
    /// A name already taken is refused. The shelf is keyed by name, so two salts sharing one would
    /// be selected and deselected together and one would silently ride along in every recipe.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryAddCustomSalt_WithATakenName_IsRefused()
    {
        CalculatorModel model = new();
        string existing = model.Catalogue[0].Name.Value;

        model.TryAddCustomSalt(
            new CustomSalt { Name = existing, Formula = "KNO3" }, out string? error)
            .Should().BeFalse();

        error.Should().Contain(existing);
    }

    /// <summary>
    /// The same custom name twice is refused for the same reason.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryAddCustomSalt_Twice_IsRefused()
    {
        CalculatorModel model = new();
        model.TryAddCustomSalt(Kno3(), out _).Should().BeTrue();

        model.TryAddCustomSalt(Kno3(), out string? error).Should().BeFalse();
        error.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// A custom salt reaches a real recipe — the whole point of adding one.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_CanBuildARecipeFromACustomSalt()
    {
        CalculatorModel model = new();
        model.Selected.Clear();
        model.TryAddCustomSalt(Kno3(), out _).Should().BeTrue();
        model.TryAddCustomSalt(
            new CustomSalt { Name = "Shop CalNit", Formula = "Ca(NO3)2*4H2O", Tank = ConcentrateType.A },
            out _).Should().BeTrue();

        model.TargetText = "N=100 K=150 Ca=80 L=100";
        model.Recalculate();

        model.Recipes.Should().NotBeEmpty();
    }

    /// <summary>
    /// Removing one takes it off the shelf and out of the selection.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void RemoveCustomSalt_TakesItOff()
    {
        CalculatorModel model = new();
        model.TryAddCustomSalt(Kno3(), out _).Should().BeTrue();

        model.RemoveCustomSalt("Shop KNO3");

        model.CustomSalts.Should().BeEmpty();
        model.Catalogue.Should().NotContain(f => f.Name.Value == "Shop KNO3");
        model.Selected.Should().NotContain("Shop KNO3");
    }

    /// <summary>
    /// An unusable description never reaches the shelf.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryAddCustomSalt_WithAnUnreadableFormula_IsRefused()
    {
        CalculatorModel model = new();

        model.TryAddCustomSalt(
            new CustomSalt { Name = "Bad", Formula = "Zz9" }, out string? error).Should().BeFalse();

        error.Should().NotBeNullOrWhiteSpace();
        model.CustomSalts.Should().BeEmpty();
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release --filter "FullyQualifiedName~CustomSaltShelfTests"`
Expected: build failure — `TryAddCustomSalt` does not exist.

- [ ] **Step 3: Change `CalculatorModel`**

`Catalogue` is currently a get-only property assigned in the constructor. Rename that field to
`_builtIns` and make `Catalogue` computed, so adding a salt does not need every consumer to refresh:

```csharp
    private readonly IReadOnlyList<Fertilizer> _builtIns;
    private readonly List<CustomSalt> _customSalts = [];
    private readonly List<Fertilizer> _materialised = [];

    /// <summary>Every salt on offer: the library's, then the grower's own.</summary>
    /// <remarks>
    /// Custom salts come last and in the order they were added, which is what keeps a link's
    /// built-in indices meaning what they meant before any were added.
    /// </remarks>
    public IReadOnlyList<Fertilizer> Catalogue => [.. _builtIns, .. _materialised];

    /// <summary>The salts the grower described themselves.</summary>
    public IReadOnlyList<CustomSalt> CustomSalts => _customSalts;

    /// <summary>
    /// Adds a salt of the grower's own, ticked and ready to use.
    /// </summary>
    /// <param name="salt">The description.</param>
    /// <param name="error">Why it was refused, or null on success.</param>
    /// <returns><see langword="true"/> when the salt was added.</returns>
    public bool TryAddCustomSalt(CustomSalt salt, out string? error)
    {
        ArgumentNullException.ThrowIfNull(salt);

        if (!salt.TryMaterialise(out Fertilizer? built, out error))
        {
            return false;
        }

        string name = built!.Name.Value;
        if (Catalogue.Any(f => string.Equals(f.Name.Value, name, StringComparison.OrdinalIgnoreCase)))
        {
            error = $"There is already a salt called '{name}'. Pick another name.";
            return false;
        }

        _customSalts.Add(salt);
        _materialised.Add(built);
        Selected.Add(name);
        error = null;
        return true;
    }

    /// <summary>
    /// Removes one of the grower's own salts.
    /// </summary>
    /// <param name="name">The salt's name. Unknown names are ignored.</param>
    public void RemoveCustomSalt(string name)
    {
        _customSalts.RemoveAll(s => string.Equals(s.Name, name, StringComparison.Ordinal));
        _materialised.RemoveAll(f => string.Equals(f.Name.Value, name, StringComparison.Ordinal));
        Selected.Remove(name);
    }
```

In the constructor, assign `_builtIns` where `Catalogue` was assigned, and keep ticking everything.

- [ ] **Step 4: Run the tests**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release --filter "FullyQualifiedName~CustomSaltShelfTests"`
Expected: PASS.

- [ ] **Step 5: Run every test, since `Catalogue` changed shape**

Run: `dotnet test SYT.NPKTools.slnx -c Release`
Expected: PASS. `CatalogueNames` and the link's index handling both read `Catalogue`; if a state
test fails, it is because custom salts now extend the list — Task 5 is where that is handled, so a
failure here means the ordering rule above was not followed.

- [ ] **Step 6: Format and commit**

```bash
dotnet format
git add web/SYT.NPKTools.Calculator/CalculatorModel.cs tests/SYT.NPKTools.Calculator.Tests/CustomSaltShelfTests.cs
git commit -m "feat(calculator): let the grower's own salts join the shelf"
```

---

### Task 5: Carrying custom salts between sessions

**Files:**
- Modify: `web/SYT.NPKTools.Calculator/CalculatorState.cs`
- Modify: `web/SYT.NPKTools.Calculator/CalculatorModel.cs` — `Capture` and `Apply`
- Test: `tests/SYT.NPKTools.Calculator.Tests/CustomSaltStateTests.cs`

**Interfaces:**
- Consumes: `CustomSalt` (Task 3), the model members from Task 4.
- Produces: `CalculatorState.CustomSalts` (`List<CustomSalt>`), carried in JSON as `customSalts` and
  in the fragment as one `cs=` entry per salt.

Fragment encoding, pipe-separated within an entry and comma-separated between them, everything
URL-escaped as a whole: `name~formula~tank~form:percent;form:percent`. A formula-defined salt leaves
the last field empty; a percentage-defined one leaves the formula empty.

- [ ] **Step 1: Write the failing test**

```csharp
using AwesomeAssertions;
using SYT.NPKTools.Fertilizers.Enums;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers custom salts surviving a file, a link, and an older link that has none.
/// </summary>
public class CustomSaltStateTests
{
    private static readonly string[] Catalogue = ["Calcium nitrate", "Potassium nitrate"];

    /// <summary>
    /// A formula-defined salt round-trips through a file.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Capture_ThenApply_RoundTripsAFormulaSalt()
    {
        CalculatorModel written = new();
        written.TryAddCustomSalt(
            new CustomSalt { Name = "Shop KNO3", Formula = "KNO3", Tank = ConcentrateType.A },
            out _).Should().BeTrue();

        CalculatorModel read = new();
        read.Apply(CalculatorState.FromJson(written.Capture().ToJson())!);

        read.CustomSalts.Should().ContainSingle();
        read.CustomSalts[0].Formula.Should().Be("KNO3");
        read.Catalogue.Should().Contain(f => f.Name.Value == "Shop KNO3");
    }

    /// <summary>
    /// A percentage-defined salt keeps its forms through a file.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Capture_ThenApply_RoundTripsAPercentageSalt()
    {
        CalculatorModel written = new();
        written.TryAddCustomSalt(
            new CustomSalt
            {
                Name = "Fe chelate",
                Tank = ConcentrateType.B,
                Percentages = { ["FeEdta"] = 13 },
            },
            out _).Should().BeTrue();

        CalculatorModel read = new();
        read.Apply(CalculatorState.FromJson(written.Capture().ToJson())!);

        read.CustomSalts.Should().ContainSingle();
        read.CustomSalts[0].Percentages.Should().ContainKey("FeEdta").WhoseValue.Should().Be(13);
    }

    /// <summary>
    /// And through a link.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ToFragment_CarriesCustomSalts()
    {
        CalculatorModel written = new();
        written.TryAddCustomSalt(
            new CustomSalt { Name = "Shop KNO3", Formula = "KNO3", Tank = ConcentrateType.A },
            out _).Should().BeTrue();

        string fragment = written.Capture().ToFragment(Catalogue);
        (CalculatorState? carried, _) = CalculatorState.FromFragment(fragment, Catalogue);

        CalculatorModel read = new();
        read.Apply(carried!);

        read.CustomSalts.Should().ContainSingle();
        read.CustomSalts[0].Name.Should().Be("Shop KNO3");
    }

    /// <summary>
    /// A link written before custom salts existed still opens, and brings none.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void FromFragment_ForALinkWithoutCustomSalts_StillOpens()
    {
        (CalculatorState? read, _) = CalculatorState.FromFragment("v=1&t=N%3D150%20L%3D50", Catalogue);

        read.Should().NotBeNull();
        read!.CustomSalts.Should().BeEmpty();
    }

    /// <summary>
    /// A stale file naming a salt that no longer materialises is dropped rather than crashing the
    /// load, the same way an unknown salt name already is.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Apply_ForAnUnusableCustomSalt_DropsIt()
    {
        CalculatorState state = new()
        {
            CustomSalts = [new CustomSalt { Name = "Broken", Formula = "Zz9" }],
        };

        CalculatorModel read = new();
        read.Apply(state);

        read.CustomSalts.Should().BeEmpty();
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test tests/SYT.NPKTools.Calculator.Tests -c Release --filter "FullyQualifiedName~CustomSaltStateTests"`
Expected: build failure — `CalculatorState.CustomSalts` does not exist.

- [ ] **Step 3: Add the field to `CalculatorState`**

```csharp
    /// <summary>Salts the grower described themselves. Absent from version 1 links.</summary>
    [JsonPropertyName("customSalts")]
    public List<CustomSalt> CustomSalts { get; set; } = [];
```

In `ToFragment`, after the existing version-2 keys:

```csharp
        foreach (CustomSalt salt in CustomSalts)
        {
            string forms = string.Join(
                ";",
                salt.Percentages.Where(p => p.Value > 0)
                    .OrderBy(p => p.Key, StringComparer.Ordinal)
                    .Select(p => $"{p.Key}:{p.Value.ToString("R", CultureInfo.InvariantCulture)}"));

            string entry = string.Join('~', salt.Name, salt.Formula ?? string.Empty, salt.Tank, forms);
            builder.Append("&cs=").Append(Uri.EscapeDataString(entry));
        }
```

`FromFragment` collects every `cs=` — note the existing parser keeps only the last value for a
repeated key, so gather them before that dictionary is built:

```csharp
        List<string> customEntries = [];
        foreach (string pair in fragment.TrimStart('#').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            if (pair.StartsWith("cs=", StringComparison.Ordinal))
            {
                customEntries.Add(Uri.UnescapeDataString(pair[3..]));
            }
        }
```

and then, after the state is created:

```csharp
        foreach (string entry in customEntries)
        {
            string[] fields = entry.Split('~');
            if (fields.Length < 3)
            {
                continue;
            }

            CustomSalt salt = new()
            {
                Name = fields[0],
                Formula = string.IsNullOrWhiteSpace(fields[1]) ? null : fields[1],
                Tank = Enum.TryParse(fields[2], out ConcentrateType tank) ? tank : ConcentrateType.A,
            };

            if (fields.Length > 3)
            {
                foreach (string form in fields[3].Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] halves = form.Split(':', 2);
                    if (halves.Length == 2 &&
                        double.TryParse(halves[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                    {
                        salt.Percentages[halves[0]] = value;
                    }
                }
            }

            state.CustomSalts.Add(salt);
        }
```

Add `using SYT.NPKTools.Fertilizers.Enums;` to the file.

- [ ] **Step 4: Extend `Capture` and `Apply`**

`Capture` gains `CustomSalts = [.. CustomSalts],`.

`Apply` gains, before the salt-selection block so the names exist when the selection is applied:

```csharp
        // Rebuilt from scratch: a file may name a salt this version can no longer build, and one
        // that cannot be materialised is dropped rather than failing the whole load.
        foreach (CustomSalt existing in CustomSalts.ToList())
        {
            RemoveCustomSalt(existing.Name);
        }

        foreach (CustomSalt salt in state.CustomSalts)
        {
            TryAddCustomSalt(salt, out _);
        }
```

- [ ] **Step 5: Run the tests**

Run: `dotnet test SYT.NPKTools.slnx -c Release`
Expected: PASS, every project.

- [ ] **Step 6: Format and commit**

```bash
dotnet format
git add web/SYT.NPKTools.Calculator tests/SYT.NPKTools.Calculator.Tests
git commit -m "feat(calculator): carry the grower's own salts in links and files"
```

---

### Task 6: The form, and lifting the picker out

**Files:**
- Create: `web/SYT.NPKTools.Calculator/Components/CustomSaltForm.razor`
- Create: `web/SYT.NPKTools.Calculator/Components/SaltPicker.razor`
- Modify: `web/SYT.NPKTools.Calculator/Pages/Home.razor` — replace the salt card with `<SaltPicker />`
- Modify: `web/SYT.NPKTools.Calculator/wwwroot/css/app.css`

**Interfaces:**
- Consumes: everything from Tasks 1–5.
- Produces: `<SaltPicker Changed="..." />`, which contains `<CustomSaltForm />`.

`Home.razor` is past 200 lines again and the salt card is the largest thing in it; the card plus a
form does not belong inline.

- [ ] **Step 1: Write `CustomSaltForm.razor`**

```razor
@inject CalculatorModel Model
@using SYT.NPKTools.Fertilizers
@using SYT.NPKTools.Fertilizers.Enums

@*
    Two ways in. The formula tab covers plain salts and works out the percentages, which also avoids
    the oxide trap: labels quote P₂O₅ and K₂O, and a figure copied off one overstates phosphorus by
    a factor of 2.3. The percentages tab covers blends and chelates, where a formula says nothing
    useful — what matters for a chelate is which agent holds the metal.
*@

<div class="segmented" role="group" aria-label="How the salt is described">
    <button type="button" class="@(_byFormula ? "on" : null)" @onclick="() => _byFormula = true">
        By formula
    </button>
    <button type="button" class="@(_byFormula ? null : "on")" @onclick="() => _byFormula = false">
        By percentages
    </button>
</div>

<div class="field">
    <label for="cs-name">Name</label>
    <input id="cs-name" type="text" value="@_name" @onchange="e => _name = e.Value?.ToString() ?? string.Empty" />
</div>

@if (_byFormula)
{
    <div class="field" style="margin-top:var(--grid-gap)">
        <label for="cs-formula">Formula</label>
        <input id="cs-formula" type="text" value="@_formula" placeholder="KNO3, Ca(NO3)2*4H2O"
               @onchange="OnFormulaChanged" />
    </div>

    @if (_parsed is { } parsed)
    {
        <p class="card-note">
            M = @parsed.MolarMass.ToString("F2") g/mol —
            @string.Join(", ", Breakdown(parsed))
        </p>
    }
}
else
{
    <div class="field-grid" style="margin-top:var(--grid-gap); grid-template-columns:repeat(3,1fr)">
        @foreach (string form in Forms)
        {
            <div class="field">
                <label for="cs-@form">@form</label>
                <input id="cs-@form" type="number" min="0" step="0.01"
                       value="@(_percentages.GetValueOrDefault(form))"
                       @onchange="e => SetPercent(form, e)" />
            </div>
        }
    </div>
}

<div class="field-grid" style="margin-top:var(--grid-gap); grid-template-columns:repeat(2,1fr)">
    <div class="field">
        <label for="cs-tank">Concentrate tank</label>
        <select id="cs-tank" value="@_tank" @onchange="OnTankChanged">
            <option value="@ConcentrateType.A">A — with calcium</option>
            <option value="@ConcentrateType.B">B — with sulfates and phosphates</option>
        </select>
    </div>
    <div class="field">
        <label for="cs-sol">Solubility, g/L — optional</label>
        <input id="cs-sol" type="number" min="0" step="1" value="@_solubility"
               @onchange="OnSolubilityChanged" />
    </div>
</div>

@if (_error is not null)
{
    <div class="notice error" role="alert">
        <Icon Kind="IconKind.Error" />
        <span>@_error</span>
    </div>
}

<div class="button-row">
    <button @onclick="Save">Add salt</button>
    <button class="secondary" @onclick="() => Cancelled.InvokeAsync()">Cancel</button>
</div>

@code {
    /// <summary>Raised after a salt is added, so the page can recalculate and persist.</summary>
    [Parameter]
    public EventCallback Changed { get; set; }

    /// <summary>Raised when the form should close without adding anything.</summary>
    [Parameter]
    public EventCallback Cancelled { get; set; }

    private static readonly string[] Forms =
    [
        "No3", "Nh4", "Nh2", "P", "K", "Ca", "CaEdta", "Mg", "MgEdta", "S",
        "Fe", "FeEdta", "FeDtpa", "FeEddha", "FeHbed", "Cu", "CuEdta",
        "Mn", "MnEdta", "Zn", "ZnEdta", "B", "Mo", "Cl", "Si", "Se", "Na",
    ];

    private bool _byFormula = true;
    private string _name = string.Empty;
    private string _formula = string.Empty;
    private ConcentrateType _tank = ConcentrateType.A;
    private double? _solubility;
    private string? _error;
    private ChemicalFormula? _parsed;
    private readonly Dictionary<string, double> _percentages = new(StringComparer.Ordinal);

    private static IEnumerable<string> Breakdown(ChemicalFormula formula) =>
        new (string Symbol, double Percent)[]
        {
            ("N-NO₃", formula.NitratePercent), ("N-NH₄", formula.AmmoniumPercent),
            ("N-amide", formula.AmidePercent), ("P", formula.PercentOf("P")),
            ("K", formula.PercentOf("K")), ("Ca", formula.PercentOf("Ca")),
            ("Mg", formula.PercentOf("Mg")), ("S", formula.PercentOf("S")),
            ("Fe", formula.PercentOf("Fe")), ("Mn", formula.PercentOf("Mn")),
            ("Zn", formula.PercentOf("Zn")), ("Cu", formula.PercentOf("Cu")),
            ("B", formula.PercentOf("B")), ("Mo", formula.PercentOf("Mo")),
            ("Cl", formula.PercentOf("Cl")), ("Na", formula.PercentOf("Na")),
        }
        .Where(x => x.Percent > 0)
        .Select(x => $"{x.Symbol} {x.Percent:F2}%");

    private void OnFormulaChanged(ChangeEventArgs e)
    {
        _formula = e.Value?.ToString() ?? string.Empty;
        _error = null;

        if (ChemicalFormula.TryParse(_formula, out ChemicalFormula? parsed, out string? error))
        {
            _parsed = parsed;
            _tank = FormulaComposition.SuggestTank(parsed!);
        }
        else
        {
            _parsed = null;
            _error = string.IsNullOrWhiteSpace(_formula) ? null : error;
        }
    }

    private void OnTankChanged(ChangeEventArgs e) =>
        _tank = Enum.TryParse(e.Value?.ToString(), out ConcentrateType tank) ? tank : ConcentrateType.A;

    private void OnSolubilityChanged(ChangeEventArgs e) =>
        _solubility = double.TryParse(
            e.Value?.ToString(),
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out double parsed) && parsed > 0
            ? parsed
            : null;

    private void SetPercent(string form, ChangeEventArgs e) =>
        _percentages[form] = double.TryParse(
            e.Value?.ToString(),
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out double parsed) && parsed >= 0
            ? parsed
            : 0;

    private async Task Save()
    {
        CustomSalt salt = new()
        {
            Name = _name,
            Formula = _byFormula ? _formula : null,
            Tank = _tank,
            SolubilityGramsPerLitre = _solubility,
        };

        if (!_byFormula)
        {
            foreach ((string form, double percent) in _percentages.Where(p => p.Value > 0))
            {
                salt.Percentages[form] = percent;
            }
        }

        if (!Model.TryAddCustomSalt(salt, out string? error))
        {
            _error = error;
            return;
        }

        _name = string.Empty;
        _formula = string.Empty;
        _parsed = null;
        _percentages.Clear();
        _error = null;

        await Changed.InvokeAsync();
        await Cancelled.InvokeAsync();
    }
}
```

- [ ] **Step 2: Write `SaltPicker.razor`**

Move the existing `<section class="card">` holding `<h2>Salts you have …</h2>` out of `Home.razor`
verbatim, then add the custom-salt affordances: an "Add salt" button beside All and None that
toggles `_adding`, the form when `_adding` is true, and a badge plus a remove button on custom rows.

```razor
@inject CalculatorModel Model

<section class="card">
    <h2>Salts you have — @Model.Selected.Count / @Model.Catalogue.Count</h2>
    <div class="button-row" style="margin-top:0">
        <button class="secondary" @onclick="SelectAll">All</button>
        <button class="secondary" @onclick="SelectNone">None</button>
        <button @onclick="() => _adding = !_adding">@(_adding ? "Close" : "Add salt")</button>
    </div>

    @if (_adding)
    {
        <div class="group-divider">Your own salt</div>
        <CustomSaltForm Changed="Changed" Cancelled="() => _adding = false" />
    }

    <div class="salt-list">
        @foreach (IGrouping<bool, Fertilizer> group in Model.Catalogue
            .GroupBy(FertilizerBundleGenerator.IsMicro)
            .OrderBy(g => g.Key))
        {
            <div class="salt-group">@(group.Key ? "Micronutrient" : "Macronutrient")</div>
            @foreach (Fertilizer salt in group)
            {
                <label class="salt-row">
                    <input type="checkbox" checked="@Model.Selected.Contains(salt.Name.Value)"
                           @onchange="e => OnToggled(salt.Name.Value, e)" />
                    <span>@salt.Name.Value</span>
                    @if (IsCustom(salt))
                    {
                        <button class="secondary tiny" title="Remove this salt"
                                @onclick:preventDefault @onclick:stopPropagation
                                @onclick="() => Remove(salt.Name.Value)">✕</button>
                    }
                    <span class="formula">@salt.Formula.Value</span>
                </label>
            }
        }
    </div>

    @if (Model.UncoveredElements.Count > 0)
    {
        <div class="notice warn" role="status">
            <Icon Kind="IconKind.Warning" />
            <span>
                <strong>No source for</strong> @string.Join(", ", Model.UncoveredElements).
                Nothing ticked above supplies these, so no recipe can reach them.
            </span>
        </div>
    }
</section>

@code {
    /// <summary>Raised after any edit, so the page can recalculate and persist.</summary>
    [Parameter]
    public EventCallback Changed { get; set; }

    private bool _adding;

    private bool IsCustom(Fertilizer salt) =>
        Model.CustomSalts.Any(c => string.Equals(c.Name, salt.Name.Value, StringComparison.Ordinal));

    private Task OnToggled(string name, ChangeEventArgs e)
    {
        if (e.Value is true)
        {
            Model.Selected.Add(name);
        }
        else
        {
            Model.Selected.Remove(name);
        }

        return Changed.InvokeAsync();
    }

    private Task Remove(string name)
    {
        Model.RemoveCustomSalt(name);
        return Changed.InvokeAsync();
    }

    private Task SelectAll()
    {
        foreach (Fertilizer salt in Model.Catalogue)
        {
            Model.Selected.Add(salt.Name.Value);
        }

        return Changed.InvokeAsync();
    }

    private Task SelectNone()
    {
        Model.Selected.Clear();
        return Changed.InvokeAsync();
    }
}
```

- [ ] **Step 3: Wire it into `Home.razor`**

Delete the salt `<section>` and its `OnSaltToggled`, `SelectAll` and `SelectNone` handlers, and put
in their place:

```razor
        <SaltPicker Changed="Edited" />
```

- [ ] **Step 4: Add the CSS for the remove button**

```css
/* A row action, sized so it does not compete with the salt's own name. */
button.tiny {
  min-height: 22px;
  margin-left: auto;
  padding: 0 6px;
  font-size: var(--text-xs);
  line-height: 1;
}

.salt-row button.tiny + .formula { margin-left: 8px; }
```

- [ ] **Step 5: Build, run and check**

```bash
dotnet build SYT.NPKTools.slnx -c Release
dotnet run --project web/SYT.NPKTools.Calculator
```

Check: "Add salt" opens the form; typing `Ca(NO3)2*4H2O` shows `M = 236.15 g/mol — N-NO₃ 11.86%,
Ca 16.97%` and preselects tank A; adding it puts a ticked row in the macronutrient group with a ✕;
typing a name that already exists is refused with the clash named; a salt entered with `FeEdta 13`
on the percentages tab lands in the **micronutrient** group; ✕ removes it; the recipes on the right
change with every one of these.

- [ ] **Step 6: Format and commit**

```bash
dotnet format
git add web/SYT.NPKTools.Calculator
git commit -m "feat(calculator): add salts the catalogue does not carry"
```

---

### Task 7: Changelog

**Files:**
- Modify: `CHANGELOG.md`

- [ ] **Step 1: Verify everything**

```bash
dotnet format --verify-no-changes
dotnet build SYT.NPKTools.slnx -c Release
dotnet test SYT.NPKTools.slnx -c Release
```

- [ ] **Step 2: Add the entry under `## [Unreleased]`**

```markdown
- Salts the catalogue does not carry can be added by the grower — by chemical formula, which derives
  the percentages, or by percentages for blends and chelates. They take part in the recipe search
  exactly as built-in salts do, and are classified macro or micro by composition rather than by a
  flag. `ChemicalFormula` and `FormulaComposition` in `SYT.NPKTools`.
```

- [ ] **Step 3: Commit**

```bash
git add CHANGELOG.md
git commit -m "docs: record custom salts"
```

---

## Self-review

**Spec coverage.** Formula path → Tasks 1, 2, 6. Percentages path and chelates → Tasks 3, 6. Parser
and its catalogue-derived test set → Task 1. Unique names → Task 4. Storage in file and link, with
built-in indices unmoved → Task 5. Concentrate-tank suggestion → Task 2, overridable in Task 6.
Optional solubility → Tasks 3 and 6. Macro/micro by composition → asserted in Tasks 2 and 4, checked
by eye in Task 6. Interface → Task 6.

**Type consistency.** `ChemicalFormula.TryParse(text, out formula, out error)` is used identically in
Tasks 1, 2 and 6. `FormulaComposition.TryCreate(name, formula, type, out fertilizer, out error)` in
Tasks 2 and 3. `CustomSalt.TryMaterialise(out fertilizer, out error)` in Tasks 3, 4 and 5.
`TryAddCustomSalt(salt, out error)` and `RemoveCustomSalt(name)` in Tasks 4, 5 and 6. The
percentage-form keys are one list, defined in Task 3 and reused verbatim in Task 6.

**Known soft spots**, stated rather than hidden:

- **Nitrogen may be double-counted in bracketed groups** — `Classify` and the recursive call both
  attribute `(NH4)2`. Task 1 Step 4 says which test catches it and what to do.
- The catalogue cross-check assumes `Nitrogen.Value` sums the three forms and that the element value
  objects hold percentages. Both are consistent with how `FertilizerCollectionBuilder` declares
  salts, but read the value objects if a comparison fails by a round factor.
- `Percentages` uses string keys rather than an enum, so a typo is a runtime refusal rather than a
  compile error. The keys are produced by a fixed list in the form, and `Apply` refuses unknown ones
  by name, which keeps the failure loud.
- No component tests: there is no bUnit here, so Task 6 ends with a specific list to check by eye.
