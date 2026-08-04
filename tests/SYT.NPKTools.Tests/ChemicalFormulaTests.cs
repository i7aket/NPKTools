using AwesomeAssertions;
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
        ChemicalFormula.TryParse(text, out ChemicalFormula? formula, out FormulaProblem? problem)
            .Should().BeTrue(because: problem?.ToString());
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
    /// <param name="text">The formula.</param>
    /// <param name="nitrate">Expected nitrate nitrogen, as a percentage.</param>
    /// <param name="ammonium">Expected ammonium nitrogen, as a percentage.</param>
    /// <param name="amide">Expected amide nitrogen, as a percentage.</param>
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
            // Chelated salts are outside what a formula can describe, and the reason is worth
            // stating: EDTA spelled out as atoms carries nitrogen, but that nitrogen belongs to the
            // ligand and no plant can reach it. The catalogue rightly declares zero, so comparing a
            // parsed EDTA formula against it would be comparing two different questions.
            if (IsChelated(salt))
            {
                continue;
            }

            if (!ChemicalFormula.TryParse(salt.Formula.Value, out ChemicalFormula? formula, out _))
            {
                continue;
            }

            checkedSalts++;
            string why = salt.Name.Value;
            formula!.PercentOf("K").Should().BeApproximately(salt.Potassium.Value, 0.02, why);
            formula.PercentOf("P").Should().BeApproximately(salt.Phosphorus.Value, 0.02, why);
            formula.PercentOf("S").Should().BeApproximately(salt.Sulfur.Value, 0.02, why);
            formula.PercentOf("Ca").Should().BeApproximately(salt.Calcium.Value, 0.02, why);
            formula.PercentOf("Mg").Should().BeApproximately(salt.Magnesium.Value, 0.02, why);
            formula.PercentOf("N").Should().BeApproximately(salt.Nitrogen.Value, 0.02, why);
        }

        checkedSalts.Should().BeGreaterThan(15);
    }

    /// <summary>
    /// Whether any of the salt's nutrients is held by a chelating agent.
    /// </summary>
    private static bool IsChelated(Fertilizer salt) =>
        salt.Calcium.CaEdta > 0 || salt.Magnesium.MgEdta > 0 || salt.Copper.CuEdta > 0
        || salt.Manganese.MnEdta > 0 || salt.Zinc.ZnEdta > 0
        || salt.Iron.FeEdta > 0 || salt.Iron.FeDtpa > 0
        || salt.Iron.FeEddha > 0 || salt.Iron.FeHbed > 0;

    /// <summary>
    /// A formula that cannot be read says where it gave up, rather than failing blankly.
    /// </summary>
    /// <param name="text">The unreadable formula.</param>
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
        bool parsed = ChemicalFormula.TryParse(text, out ChemicalFormula? formula, out FormulaProblem? problem);

        parsed.Should().BeFalse();
        formula.Should().BeNull();
        problem.Should().NotBeNull();
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
