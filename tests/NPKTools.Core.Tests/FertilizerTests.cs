using System.Text;
using NPKTools.Core.Domain.Fertilizers;
using NPKTools.Core.Domain.Fertilizers.Builders;
using NPKTools.Core.Domain.Fertilizers.Enums;
using NPKTools.Core.Domain.Fertilizers.Extensions;
using Xunit;

namespace NPKTools.Core.Tests;

public class FertilizerTests
{
    [Fact]
    public void Report_WithNonZeroValues_ReturnsCorrectReport()
    {
        // Arrange
        FertilizerBuilder builder = new FertilizerBuilder()
            .AddName("Complete Mix")
            .AddFormula("NPK-15-10-20")
            .AddType(ConcentrateType.A)
            .AddId(Guid.NewGuid())
            .AddPrice(50.75)
            .AddNo3(15)
            .AddNh4(10)
            .AddNh2(5)
            .AddP(10)
            .AddK(20)
            .AddCaNonChelated(5)
            .AddCaEdta(2)
            .AddMgNonChelated(3)
            .AddMgEdta(1)
            .AddS(4)
            .AddFeNonChelated(1)
            .AddFeEdta(0.5)
            .AddFeDtpa(0.3)
            .AddFeEddha(0.2)
            .AddFeHbed(0.1)
            .AddFeOrthoPart(0.05)
            .AddCuNonChelated(0.5)
            .AddCuEdta(0.25)
            .AddMnNonChelated(0.7)
            .AddMnEdta(0.35)
            .AddZnNonChelated(0.4)
            .AddZnEdta(0.2)
            .AddB(0.3)
            .AddMo(0.1)
            .AddCl(0.1)
            .AddSi(0.5)
            .AddSe(0.05)
            .AddNa(0.2)
            .AddWeight(100);
        Fertilizer fertilizer = builder.Build();

        // Act
        string report = fertilizer.Report();

        // Assert
        string expectedOutput = new StringBuilder()
            .AppendLine("Name: Complete Mix")
            .AppendLine("Formula: NPK-15-10-20")
            .AppendLine("Concentrate Type: A")  
            .AppendLine("Weight: 100,000")
            .AppendLine("Nitrogen: 30,000")
            .AppendLine("\u2514Nitrate NO\u2083: 15,000")
            .AppendLine("\u2514Ammonium NH\u2084: 10,000")
            .AppendLine("\u2514Amine NH\u2082: 5,000")
            .AppendLine("Phosphorus P: 10,000")
            .AppendLine("Potassium K: 20,000")
            .AppendLine("Calcium Ca: 7,000")
            .AppendLine("\u2514EDTA: 2,000")
            .AppendLine("Magnesium Mg: 4,000")
            .AppendLine("\u2514EDTA: 1,000")
            .AppendLine("Sulfur S: 4,000")
            .AppendLine("Chlorine Cl: 0,100")
            .AppendLine("Iron Fe: 2,100")
            .AppendLine("\u2514EDTA: 0,500")
            .AppendLine("\u2514DTPA: 0,300")
            .AppendLine("\u2514FeEDDHA: 0,200")
            .AppendLine("\u2514FeHBED: 0,100")
            .AppendLine("\u2514Ortho-ortho: 0,050")
            .AppendLine("Copper Cu: 0,750")
            .AppendLine("\u2514EDTA: 0,250")
            .AppendLine("Manganese Mn: 1,050")
            .AppendLine("\u2514EDTA: 0,350")
            .AppendLine("Zinc Zn: 0,600")
            .AppendLine("\u2514EDTA: 0,200")
            .AppendLine("Boron B: 0,300")
            .AppendLine("Molybdenum Mo: 0,100")
            .AppendLine("Silicon Si: 0,500")
            .AppendLine("Selenium Se: 0,050")
            .AppendLine("Sodium Na: 0,200")
            .ToString();


        Assert.Equal(expectedOutput, report);
    }
}


