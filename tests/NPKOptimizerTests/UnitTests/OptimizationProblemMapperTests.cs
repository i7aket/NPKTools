using NPKOptimizer.Components;
using NPKOptimizer.Contracts;
using NPKOptimizer.Domain.Collections;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.Fertilizers.Builders;
using NPKOptimizer.Domain.PpmTarget;
using NPKOptimizer.Domain.PpmTarget.Builder;
using NPKOptimizer.Domain.SolutionsFinderSettings;
using NPKOptimizer.Domain.SolutionsFinderSettings.Builder;
using Xunit;

namespace NPKOptimizer.Tests.UnitTests;

public class OptimizationProblemMapperTests
{
    protected IOptimizationProblemMapper Mapper;

    public OptimizationProblemMapperTests()
    {
        Mapper = new OptimizationProblemMapper();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void CreateOptimizationProblem_ShouldCorrectlyMapInputsToOptimizationProblem()
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

        OptimizationProblem expectedProblem = new OptimizationProblem
        {
            Variables = new Dictionary<string, double>
            {
                { "72f90e90-804c-4955-9e51-8e7b921836c5", 0 }, // CalciumNitrate
                { "77cb9e8e-4e4c-454d-9411-e5d859f32781", 0 }, // PotassiumNitrate
                { "2fc8a292-2095-42c0-bd50-9b6b355bf92a", 0 }, // AmmoniumNitrate
                { "8389dbfb-792f-40d0-8fd9-701a506af48b", 0 }, // MagnesiumSulfate
                { "9d0460fc-596f-4b70-8ebb-af217aecc097", 0 }, // PotassiumDihydrogenPhosphate
                { "4fa956e7-114a-41e4-b6da-10643cdd8086", 0 }, // PotassiumSulfate
                { "6aeff6a8-a8dc-4b3c-acf5-817ada864557", 0 }, // MagnesiumNitrate
                { "a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb", 0 }, // IronSulfate
                { "9f0c6df5-6dc1-4113-b911-e50af9410248", 0 }, // CopperSulfate
                { "31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43", 0 }, // ManganeseSulfate
                { "39bb0466-fe87-4144-82c0-9aa460ac77c6", 0 }, // ZincSulfate
                { "10f35129-8b50-448d-98ed-8442cfc26985", 0 }, // BoricAcid
                { "d190bcb4-1236-4bb4-95d6-584c719f4bde", 0 }, // SodiumMolybdate
                { "337401e7-cbe4-48aa-968b-cceba18d6478", 0 }, // CalciumChloride
                { "0f886cf2-729e-4d89-a1cf-08acb99d5ffe", 0 }, // SodiumSilicate
                { "2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb", 0 } // SodiumSelenate
            },
            Objective = new OptimizationProblem.OptimizationObjective
            {
                Coefficients = new Dictionary<string, double>
                {
                    { "72f90e90-804c-4955-9e51-8e7b921836c5", 1 },
                    { "77cb9e8e-4e4c-454d-9411-e5d859f32781", 1 },
                    { "2fc8a292-2095-42c0-bd50-9b6b355bf92a", 1 },
                    { "8389dbfb-792f-40d0-8fd9-701a506af48b", 1 },
                    { "9d0460fc-596f-4b70-8ebb-af217aecc097", 1 },
                    { "4fa956e7-114a-41e4-b6da-10643cdd8086", 1 },
                    { "6aeff6a8-a8dc-4b3c-acf5-817ada864557", 1 },
                    { "a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb", 1 },
                    { "9f0c6df5-6dc1-4113-b911-e50af9410248", 1 },
                    { "31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43", 1 },
                    { "39bb0466-fe87-4144-82c0-9aa460ac77c6", 1 },
                    { "10f35129-8b50-448d-98ed-8442cfc26985", 1 },
                    { "d190bcb4-1236-4bb4-95d6-584c719f4bde", 1 },
                    { "337401e7-cbe4-48aa-968b-cceba18d6478", 1 },
                    { "0f886cf2-729e-4d89-a1cf-08acb99d5ffe", 1 },
                    { "2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb", 1 }
                },
                IsMinimization = true
            },
            Constraints = new List<OptimizationProblem.OptimizationConstraint>
            {
                new()
                {
                    Name = "N",
                    LowerBound = 15,
                    UpperBound = 15,
                    Coefficients = new Dictionary<string, double>
                    {
                        { "72f90e90-804c-4955-9e51-8e7b921836c5", 11.863 },
                        { "77cb9e8e-4e4c-454d-9411-e5d859f32781", 13.854 },
                        { "2fc8a292-2095-42c0-bd50-9b6b355bf92a", 34.998 },
                        { "6aeff6a8-a8dc-4b3c-acf5-817ada864557", 10.925 }, // MagnesiumNitrate
                        { "8389dbfb-792f-40d0-8fd9-701a506af48b", 0 },
                        { "9d0460fc-596f-4b70-8ebb-af217aecc097", 0 }, // PotassiumDihydrogenPhosphate
                        { "4fa956e7-114a-41e4-b6da-10643cdd8086", 0 }, // PotassiumSulfate
                        { "a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb", 0 }, // IronSulfate
                        { "9f0c6df5-6dc1-4113-b911-e50af9410248", 0 }, // CopperSulfate
                        { "31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43", 0 }, // ManganeseSulfate
                        { "39bb0466-fe87-4144-82c0-9aa460ac77c6", 0 }, // ZincSulfate
                        { "10f35129-8b50-448d-98ed-8442cfc26985", 0 }, // BoricAcid
                        { "d190bcb4-1236-4bb4-95d6-584c719f4bde", 0 }, // SodiumMolybdate
                        { "337401e7-cbe4-48aa-968b-cceba18d6478", 0 }, // CalciumChloride
                        { "0f886cf2-729e-4d89-a1cf-08acb99d5ffe", 0 }, // SodiumSilicate
                        { "2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb", 0 } // SodiumSelenate
                    }
                },
                new()
                {
                    Name = "P",
                    LowerBound = 5,
                    UpperBound = 5,
                    Coefficients = new Dictionary<string, double>
                    {
                        { "9d0460fc-596f-4b70-8ebb-af217aecc097", 22.761 },
                        { "72f90e90-804c-4955-9e51-8e7b921836c5", 0 }, // CalciumNitrate
                        { "77cb9e8e-4e4c-454d-9411-e5d859f32781", 0 }, // PotassiumNitrate
                        { "2fc8a292-2095-42c0-bd50-9b6b355bf92a", 0 }, // AmmoniumNitrate
                        { "8389dbfb-792f-40d0-8fd9-701a506af48b", 0 }, // MagnesiumSulfate
                        { "4fa956e7-114a-41e4-b6da-10643cdd8086", 0 }, // PotassiumSulfate
                        { "6aeff6a8-a8dc-4b3c-acf5-817ada864557", 0 }, // MagnesiumNitrate
                        { "a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb", 0 }, // IronSulfate
                        { "9f0c6df5-6dc1-4113-b911-e50af9410248", 0 }, // CopperSulfate
                        { "31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43", 0 }, // ManganeseSulfate
                        { "39bb0466-fe87-4144-82c0-9aa460ac77c6", 0 }, // ZincSulfate
                        { "10f35129-8b50-448d-98ed-8442cfc26985", 0 }, // BoricAcid
                        { "d190bcb4-1236-4bb4-95d6-584c719f4bde", 0 }, // SodiumMolybdate
                        { "337401e7-cbe4-48aa-968b-cceba18d6478", 0 }, // CalciumChloride
                        { "0f886cf2-729e-4d89-a1cf-08acb99d5ffe", 0 }, // SodiumSilicate
                        { "2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb", 0 } // SodiumSelenate
                    }
                },
                new()
                {
                    Name = "K",
                    LowerBound = 20,
                    UpperBound = 20,
                    Coefficients = new Dictionary<string, double>
                    {
                        { "77cb9e8e-4e4c-454d-9411-e5d859f32781", 38.670 },
                        { "9d0460fc-596f-4b70-8ebb-af217aecc097", 28.731 },
                        { "4fa956e7-114a-41e4-b6da-10643cdd8086", 44.874 },
                        { "72f90e90-804c-4955-9e51-8e7b921836c5", 0 }, // CalciumNitrate
                        { "2fc8a292-2095-42c0-bd50-9b6b355bf92a", 0 }, // AmmoniumNitrate
                        { "8389dbfb-792f-40d0-8fd9-701a506af48b", 0 }, // MagnesiumSulfate
                        { "6aeff6a8-a8dc-4b3c-acf5-817ada864557", 0 }, // MagnesiumNitrate
                        { "a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb", 0 }, // IronSulfate
                        { "9f0c6df5-6dc1-4113-b911-e50af9410248", 0 }, // CopperSulfate
                        { "31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43", 0 }, // ManganeseSulfate
                        { "39bb0466-fe87-4144-82c0-9aa460ac77c6", 0 }, // ZincSulfate
                        { "10f35129-8b50-448d-98ed-8442cfc26985", 0 }, // BoricAcid
                        { "d190bcb4-1236-4bb4-95d6-584c719f4bde", 0 }, // SodiumMolybdate
                        { "337401e7-cbe4-48aa-968b-cceba18d6478", 0 }, // CalciumChloride
                        { "0f886cf2-729e-4d89-a1cf-08acb99d5ffe", 0 }, // SodiumSilicate
                        { "2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb", 0 } // SodiumSelenate
                    }
                },

                new()
                {
                    Name = "Ca",
                    LowerBound = 10,
                    UpperBound = 10,
                    Coefficients = new Dictionary<string, double>
                    {
                        { "72f90e90-804c-4955-9e51-8e7b921836c5", 16.972 },
                        { "337401e7-cbe4-48aa-968b-cceba18d6478", 18.295 },
                        { "77cb9e8e-4e4c-454d-9411-e5d859f32781", 0 }, // PotassiumNitrate
                        { "2fc8a292-2095-42c0-bd50-9b6b355bf92a", 0 }, // AmmoniumNitrate
                        { "8389dbfb-792f-40d0-8fd9-701a506af48b", 0 }, // MagnesiumSulfate
                        { "9d0460fc-596f-4b70-8ebb-af217aecc097", 0 }, // PotassiumDihydrogenPhosphate
                        { "4fa956e7-114a-41e4-b6da-10643cdd8086", 0 }, // PotassiumSulfate
                        { "6aeff6a8-a8dc-4b3c-acf5-817ada864557", 0 }, // MagnesiumNitrate
                        { "a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb", 0 }, // IronSulfate
                        { "9f0c6df5-6dc1-4113-b911-e50af9410248", 0 }, // CopperSulfate
                        { "31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43", 0 }, // ManganeseSulfate
                        { "39bb0466-fe87-4144-82c0-9aa460ac77c6", 0 }, // ZincSulfate
                        { "10f35129-8b50-448d-98ed-8442cfc26985", 0 }, // BoricAcid
                        { "d190bcb4-1236-4bb4-95d6-584c719f4bde", 0 }, // SodiumMolybdate
                        { "0f886cf2-729e-4d89-a1cf-08acb99d5ffe", 0 }, // SodiumSilicate
                        { "2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb", 0 } // SodiumSelenate
                    }
                },
                new()
                {
                    Name = "Mg",
                    LowerBound = 6,
                    UpperBound = 6,
                    Coefficients = new Dictionary<string, double>
                    {
                        { "8389dbfb-792f-40d0-8fd9-701a506af48b", 9.861 },
                        { "6aeff6a8-a8dc-4b3c-acf5-817ada864557", 9.479 },
                        { "72f90e90-804c-4955-9e51-8e7b921836c5", 0 }, // CalciumNitrate
                        { "77cb9e8e-4e4c-454d-9411-e5d859f32781", 0 }, // PotassiumNitrate
                        { "2fc8a292-2095-42c0-bd50-9b6b355bf92a", 0 }, // AmmoniumNitrate
                        { "9d0460fc-596f-4b70-8ebb-af217aecc097", 0 }, // PotassiumDihydrogenPhosphate
                        { "4fa956e7-114a-41e4-b6da-10643cdd8086", 0 }, // PotassiumSulfate
                        { "a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb", 0 }, // IronSulfate
                        { "9f0c6df5-6dc1-4113-b911-e50af9410248", 0 }, // CopperSulfate
                        { "31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43", 0 }, // ManganeseSulfate
                        { "39bb0466-fe87-4144-82c0-9aa460ac77c6", 0 }, // ZincSulfate
                        { "10f35129-8b50-448d-98ed-8442cfc26985", 0 }, // BoricAcid
                        { "d190bcb4-1236-4bb4-95d6-584c719f4bde", 0 }, // SodiumMolybdate
                        { "337401e7-cbe4-48aa-968b-cceba18d6478", 0 }, // CalciumChloride
                        { "0f886cf2-729e-4d89-a1cf-08acb99d5ffe", 0 }, // SodiumSilicate
                        { "2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb", 0 } // SodiumSelenate
                    }
                },
                new()
                {
                    Name = "S",
                    LowerBound = 10,
                    UpperBound = 10,
                    Coefficients = new Dictionary<string, double>
                    {
                        { "8389dbfb-792f-40d0-8fd9-701a506af48b", 13.008 },
                        { "4fa956e7-114a-41e4-b6da-10643cdd8086", 18.401 },
                        { "a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb", 11.532 },
                        { "9f0c6df5-6dc1-4113-b911-e50af9410248", 12.841 },
                        { "31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43", 18.969 },
                        { "39bb0466-fe87-4144-82c0-9aa460ac77c6", 17.866 },
                        { "72f90e90-804c-4955-9e51-8e7b921836c5", 0 }, // CalciumNitrate
                        { "77cb9e8e-4e4c-454d-9411-e5d859f32781", 0 }, // PotassiumNitrate
                        { "2fc8a292-2095-42c0-bd50-9b6b355bf92a", 0 }, // AmmoniumNitrate
                        { "9d0460fc-596f-4b70-8ebb-af217aecc097", 0 }, // PotassiumDihydrogenPhosphate
                        { "6aeff6a8-a8dc-4b3c-acf5-817ada864557", 0 }, // MagnesiumNitrate
                        { "10f35129-8b50-448d-98ed-8442cfc26985", 0 }, // BoricAcid
                        { "d190bcb4-1236-4bb4-95d6-584c719f4bde", 0 }, // SodiumMolybdate
                        { "337401e7-cbe4-48aa-968b-cceba18d6478", 0 }, // CalciumChloride
                        { "0f886cf2-729e-4d89-a1cf-08acb99d5ffe", 0 }, // SodiumSilicate
                        { "2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb", 0 } // SodiumSelenate
                    }
                },
                new()
                {
                    Name = "Fe",
                    LowerBound = 0.2,
                    UpperBound = 0.2,
                    Coefficients = new Dictionary<string, double>
                    {
                        { "a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb", 20.088 },
                        { "72f90e90-804c-4955-9e51-8e7b921836c5", 0 }, // CalciumNitrate
                        { "77cb9e8e-4e4c-454d-9411-e5d859f32781", 0 }, // PotassiumNitrate
                        { "2fc8a292-2095-42c0-bd50-9b6b355bf92a", 0 }, // AmmoniumNitrate
                        { "8389dbfb-792f-40d0-8fd9-701a506af48b", 0 }, // MagnesiumSulfate
                        { "9d0460fc-596f-4b70-8ebb-af217aecc097", 0 }, // PotassiumDihydrogenPhosphate
                        { "4fa956e7-114a-41e4-b6da-10643cdd8086", 0 }, // PotassiumSulfate
                        { "6aeff6a8-a8dc-4b3c-acf5-817ada864557", 0 }, // MagnesiumNitrate
                        { "9f0c6df5-6dc1-4113-b911-e50af9410248", 0 }, // CopperSulfate
                        { "31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43", 0 }, // ManganeseSulfate
                        { "39bb0466-fe87-4144-82c0-9aa460ac77c6", 0 }, // ZincSulfate
                        { "10f35129-8b50-448d-98ed-8442cfc26985", 0 }, // BoricAcid
                        { "d190bcb4-1236-4bb4-95d6-584c719f4bde", 0 }, // SodiumMolybdate
                        { "337401e7-cbe4-48aa-968b-cceba18d6478", 0 }, // CalciumChloride
                        { "0f886cf2-729e-4d89-a1cf-08acb99d5ffe", 0 }, // SodiumSilicate
                        { "2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb", 0 } // SodiumSelenate
                    }
                },
                new()
                {
                    Name = "Cu",
                    LowerBound = 0.005,
                    UpperBound = 0.005,
                    Coefficients = new Dictionary<string, double>
                    {
                        { "9f0c6df5-6dc1-4113-b911-e50af9410248", 25.451 },
                        { "72f90e90-804c-4955-9e51-8e7b921836c5", 0 }, // CalciumNitrate
                        { "77cb9e8e-4e4c-454d-9411-e5d859f32781", 0 }, // PotassiumNitrate
                        { "2fc8a292-2095-42c0-bd50-9b6b355bf92a", 0 }, // AmmoniumNitrate
                        { "8389dbfb-792f-40d0-8fd9-701a506af48b", 0 }, // MagnesiumSulfate
                        { "9d0460fc-596f-4b70-8ebb-af217aecc097", 0 }, // PotassiumDihydrogenPhosphate
                        { "4fa956e7-114a-41e4-b6da-10643cdd8086", 0 }, // PotassiumSulfate
                        { "6aeff6a8-a8dc-4b3c-acf5-817ada864557", 0 }, // MagnesiumNitrate
                        { "a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb", 0 }, // IronSulfate
                        { "31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43", 0 }, // ManganeseSulfate
                        { "39bb0466-fe87-4144-82c0-9aa460ac77c6", 0 }, // ZincSulfate
                        { "10f35129-8b50-448d-98ed-8442cfc26985", 0 }, // BoricAcid
                        { "d190bcb4-1236-4bb4-95d6-584c719f4bde", 0 }, // SodiumMolybdate
                        { "337401e7-cbe4-48aa-968b-cceba18d6478", 0 }, // CalciumChloride
                        { "0f886cf2-729e-4d89-a1cf-08acb99d5ffe", 0 }, // SodiumSilicate
                        { "2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb", 0 } // SodiumSelenate
                    }
                },
                new()
                {
                    Name = "Mn",
                    LowerBound = 0.055,
                    UpperBound = 0.055,
                    Coefficients = new Dictionary<string, double>
                    {
                        { "31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43", 32.506 },
                        { "72f90e90-804c-4955-9e51-8e7b921836c5", 0 }, // CalciumNitrate
                        { "77cb9e8e-4e4c-454d-9411-e5d859f32781", 0 }, // PotassiumNitrate
                        { "2fc8a292-2095-42c0-bd50-9b6b355bf92a", 0 }, // AmmoniumNitrate
                        { "8389dbfb-792f-40d0-8fd9-701a506af48b", 0 }, // MagnesiumSulfate
                        { "9d0460fc-596f-4b70-8ebb-af217aecc097", 0 }, // PotassiumDihydrogenPhosphate
                        { "4fa956e7-114a-41e4-b6da-10643cdd8086", 0 }, // PotassiumSulfate
                        { "6aeff6a8-a8dc-4b3c-acf5-817ada864557", 0 }, // MagnesiumNitrate
                        { "a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb", 0 }, // IronSulfate
                        { "9f0c6df5-6dc1-4113-b911-e50af9410248", 0 }, // CopperSulfate
                        { "39bb0466-fe87-4144-82c0-9aa460ac77c6", 0 }, // ZincSulfate
                        { "10f35129-8b50-448d-98ed-8442cfc26985", 0 }, // BoricAcid
                        { "d190bcb4-1236-4bb4-95d6-584c719f4bde", 0 }, // SodiumMolybdate
                        { "337401e7-cbe4-48aa-968b-cceba18d6478", 0 }, // CalciumChloride
                        { "0f886cf2-729e-4d89-a1cf-08acb99d5ffe", 0 }, // SodiumSilicate
                        { "2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb", 0 } // SodiumSelenate
                    }
                },
                new()
                {
                    Name = "Zn",
                    LowerBound = 0.033,
                    UpperBound = 0.033,
                    Coefficients = new Dictionary<string, double>
                    {
                        { "39bb0466-fe87-4144-82c0-9aa460ac77c6", 36.433 },
                        { "72f90e90-804c-4955-9e51-8e7b921836c5", 0 }, // CalciumNitrate
                        { "77cb9e8e-4e4c-454d-9411-e5d859f32781", 0 }, // PotassiumNitrate
                        { "2fc8a292-2095-42c0-bd50-9b6b355bf92a", 0 }, // AmmoniumNitrate
                        { "8389dbfb-792f-40d0-8fd9-701a506af48b", 0 }, // MagnesiumSulfate
                        { "9d0460fc-596f-4b70-8ebb-af217aecc097", 0 }, // PotassiumDihydrogenPhosphate
                        { "4fa956e7-114a-41e4-b6da-10643cdd8086", 0 }, // PotassiumSulfate
                        { "6aeff6a8-a8dc-4b3c-acf5-817ada864557", 0 }, // MagnesiumNitrate
                        { "a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb", 0 }, // IronSulfate
                        { "9f0c6df5-6dc1-4113-b911-e50af9410248", 0 }, // CopperSulfate
                        { "31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43", 0 }, // ManganeseSulfate
                        { "10f35129-8b50-448d-98ed-8442cfc26985", 0 }, // BoricAcid
                        { "d190bcb4-1236-4bb4-95d6-584c719f4bde", 0 }, // SodiumMolybdate
                        { "337401e7-cbe4-48aa-968b-cceba18d6478", 0 }, // CalciumChloride
                        { "0f886cf2-729e-4d89-a1cf-08acb99d5ffe", 0 }, // SodiumSilicate
                        { "2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb", 0 } // SodiumSelenate
                    }
                },
                new()
                {
                    Name = "B",
                    LowerBound = 0.028,
                    UpperBound = 0.028,
                    Coefficients = new Dictionary<string, double>
                    {
                        { "10f35129-8b50-448d-98ed-8442cfc26985", 17.483 },
                        { "72f90e90-804c-4955-9e51-8e7b921836c5", 0 }, // CalciumNitrate
                        { "77cb9e8e-4e4c-454d-9411-e5d859f32781", 0 }, // PotassiumNitrate
                        { "2fc8a292-2095-42c0-bd50-9b6b355bf92a", 0 }, // AmmoniumNitrate
                        { "8389dbfb-792f-40d0-8fd9-701a506af48b", 0 }, // MagnesiumSulfate
                        { "9d0460fc-596f-4b70-8ebb-af217aecc097", 0 }, // PotassiumDihydrogenPhosphate
                        { "4fa956e7-114a-41e4-b6da-10643cdd8086", 0 }, // PotassiumSulfate
                        { "6aeff6a8-a8dc-4b3c-acf5-817ada864557", 0 }, // MagnesiumNitrate
                        { "a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb", 0 }, // IronSulfate
                        { "9f0c6df5-6dc1-4113-b911-e50af9410248", 0 }, // CopperSulfate
                        { "31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43", 0 }, // ManganeseSulfate
                        { "39bb0466-fe87-4144-82c0-9aa460ac77c6", 0 }, // ZincSulfate
                        { "d190bcb4-1236-4bb4-95d6-584c719f4bde", 0 }, // SodiumMolybdate
                        { "337401e7-cbe4-48aa-968b-cceba18d6478", 0 }, // CalciumChloride
                        { "0f886cf2-729e-4d89-a1cf-08acb99d5ffe", 0 }, // SodiumSilicate
                        { "2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb", 0 } // SodiumSelenate
                    }
                },
                new()
                {
                    Name = "Mo",
                    LowerBound = 0.005,
                    UpperBound = 0.005,
                    Coefficients = new Dictionary<string, double>
                    {
                        { "d190bcb4-1236-4bb4-95d6-584c719f4bde", 39.656 },
                        { "72f90e90-804c-4955-9e51-8e7b921836c5", 0 }, // CalciumNitrate
                        { "77cb9e8e-4e4c-454d-9411-e5d859f32781", 0 }, // PotassiumNitrate
                        { "2fc8a292-2095-42c0-bd50-9b6b355bf92a", 0 }, // AmmoniumNitrate
                        { "8389dbfb-792f-40d0-8fd9-701a506af48b", 0 }, // MagnesiumSulfate
                        { "9d0460fc-596f-4b70-8ebb-af217aecc097", 0 }, // PotassiumDihydrogenPhosphate
                        { "4fa956e7-114a-41e4-b6da-10643cdd8086", 0 }, // PotassiumSulfate
                        { "6aeff6a8-a8dc-4b3c-acf5-817ada864557", 0 }, // MagnesiumNitrate
                        { "a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb", 0 }, // IronSulfate
                        { "9f0c6df5-6dc1-4113-b911-e50af9410248", 0 }, // CopperSulfate
                        { "31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43", 0 }, // ManganeseSulfate
                        { "39bb0466-fe87-4144-82c0-9aa460ac77c6", 0 }, // ZincSulfate
                        { "10f35129-8b50-448d-98ed-8442cfc26985", 0 }, // BoricAcid
                        { "337401e7-cbe4-48aa-968b-cceba18d6478", 0 }, // CalciumChloride
                        { "0f886cf2-729e-4d89-a1cf-08acb99d5ffe", 0 }, // SodiumSilicate
                        { "2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb", 0 } // SodiumSelenate
                    }
                },
                new()
                {
                    Name = "Cl",
                    LowerBound = 0.001,
                    UpperBound = 0.001,
                    Coefficients = new Dictionary<string, double>
                    {
                        { "337401e7-cbe4-48aa-968b-cceba18d6478", 32.364 },
                        { "72f90e90-804c-4955-9e51-8e7b921836c5", 0 }, // CalciumNitrate
                        { "77cb9e8e-4e4c-454d-9411-e5d859f32781", 0 }, // PotassiumNitrate
                        { "2fc8a292-2095-42c0-bd50-9b6b355bf92a", 0 }, // AmmoniumNitrate
                        { "8389dbfb-792f-40d0-8fd9-701a506af48b", 0 }, // MagnesiumSulfate
                        { "9d0460fc-596f-4b70-8ebb-af217aecc097", 0 }, // PotassiumDihydrogenPhosphate
                        { "4fa956e7-114a-41e4-b6da-10643cdd8086", 0 }, // PotassiumSulfate
                        { "6aeff6a8-a8dc-4b3c-acf5-817ada864557", 0 }, // MagnesiumNitrate
                        { "a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb", 0 }, // IronSulfate
                        { "9f0c6df5-6dc1-4113-b911-e50af9410248", 0 }, // CopperSulfate
                        { "31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43", 0 }, // ManganeseSulfate
                        { "39bb0466-fe87-4144-82c0-9aa460ac77c6", 0 }, // ZincSulfate
                        { "10f35129-8b50-448d-98ed-8442cfc26985", 0 }, // BoricAcid
                        { "d190bcb4-1236-4bb4-95d6-584c719f4bde", 0 }, // SodiumMolybdate
                        { "0f886cf2-729e-4d89-a1cf-08acb99d5ffe", 0 }, // SodiumSilicate
                        { "2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb", 0 } // SodiumSelenate
                    }
                },
                new()
                {
                    Name = "Si",
                    LowerBound = 0.001,
                    UpperBound = 0.001,
                    Coefficients = new Dictionary<string, double>
                    {
                        { "0f886cf2-729e-4d89-a1cf-08acb99d5ffe", 23.009 },
                        { "72f90e90-804c-4955-9e51-8e7b921836c5", 0 }, // CalciumNitrate
                        { "77cb9e8e-4e4c-454d-9411-e5d859f32781", 0 }, // PotassiumNitrate
                        { "2fc8a292-2095-42c0-bd50-9b6b355bf92a", 0 }, // AmmoniumNitrate
                        { "8389dbfb-792f-40d0-8fd9-701a506af48b", 0 }, // MagnesiumSulfate
                        { "9d0460fc-596f-4b70-8ebb-af217aecc097", 0 }, // PotassiumDihydrogenPhosphate
                        { "4fa956e7-114a-41e4-b6da-10643cdd8086", 0 }, // PotassiumSulfate
                        { "6aeff6a8-a8dc-4b3c-acf5-817ada864557", 0 }, // MagnesiumNitrate
                        { "a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb", 0 }, // IronSulfate
                        { "9f0c6df5-6dc1-4113-b911-e50af9410248", 0 }, // CopperSulfate
                        { "31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43", 0 }, // ManganeseSulfate
                        { "39bb0466-fe87-4144-82c0-9aa460ac77c6", 0 }, // ZincSulfate
                        { "10f35129-8b50-448d-98ed-8442cfc26985", 0 }, // BoricAcid
                        { "d190bcb4-1236-4bb4-95d6-584c719f4bde", 0 }, // SodiumMolybdate
                        { "337401e7-cbe4-48aa-968b-cceba18d6478", 0 }, // CalciumChloride
                        { "2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb", 0 } // SodiumSelenate
                    }
                },
                new()
                {
                    Name = "Se",
                    LowerBound = 0.001,
                    UpperBound = 0.001,
                    Coefficients = new Dictionary<string, double>
                    {
                        { "2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb", 41.795 },
                        { "72f90e90-804c-4955-9e51-8e7b921836c5", 0 }, // CalciumNitrate
                        { "77cb9e8e-4e4c-454d-9411-e5d859f32781", 0 }, // PotassiumNitrate∂
                        { "2fc8a292-2095-42c0-bd50-9b6b355bf92a", 0 }, // AmmoniumNitrate
                        { "8389dbfb-792f-40d0-8fd9-701a506af48b", 0 }, // MagnesiumSulfate
                        { "9d0460fc-596f-4b70-8ebb-af217aecc097", 0 }, // PotassiumDihydrogenPhosphate
                        { "4fa956e7-114a-41e4-b6da-10643cdd8086", 0 }, // PotassiumSulfate
                        { "6aeff6a8-a8dc-4b3c-acf5-817ada864557", 0 }, // MagnesiumNitrate
                        { "a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb", 0 }, // IronSulfate
                        { "9f0c6df5-6dc1-4113-b911-e50af9410248", 0 }, // CopperSulfate
                        { "31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43", 0 }, // ManganeseSulfate
                        { "39bb0466-fe87-4144-82c0-9aa460ac77c6", 0 }, // ZincSulfate
                        { "10f35129-8b50-448d-98ed-8442cfc26985", 0 }, // BoricAcid
                        { "d190bcb4-1236-4bb4-95d6-584c719f4bde", 0 }, // SodiumMolybdate
                        { "337401e7-cbe4-48aa-968b-cceba18d6478", 0 }, // CalciumChloride
                        { "0f886cf2-729e-4d89-a1cf-08acb99d5ffe", 0 }, // SodiumSilicate
                    }
                }
            }
        };

        // Act
        OptimizationProblem optimizationProblem = Mapper.CreateOptimizationProblem(target, sourceCollection, settings);

        // Assert
        double tolerance = 0.0001;

        Assert.NotNull(optimizationProblem);
        Assert.Equal(expectedProblem.Variables.Count, optimizationProblem.Variables.Count);

        foreach (string key in expectedProblem.Variables.Keys)
        {
            Assert.True(optimizationProblem.Variables.ContainsKey(key));
            double expectedValue = expectedProblem.Variables[key];
            double actualValue = optimizationProblem.Variables[key];
            double lowerBound = expectedValue * (1 - tolerance);
            double upperBound = expectedValue * (1 + tolerance);
            Assert.InRange(actualValue, lowerBound, upperBound);
        }

        Assert.Equal(expectedProblem.Objective.IsMinimization, optimizationProblem.Objective.IsMinimization);
        Assert.Equal(expectedProblem.Objective.Coefficients.Count, optimizationProblem.Objective.Coefficients.Count);
        foreach (string key in expectedProblem.Objective.Coefficients.Keys)
        {
            Assert.True(optimizationProblem.Objective.Coefficients.ContainsKey(key));
            double expectedValue = expectedProblem.Objective.Coefficients[key];
            double actualValue = optimizationProblem.Objective.Coefficients[key];
            double lowerBound = expectedValue * (1 - tolerance);
            double upperBound = expectedValue * (1 + tolerance);
            Assert.InRange(actualValue, lowerBound, upperBound);
        }

        Assert.Equal(expectedProblem.Constraints.Count, optimizationProblem.Constraints.Count);
        for (int i = 0; i < expectedProblem.Constraints.Count; i++)
        {
            OptimizationProblem.OptimizationConstraint expectedConstraint = expectedProblem.Constraints[i];
            OptimizationProblem.OptimizationConstraint actualConstraint =
                optimizationProblem.Constraints.FirstOrDefault(c => c.Name == expectedConstraint.Name);
            Assert.NotNull(actualConstraint);

            Assert.InRange(actualConstraint.LowerBound, expectedConstraint.LowerBound * (1 - tolerance),
                expectedConstraint.LowerBound * (1 + tolerance));
            Assert.InRange(actualConstraint.UpperBound, expectedConstraint.UpperBound * (1 - tolerance),
                expectedConstraint.UpperBound * (1 + tolerance));

            Assert.Equal(expectedConstraint.Coefficients.Count, actualConstraint.Coefficients.Count);

            foreach (string key in expectedConstraint.Coefficients.Keys)
            {
                Assert.True(actualConstraint.Coefficients.ContainsKey(key), $"Key missing: {key}");
                double expectedValue = expectedConstraint.Coefficients[key];
                double actualValue = actualConstraint.Coefficients[key];
                double lowerBound = expectedValue * (1 - tolerance);
                double upperBound = expectedValue * (1 + tolerance);
                Assert.InRange(actualValue, lowerBound, upperBound);
            }
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(0.5)]
    [Trait("Category", "Unit")]
    public void Map_ShouldCorrectlyCalculateSolution_WithGivenWaterVolume(double waterLiters)
    {
        //Arrange
        Dictionary<string, double> solution = new Dictionary<string, double>
        {
            { "72f90e90-804c-4955-9e51-8e7b921836c5", 0.58917244351919262 },
            { "77cb9e8e-4e4c-454d-9411-e5d859f32781", 0 },
            { "2fc8a292-2095-42c0-bd50-9b6b355bf92a", 0.13668047684777451 },
            { "8389dbfb-792f-40d0-8fd9-701a506af48b", 0.32451331106241954 },
            { "9d0460fc-596f-4b70-8ebb-af217aecc097", 0.21967400377839288 },
            { "4fa956e7-114a-41e4-b6da-10643cdd8086", 0.30504403880739389 },
            { "6aeff6a8-a8dc-4b3c-acf5-817ada864557", 0.29538709142456809 },
            { "a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb", 0.0099561927518916765 },
            { "9f0c6df5-6dc1-4113-b911-e50af9410248", 0.00019645593493379434 },
            { "31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43", 0.0016919953239401958 },
            { "39bb0466-fe87-4144-82c0-9aa460ac77c6", 0.00090577223945324293 },
            { "10f35129-8b50-448d-98ed-8442cfc26985", 0.0016015557970600013 },
            { "d190bcb4-1236-4bb4-95d6-584c719f4bde", 0.00012608432519669156 },
            { "337401e7-cbe4-48aa-968b-cceba18d6478", 3.0898529230008654E-05 },
            { "0f886cf2-729e-4d89-a1cf-08acb99d5ffe", 4.3461254291798863E-05 },
            { "2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb", 2.3926306974518482E-05 }
        };

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

        Solution expectedMapResult = new Solution
        {
            new FertilizerBuilder()
                .AddId(Guid.Parse("72f90e90-804c-4955-9e51-8e7b921836c5")) //CalciumNitrate
                .AddWeight(0.589 * waterLiters)
                .AddNo3(11.863)
                .AddCaNonChelated(16.972)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("2fc8a292-2095-42c0-bd50-9b6b355bf92a")) //AmmoniumNitrate 
                .AddWeight(0.137 * waterLiters)
                .AddNo3(17.499)
                .AddNh4(17.499)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("8389dbfb-792f-40d0-8fd9-701a506af48b")) //MagnesiumSulfate 
                .AddWeight(0.325 * waterLiters)
                .AddMgNonChelated(9.861)
                .AddS(13.008)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("9d0460fc-596f-4b70-8ebb-af217aecc097")) //PotassiumDihydrogenPhosphate 
                .AddWeight(0.22 * waterLiters)
                .AddP(22.761)
                .AddK(28.731)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("4fa956e7-114a-41e4-b6da-10643cdd8086")) //PotassiumSulfat
                .AddWeight(0.305 * waterLiters)
                .AddS(18.401)
                .AddK(44.874)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("6aeff6a8-a8dc-4b3c-acf5-817ada864557")) // MagnesiumNitrate
                .AddWeight(0.295 * waterLiters)
                .AddMgNonChelated(9.479)
                .AddNo3(10.925)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("a3ce72b5-8496-48d6-a8eb-4df3f6ca01fb")) // IronSulfate
                .AddFeNonChelated(20.088)
                .AddS(11.532)
                .AddWeight(0.01 * waterLiters)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("9f0c6df5-6dc1-4113-b911-e50af9410248")) //CopperSulfate
                .AddS(12.841)
                .AddCuNonChelated(25.451)
                .AddWeight(0.0002 * waterLiters)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("31c557f5-ced3-4b5a-ab9f-b1b8aa34dc43")) //ManganeseSulfate
                .AddS(18.969)
                .AddMnNonChelated(32.506)
                .AddWeight(0.0017 * waterLiters)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("39bb0466-fe87-4144-82c0-9aa460ac77c6")) //ZincSulfate
                .AddS(17.866)
                .AddZnNonChelated(36.433)
                .AddWeight(0.0009 * waterLiters)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("10f35129-8b50-448d-98ed-8442cfc26985")) //BoricAcid
                .AddB(17.483)
                .AddWeight(0.0016 * waterLiters)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("d190bcb4-1236-4bb4-95d6-584c719f4bde")) //SodiumMolybdate
                .AddNa(19.003)
                .AddMo(39.656)
                .AddWeight(0.000126 * waterLiters)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("337401e7-cbe4-48aa-968b-cceba18d6478")) //CalciumChloride
                .AddCaNonChelated(18.295)
                .AddCl(32.364)
                .AddWeight(3.09E-05 * waterLiters)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("0f886cf2-729e-4d89-a1cf-08acb99d5ffe")) //SodiumSilicate
                .AddSi(23.009)
                .AddNa(37.669)
                .AddWeight(4.346E-05 * waterLiters)
                .Build(),

            new FertilizerBuilder()
                .AddId(Guid.Parse("2e7b59c1-b44b-4e82-a408-a6cb7d9db2cb")) //SodiumSelenate 
                .AddSe(41.795)
                .AddNa(24.335)
                .AddWeight(2.393E-05 * waterLiters)
                .Build(),
        };

        expectedMapResult.WaterLiters = waterLiters;

        //Act
        Solution result = Mapper.CreateSolution(solution, sourceCollection, waterLiters);
        //Assert

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMapResult.Count, result.Count);
        Assert.Equal(expectedMapResult.WaterLiters, waterLiters);

        double tolerancePercent = 0.1; // 1% допуск

        foreach (Fertilizer expected in expectedMapResult)
        {
            Fertilizer actual = result.FirstOrDefault(r => r.RefId == expected.RefId);
            Assert.NotNull(actual);

            Assert.InRange(actual.Weight.Value, expected.Weight.Value * (1 - tolerancePercent),
                expected.Weight.Value * (1 + tolerancePercent));
            Assert.InRange(actual.Nitrogen.Value, expected.Nitrogen.Value * (1 - tolerancePercent),
                expected.Nitrogen.Value * (1 + tolerancePercent));
            Assert.InRange(actual.Calcium.Value, expected.Calcium.Value * (1 - tolerancePercent),
                expected.Calcium.Value * (1 + tolerancePercent));
            Assert.InRange(actual.Potassium.Value, expected.Potassium.Value * (1 - tolerancePercent),
                expected.Potassium.Value * (1 + tolerancePercent));
            Assert.InRange(actual.Phosphorus.Value, expected.Phosphorus.Value * (1 - tolerancePercent),
                expected.Phosphorus.Value * (1 + tolerancePercent));
            Assert.InRange(actual.Magnesium.Value, expected.Magnesium.Value * (1 - tolerancePercent),
                expected.Magnesium.Value * (1 + tolerancePercent));
            Assert.InRange(actual.Sulfur.Value, expected.Sulfur.Value * (1 - tolerancePercent),
                expected.Sulfur.Value * (1 + tolerancePercent));
            Assert.InRange(actual.Iron.Value, expected.Iron.Value * (1 - tolerancePercent),
                expected.Iron.Value * (1 + tolerancePercent));
            Assert.InRange(actual.Copper.Value, expected.Copper.Value * (1 - tolerancePercent),
                expected.Copper.Value * (1 + tolerancePercent));
            Assert.InRange(actual.Manganese.Value, expected.Manganese.Value * (1 - tolerancePercent),
                expected.Manganese.Value * (1 + tolerancePercent));
            Assert.InRange(actual.Zinc.Value, expected.Zinc.Value * (1 - tolerancePercent),
                expected.Zinc.Value * (1 + tolerancePercent));
            Assert.InRange(actual.Boron.Value, expected.Boron.Value * (1 - tolerancePercent),
                expected.Boron.Value * (1 + tolerancePercent));
            Assert.InRange(actual.Molybdenum.Value, expected.Molybdenum.Value * (1 - tolerancePercent),
                expected.Molybdenum.Value * (1 + tolerancePercent));
            Assert.InRange(actual.Sodium.Value, expected.Sodium.Value * (1 - tolerancePercent),
                expected.Sodium.Value * (1 + tolerancePercent));
            Assert.InRange(actual.Chlorine.Value, expected.Chlorine.Value * (1 - tolerancePercent),
                expected.Chlorine.Value * (1 + tolerancePercent));
            Assert.InRange(actual.Silicon.Value, expected.Silicon.Value * (1 - tolerancePercent),
                expected.Silicon.Value * (1 + tolerancePercent));
            Assert.InRange(actual.Selenium.Value, expected.Selenium.Value * (1 - tolerancePercent),
                expected.Selenium.Value * (1 + tolerancePercent));
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void CreateOptimizationProblem_WithDuplicateFertilizerIds_ThrowsInvalidOperationException()
    {
        // Arrange
        IList<FertilizerOptimizationModel> sourceCollection = new List<FertilizerOptimizationModel>()
        {
            new FertilizerBuilder()
                .AddId(Guid.Parse("72f90e90-804c-4955-9e51-8e7b921836c5"))
                .Build(),
            new FertilizerBuilder()
                .AddId(Guid.Parse("72f90e90-804c-4955-9e51-8e7b921836c5")) // Duplicate ID
                .Build()
        };

        PpmTarget target = new PpmTargetBuilder().Build();
        SolutionFinderSettings settings = new SolutionFinderSettingsBuilder().Build();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            Mapper.CreateOptimizationProblem(target, sourceCollection, settings));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void MapSolution_WithNegativeWeights_ThrowsArgumentException()
    {
        // Arrange
        Dictionary<string, double> solution = new Dictionary<string, double>
        {
            { "72f90e90-804c-4955-9e51-8e7b921836c5", -0.1 }
        };

        IList<FertilizerOptimizationModel> sourceCollection = new List<FertilizerOptimizationModel>()
        {
            new FertilizerBuilder()
                .AddId(Guid.Parse("72f90e90-804c-4955-9e51-8e7b921836c5"))
                .Build()
        };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Mapper.CreateSolution(solution, sourceCollection));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void MapSolution_WithUnmatchedFertilizerId_ThrowsInvalidOperationException()
    {
        // Arrange
        Dictionary<string, double> solution = new Dictionary<string, double>
        {
            { "72f90e90-804c-4955-9e51-8e7b921836c5", 0.5 }
        };

        IList<FertilizerOptimizationModel> sourceCollection = new List<FertilizerOptimizationModel>();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Mapper.CreateSolution(solution, sourceCollection));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void CreateOptimizationProblem_WithDuplicateFertilizerAttributes_ThrowsInvalidOperationException()
    {
        // Arrange
        IList<FertilizerOptimizationModel> sourceCollection = new List<FertilizerOptimizationModel>()
        {
            new FertilizerBuilder().AddId(Guid.NewGuid()).AddNo3(11).AddCaNonChelated(16).Build(),
            new FertilizerBuilder().AddId(Guid.NewGuid()).AddNo3(11).AddCaNonChelated(16).Build()
        };

        PpmTarget target = new PpmTargetBuilder().Build();
        SolutionFinderSettings settings = new SolutionFinderSettingsBuilder().Build();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            Mapper.CreateOptimizationProblem(target, sourceCollection, settings));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void CreateOptimizationProblem_IncludesSodiumInCalculations()
    {
        // Arrange
        List<FertilizerOptimizationModel> sourceCollection = new List<FertilizerOptimizationModel>
        {
            new FertilizerBuilder()
                .AddId(Guid.NewGuid())
                .AddNa(5.0)
                .Build()
        };

        PpmTarget target = new PpmTargetBuilder()
            .AddNa(10)
            .Build();

        SolutionFinderSettings settings = new SolutionFinderSettingsBuilder()
            .AddNa(0.1)
            .Build();

        // Act
        OptimizationProblem problem = Mapper.CreateOptimizationProblem(target, sourceCollection, settings);

        // Assert
        Assert.Contains("Na", problem.Constraints.Select(c => c.Name));
        Assert.NotEmpty(problem.Constraints.Where(c => c.Name == "Na").SelectMany(c => c.Coefficients));
    }

    [Fact]
    public void CreateSolution_NullSolutionValues_ThrowsArgumentNullException()
    {
        // Arrange
        OptimizationProblemMapper mapper = new OptimizationProblemMapper();
        Dictionary<string, double> nullSolutionValues = null;
        IList<FertilizerOptimizationModel> sourceCollection = new List<FertilizerOptimizationModel>
            { new FertilizerOptimizationModel() };
        double waterLiters = 1.0;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            mapper.CreateSolution(nullSolutionValues, sourceCollection, waterLiters));
    }

    [Fact]
    public void CreateSolution_EmptySolutionValues_ThrowsArgumentException()
    {
        // Arrange
        OptimizationProblemMapper mapper = new OptimizationProblemMapper();
        Dictionary<string, double> emptySolutionValues = new Dictionary<string, double>();
        IList<FertilizerOptimizationModel> sourceCollection = new List<FertilizerOptimizationModel>
            { new FertilizerOptimizationModel() };
        double waterLiters = 1.0;

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => mapper.CreateSolution(emptySolutionValues, sourceCollection, waterLiters));
    }

    [Fact]
    public void CreateSolution_NullSourceCollection_ThrowsArgumentNullException()
    {
        // Arrange
        OptimizationProblemMapper mapper = new OptimizationProblemMapper();
        Dictionary<string, double> solutionValues = new Dictionary<string, double> { { "key", 1.0 } };
        IList<FertilizerOptimizationModel> nullSourceCollection = null;
        double waterLiters = 1.0;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            mapper.CreateSolution(solutionValues, nullSourceCollection, waterLiters));
    }

    [Fact]
    public void CreateSolution_EmptySourceCollection_ThrowsArgumentException()
    {
        // Arrange
        OptimizationProblemMapper mapper = new OptimizationProblemMapper();
        Dictionary<string, double> solutionValues = new Dictionary<string, double> { { "key", 1.0 } };
        IList<FertilizerOptimizationModel> emptySourceCollection = new List<FertilizerOptimizationModel>();
        double waterLiters = 1.0;

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => mapper.CreateSolution(solutionValues, emptySourceCollection, waterLiters));
    }

    // Additional test for checking non-positive waterLiters
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateSolution_NonPositiveWaterLiters_ThrowsArgumentException(double waterLiters)
    {
        // Arrange
        OptimizationProblemMapper mapper = new OptimizationProblemMapper();
        Dictionary<string, double> solutionValues = new Dictionary<string, double> { { "key", 1.0 } };
        IList<FertilizerOptimizationModel> sourceCollection = new List<FertilizerOptimizationModel>
            { new FertilizerOptimizationModel() };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => mapper.CreateSolution(solutionValues, sourceCollection, waterLiters));
    }
}