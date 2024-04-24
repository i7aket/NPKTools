using NPKOptimizer.Components;
using NPKOptimizer.Contracts;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.Fertilizers.Builders;
using NPKOptimizer.Domain.PartsPerMillion;
using NPKOptimizer.Domain.PartsPerMillion.Builder;
using Xunit;

namespace NPKOptimizer.Tests.UnitTests;

public class PpmCalculationServiceTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void CalculatePpm_WithValidMix_ReturnsCorrectPpmValues()
    {
        // Arrange
        IPpmCalculationService ppmCalculationService = new PpmCalculationService();

        IList<Fertilizer> fertilizers = new List<Fertilizer>
        {
            new FertilizerBuilder()
                .AddId(Guid.Parse("72f90e90-804c-4955-9e51-8e7b921836c5"))
                .AddWeight(0.589)
                .AddNo3(11.863)
                .AddCaNonChelated(16.972)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("2fc8a292-2095-42c0-bd50-9b6b355bf92a"))
                .AddWeight(0.137)
                .AddNo3(17.499)
                .AddNh4(17.499)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("8389dbfb-792f-40d0-8fd9-701a506af48b"))
                .AddWeight(0.325)
                .AddMgNonChelated(9.861)
                .AddS(13.008)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("9d0460fc-596f-4b70-8ebb-af217aecc097"))
                .AddWeight(0.22)
                .AddP(22.761)
                .AddK(28.731)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("4fa956e7-114a-41e4-b6da-10643cdd8086"))
                .AddWeight(0.305)
                .AddS(18.401)
                .AddK(44.874)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("6aeff6a8-a8dc-4b3c-acf5-817ada864557"))
                .AddWeight(0.295)
                .AddMgNonChelated(9.479)
                .AddNo3(10.925)
                .Build(),
            //
            new FertilizerBuilder()
                .AddId(Guid.Parse("a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb"))
                .AddFeNonChelated(20.088)
                .AddS(11.532)
                .AddWeight(0.01)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("9f0c6df5-6dc1-4113-b911-e50af9410248"))
                .AddS(12.841)
                .AddCuNonChelated(25.451)
                .AddWeight(0.0002)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43"))
                .AddS(18.969)
                .AddMnNonChelated(32.506)
                .AddWeight(0.0017)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("39bb0466-fe87-4144-82c0-9aa460ac77c6"))
                .AddS(17.866)
                .AddZnNonChelated(36.433)
                .AddWeight(0.0009)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("10f35129-8b50-448d-98ed-8442cfc26985"))
                .AddB(17.483)
                .AddWeight(0.0016)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("d190bcb4-1236-4bb4-95d6-584c719f4bde"))
                .AddNa(19.003)
                .AddMo(39.656)
                .AddWeight(0.000126)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("337401e7-cbe4-48aa-968b-cceba18d6478"))
                .AddCaNonChelated(18.295)
                .AddCl(32.364)
                .AddWeight(3.09E-05)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("0f886cf2-729e-4d89-a1cf-08acb99d5ffe"))
                .AddSi(23.009)
                .AddNa(37.669)
                .AddWeight(4.346E-05)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb"))
                .AddSe(41.795)
                .AddNa(24.335)
                .AddWeight(2.393E-05)
                .Build()
        };


        Ppm expectedPpm = new PpmBuilder()
            .AddNitrate(126)
            .AddAmmonium(24)
            .AddP(50)
            .AddK(200)
            .AddMg(60)
            .AddCa(100)
            .AddS(100)
            .AddFe(2)
            .AddCu(0.051)
            .AddMn(0.55)
            .AddZn(0.33)
            .AddB(0.28)
            .AddMo(0.05)
            .AddCl(0.01)
            .AddSi(0.01)
            .AddSe(0.01)
            .AddNa(0.046)
            .Build();

        // Act
        Ppm actualPpm = ppmCalculationService.CalculatePpm(fertilizers);
        const double precision = 1;

        // Assert
        Assert.InRange(actualPpm.Nitrogen.Nitrate, expectedPpm.Nitrogen.Nitrate * 0.99, expectedPpm.Nitrogen.Nitrate * 1.01);
        Assert.InRange(actualPpm.Nitrogen.Ammonium, expectedPpm.Nitrogen.Ammonium * 0.99, expectedPpm.Nitrogen.Ammonium * 1.01);
        Assert.InRange(actualPpm.Phosphorus.Value, expectedPpm.Phosphorus.Value * 0.99, expectedPpm.Phosphorus.Value * 1.01);
        Assert.InRange(actualPpm.Potassium.Value, expectedPpm.Potassium.Value * 0.99, expectedPpm.Potassium.Value * 1.01);
        Assert.InRange(actualPpm.Magnesium.Value, expectedPpm.Magnesium.Value * 0.99, expectedPpm.Magnesium.Value * 1.01);
        Assert.InRange(actualPpm.Calcium.Value, expectedPpm.Calcium.Value * 0.99, expectedPpm.Calcium.Value * 1.01);
        Assert.InRange(actualPpm.Sulfur.Value, expectedPpm.Sulfur.Value * 0.99, expectedPpm.Sulfur.Value * 1.01);
        Assert.InRange(actualPpm.Iron.Value, expectedPpm.Iron.Value * 0.99, expectedPpm.Iron.Value * 1.01);
        Assert.InRange(actualPpm.Copper.Value, expectedPpm.Copper.Value * 0.99, expectedPpm.Copper.Value * 1.01);
        Assert.InRange(actualPpm.Manganese.Value, expectedPpm.Manganese.Value * 0.99, expectedPpm.Manganese.Value * 1.01);
        Assert.InRange(actualPpm.Zinc.Value, expectedPpm.Zinc.Value * 0.99, expectedPpm.Zinc.Value * 1.01);
        Assert.InRange(actualPpm.Boron.Value, expectedPpm.Boron.Value * 0.99, expectedPpm.Boron.Value * 1.01);
        Assert.InRange(actualPpm.Molybdenum.Value, expectedPpm.Molybdenum.Value * 0.99, expectedPpm.Molybdenum.Value * 1.01);
        Assert.InRange(actualPpm.Chlorine.Value, expectedPpm.Chlorine.Value * 0.99, expectedPpm.Chlorine.Value * 1.01);
        Assert.InRange(actualPpm.Silicon.Value, expectedPpm.Silicon.Value * 0.99, expectedPpm.Silicon.Value * 1.01);
        Assert.InRange(actualPpm.Selenium.Value, expectedPpm.Selenium.Value * 0.99, expectedPpm.Selenium.Value * 1.01);
        Assert.InRange(actualPpm.Sodium.Value, expectedPpm.Sodium.Value * 0.99, expectedPpm.Sodium.Value * 1.01);
    }
}