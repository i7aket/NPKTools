using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers the four ways a grower can describe their water.
/// </summary>
public class WaterModeTests
{
    /// <summary>
    /// Reverse osmosis is the default, and it deducts nothing.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_InOsmosisMode_LeavesTheTargetAlone()
    {
        CalculatorModel model = new();

        model.Recalculate();

        model.Mode.Should().Be(WaterInputMode.Osmosis);
        model.WaterProfile.Calcium.Value.Should().Be(0);
        model.Alkalinity.Should().Be(0);
    }

    /// <summary>
    /// A meter reading produces an analysis whose conductivity matches it.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_InConductivityMode_MatchesTheReading()
    {
        CalculatorModel model = new()
        {
            Mode = WaterInputMode.Conductivity,
            WaterPresetId = WaterPreset.CalciumBicarbonateModerate.Id,
            WaterEc = 0.45,
            WaterEcUnit = EcUnit.MilliSiemensPerCm,
        };

        model.Recalculate();

        model.WaterEstimate.Should().NotBeNull();
        model.WaterEstimate!.Feasible.Should().BeTrue();
        model.WaterProfile.EstimateConductivity().MicroSiemensPerCm
            .Should().BeApproximately(450, 5);
        model.WaterProfile.Calcium.Value.Should().BeGreaterThan(20);
    }

    /// <summary>
    /// A ppm meter says the same thing on a different scale. 0.45 mS/cm is 225 on the 500 scale and
    /// 315 on the 700 scale, and all three must land on the same water.
    /// </summary>
    /// <param name="unit">The scale the reading is in.</param>
    /// <param name="reading">The reading on that scale.</param>
    [Theory]
    [InlineData(EcUnit.MilliSiemensPerCm, 0.45)]
    [InlineData(EcUnit.Ppm500, 225)]
    [InlineData(EcUnit.Ppm700, 315)]
    [Trait("Category", "Unit")]
    public void Recalculate_AcrossEcUnits_DescribesTheSameWater(EcUnit unit, double reading)
    {
        CalculatorModel model = new()
        {
            Mode = WaterInputMode.Conductivity,
            WaterPresetId = WaterPreset.CalciumBicarbonateModerate.Id,
            WaterEc = reading,
            WaterEcUnit = unit,
        };

        model.Recalculate();

        model.WaterProfile.EstimateConductivity().MicroSiemensPerCm
            .Should().BeApproximately(450, 5);
    }

    /// <summary>
    /// Drop tests are honoured exactly, and the mode is what decides whether they are read at all —
    /// values left over from a previous mode must not leak into this one.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_InConductivityMode_IgnoresHardnessEnteredForTheOtherMode()
    {
        CalculatorModel model = new()
        {
            Mode = WaterInputMode.Conductivity,
            WaterPresetId = WaterPreset.CalciumBicarbonateModerate.Id,
            WaterEc = 0.45,
            WaterGh = 2,
            WaterKh = 1,
        };

        model.Recalculate();

        model.WaterProfile.GeneralHardness().Should().BeGreaterThan(5);
    }

    /// <summary>
    /// With the tests read, both come back out of the estimate unchanged.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_WithTests_HonoursThem()
    {
        CalculatorModel model = new()
        {
            Mode = WaterInputMode.ConductivityWithTests,
            WaterPresetId = WaterPreset.CalciumBicarbonateModerate.Id,
            WaterEc = 0.45,
            WaterGh = 9,
            WaterKh = 7,
        };

        model.Recalculate();

        model.WaterProfile.GeneralHardness().Should().BeApproximately(9, 0.1);
        model.WaterProfile.CarbonateHardness().Should().BeApproximately(7, 0.1);
    }

    /// <summary>
    /// A typed-in analysis is used as typed, with no estimate involved.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Recalculate_InAnalysisMode_UsesTheTypedFigures()
    {
        CalculatorModel model = new() { Mode = WaterInputMode.Analysis };
        model.Water["Ca"] = 42;

        model.Recalculate();

        model.WaterProfile.Calcium.Value.Should().Be(42);
        model.WaterEstimate.Should().BeNull();
    }
}
