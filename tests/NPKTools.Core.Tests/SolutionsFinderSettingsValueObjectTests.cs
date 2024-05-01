using FluentAssertions;
using NPKTools.Core.Domain.SolutionsFinderSettings.ValueObjects;
using Xunit;

namespace NPKTools.Core.Tests;

public class SolutionsFinderSettingsValueObjectTests
{
    [Theory]    
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(0.1)]
    [InlineData(0.5)]
    [InlineData(1)]
    public void BoronSettings_WithValidValue_ShouldInitializeCorrectly(double validValue)
    {
        BoronSettings settings = new BoronSettings(validValue);
        settings.Value.Should().Be(validValue);
    }

    [Theory]    
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(0.2)]
    [InlineData(0.7)]
    [InlineData(1)]
    public void CalciumSettings_WithValidValue_ShouldInitializeCorrectly(double validValue)
    {
        CalciumSettings settings = new CalciumSettings(validValue);
        settings.Value.Should().Be(validValue);
    }

    [Theory]    
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(0.3)]
    [InlineData(0.8)]
    [InlineData(1)]
    public void ChlorineSettings_WithValidValue_ShouldInitializeCorrectly(double validValue)
    {
        ChlorineSettings settings = new ChlorineSettings(validValue);
        settings.Value.Should().Be(validValue);
    }

    [Theory]    
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(0.25)]
    [InlineData(0.9)]
    [InlineData(1)]
    public void CopperSettings_WithValidValue_ShouldInitializeCorrectly(double validValue)
    {
        CopperSettings settings = new CopperSettings(validValue);
        settings.Value.Should().Be(validValue);
    }

    [Theory]    
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(0.4)]
    [InlineData(0.95)]
    [InlineData(1)]
    public void IronSettings_WithValidValue_ShouldInitializeCorrectly(double validValue)
    {
        IronSettings settings = new IronSettings(validValue);
        settings.Value.Should().Be(validValue);
    }

    [Theory]    
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(0.5)]
    [InlineData(1)]
    public void MagnesiumSettings_WithValidValue_ShouldInitializeCorrectly(double validValue)
    {
        MagnesiumSettings settings = new MagnesiumSettings(validValue);
        settings.Value.Should().Be(validValue);
    }

    [Theory]    
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(0.6)]
    [InlineData(1)]
    public void ManganeseSettings_WithValidValue_ShouldInitializeCorrectly(double validValue)
    {
        ManganeseSettings settings = new ManganeseSettings(validValue);
        settings.Value.Should().Be(validValue);
    }

    [Theory]    
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(0.7)]
    [InlineData(1)]
    public void MolybdenumSettings_WithValidValue_ShouldInitializeCorrectly(double validValue)
    {
        MolybdenumSettings settings = new MolybdenumSettings(validValue);
        settings.Value.Should().Be(validValue);
    }

    [Theory]    
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(0.8)]
    [InlineData(1)]
    public void NitrogenSettings_WithValidValue_ShouldInitializeCorrectly(double validValue)
    {
        NitrogenSettings settings = new NitrogenSettings(validValue);
        settings.Value.Should().Be(validValue);
    }

    [Theory]    
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(0.85)]
    [InlineData(1)]
    public void PhosphorusSettings_WithValidValue_ShouldInitializeCorrectly(double validValue)
    {
        PhosphorusSettings settings = new PhosphorusSettings(validValue);
        settings.Value.Should().Be(validValue);
    }

    [Theory]    
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(0.9)]
    [InlineData(1)]
    public void PotassiumSettings_WithValidValue_ShouldInitializeCorrectly(double validValue)
    {
        PotassiumSettings settings = new PotassiumSettings(validValue);
        settings.Value.Should().Be(validValue);
    }

    [Theory]    
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(0.95)]
    [InlineData(1)]
    public void SeleniumSettings_WithValidValue_ShouldInitializeCorrectly(double validValue)
    {
        SeleniumSettings settings = new SeleniumSettings(validValue);
        settings.Value.Should().Be(validValue);
    }

    [Theory]    
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(0.96)]
    [InlineData(1)]
    public void SiliconSettings_WithValidValue_ShouldInitializeCorrectly(double validValue)
    {
        SiliconSettings settings = new SiliconSettings(validValue);
        settings.Value.Should().Be(validValue);
    }

    [Theory]    
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(0.97)]
    [InlineData(1)]
    public void SodiumSettings_WithValidValue_ShouldInitializeCorrectly(double validValue)
    {
        SodiumSettings settings = new SodiumSettings(validValue);
        settings.Value.Should().Be(validValue);
    }

    [Theory]    
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(0.98)]
    [InlineData(1)]
    public void SulfurSettings_WithValidValue_ShouldInitializeCorrectly(double validValue)
    {
        SulfurSettings settings = new SulfurSettings(validValue);
        settings.Value.Should().Be(validValue);
    }

    [Theory]    
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(0.99)]
    [InlineData(1)]
    public void ZincSettings_WithValidValue_ShouldInitializeCorrectly(double validValue)
    {
        ZincSettings settings = new ZincSettings(validValue);
        settings.Value.Should().Be(validValue);
    }
    
    [Theory]    
    [Trait("Category", "Unit")]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void BoronSettings_WithInvalidValue_ShouldThrowException(double invalidValue)
    {
        Action act = () => new BoronSettings(invalidValue);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void CalciumSettings_WithInvalidValue_ShouldThrowException(double invalidValue)
    {
        Action act = () => new CalciumSettings(invalidValue);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void ChlorineSettings_WithInvalidValue_ShouldThrowException(double invalidValue)
    {
        Action act = () => new ChlorineSettings(invalidValue);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void CopperSettings_WithInvalidValue_ShouldThrowException(double invalidValue)
    {
        Action act = () => new CopperSettings(invalidValue);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void IronSettings_WithInvalidValue_ShouldThrowException(double invalidValue)
    {
        Action act = () => new IronSettings(invalidValue);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void MagnesiumSettings_WithInvalidValue_ShouldThrowException(double invalidValue)
    {
        Action act = () => new MagnesiumSettings(invalidValue);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void ManganeseSettings_WithInvalidValue_ShouldThrowException(double invalidValue)
    {
        Action act = () => new ManganeseSettings(invalidValue);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void MolybdenumSettings_WithInvalidValue_ShouldThrowException(double invalidValue)
    {
        Action act = () => new MolybdenumSettings(invalidValue);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void NitrogenSettings_WithInvalidValue_ShouldThrowException(double invalidValue)
    {
        Action act = () => new NitrogenSettings(invalidValue);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void PhosphorusSettings_WithInvalidValue_ShouldThrowException(double invalidValue)
    {
        Action act = () => new PhosphorusSettings(invalidValue);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void PotassiumSettings_WithInvalidValue_ShouldThrowException(double invalidValue)
    {
        Action act = () => new PotassiumSettings(invalidValue);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void SeleniumSettings_WithInvalidValue_ShouldThrowException(double invalidValue)
    {
        Action act = () => new SeleniumSettings(invalidValue);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void SiliconSettings_WithInvalidValue_ShouldThrowException(double invalidValue)
    {
        Action act = () => new SiliconSettings(invalidValue);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void SodiumSettings_WithInvalidValue_ShouldThrowException(double invalidValue)
    {
        Action act = () => new SodiumSettings(invalidValue);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void SulfurSettings_WithInvalidValue_ShouldThrowException(double invalidValue)
    {
        Action act = () => new SulfurSettings(invalidValue);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void ZincSettings_WithInvalidValue_ShouldThrowException(double invalidValue)
    {
        Action act = () => new ZincSettings(invalidValue);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(0.1)]
    [InlineData(0.5)]
    [InlineData(1)]
    public void RangeFactorSettings_WithValidValue_ShouldInitializeCorrectly(double validValue)
    {
        RangeFactorSettings settings = new RangeFactorSettings(validValue);
        settings.Value.Should().Be(validValue);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-1)]
    [InlineData(-0.5)]
    [InlineData(0)]
    public void RangeFactorSettings_WithInvalidValue_ShouldThrowException(double invalidValue)
    {
        Action act = () => new RangeFactorSettings(invalidValue);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(1.1)]
    [InlineData(1.5)]
    public void RangeFactorSettings_WithValueGreaterThanOne_ShouldThrowException(double invalidValue)
    {
        Action act = () => new RangeFactorSettings(invalidValue);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}