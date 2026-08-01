using SYT.NPKTools.Fertilizers;
using Xunit;

namespace SYT.NPKTools.Tests;

public class FertilizerBundleRepositoryTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void Macro_InitializesCorrectly_EnsuresSingleInitialization()
    {
        // Arrange
        FertilizerBundleRepository repository = new FertilizerBundleRepository();

        // Act
        IReadOnlyList<IReadOnlyList<Fertilizer>> firstCallResult = repository.Macro();
        IReadOnlyList<IReadOnlyList<Fertilizer>> secondCallResult = repository.Macro();

        // Assert
        Assert.NotNull(firstCallResult);
        Assert.Equal(firstCallResult, secondCallResult);
        Assert.Same(firstCallResult, secondCallResult);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void InitializeMacro_CreatesExpectedGroups()
    {
        // Arrange
        FertilizerBundleRepository repository = new FertilizerBundleRepository();

        // Act
        IReadOnlyList<IReadOnlyList<Fertilizer>> result = repository.Macro();

        // Assert
        Assert.NotEmpty(result);
        Assert.True(result.Count > 1);
    }
}
