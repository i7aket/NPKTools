using SYT.NPKTools.Fertilizers;
using SYT.NPKTools.Nutrients;
using SYT.NPKTools.Optimization;
using SYT.NPKTools.OrToolsOracle;
using Xunit;

namespace SYT.NPKTools.OrTools.Tests;

/// <summary>
/// Establishes that the managed <see cref="SimplexOptimizationSolver"/> is interchangeable with the
/// native <see cref="GoogleOrToolsOptimizationSolver"/> on the real preset problems.
/// </summary>
/// <remarks>
/// This matters beyond tidiness: OR-Tools ships native binaries only for linux-x64/arm64,
/// osx-x64/arm64 and win-x64, so it cannot run in WebAssembly. The managed solver is what makes a
/// browser-side calculator possible, and these tests are the evidence that it produces the same
/// answers.
/// <para>
/// Two linear programs with the same optimum can report different vertices when the optimum is
/// degenerate, so the assertions compare what is actually well defined: the feasibility verdict, the
/// optimal objective value, and whether the resulting mix hits the nutrient target.
/// </para>
/// </remarks>
public class SolverEquivalenceTests
{
    private static readonly OptimizationProblemMapper Mapper = new();
    private static readonly GoogleOrToolsOptimizationSolver OrTools = new();
    private static readonly SimplexOptimizationSolver Simplex = new();
    private static readonly FertilizerBundleRepository Bundles = new();
    private static readonly PpmCalculationService Calculator = new();

    private static readonly SolutionFinderSettings MacroSettings = new SolutionFinderSettingsBuilder()
        .AddN(1).AddP(1).AddK(1).AddCa(1).AddMg(1).AddS(1).AddCl(1)
        .Build();

    private static readonly SolutionFinderSettings MicroSettings = new SolutionFinderSettingsBuilder()
        .AddFe(1).AddCu(1).AddMn(1).AddZn(1).AddB(1).AddMo(1).AddSi(1).AddSe(1)
        .Build();

    public static TheoryData<string> MacroTargets => new()
    {
        "N=150 P=50 K=200 Ca=100 Mg=50 S=60",
        "N=100 P=40 K=150 Ca=80 Mg=40 S=50",
        "N=200 P=60 K=250 Ca=120 Mg=60 S=80",
        "N=1",
        "N=150 P=50 K=200",
        "N=9999 P=9999 K=9999 Ca=9999 Mg=9999 S=9999",

        // Infeasible on purpose: at precision 1 every non-targeted element is pinned to zero, and
        // this catalogue has no potassium source without a counter-ion (nitrate, phosphate, sulfate
        // or chloride). Both solvers must agree there is nothing to find.
        "K=200"
    };

    public static TheoryData<string> MicroTargets => new()
    {
        "Fe=2 Cu=0.05 Mn=0.55 Zn=0.33 B=0.28 Mo=0.05",
        "Fe=3 Mn=1 Zn=0.5 B=0.5",
        "Cu=1",
        "Fe=1 Cu=1 Mn=1 Zn=1 B=1 Mo=1 Si=1 Se=1"
    };

    /// <summary>
    /// The subset of <see cref="MacroTargets"/> the preset catalogue can actually satisfy.
    /// </summary>
    public static TheoryData<string> FeasibleMacroTargets => new()
    {
        "N=150 P=50 K=200 Ca=100 Mg=50 S=60",
        "N=100 P=40 K=150 Ca=80 Mg=40 S=50",
        "N=200 P=60 K=250 Ca=120 Mg=60 S=80",
        "N=1",
        "N=150 P=50 K=200"
    };

    [Theory]
    [MemberData(nameof(MacroTargets))]
    [Trait("Category", "Integration")]
    public void MacroBundles_BothSolvers_AgreeOnFeasibilityAndCost(string targetSpec)
    {
        AssertEquivalentAcross(Bundles.Macro(), MacroSettings, Parse(targetSpec));
    }

    [Theory]
    [MemberData(nameof(MicroTargets))]
    [Trait("Category", "Integration")]
    public void MicroBundles_BothSolvers_AgreeOnFeasibilityAndCost(string targetSpec)
    {
        AssertEquivalentAcross(Bundles.Micro(), MicroSettings, Parse(targetSpec));
    }

    [Theory]
    [MemberData(nameof(FeasibleMacroTargets))]
    [Trait("Category", "Integration")]
    public void MacroBundles_SimplexSolutions_HitTheTarget(string targetSpec)
    {
        PpmTarget target = Parse(targetSpec);
        int feasibleCount = 0;

        foreach (IReadOnlyList<Fertilizer> bundle in Bundles.Macro())
        {
            OptimizationProblem problem = Mapper.CreateOptimizationProblem(target, bundle, MacroSettings);
            Dictionary<string, double>? values = Simplex.Solve(problem);
            if (values is null)
            {
                continue;
            }

            Solution? solution = Mapper.CreateSolution(values, bundle);
            if (solution is null)
            {
                continue;
            }

            Ppm actual = Calculator.CalculatePpm(solution, solution.WaterLiters);

            // The mapper builds equality constraints at precision 1, so the mix must land on target.
            AssertClose(target.N.Value, actual.Nitrogen.Value);
            AssertClose(target.P.Value, actual.Phosphorus.Value);
            AssertClose(target.K.Value, actual.Potassium.Value);
            AssertClose(target.Ca.Value, actual.Calcium.Value);
            AssertClose(target.Mg.Value, actual.Magnesium.Value);
            feasibleCount++;
        }

        Assert.True(feasibleCount > 0, $"No feasible macro solution was produced for '{targetSpec}'.");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void SimplexSolver_NeverReturnsNegativeQuantities()
    {
        PpmTarget target = Parse("N=150 P=50 K=200 Ca=100 Mg=50 S=60");

        foreach (IReadOnlyList<Fertilizer> bundle in Bundles.Macro())
        {
            OptimizationProblem problem = Mapper.CreateOptimizationProblem(target, bundle, MacroSettings);
            Dictionary<string, double>? values = Simplex.Solve(problem);

            if (values is not null)
            {
                Assert.All(values, pair => Assert.True(pair.Value >= 0, $"{pair.Key} = {pair.Value}"));
            }
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void SimplexSolver_ReturnsAValueForEveryVariable()
    {
        PpmTarget target = Parse("N=150 P=50 K=200 Ca=100 Mg=50 S=60");

        foreach (IReadOnlyList<Fertilizer> bundle in Bundles.Macro())
        {
            OptimizationProblem problem = Mapper.CreateOptimizationProblem(target, bundle, MacroSettings);
            Dictionary<string, double>? values = Simplex.Solve(problem);

            if (values is not null)
            {
                Assert.Equal(problem.Variables.Count, values.Count);
            }
        }
    }

    /// <summary>
    /// An infeasible target must be rejected by both solvers, not answered differently.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void ImpossibleTarget_BothSolvers_ReturnNull()
    {
        // Phosphorus with no phosphorus-bearing fertilizer in the bundle.
        PpmTarget target = Parse("P=50");
        IReadOnlyList<Fertilizer> firstMacroBundle = Bundles.Macro()[0];

        SolutionFinderSettings phosphorusOnly = new SolutionFinderSettingsBuilder().AddP(1).Build();
        OptimizationProblem problem = Mapper.CreateOptimizationProblem(target, firstMacroBundle, phosphorusOnly);

        Assert.Null(OrTools.Solve(problem));
        Assert.Null(Simplex.Solve(problem));
    }

    /// <summary>
    /// Potassium alone cannot be hit at precision 1: the constraint set pins every other element to
    /// zero, and every potassium fertilizer in the catalogue carries nitrate, phosphate, sulfate or
    /// chloride with it. Both solvers must reject every bundle rather than disagree.
    /// </summary>
    [Fact]
    [Trait("Category", "Integration")]
    public void PotassiumOnlyTarget_IsInfeasibleForEveryBundle_InBothSolvers()
    {
        PpmTarget target = Parse("K=200");

        foreach (IReadOnlyList<Fertilizer> bundle in Bundles.Macro())
        {
            OptimizationProblem problem = Mapper.CreateOptimizationProblem(target, bundle, MacroSettings);

            Assert.Null(OrTools.Solve(problem));
            Assert.Null(Simplex.Solve(problem));
        }
    }

    private static void AssertEquivalentAcross(
        IReadOnlyList<IReadOnlyList<Fertilizer>> bundles,
        SolutionFinderSettings settings,
        PpmTarget target)
    {
        foreach (IReadOnlyList<Fertilizer> bundle in bundles)
        {
            OptimizationProblem problem = Mapper.CreateOptimizationProblem(target, bundle, settings);

            Dictionary<string, double>? fromOrTools = OrTools.Solve(problem);
            Dictionary<string, double>? fromSimplex = Simplex.Solve(problem);

            Assert.Equal(fromOrTools is null, fromSimplex is null);

            if (fromOrTools is null || fromSimplex is null)
            {
                continue;
            }

            double orToolsCost = Cost(problem, fromOrTools);
            double simplexCost = Cost(problem, fromSimplex);

            // Relative comparison: costs here range from fractions of a unit to thousands.
            double scale = Math.Max(1, Math.Max(Math.Abs(orToolsCost), Math.Abs(simplexCost)));
            Assert.True(Math.Abs(orToolsCost - simplexCost) / scale < 1e-6,
                $"Optimal cost differs: OR-Tools {orToolsCost}, simplex {simplexCost}.");
        }
    }

    private static double Cost(OptimizationProblem problem, Dictionary<string, double> values) =>
        problem.Objective.Coefficients.Sum(term => term.Value * values[term.Key]);

    private static void AssertClose(double expected, double actual)
    {
        if (expected == 0)
        {
            return;
        }

        Assert.InRange(actual, expected - 0.05, expected + 0.05);
    }

    private static PpmTarget Parse(string spec)
    {
        PpmTargetBuilder builder = new();

        foreach (string pair in spec.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            string[] parts = pair.Split('=');
            double value = double.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);

            _ = parts[0] switch
            {
                "N" => builder.AddN(value),
                "P" => builder.AddP(value),
                "K" => builder.AddK(value),
                "Ca" => builder.AddCa(value),
                "Mg" => builder.AddMg(value),
                "S" => builder.AddS(value),
                "Fe" => builder.AddFe(value),
                "Cu" => builder.AddCu(value),
                "Mn" => builder.AddMn(value),
                "Zn" => builder.AddZn(value),
                "B" => builder.AddB(value),
                "Mo" => builder.AddMo(value),
                "Si" => builder.AddSi(value),
                "Se" => builder.AddSe(value),
                _ => throw new ArgumentException($"Unsupported element '{parts[0]}'.", nameof(spec))
            };
        }

        return builder.Build();
    }
}
