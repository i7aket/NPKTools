using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers the acid dose and, more importantly, the nutrients it brings with it.
/// </summary>
/// <remarks>
/// The dose itself is covered in the library. What is covered here is that the calculator subtracts
/// what the acid contributes — a step every competing calculator treats as unrelated to feeding, and
/// the reason a nitric-acid grower's nitrogen runs a fifth high.
/// </remarks>
public class AcidIntegrationTests
{
    private static CalculatorModel HardWater() => new()
    {
        Mode = WaterInputMode.Conductivity,
        WaterPresetId = WaterPreset.CalciumBicarbonateModerate.Id,
        WaterEc = 0.471,
        AcidEnabled = true,
        AcidId = Acid.Nitric60.Id,
        WaterPh = 7.6,
        TargetPh = 5.8,
    };

    /// <summary>
    /// Water with no alkalinity needs no acid, whatever the setting says.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_OnOsmosisWater_PlansNoAcid()
    {
        CalculatorModel model = new() { AcidEnabled = true, AcidId = Acid.Nitric60.Id };

        model.Recalculate();

        model.Acid.Should().BeNull();
    }

    /// <summary>
    /// On alkaline water the plan appears, with the volume for the reservoir as entered.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_OnAlkalineWater_PlansTheDose()
    {
        CalculatorModel model = HardWater();

        model.Recalculate();

        model.Acid.Should().NotBeNull();
        model.Acid!.MilliequivalentsPerLitre.Should().BeApproximately(2.09, 0.05);
        model.Acid.Millilitres.Should().BeApproximately(16.05, 0.5);
        model.Acid.NutrientPpm.Should().BeApproximately(29.3, 0.5);
    }

    /// <summary>
    /// The nitrogen the acid carries is deducted from the target, so the salts supply the remainder
    /// rather than the whole — and the reported tank contents still add it back, because it really is
    /// in the reservoir. Reporting the deduction without the contribution is the subtle way to get
    /// this wrong, and it would show here as 121 ppm against 150.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_DeductsWhatTheAcidContributes()
    {
        CalculatorModel with = HardWater();
        with.Recalculate();

        CalculatorModel without = HardWater();
        without.AcidEnabled = false;
        without.Recalculate();

        with.Recipes.Should().NotBeEmpty();
        without.Recipes.Should().NotBeEmpty();

        with.Acid!.NutrientPpm.Should().BeGreaterThan(20);
        without.Recipes[0].InTank.Nitrogen.Value.Should().BeApproximately(150, 5);
        with.Recipes[0].InTank.Nitrogen.Value.Should().BeApproximately(150, 5);
    }

    /// <summary>
    /// Turning it off removes the plan and the deduction together.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_WhenDisabled_PlansNothing()
    {
        CalculatorModel model = HardWater();
        model.AcidEnabled = false;

        model.Recalculate();

        model.Acid.Should().BeNull();
    }
}
