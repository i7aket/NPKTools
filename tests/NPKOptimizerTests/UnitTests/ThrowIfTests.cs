using NPKOptimizer.Common;
using Xunit;

namespace NPKOptimizer.Tests.UnitTests;

public class ThrowIfTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void Default_WithDefaultValue_ThrowsArgumentException()
    {
        Guid defaultValue = default;

        ArgumentException exception = Assert.Throws<ArgumentException>(() => ThrowIf.Default(defaultValue, nameof(defaultValue)));
        Assert.StartsWith("Value cannot be the default value.", exception.Message);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Default_WithNonDefaultValue_DoesNotThrow()
    {
        Guid nonDefaultValue = Guid.NewGuid();

        Exception exceptionRecord = Record.Exception(() => ThrowIf.Default(nonDefaultValue, nameof(nonDefaultValue)));
        Assert.Null(exceptionRecord);
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public void NotEmpty_WithNullCollection_ThrowsArgumentNullException()
    {
        IEnumerable<int> nullCollection = null;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => ThrowIf.NullOrEmpty(nullCollection, nameof(nullCollection)));
        Assert.StartsWith("The collection cannot be null.", exception.Message);
        Assert.Equal("nullCollection", exception.ParamName);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void NotEmpty_WithEmptyCollection_ThrowsArgumentException()
    {
        List<int> emptyCollection = new List<int>();

        ArgumentException exception = Assert.Throws<ArgumentException>(() => ThrowIf.NullOrEmpty(emptyCollection, nameof(emptyCollection)));
        Assert.StartsWith("The collection cannot be empty.", exception.Message);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void NotEmpty_WithNonEmptyCollection_DoesNotThrow()
    {
        List<int> nonEmptyCollection = new List<int> { 1, 2, 3 };

        Exception exceptionRecord = Record.Exception(() => ThrowIf.NullOrEmpty(nonEmptyCollection, nameof(nonEmptyCollection)));
        Assert.Null(exceptionRecord);
    }
    
}