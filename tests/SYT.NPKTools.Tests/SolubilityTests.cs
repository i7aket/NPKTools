using AwesomeAssertions;
using SYT.NPKTools.Concentrates;
using SYT.NPKTools.Fertilizers;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers the solubility check on a concentrate.
/// </summary>
/// <remarks>
/// A concentrate is the one place where a recipe can be arithmetically perfect and physically impossible,
/// so these tests pin both halves of the check and, just as importantly, the third state. A salt with no
/// published figure must be reported as unchecked rather than passed as fine — a check that quietly treats
/// "unknown" as "safe" is worse than no check, because it reads as a verdict.
/// </remarks>
public class SolubilityTests
{
    private static Fertilizer Salt(string name, double grams, ConcentrateType tank = ConcentrateType.A) =>
        new FertilizerBuilder()
            .AddName(name)
            .AddType(tank)
            .AddK(30)
            .AddWeight(grams)
            .Build();

    // ---------------------------------------------------------------- the single-salt check

    /// <summary>
    /// The certain half of the check: one salt, one published figure, arithmetic. Monocalcium phosphate is
    /// the case that matters, at 18 g/L against nitrates in the hundreds — it is what actually binds first
    /// in a real concentrate.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_SaltPastItsOwnLimit_IsFlagged()
    {
        // Arrange — 200 g of a salt that dissolves to 18 g/L, into 1 litre
        Fertilizer monocalciumPhosphate = new FertilizerBuilder()
            .AddName("Calcium Monobasic Phosphate")
            .AddType(ConcentrateType.B)
            .AddCaNonChelated(15.9).AddP(24.6)
            .AddWeight(200)
            .Build();
        Solution solution = new([monocalciumPhosphate], waterLiters: 100);

        // Act
        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters: 1);

        // Assert
        plan.ExceedsSolubility.Should().BeTrue();
        plan.TankB.Components[0].SolubilityLimit.Should().Be(18);
        plan.TankB.Components[0].ExceedsSolubility.Should().BeTrue();
        plan.Warnings.Should().Contain(w => w.Kind == ConcentrateWarningKind.SolubilityExceeded);
    }

    /// <summary>
    /// The same salt at a strength it can reach must not be flagged, or the check would block every
    /// concentrate containing it.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_SaltWithinItsLimit_IsNotFlagged()
    {
        // Arrange — 10 g into 1 litre, against a limit of 18
        Fertilizer monocalciumPhosphate = new FertilizerBuilder()
            .AddName("Calcium Monobasic Phosphate")
            .AddType(ConcentrateType.B)
            .AddCaNonChelated(15.9).AddP(24.6)
            .AddWeight(10)
            .Build();
        Solution solution = new([monocalciumPhosphate], waterLiters: 100);

        // Act
        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters: 1);

        // Assert
        plan.ExceedsSolubility.Should().BeFalse();
        plan.TankB.Components[0].SaturationFraction.Should().BeApproximately(10 / 18d, 1e-9);
    }

    // ---------------------------------------------------------------- the mixture screen

    /// <summary>
    /// Salts in one tank compete for the same water, so checking each against its own limit alone is too
    /// permissive: several salts each comfortably under their own limit will still fail to dissolve
    /// together. Adding the fractions is the screen for that, and without it a tank at 600 g/L of very
    /// soluble nitrates passes while being impossible.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_SaltsEachUnderTheirLimitButSaturatedTogether_IsFlagged()
    {
        // Arrange — two salts at roughly 60% of their own limits, so neither trips the single-salt check
        Fertilizer potassiumNitrate = new FertilizerBuilder()
            .AddName("Potassium Nitrate").AddType(ConcentrateType.A)
            .AddNo3(13.85).AddK(38.67).AddWeight(190).Build();          // limit 316
        Fertilizer potassiumSulfate = new FertilizerBuilder()
            .AddName("Potassium Sulfate (SOP)").AddType(ConcentrateType.A)
            .AddK(44.87).AddS(18.4).AddWeight(67).Build();              // limit 111
        Solution solution = new([potassiumNitrate, potassiumSulfate], waterLiters: 100);

        // Act
        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters: 1);

        // Assert
        plan.TankA.Components.Should().AllSatisfy(c => c.ExceedsSolubility.Should().BeFalse());
        plan.TankA.IsSaturated.Should().BeTrue();
        plan.TankA.SaturationFraction.Should().BeGreaterThan(1);
        plan.Warnings.Should().Contain(w => w.Kind == ConcentrateWarningKind.TankSaturated);
        plan.ExceedsSolubility.Should().BeTrue();
    }

    /// <summary>
    /// The saturation figure has to be the sum of the shares, because that is the claim being made. Pinning
    /// the arithmetic keeps a later change from turning it into a maximum or an average, which would look
    /// similar and behave differently.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_TankSaturation_IsTheSumOfEachSaltsShare()
    {
        // Arrange
        Fertilizer potassiumNitrate = new FertilizerBuilder()
            .AddName("Potassium Nitrate").AddType(ConcentrateType.A)
            .AddNo3(13.85).AddK(38.67).AddWeight(31.6).Build();         // 10% of 316
        Fertilizer potassiumSulfate = new FertilizerBuilder()
            .AddName("Potassium Sulfate (SOP)").AddType(ConcentrateType.A)
            .AddK(44.87).AddS(18.4).AddWeight(22.2).Build();            // 20% of 111
        Solution solution = new([potassiumNitrate, potassiumSulfate], waterLiters: 100);

        // Act
        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters: 1);

        // Assert
        plan.TankA.SaturationFraction.Should().BeApproximately(0.30, 1e-6);
        plan.TankA.IsSaturated.Should().BeFalse();
    }

    // ---------------------------------------------------------------- the ceiling

    /// <summary>
    /// When a concentrate is too strong, the actionable answer is how strong it may be. A plan at more than
    /// the ceiling must flag; the same recipe just inside it must not — which is what makes the number
    /// usable rather than decorative.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_MaxDilutionRatio_IsTheStrengthAtWhichTheCheckStopsFiring()
    {
        // Arrange
        Fertilizer potassiumNitrate = new FertilizerBuilder()
            .AddName("Potassium Nitrate").AddType(ConcentrateType.A)
            .AddNo3(13.85).AddK(38.67).AddWeight(100).Build();
        Solution solution = new([potassiumNitrate], waterLiters: 100);

        // Act
        double ceiling = solution.AsConcentrate(concentrateLiters: 1).MaxDilutionRatio!.Value;

        // Assert — 100 g at 316 g/L fits in 0.316 L, so the ceiling is 100 / 0.316
        ceiling.Should().BeApproximately(316, 0.5);

        // just inside the ceiling: fine; just outside: flagged
        solution.AsConcentrate(100 / (ceiling * 0.98)).ExceedsSolubility.Should().BeFalse();
        solution.AsConcentrate(100 / (ceiling * 1.02)).ExceedsSolubility.Should().BeTrue();
    }

    // ---------------------------------------------------------------- unknown is not safe

    /// <summary>
    /// The third state, and the one a naive check gets wrong. A salt with no published figure must come
    /// back as unchecked, not as fine — and no ceiling can be computed either, because the missing salt
    /// could be the one that binds.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_SaltWithNoKnownLimit_IsReportedAsUncheckedRatherThanSafe()
    {
        // Arrange — a made-up name, at a strength nothing could dissolve at
        Solution solution = new([Salt("My Own Mystery Salt", grams: 5000)], waterLiters: 100);

        // Act
        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters: 1);

        // Assert
        plan.UnknownSolubility.Should().BeEquivalentTo(["My Own Mystery Salt"]);
        plan.TankA.Components[0].SolubilityLimit.Should().BeNull();
        plan.TankA.Components[0].SaturationFraction.Should().BeNull();
        plan.TankA.SaturationFraction.Should().BeNull();
        plan.MaxDilutionRatio.Should().BeNull();
        plan.ExceedsSolubility.Should().BeFalse("nothing was checked, so nothing can be claimed");
    }

    /// <summary>
    /// A tank holding one known salt and one unknown one reports no saturation figure at all, rather than a
    /// partial sum. A sum missing a term reads as a verdict on the tank while being one.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_TankWithOneUnknownSalt_ReportsNoSaturationRatherThanAPartialSum()
    {
        // Arrange
        Solution solution = new(
            [Salt("Potassium Nitrate", grams: 100), Salt("My Own Mystery Salt", grams: 100)],
            waterLiters: 100);

        // Act
        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters: 1);

        // Assert
        plan.TankA.Components.Should().HaveCount(2);
        plan.TankA.SaturationFraction.Should().BeNull();
        plan.TankA.IsSaturated.Should().BeFalse();
        plan.UnknownSolubility.Should().BeEquivalentTo(["My Own Mystery Salt"]);
    }

    /// <summary>
    /// A caller's own salt becomes checkable the moment they supply a figure from the bag, which is the
    /// intended way out of the unknown state.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_CustomSaltWithASuppliedFigure_IsCheckedLikeAnyOther()
    {
        // Arrange
        SolubilityTable table = SolubilityTable.Default.With("My Own Mystery Salt", 50);
        Solution solution = new([Salt("My Own Mystery Salt", grams: 100)], waterLiters: 100);

        // Act
        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters: 1, table);

        // Assert — 100 g into 1 L is 100 g/L against a supplied 50
        plan.UnknownSolubility.Should().BeEmpty();
        plan.ExceedsSolubility.Should().BeTrue();
        plan.MaxDilutionRatio.Should().BeApproximately(50, 1e-6);
    }

    /// <summary>
    /// A miscible substance has no limit to exceed, so it must never be flagged and must never cap the
    /// ceiling. Phosphoric acid is the case in the catalogue.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsConcentrate_MiscibleSubstance_NeitherFlagsNorCapsTheCeiling()
    {
        // Arrange
        Fertilizer acid = new FertilizerBuilder()
            .AddName("Phosphoric Acid").AddType(ConcentrateType.B)
            .AddP(31.6).AddWeight(500).Build();
        Solution solution = new([acid], waterLiters: 100);

        // Act
        ConcentratePlan plan = solution.AsConcentrate(concentrateLiters: 1);

        // Assert
        plan.TankB.Components[0].SolubilityLimit.Should().Be(double.PositiveInfinity);
        plan.ExceedsSolubility.Should().BeFalse();
        plan.UnknownSolubility.Should().BeEmpty();
        plan.MaxDilutionRatio.Should().BeNull("nothing in the tank has a finite limit to bind against");
    }

    // ---------------------------------------------------------------- the table itself

    /// <summary>
    /// Every name in the table has to match a catalogue entry exactly, because a mismatch fails silently:
    /// the salt simply reports as unchecked and the check appears to work. This is the one test that would
    /// have caught the five misspelled micro-salt names in the first version of the table.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Default_EveryNameInTheTable_MatchesACatalogueSalt()
    {
        // Arrange
        IFertilizerBundleRepository catalogue = NpkTools.CreateBundleRepository();
        HashSet<string> catalogueNames =
        [
            .. catalogue.Macro().SelectMany(b => b)
                .Concat(catalogue.Micro().SelectMany(b => b))
                .Select(f => f.Name.Value)
        ];

        // Act — every catalogue salt the table claims to know about must actually resolve
        int resolved = catalogueNames.Count(name => SolubilityTable.Default.Limit(name) is not null);

        // Assert
        resolved.Should().Be(SolubilityTable.Default.Count,
            "a name in the table that matches no catalogue salt is a silent no-op");
    }

    /// <summary>
    /// The salts a concentrate is most likely to run into are the barely soluble ones, so their figures are
    /// pinned by value. A typo turning 18 into 180 would let an impossible tank through unnoticed.
    /// </summary>
    [Theory]
    [InlineData("Calcium Monobasic Phosphate", 18)]
    [InlineData("Boric Acid", 49)]
    [InlineData("Sodium Borate Decahydrate", 51)]
    [InlineData("Potassium Sulfate (SOP)", 111)]
    [InlineData("Potassium Dihydrogen Phosphate (MKP)", 226)]
    [Trait("Category", "Unit")]
    public void Default_TheLowSolubilityFigures_ArePinned(string name, double expected)
    {
        // Act & Assert
        SolubilityTable.Default.Limit(name).Should().Be(expected);
    }

    /// <summary>
    /// Names are matched case-insensitively, because a caller retyping a salt name will not match its
    /// capitalisation and a silent miss is the failure mode here.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Limit_MatchesNamesWithoutRegardToCase()
    {
        // Act & Assert
        SolubilityTable.Default.Limit("potassium nitrate").Should().Be(316);
        SolubilityTable.Default.Limit("POTASSIUM NITRATE").Should().Be(316);
        SolubilityTable.Default.Limit("not a salt").Should().BeNull();
    }

    /// <summary>
    /// <c>With</c> must not mutate the table it was called on; <see cref="SolubilityTable.Default"/> is a
    /// shared static and a caller adding their own salt must not change it for everyone else.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void With_LeavesTheOriginalTableUntouched()
    {
        // Arrange
        int before = SolubilityTable.Default.Count;

        // Act
        SolubilityTable extended = SolubilityTable.Default.With("Something Of My Own", 42);

        // Assert
        extended.Limit("Something Of My Own").Should().Be(42);
        SolubilityTable.Default.Limit("Something Of My Own").Should().BeNull();
        SolubilityTable.Default.Count.Should().Be(before);
    }

    /// <summary>
    /// A solubility of zero or less is not a quantity that can dissolve, and NaN would make every
    /// comparison false — silently disabling the check for that salt.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [Trait("Category", "Unit")]
    public void Constructor_ImpossibleSolubility_Throws(double limit)
    {
        // Act
        Action act = () => new SolubilityTable(new Dictionary<string, double> { ["Salt"] = limit });

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// Guards the remaining entry points.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Table_NullOrBlankArguments_Throw()
    {
        // Act & Assert
        ((Action)(() => new SolubilityTable(null!))).Should().Throw<ArgumentNullException>();
        ((Action)(() => SolubilityTable.Default.With(null!, 10))).Should().Throw<ArgumentException>();
        ((Action)(() => SolubilityTable.Default.With("  ", 10))).Should().Throw<ArgumentException>();
        ((Action)(() => SolubilityTable.Default.With("Salt", -1))).Should().Throw<ArgumentOutOfRangeException>();
        SolubilityTable.Empty.Count.Should().Be(0);
    }
}
