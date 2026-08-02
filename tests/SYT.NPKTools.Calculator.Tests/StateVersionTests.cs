using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers carrying the new settings between sessions, and the old ones still arriving.
/// </summary>
/// <remarks>
/// A link someone saved is a promise. Version 1 links carry no water mode, so one has to be inferred,
/// and the only honest inference is what the link's own numbers say: water values mean an analysis,
/// no water values mean reverse osmosis.
/// </remarks>
public class StateVersionTests
{
    private static readonly string[] Catalogue = ["Calcium nitrate", "Potassium nitrate"];

    /// <summary>
    /// Everything entered survives a trip through a file.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Capture_ThenApply_RoundTripsTheWaterAndAcidSettings()
    {
        CalculatorModel written = new()
        {
            Mode = WaterInputMode.ConductivityWithTests,
            WaterPresetId = WaterPreset.SodiumExchangeSoftened.Id,
            WaterEc = 320,
            WaterEcUnit = EcUnit.Ppm500,
            WaterGh = 1.5,
            WaterKh = 9,
            AcidEnabled = true,
            AcidId = Acid.Phosphoric85.Id,
            WaterPh = 7.9,
            TargetPh = 6.1,
        };

        CalculatorModel read = new();
        read.Apply(CalculatorState.FromJson(written.Capture().ToJson())!);

        read.Mode.Should().Be(WaterInputMode.ConductivityWithTests);
        read.WaterPresetId.Should().Be(WaterPreset.SodiumExchangeSoftened.Id);
        read.WaterEc.Should().Be(320);
        read.WaterEcUnit.Should().Be(EcUnit.Ppm500);
        read.WaterGh.Should().Be(1.5);
        read.WaterKh.Should().Be(9);
        read.AcidEnabled.Should().BeTrue();
        read.AcidId.Should().Be(Acid.Phosphoric85.Id);
        read.WaterPh.Should().Be(7.9);
        read.TargetPh.Should().Be(6.1);
    }

    /// <summary>
    /// And through a link.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ToFragment_RoundTripsTheWaterSettings()
    {
        CalculatorModel written = new()
        {
            Mode = WaterInputMode.Conductivity,
            WaterPresetId = WaterPreset.CalciumBicarbonateHard.Id,
            WaterEc = 0.82,
            WaterEcUnit = EcUnit.MilliSiemensPerCm,
        };

        string fragment = written.Capture().ToFragment(Catalogue);
        (CalculatorState? carried, _) = CalculatorState.FromFragment(fragment, Catalogue);
        CalculatorModel read = new();
        read.Apply(carried!);

        read.Mode.Should().Be(WaterInputMode.Conductivity);
        read.WaterPresetId.Should().Be(WaterPreset.CalciumBicarbonateHard.Id);
        read.WaterEc.Should().BeApproximately(0.82, 1e-9);
    }

    /// <summary>
    /// A version 1 link with water values means someone typed an analysis in.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Apply_ForAVersionOneStateWithWater_InfersAnalysisMode()
    {
        CalculatorState state = new()
        {
            Target = "N=150 L=100",
            Water = new Dictionary<string, double> { ["Ca"] = 40 },
        };

        CalculatorModel read = new();
        read.Apply(state);

        read.Mode.Should().Be(WaterInputMode.Analysis);
        read.Water["Ca"].Should().Be(40);
    }

    /// <summary>
    /// A version 1 link with no water values means reverse osmosis, which is what it always meant.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Apply_ForAVersionOneStateWithoutWater_InfersOsmosis()
    {
        CalculatorState state = new() { Target = "N=150 L=100" };

        CalculatorModel read = new();
        read.Apply(state);

        read.Mode.Should().Be(WaterInputMode.Osmosis);
        read.AcidEnabled.Should().BeFalse();
    }

    /// <summary>
    /// An old link still opens: the salts and the target arrive as they always did.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void FromFragment_ReadsAVersionOneFragment()
    {
        (CalculatorState? read, _) = CalculatorState.FromFragment("v=1&t=N%3D150%20L%3D50", Catalogue);

        read.Should().NotBeNull();
        read!.Target.Should().Be("N=150 L=50");
    }

    /// <summary>
    /// An unknown preset in a stale file is dropped rather than accepted, the same way an unknown salt
    /// name is.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Apply_ForAnUnknownPreset_KeepsTheDefault()
    {
        CalculatorState state = new() { WaterPresetId = "SeaWaterFromMars" };

        CalculatorModel read = new();
        read.Apply(state);

        read.WaterPresetId.Should().Be(WaterPreset.CalciumBicarbonateModerate.Id);
    }
}
