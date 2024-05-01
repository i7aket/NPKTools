using FluentAssertions;
using NPKTools.Core.Domain.Fertilizers.ValueObjects;
using Xunit;

namespace NPKTools.Core.Tests;

public class FertilizerValueObjectTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerBoron_ShouldInitializeCorrectly()
    {
        FertilizerBoron boron = new FertilizerBoron(10);
        boron.Value.Should().Be(10);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerCalcium_ShouldInitializeCorrectly()
    {
        FertilizerCalcium calcium = new FertilizerCalcium(5, 3);
        calcium.CaNonChelated.Should().Be(5);
        calcium.CaEdta.Should().Be(3);
        calcium.Value.Should().Be(8);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerChlorine_ShouldInitializeCorrectly()
    {
        FertilizerChlorine chlorine = new FertilizerChlorine(15);
        chlorine.Value.Should().Be(15);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerCopper_ShouldInitializeCorrectly()
    {
        FertilizerCopper copper = new FertilizerCopper(4, 1);
        copper.CuNonChelated.Should().Be(4);
        copper.CuEdta.Should().Be(1);
        copper.Value.Should().Be(5);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerIron_ShouldInitializeCorrectly()
    {
        FertilizerIron iron = new FertilizerIron(2, 3, 1, 0.5, 0.3, 0);
        iron.FeNonChelated.Should().Be(2);
        iron.FeEdta.Should().Be(3);
        iron.FeDtpa.Should().Be(1);
        iron.FeEddha.Should().Be(0.5);
        iron.FeHbed.Should().Be(0.3);
        iron.FeOrthoPart.Should().Be(0);
        iron.Value.Should().Be(6.8);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerMagnesium_ShouldInitializeCorrectly()
    {
        FertilizerMagnesium magnesium = new FertilizerMagnesium(10, 5);
        magnesium.MgNonChelated.Should().Be(10);
        magnesium.MgEdta.Should().Be(5);
        magnesium.Value.Should().Be(15);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerManganese_ShouldInitializeCorrectly()
    {
        FertilizerManganese manganese = new FertilizerManganese(2, 1);
        manganese.MnNonChelated.Should().Be(2);
        manganese.MnEdta.Should().Be(1);
        manganese.Value.Should().Be(3);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerMolybdenum_ShouldInitializeCorrectly()
    {
        FertilizerMolybdenum molybdenum = new FertilizerMolybdenum(0.2);
        molybdenum.Value.Should().Be(0.2);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerNitrogen_ShouldInitializeCorrectly()
    {
        FertilizerNitrogen nitrogen = new FertilizerNitrogen(10, 5, 1);
        nitrogen.Nitrate.Should().Be(10);
        nitrogen.Ammonium.Should().Be(5);
        nitrogen.Amine.Should().Be(1);
        nitrogen.Value.Should().Be(16);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerPhosphorus_ShouldInitializeCorrectly()
    {
        FertilizerPhosphorus phosphorus = new FertilizerPhosphorus(25);
        phosphorus.Value.Should().Be(25);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerPotassium_ShouldInitializeCorrectly()
    {
        FertilizerPotassium potassium = new FertilizerPotassium(30);
        potassium.Value.Should().Be(30);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerPrice_ShouldInitializeCorrectly()
    {
        FertilizerPrice price = new FertilizerPrice(100);
        price.Value.Should().Be(100);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerReferenceId_ShouldInitializeCorrectly()
    {
        Guid guid = Guid.NewGuid();
        FertilizerReferenceId referenceId = new FertilizerReferenceId(guid);
        referenceId.Value.Should().Be(guid);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerSelenium_ShouldInitializeCorrectly()
    {
        FertilizerSelenium selenium = new FertilizerSelenium(0.1);
        selenium.Value.Should().Be(0.1);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerSilicon_ShouldInitializeCorrectly()
    {
        FertilizerSilicon silicon = new FertilizerSilicon(8);
        silicon.Value.Should().Be(8);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerSodium_ShouldInitializeCorrectly()
    {
        FertilizerSodium sodium = new FertilizerSodium(12);
        sodium.Value.Should().Be(12);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerSulfur_ShouldInitializeCorrectly()
    {
        FertilizerSulfur sulfur = new FertilizerSulfur(20);
        sulfur.Value.Should().Be(20);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerWeight_ShouldInitializeCorrectly()
    {
        FertilizerWeight weight = new FertilizerWeight(50);
        weight.Value.Should().Be(50);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerZinc_ShouldInitializeCorrectly()
    {
        FertilizerZinc zinc = new FertilizerZinc(3, 2);
        zinc.ZnNonChelated.Should().Be(3);
        zinc.ZnEdta.Should().Be(2);
        zinc.Value.Should().Be(5);
    }

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
        act.Should().Throw<ArgumentOutOfRangeException>();
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

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-5)]
    [InlineData(-10)]
    public void FertilizerBoron_WithNegativeValue_ShouldThrowException(double value)
    {
        Action act = () => new FertilizerBoron(value);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-5, 0)]
    [InlineData(0, -10)]
    public void FertilizerCalcium_WithNegativeValue_ShouldThrowException(double caNonChelated, double caEdta)
    {
        Action act = () => new FertilizerCalcium(caNonChelated, caEdta);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-15)]
    public void FertilizerChlorine_WithNegativeValue_ShouldThrowException(double value)
    {
        Action act = () => new FertilizerChlorine(value);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-4, 0)]
    [InlineData(0, -1)]
    public void FertilizerCopper_WithNegativeValue_ShouldThrowException(double cuNonChelated, double cuEdta)
    {
        Action act = () => new FertilizerCopper(cuNonChelated, cuEdta);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-2, 0, 0, 0, 0, 0)]
    public void FertilizerIron_WithNegativeValue_ShouldThrowException(double feNonChelated, double feEdta,
        double feDtpa, double feEddha, double feHbed, double feOrthoPart)
    {
        Action act = () => new FertilizerIron(feNonChelated, feEdta, feDtpa, feEddha, feHbed, feOrthoPart);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-10, 0)]
    [InlineData(0, -5)]
    public void FertilizerMagnesium_WithNegativeValue_ShouldThrowException(double mgNonChelated, double mgEdta)
    {
        Action act = () => new FertilizerMagnesium(mgNonChelated, mgEdta);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-2, 0)]
    [InlineData(0, -1)]
    public void FertilizerManganese_WithNegativeValue_ShouldThrowException(double mnNonChelated, double mnEdta)
    {
        Action act = () => new FertilizerManganese(mnNonChelated, mnEdta);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-0.2)]
    public void FertilizerMolybdenum_WithNegativeValue_ShouldThrowException(double value)
    {
        Action act = () => new FertilizerMolybdenum(value);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-10, 0, 0)]
    [InlineData(0, -5, 0)]
    [InlineData(0, 0, -1)]
    public void FertilizerNitrogen_WithNegativeValue_ShouldThrowException(double nitrate, double ammonium,
        double amine)
    {
        Action act = () => new FertilizerNitrogen(nitrate, ammonium, amine);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-25)]
    public void FertilizerPhosphorus_WithNegativeValue_ShouldThrowException(double value)
    {
        Action act = () => new FertilizerPhosphorus(value);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-30)]
    public void FertilizerPotassium_WithNegativeValue_ShouldThrowException(double value)
    {
        Action act = () => new FertilizerPotassium(value);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-100)]
    public void FertilizerPrice_WithNegativeValue_ShouldThrowException(double value)
    {
        Action act = () => new FertilizerPrice(value);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-0.1)]
    public void FertilizerSelenium_WithNegativeValue_ShouldThrowException(double value)
    {
        Action act = () => new FertilizerSelenium(value);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-8)]
    public void FertilizerSilicon_WithNegativeValue_ShouldThrowException(double value)
    {
        Action act = () => new FertilizerSilicon(value);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-12)]
    public void FertilizerSodium_WithNegativeValue_ShouldThrowException(double value)
    {
        Action act = () => new FertilizerSodium(value);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-20)]
    public void FertilizerSulfur_WithNegativeValue_ShouldThrowException(double value)
    {
        Action act = () => new FertilizerSulfur(value);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-3, 0)]
    [InlineData(0, -2)]
    public void FertilizerZinc_WithNegativeValue_ShouldThrowException(double znNonChelated, double znEdta)
    {
        Action act = () => new FertilizerZinc(znNonChelated, znEdta);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerCalcium_ShouldCorrectlyCalculateTotalValue()
    {
        // Arrange
        double caNonChelated = 5;
        double caEdta = 10;
        double expectedValue = 15;

        // Act
        FertilizerCalcium fertilizerCalcium = new FertilizerCalcium(caNonChelated, caEdta);

        // Assert
        fertilizerCalcium.Value.Should().Be(expectedValue);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerCopper_ShouldCorrectlyCalculateTotalValue()
    {
        // Arrange
        double cuNonChelated = 3;
        double cuEdta = 2;
        double expectedValue = 5;

        // Act
        FertilizerCopper fertilizerCopper = new FertilizerCopper(cuNonChelated, cuEdta);

        // Assert
        fertilizerCopper.Value.Should().Be(expectedValue);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerIron_ShouldCorrectlyCalculateTotalValue()
    {
        // Arrange
        double feNonChelated = 1;
        double feEdta = 2;
        double feDtpa = 3;
        double feEddha = 4;
        double feHbed = 5;
        double feOrthoPart = 5;
        double expectedValue = 15;

        // Act
        FertilizerIron fertilizerIron = new FertilizerIron(feNonChelated, feEdta, feDtpa, feEddha, feHbed, feOrthoPart);

        // Assert
        fertilizerIron.Value.Should().Be(expectedValue);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerMagnesium_ShouldCorrectlyCalculateTotalValue()
    {
        // Arrange
        double mgNonChelated = 7;
        double mgEdta = 3;
        double expectedValue = 10;

        // Act
        FertilizerMagnesium fertilizerMagnesium = new FertilizerMagnesium(mgNonChelated, mgEdta);

        // Assert
        fertilizerMagnesium.Value.Should().Be(expectedValue);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerManganese_ShouldCorrectlyCalculateTotalValue()
    {
        // Arrange
        double mnNonChelated = 4;
        double mnEdta = 1;
        double expectedValue = 5;

        // Act
        FertilizerManganese fertilizerManganese = new FertilizerManganese(mnNonChelated, mnEdta);

        // Assert
        fertilizerManganese.Value.Should().Be(expectedValue);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerNitrogen_ShouldCorrectlyCalculateTotalValue()
    {
        // Arrange
        double nitrate = 10;
        double ammonium = 5;
        double amine = 1;
        double expectedValue = 16;

        // Act
        FertilizerNitrogen fertilizerNitrogen = new FertilizerNitrogen(nitrate, ammonium, amine);

        // Assert
        fertilizerNitrogen.Value.Should().Be(expectedValue);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FertilizerZinc_ShouldCorrectlyCalculateTotalValue()
    {
        // Arrange
        double znNonChelated = 2;
        double znEdta = 3;
        double expectedValue = 5;

        // Act
        FertilizerZinc fertilizerZinc = new FertilizerZinc(znNonChelated, znEdta);

        // Assert
        fertilizerZinc.Value.Should().Be(expectedValue);
    }
}