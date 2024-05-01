using NPKTools.Core.Domain.Fertilizers;
using NPKTools.Core.Domain.Fertilizers.Builders;
using NPKTools.Core.Domain.PartsPerMillion;
using NPKTools.Core.Domain.PpmTarget;
using NPKTools.Core.Domain.PpmTarget.Builder;
using NPKTools.Core.Domain.SolutionsFinderSettings;
using NPKTools.Core.Domain.SolutionsFinderSettings.Builder;
using NPKTools.Optimizer.Components;
using NPKTools.Optimizer.Contracts;
using NPKTools.PPMCalc;
using Xunit;

namespace NPKTools.IntegrationTests;

public class FertilizerOptimizerTests
{
    protected readonly IPpmCalculationService Calc;
    protected readonly IFertilizerOptimizer Optimizer;

    public FertilizerOptimizerTests()
    {
        Calc = new PpmCalculationService();
        IOptimizationProblemSolver solver = new GoogleOrToolsOptimizationSolver();
        IOptimizationProblemMapper mapper = new OptimizationProblemMapper();
        Optimizer = new FertilizerOptimizationAdapter(solver, mapper);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Optimize_WithValidInputMacro_ReturnCorrectCollection()
    {
        //Arrange
        IList<FertilizerOptimizationModel> sourceCollection = new List<FertilizerOptimizationModel>()
        {
            new FertilizerBuilder()
                .AddId(Guid.Parse("72f90e90-804c-4955-9e51-8e7b921836c5")) //CalciumNitrate
                .AddNo3(11.863)
                .AddCaNonChelated(16.972)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("77cb9e8e-4e4c-454d-9411-e5d859f32781")) // PotassiumNitrate
                .AddNo3(13.854)
                .AddK(38.672)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("2fc8a292-2095-42c0-bd50-9b6b355bf92a")) //AmmoniumNitrate 
                .AddNo3(17.499)
                .AddNh4(17.499)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("8389dbfb-792f-40d0-8fd9-701a506af48b")) //MagnesiumSulfate 
                .AddMgNonChelated(9.861)
                .AddS(13.008)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("9d0460fc-596f-4b70-8ebb-af217aecc097")) //PotassiumDihydrogenPhosphate 
                .AddP(22.761)
                .AddK(28.731)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("4fa956e7-114a-41e4-b6da-10643cdd8086")) //PotassiumSulfate
                .AddS(18.401)
                .AddK(44.874)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("6aeff6a8-a8dc-4b3c-acf5-817ada864557")) // MagnesiumNitrate
                .AddMgNonChelated(9.479)
                .AddNo3(10.925)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb")) // IronSulfate
                .AddFeNonChelated(20.088)
                .AddS(11.532)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("9f0c6df5-6dc1-4113-b911-e50af9410248")) //CopperSulfate
                .AddS(12.841)
                .AddCuNonChelated(25.451)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43")) //ManganeseSulfate
                .AddS(18.969)
                .AddMnNonChelated(32.506)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("39bb0466-fe87-4144-82c0-9aa460ac77c6")) //ZincSulfate
                .AddS(17.866)
                .AddZnNonChelated(36.433)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("10f35129-8b50-448d-98ed-8442cfc26985")) //BoricAcid
                .AddB(17.483)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("d190bcb4-1236-4bb4-95d6-584c719f4bde")) //SodiumMolybdate
                .AddNa(19.003)
                .AddMo(39.656)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("337401e7-cbe4-48aa-968b-cceba18d6478")) //CalciumChloride
                .AddCaNonChelated(18.295)
                .AddCl(32.364)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("0f886cf2-729e-4d89-a1cf-08acb99d5ffe")) //SodiumSilicate
                .AddSi(23.009)
                .AddNa(37.669)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb")) //SodiumSelenate 
                .AddSe(41.795)
                .AddNa(24.335)
                .Build()
        };

        PpmTarget target = new PpmTargetBuilder()
            .AddN(150)
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
            .AddLitters(1)
            .Build();

        SolutionFinderSettings settings = new SolutionFinderSettingsBuilder()
            .AddRangeFactor(1)
            .AddN(1)
            .AddP(1)
            .AddK(1)
            .AddCa(1)
            .AddMg(1)
            .AddS(1)
            .AddCl(1)
            .AddFe(1)
            .AddCu(1)
            .AddMn(1)
            .AddZn(1)
            .AddB(1)
            .AddMo(1)
            .AddSi(1)
            .AddSe(1)
            .AddNa(0)
            .Build();

        IList<Fertilizer> expectedCollection = new List<Fertilizer>
        {
            new FertilizerBuilder()
                .AddId(Guid.Parse("72f90e90-804c-4955-9e51-8e7b921836c5")) //CalciumNitrate
                .AddWeight(0.589)
                .AddNo3(11.863)
                .AddCaNonChelated(16.972)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("2fc8a292-2095-42c0-bd50-9b6b355bf92a")) //AmmoniumNitrate 
                .AddWeight(0.137)
                .AddNo3(17.499)
                .AddNh4(17.499)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("8389dbfb-792f-40d0-8fd9-701a506af48b")) //MagnesiumSulfate 
                .AddWeight(0.325)
                .AddMgNonChelated(9.861)
                .AddS(13.008)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("9d0460fc-596f-4b70-8ebb-af217aecc097")) //PotassiumDihydrogenPhosphate 
                .AddWeight(0.22)
                .AddP(22.761)
                .AddK(28.731)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("4fa956e7-114a-41e4-b6da-10643cdd8086")) //PotassiumSulfat
                .AddWeight(0.305)
                .AddS(18.401)
                .AddK(44.874)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("6aeff6a8-a8dc-4b3c-acf5-817ada864557")) // MagnesiumNitrate
                .AddWeight(0.295)
                .AddMgNonChelated(9.479)
                .AddNo3(10.925)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb")) // IronSulfate
                .AddFeNonChelated(20.088)
                .AddS(11.532)
                .AddWeight(0.01)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("9f0c6df5-6dc1-4113-b911-e50af9410248")) //CopperSulfate
                .AddS(12.841)
                .AddCuNonChelated(25.451)
                .AddWeight(0.0002)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43")) //ManganeseSulfate
                .AddS(18.969)
                .AddMnNonChelated(32.506)
                .AddWeight(0.0017)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("39bb0466-fe87-4144-82c0-9aa460ac77c6")) //ZincSulfate
                .AddS(17.866)
                .AddZnNonChelated(36.433)
                .AddWeight(0.0009)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("10f35129-8b50-448d-98ed-8442cfc26985")) //BoricAcid
                .AddB(17.483)
                .AddWeight(0.0016)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("d190bcb4-1236-4bb4-95d6-584c719f4bde")) //SodiumMolybdate
                .AddNa(19.003)
                .AddMo(39.656)
                .AddWeight(0.000126)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("337401e7-cbe4-48aa-968b-cceba18d6478")) //CalciumChloride
                .AddCaNonChelated(18.295)
                .AddCl(32.364)
                .AddWeight(3.09E-05)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("0f886cf2-729e-4d89-a1cf-08acb99d5ffe")) //SodiumSilicate
                .AddSi(23.009)
                .AddNa(37.669)
                .AddWeight(4.346E-05)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb")) //SodiumSelenate 
                .AddSe(41.795)
                .AddNa(24.335)
                .AddWeight(2.393E-05)
                .Build()
        };

        //Act
        IList<Fertilizer> result = Optimizer.Optimize(target, sourceCollection, settings);

        //Assert
        Assert.Equal(15, result.Count);

        foreach (Fertilizer fertilizer in result)
        {
            Fertilizer expected = expectedCollection.FirstOrDefault(f => fertilizer.RefId.Value == f.RefId.Value);
            Assert.NotNull(expected);
            Assert.Equal(expected.Weight.Value, fertilizer.Weight.Value, 0.001);
        }

        Ppm ppmActual = Calc.CalculatePpm(result);

        Assert.InRange(ppmActual.Nitrogen.Value, target.N.Value * 0.99, target.N.Value * 1.01);
        Assert.InRange(ppmActual.Phosphorus.Value, target.P.Value * 0.99, target.P.Value * 1.01);
        Assert.InRange(ppmActual.Potassium.Value, target.K.Value * 0.99, target.K.Value * 1.01);
        Assert.InRange(ppmActual.Magnesium.Value, target.Mg.Value * 0.99, target.Mg.Value * 1.01);
        Assert.InRange(ppmActual.Calcium.Value, target.Ca.Value * 0.99, target.Ca.Value * 1.01);
        Assert.InRange(ppmActual.Sulfur.Value, target.S.Value * 0.99, target.S.Value * 1.01);
        Assert.InRange(ppmActual.Iron.Value, target.Fe.Value * 0.99, target.Fe.Value * 1.01);
        Assert.InRange(ppmActual.Copper.Value, target.Cu.Value * 0.99, target.Cu.Value * 1.01);
        Assert.InRange(ppmActual.Manganese.Value, target.Mn.Value * 0.99, target.Mn.Value * 1.01);
        Assert.InRange(ppmActual.Zinc.Value, target.Zn.Value * 0.99, target.Zn.Value * 1.01);
        Assert.InRange(ppmActual.Boron.Value, target.B.Value * 0.99, target.B.Value * 1.01);
        Assert.InRange(ppmActual.Molybdenum.Value, target.Mo.Value * 0.99, target.Mo.Value * 1.01);
        Assert.InRange(ppmActual.Chlorine.Value, target.Cl.Value * 0.99, target.Cl.Value * 1.01);
        Assert.InRange(ppmActual.Silicon.Value, target.Si.Value * 0.99, target.Si.Value * 1.01);
        Assert.InRange(ppmActual.Selenium.Value, target.Se.Value * 0.99, target.Se.Value * 1.01);
    }

    [Theory]
    [Trait("Category", "Integration")]
    [InlineData("4fa956e7-114a-41e4-b6da-10643cdd8086", "72f90e90-804c-4955-9e51-8e7b921836c5", 1)]
    [InlineData("72f90e90-804c-4955-9e51-8e7b921836c5", "72f90e90-804c-4955-9e51-8e7b921836c5", 2)]
    public void Optimize_WithDuplicates_ThrowsException(string firstId, string secondId, int secondNo3Value)
    {
        // Arrange
        Guid id1 = Guid.Parse(firstId);
        Guid id2 = Guid.Parse(secondId);

        IList<FertilizerOptimizationModel> sourceCollection = new List<FertilizerOptimizationModel>()
        {
            new FertilizerBuilder()
                .AddId(id1)
                .AddNo3(1)
                .Build(),

            new FertilizerBuilder()
                .AddId(id2)
                .AddNo3(secondNo3Value)
                .Build(),
        };

        PpmTarget target = new PpmTargetBuilder()
            .AddN(1)
            .Build();

        SolutionFinderSettings settings = new SolutionFinderSettingsBuilder()
            .AddK(1)
            .Build();

        // Act
        Action act = () => Optimizer.Optimize(target, sourceCollection, settings);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }


    [Fact]
    [Trait("Category", "Integration")]
    public void Optimize_WithPricePriority_SelectsLeastExpensiveOption()
    {
        // Arrange
        IList<FertilizerOptimizationModel> sourceCollection = new List<FertilizerOptimizationModel>()
        {
            new FertilizerBuilder()
                .AddId(Guid.Parse("00000000-0000-0000-0000-000000000001"))
                .AddNo3(10)
                .AddPrice(1.0) 
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("00000000-0000-0000-0000-000000000002"))
                .AddNo3(15)
                .AddPrice(2.0)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("00000000-0000-0000-0000-000000000003"))
                .AddNo3(5)
                .AddPrice(0.8) 
                .Build(),
        };

        PpmTarget target = new PpmTargetBuilder()
            .AddN(10) 
            .Build();

        SolutionFinderSettings settings = new SolutionFinderSettingsBuilder()
            .AddN(1) 
            .Build();

        Fertilizer expected = new FertilizerBuilder()
            .AddId(Guid.Parse("00000000-0000-0000-0000-000000000001"))
            .AddWeight(0.1) 
            .AddNo3(10)
            .Build();

        // Act
        IList<Fertilizer> result = Optimizer.Optimize(target, sourceCollection, settings);

        // Assert
        Assert.Single(result);
        Assert.Equal(expected.RefId.Value, result.First().RefId.Value);
        Assert.Equal(expected.Weight.Value, result.First().Weight.Value);
    }
}