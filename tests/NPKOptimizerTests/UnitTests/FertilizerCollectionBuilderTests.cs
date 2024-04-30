using NPKOptimizer.Components;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.Fertilizers.Builders;
using Xunit;

namespace NPKOptimizer.Tests.UnitTests;

public class FertilizerCollectionBuilderTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void Add_SingleFertilizer_AddsFertilizerCorrectly()
    {
        // Arrange
        FertilizerCollectionBuilder builder = new FertilizerCollectionBuilder();
        Fertilizer fertilizer = new FertilizerBuilder().AddK(38.672).AddNo3(13.854).Build();

        // Act
        builder.Add(fertilizer);
        IList<FertilizerOptimizationModel> result = builder.Build();

        // Assert
        Assert.Single(result);
        Assert.Equal(fertilizer, result.First());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Add_DuplicateFertilizer_ThrowsInvalidOperationException()
    {
        // Arrange
        FertilizerCollectionBuilder builder = new FertilizerCollectionBuilder();
        Fertilizer fertilizer = new FertilizerBuilder().AddK(38.672).AddNo3(13.854).Build();
        builder.Add(fertilizer);  

        // Act 
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => builder.Add(fertilizer));
    
        //Assert
        Assert.Equal("Duplicate fertilizer detected with identical attributes.", ex.Message);
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public void Build_MultipleFertilizers_BuildsCorrectCollection()
    {
        // Arrange
        FertilizerCollectionBuilder builder = new FertilizerCollectionBuilder();
        Fertilizer fert1 = new FertilizerBuilder().AddK(38.672).AddNo3(13.854).Build();
        Fertilizer fert2 = new FertilizerBuilder().AddCaNonChelated(16.972).AddNo3(11.863).Build();

        // Act
        builder.Add(fert1).Add(fert2);
        IList<FertilizerOptimizationModel> result = builder.Build();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(fert1, result);
        Assert.Contains(fert2, result);
    }
}