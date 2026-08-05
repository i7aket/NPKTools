using AwesomeAssertions;
using SYT.NPKTools.Concentrates;
using SYT.NPKTools.Fertilizers;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.IntegrationTests;

/// <summary>
/// The worked examples from <c>docs/examples.md</c>, as tests.
/// </summary>
/// <remarks>
/// <para>
/// Documentation examples rot: an API moves, the prose stays, and a reader loses an afternoon to a
/// snippet that has not compiled for two releases. These are the same examples the document walks
/// through, so the build fails rather than the reader.
/// </para>
/// <para>
/// They are written the way a caller would write them — factory methods, no test doubles, no internals —
/// which is also what makes them worth reading. If one of these needs a helper to work, the API needs
/// the helper.
/// </para>
/// </remarks>
public class DocumentedExamplesTests
{
    /// <summary>
    /// The shortest thing that works: a target string in, weights out.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void Example_TheShortestThingThatWorks()
    {
        IPpmTargetParser parser = NpkTools.CreateTargetParser();
        IFertilizerOptimizationService optimizer = NpkTools.CreateOptimizationService();

        PpmTarget target = parser.Parse("N=150 P=50 K=210 Ca=160 Mg=50 S=65 L=100");
        Solutions recipes = optimizer.FindMacroSolutions(target);

        recipes.Should().NotBeEmpty(because: "the preset catalogue can reach an ordinary feed");

        Solution recipe = recipes[0];
        recipe.WaterLiters.Should().Be(100);
        recipe.Should().OnlyContain(salt => salt.Weight.Value > 0);
    }

    /// <summary>
    /// What the water already supplies comes off the target before the optimizer sees it.
    /// </summary>
    /// <remarks>
    /// The number that matters here is the one that shrinks: a target of 160 ppm calcium against water
    /// that already carries 60 leaves 100 for the salts. Every calculator that ignores this overdoses
    /// calcium on hard water by exactly the amount the water brought.
    /// </remarks>
    [Fact]
    [Trait("Category", "Integration")]
    public void Example_SourceWaterComesOffTheTarget()
    {
        PpmTarget target = NpkTools.CreateTargetParser().Parse("N=150 K=210 Ca=160 Mg=50 S=65 L=100");
        WaterProfile water = new WaterProfileBuilder().AddCa(60).AddMg(12).Build();

        WaterAdjustedTarget adjusted = target.AdjustFor(water);

        adjusted.Target.Ca.Value.Should().BeApproximately(100, 1e-9);
        adjusted.Target.Mg.Value.Should().BeApproximately(38, 1e-9);
        adjusted.Excesses.Should().BeEmpty(because: "the water supplies less than the target of each");
    }

    /// <summary>
    /// Water that oversupplies an element is reported rather than silently ignored.
    /// </summary>
    /// <remarks>
    /// Fertilizer only adds. No recipe can bring calcium down once the water carries more than the
    /// target, so the useful behaviour is to say which element and by how much.
    /// </remarks>
    [Fact]
    [Trait("Category", "Integration")]
    public void Example_WaterThatOversuppliesIsReported()
    {
        PpmTarget target = NpkTools.CreateTargetParser().Parse("N=150 K=210 Ca=100 L=100");
        WaterProfile hardWater = new WaterProfileBuilder().AddCa(160).Build();

        WaterAdjustedTarget adjusted = target.AdjustFor(hardWater);

        NutrientExcess excess = adjusted.Excesses.Should().ContainSingle().Subject;
        excess.Element.Should().Be("Ca");
        excess.InWater.Should().BeApproximately(160, 1e-9);
        excess.Target.Should().BeApproximately(100, 1e-9);
        excess.Overshoot.Should().BeApproximately(60, 1e-9);
    }

    /// <summary>
    /// A shelf of your own salts instead of the preset catalogue.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void Example_YourOwnShelf()
    {
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
        PpmTarget target = NpkTools.CreateTargetParser().Parse("N=150 P=50 K=210 Ca=160 Mg=50 S=65 L=100");

        Solutions recipes = optimizer.FindMacroSolutions(target);

        recipes.Should().NotBeEmpty();
        recipes[0].Select(salt => salt.Name.Value).Should().BeSubsetOf(shelf.Select(s => s.Name.Value));
    }

    /// <summary>
    /// A salt described by its chemical formula, with the percentages derived rather than typed.
    /// </summary>
    /// <remarks>
    /// This is the safe way in, and the reason is the oxide convention: a bag quotes P₂O₅ and K₂O, and a
    /// figure copied straight off it overstates phosphorus by 2.29×. A formula cannot make that mistake.
    /// </remarks>
    [Fact]
    [Trait("Category", "Integration")]
    public void Example_ASaltFromItsFormula()
    {
        ChemicalFormula.TryParse("KNO3", out ChemicalFormula? formula, out FormulaProblem? problem)
            .Should().BeTrue(because: problem?.Message);

        formula!.MolarMass.Should().BeApproximately(101.1, 0.1);
        formula.PercentOf("K").Should().BeApproximately(38.67, 0.05);
        formula.NitratePercent.Should().BeApproximately(13.85, 0.05);

        FormulaComposition.TryCreate("Shop KNO3", "KNO3", ConcentrateType.A, out Fertilizer? salt, out _)
            .Should().BeTrue();
        salt!.Name.Value.Should().Be("Shop KNO3");
    }

    /// <summary>
    /// A formula that cannot be read says which failure it is, not just that it failed.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void Example_AFormulaThatCannotBeRead()
    {
        ChemicalFormula.TryParse("KNO3!", out _, out FormulaProblem? problem).Should().BeFalse();

        problem!.Kind.Should().Be(FormulaProblemKind.UnexpectedCharacter);
        problem.Value.Should().Be("!");
        problem.Position.Should().Be(5);
    }

    /// <summary>
    /// Judging a recipe rather than only computing it: what is actually in the tank, and what a meter
    /// will say about it.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void Example_JudgeTheMix()
    {
        PpmTarget target = NpkTools.CreateTargetParser().Parse("N=150 P=50 K=210 Ca=160 Mg=50 S=65 L=100");
        Solution recipe = NpkTools.CreateOptimizationService().FindMacroSolutions(target)[0];

        Ppm inTank = NpkTools.CreatePpmCalculator().CalculatePpm(recipe, recipe.WaterLiters);

        inTank.Nitrogen.Value.Should().BeApproximately(150, 1.5, because: "within 1%, which is finer than a kitchen scale");

        ConductivityEstimate ec = inTank.EstimateConductivity();
        ec.MilliSiemensPerCm.Should().BeInRange(1.0, 3.0);
        ec.IsWithinValidatedRange.Should().BeTrue(because: "an ordinary feed is inside the range the model was checked against");

        // A TDS meter is a conductivity meter with a multiplier chosen by its manufacturer.
        ec.AsTdsPpm().Should().BeApproximately(ec.MicroSiemensPerCm * 0.5, 1);
    }

    /// <summary>
    /// The acid a water's alkalinity needs, from the carbonate equilibrium rather than a rule of thumb.
    /// </summary>
    /// <remarks>
    /// The figure worth noticing is that the dose is less than the alkalinity. Neutralising all of it
    /// would take the water past the target pH; the rule of thumb overstates the dose by about a quarter.
    /// </remarks>
    [Fact]
    [Trait("Category", "Integration")]
    public void Example_HowMuchAcid()
    {
        AcidPlan plan = AcidDose.Calculate(
            alkalinityMilliequivalentsPerLitre: 3,
            waterPh: 7.6,
            targetPh: 5.8,
            acid: Acid.Nitric60,
            litres: 100);

        plan.MilliequivalentsPerLitre.Should().BeLessThan(3);
        plan.MilliequivalentsPerLitre.Should().BeApproximately(2.3, 0.1);
        plan.Millilitres.Should().BePositive();

        // Nitric acid brings nitrogen with it, and that nitrogen is in the reservoir.
        plan.NutrientSymbol.Should().Be("N");
        plan.NutrientPpm.Should().BePositive();
    }

    /// <summary>
    /// A recipe stored as an A/B concentrate, checked for whether it will physically dissolve.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void Example_AsAConcentrate()
    {
        PpmTarget target = NpkTools.CreateTargetParser().Parse("N=150 P=50 K=210 Ca=160 Mg=50 S=65 L=100");
        Solution recipe = NpkTools.CreateOptimizationService().FindMacroSolutions(target)[0];

        ConcentratePlan plan = recipe.AsConcentrate(concentrateLiters: 2);

        plan.DilutionRatio.Should().BeApproximately(50, 1e-9);
        plan.TankA.Components.Should().NotBeEmpty();
        plan.TankB.Components.Should().NotBeEmpty();

        // Calcium is never stored beside sulfate or phosphate: at concentrate strength they precipitate.
        plan.TankA.Components.Should().OnlyContain(c => c.Fertilizer.Type == ConcentrateType.A);
        plan.TankB.Components.Should().OnlyContain(c => c.Fertilizer.Type == ConcentrateType.B);
    }

    /// <summary>
    /// A concentrate too strong to dissolve says so, with the two figures behind the sentence.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void Example_AConcentrateThatWillNotDissolve()
    {
        Fertilizer phosphate = new FertilizerBuilder()
            .AddName("Calcium Monobasic Phosphate")
            .AddType(ConcentrateType.B)
            .AddP(23.5).AddCaNonChelated(15.9)
            .AddWeight(20)
            .Build();

        ConcentratePlan plan = new Solution([phosphate], waterLiters: 100).AsConcentrate(concentrateLiters: 0.5);

        ConcentrateWarning warning = plan.Warnings
            .Should().ContainSingle(w => w.Kind == ConcentrateWarningKind.SolubilityExceeded).Subject;

        warning.Actual.Should().BeApproximately(40, 1e-9, because: "20 g in 0.5 L is 40 g/L");
        warning.Allowed.Should().Be(18, because: "that is what the table gives at 20 °C");
    }
}
