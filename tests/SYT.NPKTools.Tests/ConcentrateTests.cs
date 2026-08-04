using AwesomeAssertions;
using SYT.NPKTools.Concentrates;
using SYT.NPKTools.Fertilizers;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers splitting a working-strength mix into A/B concentrate tanks.
/// </summary>
/// <remarks>
/// Two things can go wrong here and only one of them is arithmetic. The dilution figures are pinned
/// because a wrong ratio quietly under- or over-feeds every subsequent watering. The precipitation
/// heuristic is pinned in both directions because a check that fires on monocalcium phosphate — a
/// legitimately soluble salt carrying calcium and phosphate in one compound — would train users to
/// ignore it, which is worse than not having it.
/// </remarks>
public class ConcentrateTests
{
    private const double Precision = 1e-9;

    private static Fertilizer CalciumNitrate(double grams) => new FertilizerBuilder()
        .AddName("Calcium Nitrate Tetrahydrate")
        .AddType(ConcentrateType.A)
        .AddNo3(11.86).AddCaNonChelated(16.97)
        .AddWeight(grams)
        .Build();

    private static Fertilizer MagnesiumSulfate(double grams) => new FertilizerBuilder()
        .AddName("Magnesium Sulfate Heptahydrate")
        .AddType(ConcentrateType.B)
        .AddMgNonChelated(9.86).AddS(13.01)
        .AddWeight(grams)
        .Build();

    private static Fertilizer MonopotassiumPhosphate(double grams) => new FertilizerBuilder()
        .AddName("Monopotassium Phosphate")
        .AddType(ConcentrateType.B)
        .AddP(22.76).AddK(28.73)
        .AddWeight(grams)
        .Build();

    private static Fertilizer PotassiumNitrate(double grams) => new FertilizerBuilder()
        .AddName("Potassium Nitrate")
        .AddType(ConcentrateType.A)
        .AddNo3(13.85).AddK(38.67)
        .AddWeight(grams)
        .Build();

    // ---------------------------------------------------------------- the split

    /// <summary>
    /// The catalogue already knows which tank each salt belongs in. Honouring that is the whole point:
    /// the split must come from the data, not from re-deriving chemistry the catalogue has already
    /// settled.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_UsesEachFertilizersOwnTankAssignment()
    {
        // Arrange
        Solution solution = new(
            [CalciumNitrate(100), PotassiumNitrate(50), MagnesiumSulfate(40), MonopotassiumPhosphate(20)],
            waterLiters: 100);

        // Act
        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters: 1);

        // Assert
        plan.TankA.Components.Select(c => c.Fertilizer.Name.Value)
            .Should().BeEquivalentTo(["Calcium Nitrate Tetrahydrate", "Potassium Nitrate"]);
        plan.TankB.Components.Select(c => c.Fertilizer.Name.Value)
            .Should().BeEquivalentTo(["Magnesium Sulfate Heptahydrate", "Monopotassium Phosphate"]);
        plan.HasWarnings.Should().BeFalse();
    }

    /// <summary>
    /// Concentrating changes how much water the salt goes into, never how much salt is needed. If the
    /// weights moved, the finished solution would no longer match the recipe the optimizer solved for.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_KeepsTheWorkingRecipesWeights()
    {
        // Arrange
        Solution solution = new([CalciumNitrate(100), MagnesiumSulfate(40)], waterLiters: 100);

        // Act
        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters: 2);

        // Assert
        plan.TankA.TotalGrams.Should().BeApproximately(100, Precision);
        plan.TankB.TotalGrams.Should().BeApproximately(40, Precision);
    }

    /// <summary>
    /// A mix with no calcium needs no separation, so one tank stays empty. That is the expected outcome
    /// rather than a degenerate case, and callers need to be able to tell without inspecting counts.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_MixWithoutCalcium_LeavesTankAWithTheNitratesAndBWithTheRest()
    {
        // Arrange
        Solution solution = new([PotassiumNitrate(50), MonopotassiumPhosphate(20)], waterLiters: 100);

        // Act
        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters: 1);

        // Assert
        plan.TankA.IsEmpty.Should().BeFalse();
        plan.TankB.IsEmpty.Should().BeFalse();
        plan.HasPrecipitationRisk.Should().BeFalse();
    }

    // ---------------------------------------------------------------- dilution arithmetic

    /// <summary>
    /// A 100-liter recipe in a 1-liter tank is 1:100, which is 10 ml per liter. These are the two numbers
    /// a person actually acts on, so both are pinned.
    /// </summary>
    [Theory]
    [InlineData(100, 1, 100, 10)]
    [InlineData(1000, 5, 200, 5)]
    [InlineData(50, 2, 25, 40)]
    [Trait("Category", "Unit")]
    public void AsConcentrate_ComputesDilutionRatioAndDoseFromTheTwoVolumes(
        double workingLiters,
        double concentrateLiters,
        double expectedRatio,
        double expectedMillilitresPerLiter)
    {
        // Arrange
        Solution solution = new([CalciumNitrate(100)], workingLiters);

        // Act
        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters);

        // Assert
        plan.DilutionRatio.Should().BeApproximately(expectedRatio, Precision);
        plan.MillilitresPerLiter.Should().BeApproximately(expectedMillilitresPerLiter, Precision);
    }

    /// <summary>
    /// Grams per liter is the figure to check against a salt's solubility, and it is the only number in
    /// the plan that the concentration factor actually moves.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_ReportsEachSaltsStrengthInTheTank()
    {
        // Arrange
        Solution solution = new([CalciumNitrate(100), MagnesiumSulfate(40)], waterLiters: 100);

        // Act
        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters: 2);

        // Assert
        plan.TankA.Components[0].GramsPerLiter.Should().BeApproximately(50, Precision);
        plan.TankB.Components[0].GramsPerLiter.Should().BeApproximately(20, Precision);
        plan.TankA.TotalGramsPerLiter.Should().BeApproximately(50, Precision);
    }

    // ---------------------------------------------------------------- precipitation heuristic

    /// <summary>
    /// The warning that earns the feature its keep: calcium and sulfate arriving in one tank from two
    /// different salts. This is the mistake that produces a cloudy tank and a wasted month of storage,
    /// and it is exactly what a hand-edited or mislabelled catalogue entry causes.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_CalciumAndSulfateFromDifferentSaltsInOneTank_IsFlagged()
    {
        // Arrange — a sulfate mislabelled into tank A, which is how this happens in practice
        Fertilizer mislabelled = new FertilizerBuilder()
            .AddName("Magnesium Sulfate Heptahydrate")
            .AddType(ConcentrateType.A)
            .AddMgNonChelated(9.86).AddS(13.01)
            .AddWeight(40)
            .Build();
        Solution solution = new([CalciumNitrate(100), mislabelled], waterLiters: 100);

        // Act
        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters: 1);

        // Assert
        plan.HasPrecipitationRisk.Should().BeTrue();
        ConcentrateWarning warning = plan.Warnings
            .Single(w => w.Kind == ConcentrateWarningKind.PrecipitationRisk);
        warning.Tank.Should().Be(ConcentrateType.A);
        warning.Fertilizers.Should().BeEquivalentTo(
            ["Calcium Nitrate Tetrahydrate", "Magnesium Sulfate Heptahydrate"]);
    }

    /// <summary>
    /// The false positive the heuristic must not produce. Monocalcium phosphate carries calcium and
    /// phosphate in a single soluble compound; flagging it would be wrong on the chemistry and would
    /// teach users that the warning means nothing.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_SaltCarryingCalciumAndPhosphateInternally_IsNotFlagged()
    {
        // Arrange
        Fertilizer monocalciumPhosphate = new FertilizerBuilder()
            .AddName("Calcium Monobasic Phosphate")
            .AddType(ConcentrateType.B)
            .AddCaNonChelated(15.9).AddP(24.6)
            .AddWeight(30)
            .Build();
        Solution solution = new([monocalciumPhosphate, MonopotassiumPhosphate(20)], waterLiters: 100);

        // Act
        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters: 1);

        // Assert
        plan.HasPrecipitationRisk.Should().BeFalse();
        plan.TankB.Components.Should().HaveCount(2);
    }

    /// <summary>
    /// The same salt still cannot shield a genuine collision: if it sits beside a separate calcium
    /// source and a separate sulfate, the collision is between those two and must still surface.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_InternallyMixedSaltDoesNotSuppressARealCollision()
    {
        // Arrange
        Fertilizer monocalciumPhosphate = new FertilizerBuilder()
            .AddName("Calcium Monobasic Phosphate")
            .AddType(ConcentrateType.A)
            .AddCaNonChelated(15.9).AddP(24.6)
            .AddWeight(30)
            .Build();
        Fertilizer mislabelledSulfate = new FertilizerBuilder()
            .AddName("Potassium Sulfate")
            .AddType(ConcentrateType.A)
            .AddK(44.87).AddS(18.4)
            .AddWeight(20)
            .Build();
        Solution solution = new(
            [CalciumNitrate(100), monocalciumPhosphate, mislabelledSulfate],
            waterLiters: 100);

        // Act
        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters: 1);

        // Assert
        plan.HasPrecipitationRisk.Should().BeTrue();
        plan.Warnings
            .Single(w => w.Kind == ConcentrateWarningKind.PrecipitationRisk)
            .Fertilizers.Should().BeEquivalentTo(
                ["Calcium Nitrate Tetrahydrate", "Potassium Sulfate"]);
    }

    // ---------------------------------------------------------------- inferred tanks

    /// <summary>
    /// A custom salt arrives with <see cref="ConcentrateType.None"/> unless its author says otherwise.
    /// Inferring a tank is useful; inferring it silently is how a user's own sulfate ends up beside
    /// calcium with nothing to warn them.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_FertilizerWithNoTank_IsInferredAndReported()
    {
        // Arrange
        Fertilizer custom = new FertilizerBuilder()
            .AddName("My Own Sulfate")
            .AddK(20).AddS(15)
            .AddWeight(25)
            .Build();
        Solution solution = new([custom], waterLiters: 100);

        // Act
        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters: 1);

        // Assert
        plan.TankB.Components.Should().HaveCount(1);
        ConcentrateWarning warning = plan.Warnings
            .Single(w => w.Kind == ConcentrateWarningKind.TankInferred);
        warning.Tank.Should().Be(ConcentrateType.B);
        warning.Fertilizers.Should().BeEquivalentTo(["My Own Sulfate"]);
    }

    /// <summary>
    /// A salt with neither calcium nor sulfate nor phosphate is chemically welcome in either tank; it
    /// lands in A because that is where the convention puts the bulk of the nitrogen.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_UntaggedSaltWithNeitherGroup_GoesToTankA()
    {
        // Arrange
        Fertilizer urea = new FertilizerBuilder()
            .AddName("Urea")
            .AddNh2(46.6)
            .AddWeight(10)
            .Build();
        Solution solution = new([urea], waterLiters: 100);

        // Act
        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters: 1);

        // Assert
        plan.TankA.Components.Should().HaveCount(1);
        plan.Warnings
            .Single(w => w.Kind == ConcentrateWarningKind.TankInferred)
            .Tank.Should().Be(ConcentrateType.A);
    }

    // ---------------------------------------------------------------- guards

    /// <summary>
    /// A "concentrate" at or above working volume concentrates nothing, and a ratio of 1 or less would
    /// produce a dose of a liter or more per liter. Rejecting it is clearer than returning it.
    /// </summary>
    [Theory]
    [InlineData(100)]
    [InlineData(200)]
    [Trait("Category", "Unit")]
    public void AsConcentrate_VolumeNotBelowWorkingVolume_Throws(double concentrateLiters)
    {
        // Arrange
        Solution solution = new([CalciumNitrate(100)], waterLiters: 100);

        // Act
        Action act = () => solution.AsConcentrate(concentrateLiters);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName(nameof(concentrateLiters));
    }

    /// <summary>
    /// Zero or negative volume is a caller bug, not a degenerate concentrate.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [Trait("Category", "Unit")]
    public void AsConcentrate_NonPositiveVolume_Throws(double concentrateLiters)
    {
        // Arrange
        Solution solution = new([CalciumNitrate(100)], waterLiters: 100);

        // Act
        Action act = () => solution.AsConcentrate(concentrateLiters);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName(nameof(concentrateLiters));
    }

    /// <summary>
    /// A warning about solubility carries the two figures its sentence needs, not only a sentence.
    /// </summary>
    /// <remarks>
    /// The prose stays for logs. An application showing this to somebody deciding whether their
    /// concentrate will dissolve has to write the sentence in their language, and cannot do that from
    /// prose — so the grams per litre needed and the grams per litre that dissolve are on the record.
    /// </remarks>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_WhenASaltCannotDissolve_CarriesTheFigures()
    {
        // Calcium monobasic phosphate dissolves to 18 g/L, the lowest figure in the table, so a
        // modest weight in a modest volume is over it several times.
        Fertilizer phosphate = new FertilizerBuilder()
            .AddName("Calcium Monobasic Phosphate")
            .AddType(ConcentrateType.B)
            .AddP(23.5).AddCaNonChelated(15.9)
            .AddWeight(20)
            .Build();
        Solution solution = new([phosphate], waterLiters: 100);

        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters: 0.5);

        ConcentrateWarning warning = plan.Warnings
            .Should().ContainSingle(w => w.Kind == ConcentrateWarningKind.SolubilityExceeded).Subject;
        warning.Actual.Should().BeApproximately(40, Precision);
        warning.Allowed.Should().Be(18);
    }

    /// <summary>
    /// A saturated tank carries the fraction it is saturated to, and what a full tank would be.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_WhenATankIsSaturated_CarriesTheFraction()
    {
        // Both named as the solubility table names them, so both have a figure and the tank can be
        // judged saturated at all: 111 g/L for the sulfate, 226 for the phosphate.
        Fertilizer sulfate = new FertilizerBuilder()
            .AddName("Potassium Sulfate (SOP)")
            .AddType(ConcentrateType.B)
            .AddK(44.87).AddS(18.4)
            .AddWeight(40)
            .Build();
        Fertilizer phosphate = new FertilizerBuilder()
            .AddName("Potassium Dihydrogen Phosphate (MKP)")
            .AddType(ConcentrateType.B)
            .AddP(22.76).AddK(28.73)
            .AddWeight(40)
            .Build();
        Solution solution = new([sulfate, phosphate], waterLiters: 100);

        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters: 0.5);

        ConcentrateWarning warning = plan.Warnings
            .Should().ContainSingle(w => w.Kind == ConcentrateWarningKind.TankSaturated).Subject;
        warning.Actual.Should().BeGreaterThan(1);
        warning.Allowed.Should().Be(1);
    }

    /// <summary>
    /// Guards the extension against a null receiver, which a caller can reach through an explicit
    /// static invocation.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_NullSolution_Throws()
    {
        // Act
        Action act = () => ConcentrateExtensions.AsConcentrate(null!, 1);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
