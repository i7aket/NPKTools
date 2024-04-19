using NPKOptimizer.Common;
using NPKOptimizer.Components;
using NPKOptimizer.Components.Builders;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.PartsPerMillion;
using Xunit;

namespace NPKOptimizer.Tests;

public class FertilizerOptimizationsServiceTests
{
    [Fact]
    public void CalculatePpm_WithValidMix_ReturnsCorrectPpmValues()
    {
        // Arrange
        IPpmCalculationService ppmCalculationService = new PpmCalculationService();
        
        FertilizerCollection set = new ();
        
        set.AddRange(new List<Fertilizer>
        {
            new FertilizerBuilder()
                .Id(Guid.NewGuid())
                .Name("Calcium Nitrate Tetrahydrate")
                .Formula("Ca(NO3)2*4H2O")
                .No3(11.863)
                .CaNonChelated(16.971703945864)
                .Weight(0.35352962)
                .Build(),

            new FertilizerBuilder()
                .Id(Guid.NewGuid())
                .Name("Potassium Nitrate")
                .Formula("KNO3")
                .No3(13.854284224987957)
                .K(38.672018341818145)
                .Weight(0.22467099)
                .Build(),

            new FertilizerBuilder()
                .Id(Guid.NewGuid())
                .Name("Ammonium Nitrate")
                .Formula("NH4NO3")
                .No3(34.99868820508976)
                .Nh4(34.99868820508976)
                .Weight(0.03847891)
                .Build(),

            new FertilizerBuilder()
                .Id(Guid.NewGuid())
                .Name("Potassium Dihydrogen Phosphate")
                .Formula("KH2PO4")
                .P(22.760756509458098)
                .K(28.730991289780867)
                .Weight(0.21967635)
                .Build(),

            new FertilizerBuilder()
                .Id(Guid.NewGuid())
                .Name("Magnesium Sulfate Heptahydrate")
                .Formula("MgSO4*7H2O")
                .MgNonChelated(9.861400761159754)
                .S(13.007879382957487)
                .Weight(0.60843283)
                .Build()
        });

        Ppm expectedPpm = new PpmBuilder()
            .Nitrate(87)
            .Ammonium(13)
            .P(50)
            .K(150)
            .Mg(60)
            .Ca(60)
            .S(80)
            .Build();

        // Act
        ActionResult<Ppm> actualActionResult = ppmCalculationService.CalculatePpm(set);
        const double precision = 1;

        // Assert
        Assert.True(actualActionResult.IsSuccess);
        Assert.Equal(expectedPpm.N.Value, actualActionResult.Payload!.N.Value, precision);
        Assert.Equal(expectedPpm.N.Ammonium, actualActionResult.Payload.N.Ammonium, precision);
        Assert.Equal(expectedPpm.P.Value, actualActionResult.Payload.P.Value, precision);
        Assert.Equal(expectedPpm.K.Value, actualActionResult.Payload.K.Value, precision);
        Assert.Equal(expectedPpm.Mg.Value, actualActionResult.Payload.Mg.Value, precision);
        Assert.Equal(expectedPpm.S.Value, actualActionResult.Payload.S.Value, precision);
        Assert.Equal(expectedPpm.Ca.Value, actualActionResult.Payload.Ca.Value, precision);
    }
}