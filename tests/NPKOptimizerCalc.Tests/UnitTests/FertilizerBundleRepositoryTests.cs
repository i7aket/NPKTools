using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizerCalc.Components;
using Xunit;

namespace NPKOptimizerCalc.Tests.UnitTests;

public class FertilizerBundleRepositoryTests
{
    [Fact]
    public void Marco_InitializesCorrectly_EnsuresSingleInitialization()
    {
        // Arrange
        FertilizerBundleRepository repository = new FertilizerBundleRepository();

        // Act
        IList<IList<FertilizerOptimizationModel>> firstCallResult = repository.Marco();
        IList<IList<FertilizerOptimizationModel>> secondCallResult = repository.Marco();

        // Assert
        Assert.NotNull(firstCallResult);
        Assert.Equal(firstCallResult, secondCallResult);
        Assert.Same(firstCallResult, secondCallResult);
    }
    
    [Fact]
    public void InitializeMarco_CreatesExpectedGroups()
    {
        // Arrange
        FertilizerBundleRepository repository = new FertilizerBundleRepository();

        // Act
        IList<IList<FertilizerOptimizationModel>> result = repository.Marco();

        // Assert
        Assert.NotEmpty(result);
        Assert.True(result.Count > 1);
    }
}