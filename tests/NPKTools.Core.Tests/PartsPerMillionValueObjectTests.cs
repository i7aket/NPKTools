using FluentAssertions;
using NPKTools.Core.Domain.PartsPerMillion.ValueObjects;
using Xunit;

namespace NPKTools.Core.Tests;

public class PartsPerMillionValueObjectTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void BoronPpm_ShouldInitialize_WithNonNegativeValue()
    {
        double expectedValue = 10.5;

        BoronPpm boron = new BoronPpm(expectedValue);

        Assert.Equal(expectedValue, boron.Value);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void CalciumPpm_ShouldInitialize_WithNonNegativeValue()
    {
        double expectedValue = 20.0;

        CalciumPpm calcium = new CalciumPpm(expectedValue);

        Assert.Equal(expectedValue, calcium.Value);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ChlorinePpm_ShouldInitialize_WithNonNegativeValue()
    {
        double expectedValue = 15.0;

        ChlorinePpm chlorine = new ChlorinePpm(expectedValue);

        Assert.Equal(expectedValue, chlorine.Value);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void CopperPpm_ShouldInitialize_WithNonNegativeValue()
    {
        double expectedValue = 5.0;

        CopperPpm copper = new CopperPpm(expectedValue);

        Assert.Equal(expectedValue, copper.Value);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void IronPpm_ShouldInitialize_WithNonNegativeValue()
    {
        double expectedValue = 7.0;

        IronPpm iron = new IronPpm(expectedValue);

        Assert.Equal(expectedValue, iron.Value);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void MagnesiumPpm_ShouldInitialize_WithNonNegativeValue()
    {
        double expectedValue = 12.0;

        MagnesiumPpm magnesium = new MagnesiumPpm(expectedValue);

        Assert.Equal(expectedValue, magnesium.Value);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ManganesePpm_ShouldInitialize_WithNonNegativeValue()
    {
        double expectedValue = 3.5;

        ManganesePpm manganese = new ManganesePpm(expectedValue);

        Assert.Equal(expectedValue, manganese.Value);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void MolybdenumPpm_ShouldInitialize_WithNonNegativeValue()
    {
        double expectedValue = 1.0;

        MolybdenumPpm molybdenum = new MolybdenumPpm(expectedValue);

        Assert.Equal(expectedValue, molybdenum.Value);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void PhosphorusPpm_ShouldInitialize_WithNonNegativeValue()
    {
        double expectedValue = 8.0;

        PhosphorusPpm phosphorus = new PhosphorusPpm(expectedValue);

        Assert.Equal(expectedValue, phosphorus.Value);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void PotassiumPpm_ShouldInitialize_WithNonNegativeValue()
    {
        double expectedValue = 30.0;

        PotassiumPpm potassium = new PotassiumPpm(expectedValue);

        Assert.Equal(expectedValue, potassium.Value);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SeleniumPpm_ShouldInitialize_WithNonNegativeValue()
    {
        double expectedValue = 0.5;

        SeleniumPpm selenium = new SeleniumPpm(expectedValue);

        Assert.Equal(expectedValue, selenium.Value);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SiliconPpm_ShouldInitialize_WithNonNegativeValue()
    {
        double expectedValue = 25.0;

        SiliconPpm silicon = new SiliconPpm(expectedValue);

        Assert.Equal(expectedValue, silicon.Value);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SodiumPpm_ShouldInitialize_WithNonNegativeValue()
    {
        double expectedValue = 20.0;

        SodiumPpm sodium = new SodiumPpm(expectedValue);

        Assert.Equal(expectedValue, sodium.Value);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SulfurPpm_ShouldInitialize_WithNonNegativeValue()
    {
        double expectedValue = 13.0;

        SulfurPpm sulfur = new SulfurPpm(expectedValue);

        Assert.Equal(expectedValue, sulfur.Value);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ZincPpm_ShouldInitialize_WithNonNegativeValue()
    {
        double expectedValue = 2.0;

        ZincPpm zinc = new ZincPpm(expectedValue);

        Assert.Equal(expectedValue, zinc.Value);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void BoronPpm_ShouldThrowException_WhenInitializedWithNegativeValue()
    {
        double invalidValue = -5.0;
        Assert.Throws<ArgumentOutOfRangeException>(() => new BoronPpm(invalidValue));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void CalciumPpm_ShouldThrowException_WhenInitializedWithNegativeValue()
    {
        double invalidValue = -10.0;
        Assert.Throws<ArgumentOutOfRangeException>(() => new CalciumPpm(invalidValue));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ChlorinePpm_ShouldThrowException_WhenInitializedWithNegativeValue()
    {
        double invalidValue = -15.0;
        Assert.Throws<ArgumentOutOfRangeException>(() => new ChlorinePpm(invalidValue));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void CopperPpm_ShouldThrowException_WhenInitializedWithNegativeValue()
    {
        double invalidValue = -5.0;
        Assert.Throws<ArgumentOutOfRangeException>(() => new CopperPpm(invalidValue));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void IronPpm_ShouldThrowException_WhenInitializedWithNegativeValue()
    {
        double invalidValue = -7.0;
        Assert.Throws<ArgumentOutOfRangeException>(() => new IronPpm(invalidValue));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void MagnesiumPpm_ShouldThrowException_WhenInitializedWithNegativeValue()
    {
        double invalidValue = -12.0;
        Assert.Throws<ArgumentOutOfRangeException>(() => new MagnesiumPpm(invalidValue));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ManganesePpm_ShouldThrowException_WhenInitializedWithNegativeValue()
    {
        double invalidValue = -3.5;
        Assert.Throws<ArgumentOutOfRangeException>(() => new ManganesePpm(invalidValue));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void MolybdenumPpm_ShouldThrowException_WhenInitializedWithNegativeValue()
    {
        double invalidValue = -1.0;
        Assert.Throws<ArgumentOutOfRangeException>(() => new MolybdenumPpm(invalidValue));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void NitrogenPpm_ShouldThrowException_WhenInitializedWithNegativeValue()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new NitrogenPpm(-10.0, 0, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new NitrogenPpm(0, -10.0, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new NitrogenPpm(0, 0, -10.0));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void PhosphorusPpm_ShouldThrowException_WhenInitializedWithNegativeValue()
    {
        double invalidValue = -8.0;
        Assert.Throws<ArgumentOutOfRangeException>(() => new PhosphorusPpm(invalidValue));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void PotassiumPpm_ShouldThrowException_WhenInitializedWithNegativeValue()
    {
        double invalidValue = -30.0;
        Assert.Throws<ArgumentOutOfRangeException>(() => new PotassiumPpm(invalidValue));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SeleniumPpm_ShouldThrowException_WhenInitializedWithNegativeValue()
    {
        double invalidValue = -0.5;
        Assert.Throws<ArgumentOutOfRangeException>(() => new SeleniumPpm(invalidValue));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SiliconPpm_ShouldThrowException_WhenInitializedWithNegativeValue()
    {
        double invalidValue = -25.0;
        Assert.Throws<ArgumentOutOfRangeException>(() => new SiliconPpm(invalidValue));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SodiumPpm_ShouldThrowException_WhenInitializedWithNegativeValue()
    {
        double invalidValue = -20.0;
        Assert.Throws<ArgumentOutOfRangeException>(() => new SodiumPpm(invalidValue));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SulfurPpm_ShouldThrowException_WhenInitializedWithNegativeValue()
    {
        double invalidValue = -13.0;
        Assert.Throws<ArgumentOutOfRangeException>(() => new SulfurPpm(invalidValue));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ZincPpm_ShouldThrowException_WhenInitializedWithNegativeValue()
    {
        double invalidValue = -2.0;
        Assert.Throws<ArgumentOutOfRangeException>(() => new ZincPpm(invalidValue));
    }    
    
    [Fact]
    [Trait("Category", "Unit")]
    public void NitrogenPpm_ShouldCorrectlyCalculateTotalValue()
    {
        // Arrange
        double nitrate = 10;
        double ammonium = 5;
        double amine = 2;
        double expectedTotal = nitrate + ammonium + amine;

        // Act
        NitrogenPpm nitrogen = new NitrogenPpm(nitrate, ammonium, amine);

        // Assert
        nitrogen.Value.Should().Be(expectedTotal);
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public void WaterVolumeLitersPpm_ShouldInitialize_WithPositiveValue()
    {
        // Arrange
        double expectedValue = 100.0;

        // Act
        WaterVolumeLitersPpm waterVolume = new WaterVolumeLitersPpm(expectedValue);

        // Assert
        waterVolume.Value.Should().Be(expectedValue);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void WaterVolumeLitersPpm_ShouldThrowException_WhenInitializedWithNegativeValue()
    {
        // Arrange
        double invalidValue = -100.0;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new WaterVolumeLitersPpm(invalidValue));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void WaterVolumeLitersPpm_ShouldThrowException_WhenInitializedWithZero()
    {
        // Arrange
        double invalidValue = 0;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new WaterVolumeLitersPpm(invalidValue));
    }
}