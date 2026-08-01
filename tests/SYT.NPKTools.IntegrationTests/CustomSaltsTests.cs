using SYT.NPKTools.Concentrates;
using SYT.NPKTools.Fertilizers;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.IntegrationTests;

/// <summary>
/// Covers the whole path a person with their own salts takes: a shelf, their tap water, a target, and a
/// concentrate to dose from.
/// </summary>
/// <remarks>
/// The unit tests pin the shape of generated bundles. What they cannot show is that the shape is any
/// use, so these run the real solver end to end and assert the recipes land on target. Generation that
/// produced a tidy list of bundles none of which solved would pass every unit test and be worthless.
/// </remarks>
public class CustomSaltsTests
{
    /// <summary>
    /// A shelf of the six salts a hobbyist actually buys, defined here rather than taken from the preset
    /// catalogue so the test exercises the custom path for real.
    /// </summary>
    private static Fertilizer[] Shelf() =>
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
        new FertilizerBuilder().AddName("Magnesium Nitrate Hexahydrate").AddType(ConcentrateType.A)
            .AddNo3(10.93).AddMgNonChelated(9.48).Build(),
    ];

    private static PpmTarget Target() => new PpmTargetBuilder()
        .AddN(150).AddP(50).AddK(210).AddCa(160).AddMg(50).AddS(65).AddLiters(100)
        .Build();

    /// <summary>
    /// The claim the feature rests on: a caller's own salts yield several recipes, not one. Before
    /// generation, a custom shelf was a single bundle and therefore a single answer, while the preset
    /// catalogue offered a dozen to compare.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void OwnSalts_YieldSeveralDistinctRecipes()
    {
        // Arrange
        IFertilizerOptimizationService service = NpkTools.CreateOptimizationService(Shelf());

        // Act
        Solutions found = service.FindMacroSolutions(Target());

        // Assert
        Assert.True(found.Count > 1, $"expected several recipes from six salts, got {found.Count}");
    }

    /// <summary>
    /// Every recipe must actually hit the target. A generated bundle that returns a mix missing its
    /// calcium is worse than a bundle that returns nothing.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void OwnSalts_EveryRecipeMeetsTheTarget()
    {
        // Arrange
        IFertilizerOptimizationService service = NpkTools.CreateOptimizationService(Shelf());
        IPpmCalculationService calculator = NpkTools.CreatePpmCalculator();
        PpmTarget target = Target();

        // Act
        Solutions found = service.FindMacroSolutions(target);

        // Assert
        Assert.NotEmpty(found);
        foreach (Solution recipe in found)
        {
            Ppm actual = calculator.CalculatePpm([.. recipe], recipe.WaterLiters);

            Assert.Equal(target.N.Value, actual.Nitrogen.Value, 1);
            Assert.Equal(target.P.Value, actual.Phosphorus.Value, 1);
            Assert.Equal(target.K.Value, actual.Potassium.Value, 1);
            Assert.Equal(target.Ca.Value, actual.Calcium.Value, 1);
            Assert.Equal(target.Mg.Value, actual.Magnesium.Value, 1);
        }
    }

    /// <summary>
    /// Each generated bundle is meant to force a different route to the same target, so the recipes must
    /// differ from one another. Identical recipes would mean the bundles were doing no work.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void OwnSalts_RecipesDifferFromEachOther()
    {
        // Arrange
        IFertilizerOptimizationService service = NpkTools.CreateOptimizationService(Shelf());

        // Act
        Solutions found = service.FindMacroSolutions(Target());

        // Assert
        IEnumerable<string> signatures = found.Select(recipe => string.Join(
            '|',
            recipe.OrderBy(f => f.Name.Value)
                  .Select(f => $"{f.Name.Value}:{f.Weight.Value:F2}")));

        Assert.Equal(found.Count, signatures.Distinct().Count());
    }

    /// <summary>
    /// The three features have to compose, because in practice they are used together: the water is
    /// deducted, the caller's own salts meet what is left, and the result is stored as a concentrate.
    /// Each step is tested alone elsewhere; this asserts the chain holds — the finished tank, water
    /// included, lands on the original target.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void OwnSalts_WithSourceWaterAndConcentrate_LandOnTheOriginalTarget()
    {
        // Arrange
        WaterProfile water = new WaterProfileBuilder().AddCa(45).AddMg(12).AddS(18).Build();
        PpmTarget target = Target();
        IFertilizerOptimizationService service = NpkTools.CreateOptimizationService(Shelf());
        IPpmCalculationService calculator = NpkTools.CreatePpmCalculator();

        // Act
        PpmTarget adjusted = target.AdjustFor(water).Target;
        Solution recipe = service.FindMacroSolutions(adjusted)[0];
        ConcentratePlan plan = recipe.AsConcentrate(concentrateLiters: 1);
        Ppm fromSalts = calculator.CalculatePpm([.. recipe], recipe.WaterLiters);

        // Assert — the salts cover the shortfall, and the water supplies the rest
        Assert.Equal(target.Ca.Value, fromSalts.Calcium.Value + water.Calcium.Value, 1);
        Assert.Equal(target.Mg.Value, fromSalts.Magnesium.Value + water.Magnesium.Value, 1);

        // Assert — concentrating moves the water, not the salt, so the dose reconstitutes that recipe
        Assert.Equal(100, plan.DilutionRatio, 9);
        Assert.Equal(10, plan.MillilitresPerLiter, 9);
        Assert.Equal(
            recipe.Sum(f => f.Weight.Value),
            plan.TankA.TotalGrams + plan.TankB.TotalGrams,
            6);
        Assert.False(plan.HasPrecipitationRisk);
    }

    /// <summary>
    /// A shelf missing an element the target asks for must say which element, not merely return nothing.
    /// "No solutions" sends someone hunting through their salts; "you have no magnesium source" does not.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void ShelfMissingAnElement_NamesItRatherThanJustFailing()
    {
        // Arrange — the same shelf with both magnesium salts removed
        Fertilizer[] withoutMagnesium =
            [.. Shelf().Where(f => f.Magnesium.Value == 0)];

        // Act
        CustomFertilizerBundleRepository repository = NpkTools.CreateBundleRepository(withoutMagnesium);
        Solutions found = new FertilizerOptimizationService(NpkTools.CreateOptimizer(), repository)
            .FindMacroSolutions(Target());

        // Assert
        Assert.Contains("Mg", repository.MacroGeneration.UncoveredElements);
        Assert.False(repository.MacroGeneration.IsComplete);
        Assert.Empty(found);
    }
}
