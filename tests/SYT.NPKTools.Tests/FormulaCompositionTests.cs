using AwesomeAssertions;
using SYT.NPKTools.Fertilizers;
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
            out Fertilizer? salt, out FormulaProblem? problem);

        created.Should().BeTrue(because: problem?.ToString());
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
            out Fertilizer? salt, out FormulaProblem? problem).Should().BeFalse();

        salt.Should().BeNull();
        problem.Should().NotBeNull();
    }

    /// <summary>
    /// A salt with no name is refused before anything else is looked at.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void TryCreate_WithoutAName_Fails()
    {
        FormulaComposition.TryCreate("  ", "KNO3", ConcentrateType.A, out _, out FormulaProblem? problem)
            .Should().BeFalse();

        problem.Should().NotBeNull();
    }

    /// <summary>
    /// The tank suggestion follows the rule the concentrate split already enforces: calcium apart
    /// from sulfate and phosphate.
    /// </summary>
    /// <param name="formula">The formula to place.</param>
    /// <param name="expected">The tank it should be suggested for.</param>
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

    /// <summary>
    /// A chelate written out as atoms is flagged, because the formula path would offer the ligand's
    /// nitrogen as plant food. Plain salts of the same metals are not flagged.
    /// </summary>
    /// <param name="formula">The formula to judge.</param>
    /// <param name="expected">Whether it should be flagged as a chelate.</param>
    [Theory]
    [InlineData("C10H12N2O8FeNa", true)]
    [InlineData("FeSO4*7H2O", false)]
    [InlineData("CO(NH2)2", false)]
    [InlineData("ZnSO4*7H2O", false)]
    [Trait("Category", "Unit")]
    public void LooksChelated_FlagsAnOrganicLigandHoldingAMetal(string formula, bool expected)
    {
        ChemicalFormula.TryParse(formula, out ChemicalFormula? parsed, out FormulaProblem? problem)
            .Should().BeTrue(because: problem?.ToString());

        FormulaComposition.LooksChelated(parsed!).Should().Be(expected);
    }
}
