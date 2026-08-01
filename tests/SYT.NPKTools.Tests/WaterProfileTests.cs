using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers deducting the source water's own nutrients from a target.
/// </summary>
/// <remarks>
/// Skipping this deduction is the most common way to produce a mix that is right on paper and wrong in
/// the tank, so the arithmetic is pinned in both directions: that a blank profile changes nothing, and
/// that an oversupplied element is reported rather than silently truncated.
/// </remarks>
public class WaterProfileTests
{
    private const double Precision = 1e-9;

    private static PpmTarget Target() => new PpmTargetBuilder()
        .AddN(150).AddP(50).AddK(200).AddCa(100).AddMg(50).AddS(60).AddLiters(100)
        .Build();

    // ---------------------------------------------------------------- no water

    /// <summary>
    /// Reverse osmosis and distilled water must leave the target exactly as it was. This is the
    /// regression guard for every existing caller: adding the feature must not move any number for
    /// someone who does not use it.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AdjustFor_PureWater_LeavesTheTargetUnchanged()
    {
        // Arrange
        PpmTarget target = Target();

        // Act
        WaterAdjustedTarget result = target.AdjustFor(WaterProfile.Pure);

        // Assert
        result.Target.N.Value.Should().Be(target.N.Value);
        result.Target.P.Value.Should().Be(target.P.Value);
        result.Target.K.Value.Should().Be(target.K.Value);
        result.Target.Ca.Value.Should().Be(target.Ca.Value);
        result.Target.Mg.Value.Should().Be(target.Mg.Value);
        result.Target.S.Value.Should().Be(target.S.Value);
        result.HasExcesses.Should().BeFalse();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AdjustFor_AnyWater_CarriesTheWaterVolumeThrough()
    {
        // A water analysis is a concentration and says nothing about volume, so the target's own
        // volume must survive untouched.
        WaterProfile water = new WaterProfileBuilder().AddCa(30).Build();

        WaterAdjustedTarget result = Target().AdjustFor(water);

        result.Target.Liters.Value.Should().Be(100);
    }

    // ---------------------------------------------------------------- ordinary deduction

    /// <summary>
    /// A typical municipal analysis: hard-ish water carrying calcium, magnesium, sulfur and sodium.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AdjustFor_TapWater_DeductsEachElement()
    {
        // Arrange
        WaterProfile water = new WaterProfileBuilder()
            .AddCa(40)
            .AddMg(12)
            .AddS(15)
            .AddNa(20)
            .AddNitrate(5)
            .Build();

        // Act
        WaterAdjustedTarget result = Target().AdjustFor(water);

        // Assert
        result.Target.Ca.Value.Should().BeApproximately(60, Precision);
        result.Target.Mg.Value.Should().BeApproximately(38, Precision);
        result.Target.S.Value.Should().BeApproximately(45, Precision);
        result.Target.N.Value.Should().BeApproximately(145, Precision);
        result.HasExcesses.Should().BeFalse();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AdjustFor_UntouchedElements_AreLeftAlone()
    {
        // Arrange
        WaterProfile water = new WaterProfileBuilder().AddCa(40).Build();

        // Act
        WaterAdjustedTarget result = Target().AdjustFor(water);

        // Assert
        result.Target.P.Value.Should().Be(50);
        result.Target.K.Value.Should().Be(200);
    }

    /// <summary>
    /// Nitrogen is deducted as a total, because a target names one nitrogen figure while an analysis may
    /// report it split by form.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AdjustFor_NitrogenInSeveralForms_DeductsTheirSum()
    {
        // Arrange
        WaterProfile water = new WaterProfileBuilder()
            .AddNitrate(8)
            .AddAmmonium(2)
            .Build();

        // Act
        WaterAdjustedTarget result = Target().AdjustFor(water);

        // Assert
        result.Target.N.Value.Should().BeApproximately(140, Precision);
    }

    /// <summary>
    /// Water exactly meeting a target leaves nothing for the fertilizers, and that is not an excess.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AdjustFor_WaterExactlyMeetingATarget_LeavesZeroAndReportsNoExcess()
    {
        // Arrange
        WaterProfile water = new WaterProfileBuilder().AddCa(100).Build();

        // Act
        WaterAdjustedTarget result = Target().AdjustFor(water);

        // Assert
        result.Target.Ca.Value.Should().Be(0);
        result.HasExcesses.Should().BeFalse();
    }

    // ---------------------------------------------------------------- excess

    /// <summary>
    /// Hard water against a low calcium target: fertilizer only adds, so this target cannot be reached
    /// by mixing. It must be reported rather than quietly clamped, because the caller's only remedies
    /// are outside the calculation — raise the target, or dilute the water.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AdjustFor_WaterOversupplyingAnElement_ClampsToZeroAndReportsIt()
    {
        // Arrange
        WaterProfile water = new WaterProfileBuilder().AddCa(140).Build();

        // Act
        WaterAdjustedTarget result = Target().AdjustFor(water);

        // Assert
        result.Target.Ca.Value.Should().Be(0);
        result.HasExcesses.Should().BeTrue();

        NutrientExcess excess = result.Excesses.Should().ContainSingle().Subject;
        excess.Element.Should().Be("Ca");
        excess.InWater.Should().Be(140);
        excess.Target.Should().Be(100);
        excess.Overshoot.Should().BeApproximately(40, Precision);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AdjustFor_SeveralOversuppliedElements_ReportsEachOne()
    {
        // Arrange
        WaterProfile water = new WaterProfileBuilder()
            .AddCa(140)
            .AddMg(80)
            .AddS(90)
            .Build();

        // Act
        WaterAdjustedTarget result = Target().AdjustFor(water);

        // Assert
        result.Excesses.Select(e => e.Element).Should().BeEquivalentTo(["Ca", "Mg", "S"]);
        result.Target.Ca.Value.Should().Be(0);
        result.Target.Mg.Value.Should().Be(0);
        result.Target.S.Value.Should().Be(0);
    }

    /// <summary>
    /// Water carrying an element the target never asked for is normal — sodium and chlorine are in most
    /// supplies — and is not something the caller can act on, so it is not reported as an excess.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AdjustFor_WaterCarryingAnUnrequestedElement_IsNotAnExcess()
    {
        // Arrange: the target names no sodium or chlorine at all.
        WaterProfile water = new WaterProfileBuilder().AddNa(35).AddCl(50).Build();

        // Act
        WaterAdjustedTarget result = Target().AdjustFor(water);

        // Assert
        result.HasExcesses.Should().BeFalse();
        result.Target.Na.Value.Should().Be(0);
        result.Target.Cl.Value.Should().Be(0);
    }

    // ---------------------------------------------------------------- end to end

    /// <summary>
    /// The whole point of the feature: the adjusted target is an ordinary <see cref="PpmTarget"/>, so it
    /// goes straight into the optimizer, and the resulting mix plus the water lands on the original
    /// target rather than overshooting it.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void AdjustedTarget_FedToTheOptimizer_LandsOnTheOriginalTargetOnceWaterIsAddedBack()
    {
        // Arrange
        PpmTarget original = Target();
        WaterProfile water = new WaterProfileBuilder().AddCa(40).AddMg(12).AddS(15).Build();
        WaterAdjustedTarget adjusted = original.AdjustFor(water);

        // Act
        Solutions solutions = NpkTools.CreateOptimizationService().FindMacroSolutions(adjusted.Target);

        // Assert
        solutions.Should().NotBeEmpty();

        Solution mix = solutions[0];
        Ppm fromFertilizer = NpkTools.CreatePpmCalculator().CalculatePpm(mix, mix.WaterLiters);

        (fromFertilizer.Calcium.Value + water.Calcium.Value).Should().BeApproximately(original.Ca.Value, 0.5);
        (fromFertilizer.Magnesium.Value + water.Magnesium.Value).Should().BeApproximately(original.Mg.Value, 0.5);
    }

    // ---------------------------------------------------------------- guards

    [Fact]
    [Trait("Category", "Unit")]
    public void AdjustFor_NullWater_ThrowsArgumentNullException()
    {
        Action act = () => Target().AdjustFor(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AdjustFor_NullTarget_ThrowsArgumentNullException()
    {
        Action act = () => ((PpmTarget)null!).AdjustFor(WaterProfile.Pure);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void WaterProfileBuilder_SettingAnElementTwice_Throws()
    {
        Action act = () => new WaterProfileBuilder().AddCa(10).AddCa(20).Build();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void WaterProfileBuilder_NegativeValue_Throws()
    {
        Action act = () => new WaterProfileBuilder().AddCa(-1).Build();

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void WaterProfilePure_IsAllZero()
    {
        WaterProfile.Pure.Calcium.Value.Should().Be(0);
        WaterProfile.Pure.Nitrogen.Value.Should().Be(0);
        WaterProfile.Pure.Sodium.Value.Should().Be(0);
    }
}
