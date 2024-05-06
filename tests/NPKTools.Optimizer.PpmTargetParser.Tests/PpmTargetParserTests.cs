using NPKTools.Core.Domain.PpmTarget;
using Xunit;

namespace NPKTools.Optimizer.PpmTargetParser.Tests;

public class PpmTargetParserTests
{
    private readonly PpmTargetParser _parser;

    public PpmTargetParserTests()
    {
        _parser = new PpmTargetParser();
    }
    [Fact]
    [Trait("Category", "Unit")]
    public void Parse_LowercaseElements_ReturnsCorrectPpmTarget()
    {
        // Arrange
        string input = "n=150, p=100, k=250, ca=400, mg=50, s=30, fe=10, cu=5, mn=7, zn=8, b=3, mo=2, cl=45, si=20, se=1, l=1000";

        // Act
        PpmTarget result = _parser.Parse(input);

        // Assert
        Assert.Equal(150, result.N.Value);
        Assert.Equal(100, result.P.Value);
        Assert.Equal(250, result.K.Value);
        Assert.Equal(400, result.Ca.Value);
        Assert.Equal(50, result.Mg.Value);
        Assert.Equal(30, result.S.Value);
        Assert.Equal(10, result.Fe.Value);
        Assert.Equal(5, result.Cu.Value);
        Assert.Equal(7, result.Mn.Value);
        Assert.Equal(8, result.Zn.Value);
        Assert.Equal(3, result.B.Value);
        Assert.Equal(2, result.Mo.Value);
        Assert.Equal(45, result.Cl.Value);
        Assert.Equal(20, result.Si.Value);
        Assert.Equal(1, result.Se.Value);
        Assert.Equal(1000, result.Liters.Value);
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public void Parse_AllValidElements_ReturnsCorrectPpmTarget()
    {
        // Arrange
        string input = "N=150, P=100, K=250, Ca=400, Mg=50, S=30, Fe=10, Cu=5, Mn=7, Zn=8, B=3, Mo=2, Cl=45, Si=20, Se=1, L=1000";

        // Act
        PpmTarget result = _parser.Parse(input);

        // Assert
        Assert.Equal(150, result.N.Value);
        Assert.Equal(100, result.P.Value);
        Assert.Equal(250, result.K.Value);
        Assert.Equal(400, result.Ca.Value);
        Assert.Equal(50, result.Mg.Value);
        Assert.Equal(30, result.S.Value);
        Assert.Equal(10, result.Fe.Value);
        Assert.Equal(5, result.Cu.Value);
        Assert.Equal(7, result.Mn.Value);
        Assert.Equal(8, result.Zn.Value);
        Assert.Equal(3, result.B.Value);
        Assert.Equal(2, result.Mo.Value);
        Assert.Equal(45, result.Cl.Value);
        Assert.Equal(20, result.Si.Value);
        Assert.Equal(1, result.Se.Value);
        Assert.Equal(1000, result.Liters.Value);
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public void Parse_ValidInput_ReturnsCorrectPpmTarget()
    {
        // Arrange
        string input = "N=200, P=100, K=300, Ca=400, Mg=50";

        // Act
        PpmTarget result = _parser.Parse(input);

        // Assert
        Assert.Equal(200, result.N.Value);
        Assert.Equal(100, result.P.Value);
        Assert.Equal(300, result.K.Value);
        Assert.Equal(400, result.Ca.Value);
        Assert.Equal(50, result.Mg.Value);
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public void Parse_InvalidElement_ThrowsFormatException()
    {
        // Arrange
        string input = "X=100";

        // Act  
        FormatException ex = Assert.Throws<FormatException>(() => _parser.Parse(input));
        
        //Assert
        Assert.Contains("not recognized", ex.Message);
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public void Parse_InvalidFormat_ThrowsFormatException()
    {
        // Arrange
        string input = "N=two hundred";

        // Act 
        FormatException ex = Assert.Throws<FormatException>(() => _parser.Parse(input));
        
        //Assert
        Assert.Contains("Unable to parse", ex.Message);
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public void Parse_MissingValue_ThrowsFormatException()
    {
        // Arrange
        string input = "N=";

        // Act 
        FormatException ex = Assert.Throws<FormatException>(() => _parser.Parse(input));
        
        //Assert
        Assert.Contains("Unable to parse", ex.Message);
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public void Parse_DuplicateElements_ThrowsArgumentException()
    {
        // Arrange
        string input = "N=200, N=300";

        // Act  
        FormatException ex = Assert.Throws<FormatException>(() => _parser.Parse(input));
        
        //Assert
        Assert.Contains("Duplicate element", ex.Message);
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public void Parse_EmptyInput_ThrowsArgumentException()
    {
        // Arrange
        string input = "";

        // Act
        ArgumentException ex = Assert.Throws<ArgumentException>(() => _parser.Parse(input));
        
        // Assert
        Assert.Contains("The value cannot be an empty string or co", ex.Message);
    }

}