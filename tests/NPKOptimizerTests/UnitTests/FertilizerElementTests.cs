using FluentAssertions;
using NPKOptimizer.Domain.Fertilizers.ValueObjects;
using Xunit;

namespace NPKOptimizer.Tests.UnitTests
{
    public class FertilizerElementTests
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
    }
}
