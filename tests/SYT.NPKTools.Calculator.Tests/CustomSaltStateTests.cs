using AwesomeAssertions;
using SYT.NPKTools.Fertilizers;
using Xunit;

namespace SYT.NPKTools.Calculator.Tests;

/// <summary>
/// Covers custom salts surviving a file, a link, and an older link that has none.
/// </summary>
public class CustomSaltStateTests
{
    private static readonly string[] Catalogue = ["Calcium nitrate", "Potassium nitrate"];

    /// <summary>
    /// A formula-defined salt round-trips through a file.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Capture_ThenApply_RoundTripsAFormulaSalt()
    {
        CalculatorModel written = new();
        written.TryAddCustomSalt(
            new CustomSalt { Name = "Shop KNO3", Formula = "KNO3", Tank = ConcentrateType.A },
            out _).Should().BeTrue();

        CalculatorModel read = new();
        read.Apply(CalculatorState.FromJson(written.Capture().ToJson())!);

        read.CustomSalts.Should().ContainSingle();
        read.CustomSalts[0].Formula.Should().Be("KNO3");
        read.Catalogue.Should().Contain(f => f.Name.Value == "Shop KNO3");
    }

    /// <summary>
    /// A percentage-defined salt keeps its forms through a file.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Capture_ThenApply_RoundTripsAPercentageSalt()
    {
        CalculatorModel written = new();
        written.TryAddCustomSalt(
            new CustomSalt
            {
                Name = "Fe chelate",
                Tank = ConcentrateType.B,
                Percentages = { ["FeEdta"] = 13 },
            },
            out _).Should().BeTrue();

        CalculatorModel read = new();
        read.Apply(CalculatorState.FromJson(written.Capture().ToJson())!);

        read.CustomSalts.Should().ContainSingle();
        read.CustomSalts[0].Percentages.Should().ContainKey("FeEdta").WhoseValue.Should().Be(13);
    }

    /// <summary>
    /// And through a link.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ToFragment_CarriesCustomSalts()
    {
        CalculatorModel written = new();
        written.TryAddCustomSalt(
            new CustomSalt { Name = "Shop KNO3", Formula = "KNO3", Tank = ConcentrateType.A },
            out _).Should().BeTrue();

        string fragment = written.Capture().ToFragment(Catalogue);
        (CalculatorState? carried, _) = CalculatorState.FromFragment(fragment, Catalogue);

        CalculatorModel read = new();
        read.Apply(carried!);

        read.CustomSalts.Should().ContainSingle();
        read.CustomSalts[0].Name.Should().Be("Shop KNO3");
    }

    /// <summary>
    /// Several salts in one link all arrive. The fragment repeats the key rather than packing them,
    /// and a reader that kept only the last value would silently drop the rest.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void ToFragment_CarriesEveryCustomSalt()
    {
        CalculatorModel written = new();
        written.TryAddCustomSalt(new CustomSalt { Name = "One", Formula = "KNO3" }, out _).Should().BeTrue();
        written.TryAddCustomSalt(new CustomSalt { Name = "Two", Formula = "KCl" }, out _).Should().BeTrue();
        written.TryAddCustomSalt(
            new CustomSalt { Name = "Three", Percentages = { ["FeEdta"] = 6 } }, out _).Should().BeTrue();

        string fragment = written.Capture().ToFragment(Catalogue);
        (CalculatorState? carried, _) = CalculatorState.FromFragment(fragment, Catalogue);

        CalculatorModel read = new();
        read.Apply(carried!);

        read.CustomSalts.Select(s => s.Name).Should().Equal("One", "Two", "Three");
    }

    /// <summary>
    /// A link written before custom salts existed still opens, and brings none.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void FromFragment_ForALinkWithoutCustomSalts_StillOpens()
    {
        (CalculatorState? read, _) = CalculatorState.FromFragment("v=1&t=N%3D150%20L%3D50", Catalogue);

        read.Should().NotBeNull();
        read!.CustomSalts.Should().BeEmpty();
    }

    /// <summary>
    /// A stale file naming a salt that no longer materialises is dropped rather than crashing the
    /// load, the same way an unknown salt name already is.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Apply_ForAnUnusableCustomSalt_DropsIt()
    {
        CalculatorState state = new()
        {
            CustomSalts = [new CustomSalt { Name = "Broken", Formula = "Zz9" }],
        };

        CalculatorModel read = new();
        read.Apply(state);

        read.CustomSalts.Should().BeEmpty();
    }

    /// <summary>
    /// Applying twice does not stack duplicates, which the unique-name rule would otherwise refuse.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Apply_Twice_DoesNotAccumulate()
    {
        CalculatorModel written = new();
        written.TryAddCustomSalt(new CustomSalt { Name = "Shop KNO3", Formula = "KNO3" }, out _)
            .Should().BeTrue();
        CalculatorState state = written.Capture();

        CalculatorModel read = new();
        read.Apply(state);
        read.Apply(state);

        read.CustomSalts.Should().ContainSingle();
    }
}
