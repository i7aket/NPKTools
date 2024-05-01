using FluentAssertions;
using NPKTools.Core.Domain.PpmTarget.ValueObjects;
using Xunit;

namespace NPKTools.Core.Tests;

public class PpmTargetValueObjectTests
{
    [Fact]        
    [Trait("Category", "Unit")]
    public void BoronPpmTarget_WithValidValue_ShouldInitializeCorrectly()
    {
        BoronPpmTarget boron = new BoronPpmTarget(10.0);
        boron.Value.Should().Be(10.0);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void CalciumPpmTarget_WithValidValue_ShouldInitializeCorrectly()
    {
        CalciumPpmTarget calcium = new CalciumPpmTarget(20.0);
        calcium.Value.Should().Be(20.0);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ChlorinePpmTarget_WithValidValue_ShouldInitializeCorrectly()
    {
        ChlorinePpmTarget chlorine = new ChlorinePpmTarget(15.0);
        chlorine.Value.Should().Be(15.0);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void CopperPpmTarget_WithValidValue_ShouldInitializeCorrectly()
    {
        CopperPpmTarget copper = new CopperPpmTarget(5.0);
        copper.Value.Should().Be(5.0);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void IronPpmTarget_WithValidValue_ShouldInitializeCorrectly()
    {
        IronPpmTarget iron = new IronPpmTarget(7.0);
        iron.Value.Should().Be(7.0);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void MagnesiumPpmTarget_WithValidValue_ShouldInitializeCorrectly()
    {
        MagnesiumPpmTarget magnesium = new MagnesiumPpmTarget(12.0);
        magnesium.Value.Should().Be(12.0);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ManganesePpmTarget_WithValidValue_ShouldInitializeCorrectly()
    {
        ManganesePpmTarget manganese = new ManganesePpmTarget(3.5);
        manganese.Value.Should().Be(3.5);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void MolybdenumPpmTarget_WithValidValue_ShouldInitializeCorrectly()
    {
        MolybdenumPpmTarget molybdenum = new MolybdenumPpmTarget(1.0);
        molybdenum.Value.Should().Be(1.0);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void NitrogenPpmTarget_WithValidValue_ShouldInitializeCorrectly()
    {
        NitrogenPpmTarget nitrogen = new NitrogenPpmTarget(80.0);
        nitrogen.Value.Should().Be(80.0);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void PhosphorusPpmTarget_WithValidValue_ShouldInitializeCorrectly()
    {
        PhosphorusPpmTarget phosphorus = new PhosphorusPpmTarget(25.0);
        phosphorus.Value.Should().Be(25.0);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void PotassiumPpmTarget_WithValidValue_ShouldInitializeCorrectly()
    {
        PotassiumPpmTarget potassium = new PotassiumPpmTarget(30.0);
        potassium.Value.Should().Be(30.0);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SeleniumPpmTarget_WithValidValue_ShouldInitializeCorrectly()
    {
        SeleniumPpmTarget selenium = new SeleniumPpmTarget(0.5);
        selenium.Value.Should().Be(0.5);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SiliconPpmTarget_WithValidValue_ShouldInitializeCorrectly()
    {
        SiliconPpmTarget silicon = new SiliconPpmTarget(25.0);
        silicon.Value.Should().Be(25.0);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SodiumPpmTarget_WithValidValue_ShouldInitializeCorrectly()
    {
        SodiumPpmTarget sodium = new SodiumPpmTarget(20.0);
        sodium.Value.Should().Be(20.0);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SulfurPpmTarget_WithValidValue_ShouldInitializeCorrectly()
    {
        SulfurPpmTarget sulfur = new SulfurPpmTarget(13.0);
        sulfur.Value.Should().Be(13.0);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void WaterVolumeLitersPpm_WithValidValue_ShouldInitializeCorrectly()
    {
        WaterVolumeLitersPpmTarget waterVolume = new WaterVolumeLitersPpmTarget(100.0);
        waterVolume.Value.Should().Be(100.0);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ZincPpmTarget_WithValidValue_ShouldInitializeCorrectly()
    {
        ZincPpmTarget zinc = new ZincPpmTarget(2.0);
        zinc.Value.Should().Be(2.0);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void BoronPpmTarget_WithNegativeValue_ShouldThrowException()
    {
        Action act = () => new BoronPpmTarget(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void CalciumPpmTarget_WithNegativeValue_ShouldThrowException()
    {
        Action act = () => new CalciumPpmTarget(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ChlorinePpmTarget_WithNegativeValue_ShouldThrowException()
    {
        Action act = () => new ChlorinePpmTarget(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void CopperPpmTarget_WithNegativeValue_ShouldThrowException()
    {
        Action act = () => new CopperPpmTarget(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void IronPpmTarget_WithNegativeValue_ShouldThrowException()
    {
        Action act = () => new IronPpmTarget(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void MagnesiumPpmTarget_WithNegativeValue_ShouldThrowException()
    {
        Action act = () => new MagnesiumPpmTarget(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ManganesePpmTarget_WithNegativeValue_ShouldThrowException()
    {
        Action act = () => new ManganesePpmTarget(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void MolybdenumPpmTarget_WithNegativeValue_ShouldThrowException()
    {
        Action act = () => new MolybdenumPpmTarget(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void NitrogenPpmTarget_WithNegativeValue_ShouldThrowException()
    {
        Action act = () => new NitrogenPpmTarget(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void PhosphorusPpmTarget_WithNegativeValue_ShouldThrowException()
    {
        Action act = () => new PhosphorusPpmTarget(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void PotassiumPpmTarget_WithNegativeValue_ShouldThrowException()
    {
        Action act = () => new PotassiumPpmTarget(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SeleniumPpmTarget_WithNegativeValue_ShouldThrowException()
    {
        Action act = () => new SeleniumPpmTarget(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SiliconPpmTarget_WithNegativeValue_ShouldThrowException()
    {
        Action act = () => new SiliconPpmTarget(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SodiumPpmTarget_WithNegativeValue_ShouldThrowException()
    {
        Action act = () => new SodiumPpmTarget(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SulfurPpmTarget_WithNegativeValue_ShouldThrowException()
    {
        Action act = () => new SulfurPpmTarget(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void WaterVolumeLitersPpm_WithNegativeValue_ShouldThrowException()
    {
        Action act = () => new WaterVolumeLitersPpmTarget(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ZincPpmTarget_WithNegativeValue_ShouldThrowException()
    {
        Action act = () => new ZincPpmTarget(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}

