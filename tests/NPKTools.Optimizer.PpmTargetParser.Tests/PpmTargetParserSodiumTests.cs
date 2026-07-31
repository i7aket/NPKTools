using NPKTools.Core.Domain.PpmTarget;
using Xunit;

namespace NPKTools.Optimizer.PpmTargetParser.Tests;

/// <summary>
/// Regression tests for the sodium gap fixed in 2.0.0: "Na" was absent from the parser's
/// accepted-element set even though <see cref="PpmTarget"/> exposes a sodium target, so any
/// input mentioning sodium was rejected and the target silently stayed at zero.
/// </summary>
public class PpmTargetParserSodiumTests
{
    private readonly PpmTargetParser _parser = new();

    [Theory]
    [InlineData("Na=5")]
    [InlineData("na=5")]
    [InlineData("NA=5")]
    [Trait("Category", "Unit")]
    public void Parse_SodiumElement_IsAccepted(string input)
    {
        PpmTarget result = _parser.Parse(input);

        Assert.Equal(5, result.Na.Value);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Parse_SodiumAlongsideOtherElements_ReturnsAllValues()
    {
        PpmTarget result = _parser.Parse("N=150, K=200, Na=12.5, L=100");

        Assert.Equal(150, result.N.Value);
        Assert.Equal(200, result.K.Value);
        Assert.Equal(12.5, result.Na.Value);
        Assert.Equal(100, result.Liters.Value);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Parse_WithoutSodium_LeavesSodiumAtZero()
    {
        PpmTarget result = _parser.Parse("N=150");

        Assert.Equal(0, result.Na.Value);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Parse_DuplicateSodium_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => _parser.Parse("Na=5, Na=7"));
    }
}
