using FluentAssertions;
using NPKOptimizer.Domain.Fertilizers.ValueObjects;
using Xunit;

namespace NPKOptimizer.Tests.UnitTests.ValueObjects;

public class FertilizerManganeseTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void Constructor_WithValidValues_InitializesPropertiesCorrectly()
    {
        // Arrange
        double expectedMnNonChelated = 5.0;
        double expectedMnEdta = 3.0;

        // Act
        FertilizerManganese manganese = new FertilizerManganese(expectedMnNonChelated, expectedMnEdta);

        // Assert
        manganese.MnNonChelated.Should().Be(expectedMnNonChelated);
        manganese.MnEdta.Should().Be(expectedMnEdta);
        manganese.Value.Should().Be(expectedMnNonChelated + expectedMnEdta);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    public void Constructor_WithNegativeValues_ThrowsArgumentException(double mnNonChelated, double mnEdta)
    {
        // Act
        Action act = () => new FertilizerManganese(mnNonChelated, mnEdta);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be lower than 0*");
    }


    [Fact]
    [Trait("Category", "Unit")]
    public void Constructor_WithAllZeroValues_InitializesToZeros()
    {
        // Arrange
        double expectedMnNonChelated = 0.0;
        double expectedMnEdta = 0.0;

        // Act
        FertilizerManganese manganese = new FertilizerManganese(expectedMnNonChelated, expectedMnEdta);

        // Assert
        manganese.MnNonChelated.Should().Be(expectedMnNonChelated);
        manganese.MnEdta.Should().Be(expectedMnEdta);
        manganese.Value.Should().Be(0);
    }
}