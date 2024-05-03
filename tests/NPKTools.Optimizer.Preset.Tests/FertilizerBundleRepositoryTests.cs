using NPKTools.Core.Domain.Fertilizers;
using Xunit;

namespace NPKTools.Optimizer.Preset.Tests;

public class FertilizerBundleRepositoryTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void Marco_InitializesCorrectly_EnsuresSingleInitialization()
    {
        // Arrange
        FertilizerBundleRepository repository = new FertilizerBundleRepository();

        // Act
        IList<IList<Fertilizer>> firstCallResult = repository.Marco();
        IList<IList<Fertilizer>> secondCallResult = repository.Marco();

        // Assert
        Assert.NotNull(firstCallResult);
        Assert.Equal(firstCallResult, secondCallResult);
        Assert.Same(firstCallResult, secondCallResult);
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public void InitializeMarco_CreatesExpectedGroups()
    {
        // Arrange
        FertilizerBundleRepository repository = new FertilizerBundleRepository();

        // Act
        IList<IList<Fertilizer>> result = repository.Marco();

        // Assert
        Assert.NotEmpty(result);
        Assert.True(result.Count > 1);
    }
}