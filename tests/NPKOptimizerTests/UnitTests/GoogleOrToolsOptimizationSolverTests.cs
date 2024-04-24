using FluentAssertions;
using NPKOptimizer.Components;
using NPKOptimizer.Contracts;
using Xunit;

namespace NPKOptimizer.Tests.UnitTests;

public class GoogleOrToolsOptimizationSolverTests
{
    protected IOptimizationProblemSolver Solver;

    public GoogleOrToolsOptimizationSolverTests()
    {
        Solver = new GoogleOrToolsOptimizationSolver();   
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_ValidData_ReturnsOptimalSolution()
    {
        // Arrange
        OptimizationProblem problem = new OptimizationProblem
        {
            Variables = new Dictionary<string, double>
            {
                { "x", 0 },
                { "y", 0 }
            },
            Objective = new OptimizationProblem.OptimizationObjective
            {
                Coefficients = new Dictionary<string, double>
                {
                    { "x", 1 },
                    { "y", 1 }
                },
                IsMinimization = true
            },
            Constraints = new List<OptimizationProblem.OptimizationConstraint>
            {
                new ()
                {
                    Name = "c1",
                    Coefficients = new Dictionary<string, double>
                    {
                        { "x", 1 },
                        { "y", 2 }
                    },
                    LowerBound = 1,
                    UpperBound = 10
                },
                new()
                {
                    Name = "c2",
                    Coefficients = new Dictionary<string, double>
                    {
                        { "x", 2 },
                        { "y", 1 }
                    },
                    LowerBound = 1,
                    UpperBound = 20
                }
            }
        };

        // Act
        Dictionary<string, double> result = Solver.Solve(problem);

        // Assert
        result.Should().NotBeNull();
        result.Should().ContainKeys("x", "y");
        result["x"].Should().BePositive();
        result["y"].Should().BePositive();
        result["x"].Should().BeApproximately(0.333, 0.01);
        result["y"].Should().BeApproximately(0.333, 0.01);
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_SolverFailsToFindOptimal_ThrowsInvalidOperationException()
    {
        // Arrange
        OptimizationProblem problem = new OptimizationProblem
        {
            Variables = new Dictionary<string, double>
            {
                { "x", 0 },
                { "y", 0 }
            },
            Objective = new OptimizationProblem.OptimizationObjective
            {
                Coefficients = new Dictionary<string, double>
                {
                    { "x", 1 },
                    { "y", 1 }
                },
                IsMinimization = true
            },
            Constraints = new List<OptimizationProblem.OptimizationConstraint>
            {
                new()
                {
                    Name = "c1",
                    Coefficients = new Dictionary<string, double>
                    {
                        { "x", 1 },
                        { "y", 1 }
                    },
                    LowerBound = 100,  
                    UpperBound = double.PositiveInfinity
                },
                new ()
                {
                    Name = "c2",
                    Coefficients = new Dictionary<string, double>
                    {
                        { "x", 1 },
                        { "y", 1 }
                    },
                    LowerBound = 0,
                    UpperBound = 10 
                }
            }
        };

        // Act & Assert
        Action act = () => Solver.Solve(problem);
        act.Should().Throw<InvalidOperationException>().WithMessage(NPKOptimizer.Const.OptimizationSettings.SolverOptimalSolutionNotFoundMessage);
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_OptimizationProblemWithMicroAndMacronutrients_ReturnsValidSolution()
    {
        // Arrange
        OptimizationProblem problem = new OptimizationProblem
        {
            Variables = new Dictionary<string, double> 
            {
                {"CalciumNitrate", 0},
                {"AmmoniumNitrate", 0},
                {"PotassiumNitrate", 0},
                {"MagnesiumSulfate", 0},
                {"PotassiumDihydrogenPhosphate", 0},
                {"PotassiumSulfate", 0},
                {"MagnesiumNitrate", 0},
                {"IronSulfate", 0},
                {"CopperSulfate", 0},
                {"ManganeseSulfate", 0},
                {"ZincSulfate", 0},
                {"BoricAcid", 0},
                {"SodiumMolybdate", 0},
                {"CalciumChloride", 0},
                {"SodiumSilicate", 0},
                {"SodiumSelenate", 0}
            },
            Objective = new OptimizationProblem.OptimizationObjective
            {
                Coefficients = new Dictionary<string, double>
                {
                    {"CalciumNitrate", 1},
                    {"AmmoniumNitrate", 1},
                    {"PotassiumNitrate", 1},
                    {"MagnesiumSulfate", 1},
                    {"PotassiumDihydrogenPhosphate", 1},
                    {"PotassiumSulfate", 1},
                    {"MagnesiumNitrate", 1},
                    {"IronSulfate", 1},
                    {"CopperSulfate", 1},
                    {"ManganeseSulfate", 1},
                    {"ZincSulfate", 1},
                    {"BoricAcid", 1},
                    {"SodiumMolybdate", 1},
                    {"CalciumChloride", 1},
                    {"SodiumSilicate", 1},
                    {"SodiumSelenate", 1}
                },
                IsMinimization = true
            },
            Constraints = new List<OptimizationProblem.OptimizationConstraint>
            {
                new ()
                {
                    Name = "N",
                    LowerBound = 15,
                    UpperBound = 15,
                    Coefficients = new Dictionary<string, double>
                    {
                        {"CalciumNitrate", 11.863},
                        {"PotassiumNitrate", 13.854},
                        {"AmmoniumNitrate", 34.998},
                        {"MagnesiumNitrate", 10.925}
                        
                    }
                },
                new ()
                {
                    Name = "P",
                    LowerBound = 5,
                    UpperBound = 5,
                    Coefficients = new Dictionary<string, double>
                    {
                        {"PotassiumDihydrogenPhosphate", 22.761}
                    }
                },
                new ()
                {
                    Name = "K",
                    LowerBound = 20,
                    UpperBound = 20,
                    Coefficients = new Dictionary<string, double>
                    {
                        {"PotassiumNitrate", 38.762},
                        {"PotassiumDihydrogenPhosphate", 28.731},
                        {"PotassiumSulfate", 44.874}
                    }
                },
                
                new ()
                {
                    Name = "Ca",
                    LowerBound = 10,
                    UpperBound = 10,
                    Coefficients = new Dictionary<string, double>
                    {
                        {"CalciumNitrate", 16.972},
                        {"CalciumChloride", 18.295}
                    }
                },
                new ()
                {
                    Name = "Mg",
                    LowerBound = 6,
                    UpperBound = 6,
                    Coefficients = new Dictionary<string, double>
                    {
                        {"MagnesiumSulfate", 9.861},
                        {"MagnesiumNitrate", 9.479}
                    }
                },
                new ()
                {
                    Name = "S",
                    LowerBound = 10,
                    UpperBound = 10,
                    Coefficients = new Dictionary<string, double>
                    {
                        {"MagnesiumSulfate", 13.008},
                        {"PotassiumSulfate", 18.401},
                        {"IronSulfate", 11.532},
                        {"CopperSulfate", 12.841},
                        {"ManganeseSulfate", 18.969},
                        {"ZincSulfate", 17.866}
                    }
                },
                new ()
                {
                    Name = "Fe",
                    LowerBound = 0.2,
                    UpperBound = 0.2,
                    Coefficients = new Dictionary<string, double>
                    {
                        {"IronSulfate", 20.088}
                    }
                },
                new ()
                {
                    Name = "Cu",
                    LowerBound = 0.005,
                    UpperBound = 0.005,
                    Coefficients = new Dictionary<string, double>
                    {
                        {"CopperSulfate", 25.451}
                    }
                },
                new ()
                {
                    Name = "Mn",
                    LowerBound = 0.055,
                    UpperBound = 0.055,
                    Coefficients = new Dictionary<string, double>
                    {
                        {"ManganeseSulfate", 32.506}
                    }
                },
                new ()
                {
                    Name = "Zn",
                    LowerBound = 0.033,
                    UpperBound = 0.033,
                    Coefficients = new Dictionary<string, double>
                    {
                        {"ZincSulfate", 36.433}
                    }
                },
                new ()
                {
                    Name = "B",
                    LowerBound = 0.028,
                    UpperBound = 0.028,
                    Coefficients = new Dictionary<string, double>
                    {
                        {"BoricAcid", 17.483}
                    }
                },
                new ()
                {
                    Name = "Mo",
                    LowerBound = 0.005,
                    UpperBound = 0.005,
                    Coefficients = new Dictionary<string, double>
                    {
                        {"SodiumMolybdate", 39.656}
                    }
                },
                new ()
                {
                    Name = "Cl",
                    LowerBound = 0.001,
                    UpperBound = 0.001,
                    Coefficients = new Dictionary<string, double>
                    {
                        {"CalciumChloride", 32.364}
                    }
                },
                new ()
                {
                    Name = "Si",
                    LowerBound = 0.001,
                    UpperBound = 0.001,
                    Coefficients = new Dictionary<string, double>
                    {
                        {"SodiumSilicate", 23.009}
                    }
                },
                new ()
                {
                    Name = "Se",
                    LowerBound = 0.001,
                    UpperBound = 0.001,
                    Coefficients = new Dictionary<string, double>
                    {
                        {"SodiumSelenate", 41.795}
                    }
                }
            }
        };

        Dictionary<string, double> expectedSolution = new Dictionary<string, double>
        {
            {"CalciumNitrate", 0.58917244351919262},
            {"AmmoniumNitrate", 0.13668047684777451},
            {"MagnesiumSulfate", 0.32451331106241954},
            {"PotassiumDihydrogenPhosphate", 0.21967400377839288},
            {"PotassiumSulfate", 0.30504403880739389},
            {"MagnesiumNitrate", 0.29538709142456809},
            {"IronSulfate", 0.0099561927518916765},
            {"CopperSulfate", 0.00019645593493379434},
            {"ManganeseSulfate", 0.0016919953239401958},
            {"ZincSulfate", 0.00090577223945324293},
            {"BoricAcid", 0.0016015557970600013},
            {"SodiumMolybdate", 0.00012608432519669156},
            {"CalciumChloride", 3.0898529230008654E-05},
            {"SodiumSilicate", 4.3461254291798863E-05},
            {"SodiumSelenate", 2.3926306974518482E-05}
        };


        // Act
        Dictionary<string, double> result = Solver.Solve(problem);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(16);
        foreach (KeyValuePair<string, double> variable in problem.Variables)
        {
            result[variable.Key].Should().BeGreaterOrEqualTo(variable.Value);
        }

        foreach (KeyValuePair<string, double> expected in expectedSolution)
        {
            result[expected.Key].Should().BeApproximately(expected.Value, 0.001, $"{expected.Key} should match the expected solution within a tolerance.");
        }
    }
    [Fact]
    [Trait("Category", "Unit")]
    public void Solve_WithMaximizationObjective_ReturnsMaximizedSolution()
    {
        // Arrange
        OptimizationProblem problem = new OptimizationProblem
        {
            Variables = new Dictionary<string, double>
            {
                { "x", 0 },
                { "y", 0 }
            },
            Objective = new OptimizationProblem.OptimizationObjective
            {
                Coefficients = new Dictionary<string, double>
                {
                    { "x", 1 },
                    { "y", 1 }
                },
                IsMinimization = false  
            },
            Constraints = new List<OptimizationProblem.OptimizationConstraint>
            {
                new ()
                {
                    Name = "c1",
                    Coefficients = new Dictionary<string, double>
                    {
                        { "x", 1 },
                        { "y", 1 }
                    },
                    LowerBound = 0,
                    UpperBound = 10
                },
                new ()
                {
                    Name = "c2",
                    Coefficients = new Dictionary<string, double>
                    {
                        { "x", 1 },
                        { "y", -1 }
                    },
                    LowerBound = -5,
                    UpperBound = 5
                }
            }
        };

        // Act
        Dictionary<string, double> result = Solver.Solve(problem);

        // Assert
        result.Should().NotBeNull();
        result.Should().ContainKeys("x", "y");
        result["x"].Should().BeApproximately(2.5, 0.01);
        result["y"].Should().BeApproximately(7.5, 0.01);
    }

}
