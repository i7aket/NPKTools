using NPKTools.Core.Domain.Collections;
using NPKTools.Core.Domain.Fertilizers;
using NPKTools.Core.Domain.Fertilizers.ValueObjects;
using Xunit;

namespace NPKTools.Core.Tests;

public class FertilizerAttributesComparerTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void Equals_WithEqualAttributes_ReturnsTrue()
    {
        // Arrange
        FertilizerPrice price = new FertilizerPrice(100);
        FertilizerNitrogen nitrogen = new FertilizerNitrogen(10, 5, 2);
        FertilizerPhosphorus phosphorus = new FertilizerPhosphorus(5);
        FertilizerPotassium potassium = new FertilizerPotassium(20);
        FertilizerCalcium calcium = new FertilizerCalcium(5, 1);
        FertilizerMagnesium magnesium = new FertilizerMagnesium(3, 2);
        FertilizerSulfur sulfur = new FertilizerSulfur(4);
        FertilizerIron iron = new FertilizerIron(1, 2, 3, 4, 5, 0.5);
        FertilizerCopper copper = new FertilizerCopper(0.5, 0.25);
        FertilizerManganese manganese = new FertilizerManganese(0.7, 0.3);
        FertilizerZinc zinc = new FertilizerZinc(0.8, 0.2);
        FertilizerBoron boron = new FertilizerBoron(0.1);
        FertilizerMolybdenum molybdenum = new FertilizerMolybdenum(0.05);
        FertilizerChlorine chlorine = new FertilizerChlorine(1);
        FertilizerSilicon silicon = new FertilizerSilicon(0.5);
        FertilizerSelenium selenium = new FertilizerSelenium(0.02);
        FertilizerSodium sodium = new FertilizerSodium(0.1);

        FertilizerAttributes attributes1 = new FertilizerAttributes(price, nitrogen, phosphorus, potassium, calcium,
            magnesium,
            sulfur, iron, copper, manganese, zinc, boron, molybdenum, chlorine, silicon, selenium, sodium);
        FertilizerAttributes attributes2 = new FertilizerAttributes(price, nitrogen, phosphorus, potassium, calcium,
            magnesium,
            sulfur, iron, copper, manganese, zinc, boron, molybdenum, chlorine, silicon, selenium, sodium);

        FertilizerAttributesComparer comparer = new FertilizerAttributesComparer();

        // Act
        bool isEqual = comparer.Equals(attributes1, attributes2);

        // Assert
        Assert.True(isEqual);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GetHashCode_WithEqualAttributes_ReturnsSameHashCode()
    {
        // Arrange
        FertilizerPrice price = new FertilizerPrice(100);
        FertilizerNitrogen nitrogen = new FertilizerNitrogen(10, 5, 2);
        FertilizerPhosphorus phosphorus = new FertilizerPhosphorus(5);
        FertilizerPotassium potassium = new FertilizerPotassium(20);
        FertilizerCalcium calcium = new FertilizerCalcium(5, 1);
        FertilizerMagnesium magnesium = new FertilizerMagnesium(3, 2);
        FertilizerSulfur sulfur = new FertilizerSulfur(4);
        FertilizerIron iron = new FertilizerIron(1, 2, 3, 4, 5, 0.5);
        FertilizerCopper copper = new FertilizerCopper(0.5, 0.25);
        FertilizerManganese manganese = new FertilizerManganese(0.7, 0.3);
        FertilizerZinc zinc = new FertilizerZinc(0.8, 0.2);
        FertilizerBoron boron = new FertilizerBoron(0.1);
        FertilizerMolybdenum molybdenum = new FertilizerMolybdenum(0.05);
        FertilizerChlorine chlorine = new FertilizerChlorine(1);
        FertilizerSilicon silicon = new FertilizerSilicon(0.5);
        FertilizerSelenium selenium = new FertilizerSelenium(0.02);
        FertilizerSodium sodium = new FertilizerSodium(0.1);

        FertilizerAttributes attributes1 = new FertilizerAttributes(price, nitrogen, phosphorus, potassium, calcium,
            magnesium,
            sulfur, iron, copper, manganese, zinc, boron, molybdenum, chlorine, silicon, selenium, sodium);
        FertilizerAttributes attributes2 = new FertilizerAttributes(price, nitrogen, phosphorus, potassium, calcium,
            magnesium,
            sulfur, iron, copper, manganese, zinc, boron, molybdenum, chlorine, silicon, selenium, sodium);

        FertilizerAttributesComparer comparer = new FertilizerAttributesComparer();

        // Act
        int hash1 = comparer.GetHashCode(attributes1);
        int hash2 = comparer.GetHashCode(attributes2);

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Equals_WithDifferentAttributes_ReturnsFalse()
    {
        // Arrange
        FertilizerPrice price1 = new FertilizerPrice(100);
        FertilizerPrice price2 = new FertilizerPrice(200);
        FertilizerNitrogen nitrogen = new FertilizerNitrogen(10, 5, 2);
        FertilizerPhosphorus phosphorus = new FertilizerPhosphorus(5);
        FertilizerPotassium potassium = new FertilizerPotassium(10);
        FertilizerCalcium calcium = new FertilizerCalcium(5, 3);
        FertilizerMagnesium magnesium = new FertilizerMagnesium(2, 1);
        FertilizerSulfur sulfur = new FertilizerSulfur(6);
        FertilizerIron iron = new FertilizerIron(1, 0.5, 0.3, 0.2, 0.1, 0.05);
        FertilizerCopper copper = new FertilizerCopper(0.4, 0.2);
        FertilizerManganese manganese = new FertilizerManganese(0.6, 0.3);
        FertilizerZinc zinc = new FertilizerZinc(0.7, 0.35);
        FertilizerBoron boron = new FertilizerBoron(0.2);
        FertilizerMolybdenum molybdenum = new FertilizerMolybdenum(0.03);
        FertilizerChlorine chlorine = new FertilizerChlorine(2);
        FertilizerSilicon silicon = new FertilizerSilicon(1);
        FertilizerSelenium selenium = new FertilizerSelenium(0.01);
        FertilizerSodium sodium = new FertilizerSodium(0.5);

        FertilizerAttributes attributes1 = new FertilizerAttributes(price1, nitrogen, phosphorus, potassium,
            calcium, magnesium,
            sulfur, iron, copper, manganese, zinc, boron, molybdenum, chlorine, silicon, selenium, sodium);
        FertilizerAttributes attributes2 = new FertilizerAttributes(price2, nitrogen, phosphorus, potassium,
            calcium, magnesium,
            sulfur, iron, copper, manganese, zinc, boron, molybdenum, chlorine, silicon, selenium, sodium);

        FertilizerAttributesComparer comparer = new FertilizerAttributesComparer();

        // Act
        bool isEqual = comparer.Equals(attributes1, attributes2);

        // Assert
        Assert.False(isEqual);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Equals_WithNullPropertyInOneAttribute_ReturnsFalse()
    {
        // Arrange
        FertilizerAttributes attributes1 = new FertilizerAttributes
        {
            Price = new FertilizerPrice(100),
            Nitrogen = new FertilizerNitrogen(10, 5, 2),
            Phosphorus = null 
        };

        FertilizerAttributes attributes2 = new FertilizerAttributes
        {
            Price = new FertilizerPrice(100),
            Nitrogen = new FertilizerNitrogen(10, 5, 2),
            Phosphorus = new FertilizerPhosphorus(5) 
        };

        FertilizerAttributesComparer comparer = new FertilizerAttributesComparer();

        // Act
        bool isEqual = comparer.Equals(attributes1, attributes2);

        // Assert
        Assert.False(isEqual);
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public void Equals_WithDifferentNutrientValues_ReturnsFalse()
    {
        // Arrange
        FertilizerAttributes attributes1 = new FertilizerAttributes
        {
            Price = new FertilizerPrice(100),
            Nitrogen = new FertilizerNitrogen(10, 5, 2)
        };

        FertilizerAttributes attributes2 = new FertilizerAttributes
        {
            Price = new FertilizerPrice(100),
            Nitrogen = new FertilizerNitrogen(20, 10, 5) // Different nitrogen content
        };

        FertilizerAttributesComparer comparer = new FertilizerAttributesComparer();

        // Act
        bool isEqual = comparer.Equals(attributes1, attributes2);

        // Assert
        Assert.False(isEqual);
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public void Equals_BothObjectsNull_ReturnsTrue()
    {
        // Arrange
        FertilizerAttributes attributes1 = null;
        FertilizerAttributes attributes2 = null;
        FertilizerAttributesComparer comparer = new FertilizerAttributesComparer();

        // Act
        bool isEqual = comparer.Equals(attributes1, attributes2);

        // Assert
        Assert.True(isEqual);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Equals_OneObjectNull_ReturnsFalse()
    {
        // Arrange
        FertilizerAttributes attributes1 = new FertilizerAttributes
        {
            Price = new FertilizerPrice(100),
            Nitrogen = new FertilizerNitrogen(10, 5, 2),
            Phosphorus = new FertilizerPhosphorus(5)
        };
        FertilizerAttributes attributes2 = null;
        FertilizerAttributesComparer comparer = new FertilizerAttributesComparer();

        // Act
        bool isEqual = comparer.Equals(attributes1, attributes2);

        // Assert
        Assert.False(isEqual);
    }


    
}