using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers hardness expressed in German degrees, the unit every drop-test kit prints.
/// </summary>
/// <remarks>
/// One German degree is 17.85 mg/L as CaCO₃, which is 0.3567 meq/L. General hardness counts calcium
/// and magnesium; carbonate hardness counts the bicarbonate, which this library infers from the
/// cation surplus rather than taking as an input.
/// </remarks>
public class HardnessTests
{
    /// <summary>
    /// 100 ppm of calcium is 4.99 meq/L, which is 14.0 °dH. Checked against the definition rather
    /// than against the implementation.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GeneralHardness_ForCalciumOnly_MatchesTheDefinition()
    {
        WaterProfile water = new WaterProfileBuilder().AddCa(100).Build();

        water.GeneralHardness().Should().BeApproximately(13.99, 0.05);
    }

    /// <summary>
    /// Magnesium counts the same, per equivalent rather than per milligram: 24.3 ppm of magnesium is
    /// 2 meq and so twice the hardness of 23.0 ppm of sodium, which is not hardness at all.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GeneralHardness_CountsMagnesiumAndIgnoresSodium()
    {
        WaterProfile hard = new WaterProfileBuilder().AddMg(24.305).Build();
        WaterProfile salty = new WaterProfileBuilder().AddNa(22.990).Build();

        hard.GeneralHardness().Should().BeApproximately(5.61, 0.05);
        salty.GeneralHardness().Should().Be(0);
    }

    /// <summary>
    /// Carbonate hardness follows the inferred alkalinity, so calcium with no matching anion reads
    /// as bicarbonate — which is what calcium bicarbonate water is.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void CarbonateHardness_FollowsTheInferredAlkalinity()
    {
        WaterProfile water = new WaterProfileBuilder().AddCa(100).Build();

        water.CarbonateHardness().Should().BeApproximately(
            water.EstimatedAlkalinity() / WaterProfileExtensions.MilliequivalentsPerGermanDegree,
            1e-9);
        water.CarbonateHardness().Should().BeApproximately(13.99, 0.05);
    }

    /// <summary>
    /// Water whose anions already match its cations has no alkalinity, and so no carbonate hardness,
    /// however hard it is: calcium sulfate water is the textbook case.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void CarbonateHardness_ForCalciumSulfateWater_IsZero()
    {
        WaterProfile water = new WaterProfileBuilder().AddCa(100).AddS(80).Build();

        water.CarbonateHardness().Should().Be(0);
        water.GeneralHardness().Should().BeApproximately(13.99, 0.05);
    }
}
