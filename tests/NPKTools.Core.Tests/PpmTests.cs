using System.Text;
using NPKTools.Core.Domain.PartsPerMillion;
using NPKTools.Core.Domain.PartsPerMillion.Builder;
using NPKTools.Core.Domain.PartsPerMillion.ValueObjects;
using Xunit;

namespace NPKTools.Core.Tests;

public class PpmTests
{

    [Fact]
    public void Report_NonZeroValues_FormatsCorrectly()
    {
        // Arrange
        Ppm ppm = new PpmBuilder()
            .AddAmine(100)
            .AddNitrate(50)
            .AddAmmonium(25)
            .AddP(50)
            .AddK(200)
            .AddMg(60)
            .AddCa(100)
            .AddS(100)
            .AddFe(2)
            .AddCu(0.05)
            .AddMn(0.55)
            .AddZn(0.33)
            .AddB(0.28)
            .AddMo(0.05)
            .AddCl(0.01)
            .AddSi(0.01)
            .AddSe(0.01)
            .AddLiters(1000) 
            .Build();

        // Act
        string report = ppm.Report();

        // Assert
        string expectedOutput = new StringBuilder()
            .AppendLine("-PPM Report-")  
            .AppendLine("Total PPM: 688,290")  
            .AppendLine("Nitrogen: 175,000") 
            .AppendLine("\u2514Nitrate (NO3): 50,000")  
            .AppendLine("\u2514Ammonium (NH4): 25,000")
            .AppendLine("\u2514Amine (NH2): 100,000")
            .AppendLine("Phosphorus (P): 50,000")
            .AppendLine("Potassium (K): 200,000")
            .AppendLine("Magnesium (Mg): 60,000")
            .AppendLine("Sulfur (S): 100,000")
            .AppendLine("Calcium (Ca): 100,000")
            .AppendLine("Iron (Fe): 2,000")
            .AppendLine("Copper (Cu): 0,050")
            .AppendLine("Manganese (Mn): 0,550")
            .AppendLine("Zinc (Zn): 0,330")
            .AppendLine("Boron (B): 0,280")
            .AppendLine("Molybdenum (Mo): 0,050")
            .AppendLine("Chlorine (Cl): 0,010")
            .AppendLine("Silicon (Si): 0,010")
            .AppendLine("Selenium (Se): 0,010")
            .ToString();


        Assert.Equal(expectedOutput, report);
    }
}