using NPKTools.Core.Domain.Fertilizers;
using NPKTools.Core.Domain.Fertilizers.Builders;
using NPKTools.Core.Domain.PpmTarget;
using NPKTools.Core.Domain.PpmTarget.Builder;
using NPKTools.Core.Domain.SolutionsFinderSettings;
using NPKTools.Core.Domain.SolutionsFinderSettings.Builder;
using NPKTools.Optimizer.Components;
using NPKTools.Optimizer.Contracts;
using NPKTools.Optimizer.OrTools;
using Xunit;

namespace NPKTools.IntegrationTests;

/// <summary>
/// Randomized cross-check of the managed solver against OR-Tools on synthetic fertilizer catalogues.
/// </summary>
/// <remarks>
/// <see cref="SolverEquivalenceTests"/> covers the 22 curated preset bundles. Those are the problems
/// the library actually ships, but consumers may pass arbitrary catalogues of their own, so the
/// managed solver — which is the shipped default — needs to hold up beyond the preset data.
/// <para>
/// Seeds are fixed so a failure is reproducible; the generator is deliberately awkward, producing
/// fertilizers that share nutrients, single-nutrient fertilizers, and targets that are often
/// infeasible. Both solvers must agree on feasibility, and where feasible on the optimal cost.
/// </para>
/// <para>
/// Of the 40 seeds, 11 are feasible and 29 are not, so the run exercises both the optimal-cost
/// comparison and the agreed-infeasibility path rather than trivially passing on nulls.
/// </para>
/// </remarks>
public class SolverRandomizedEquivalenceTests
{
    private static readonly OptimizationProblemMapper Mapper = new();
    private static readonly GoogleOrToolsOptimizationSolver OrTools = new();
    private static readonly SimplexOptimizationSolver Simplex = new();

    public static TheoryData<int> Seeds
    {
        get
        {
            TheoryData<int> seeds = new();
            for (int seed = 1; seed <= 40; seed++)
            {
                seeds.Add(seed);
            }

            return seeds;
        }
    }

    [Theory]
    [MemberData(nameof(Seeds))]
    [Trait("Category", "Integration")]
    public void RandomCatalogue_BothSolvers_AgreeOnFeasibilityAndOptimalCost(int seed)
    {
        Random random = new(seed);

        int fertilizerCount = random.Next(2, 13);
        List<Fertilizer> catalogue = [];

        for (int i = 0; i < fertilizerCount; i++)
        {
            FertilizerBuilder builder = new FertilizerBuilder()
                .AddName($"Synthetic {i}")
                .AddPrice(Math.Round(0.5 + random.NextDouble() * 4, 3));

            // Give each fertilizer one to three nutrients drawn from N, P, K, Ca, Mg.
            int nutrients = random.Next(1, 4);
            HashSet<int> chosen = [];
            while (chosen.Count < nutrients)
            {
                chosen.Add(random.Next(0, 5));
            }

            foreach (int nutrient in chosen)
            {
                double percent = Math.Round(5 + random.NextDouble() * 40, 3);
                _ = nutrient switch
                {
                    0 => builder.AddNo3(percent),
                    1 => builder.AddP(percent),
                    2 => builder.AddK(percent),
                    3 => builder.AddCaNonChelated(percent),
                    _ => builder.AddMgNonChelated(percent)
                };
            }

            catalogue.Add(builder.Build());
        }

        PpmTarget target = new PpmTargetBuilder()
            .AddN(Math.Round(random.NextDouble() * 200, 2))
            .AddP(Math.Round(random.NextDouble() * 80, 2))
            .AddK(Math.Round(random.NextDouble() * 250, 2))
            .AddCa(Math.Round(random.NextDouble() * 150, 2))
            .AddMg(Math.Round(random.NextDouble() * 80, 2))
            .Build();

        // Loosen the tolerance so a decent share of the random targets are actually reachable;
        // at precision 1 every constraint is an equality and nearly everything is infeasible.
        SolutionFinderSettings settings = new SolutionFinderSettingsBuilder()
            .AddRangeFactor(0.8)
            .AddN(0.8).AddP(0.8).AddK(0.8).AddCa(0.8).AddMg(0.8)
            .Build();

        OptimizationProblem problem = Mapper.CreateOptimizationProblem(target, catalogue, settings);

        Dictionary<string, double>? fromOrTools = OrTools.Solve(problem);
        Dictionary<string, double>? fromSimplex = Simplex.Solve(problem);

        Assert.Equal(fromOrTools is null, fromSimplex is null);

        if (fromOrTools is null || fromSimplex is null)
        {
            return;
        }

        Assert.All(fromSimplex, pair => Assert.True(pair.Value >= -1e-9, $"{pair.Key} = {pair.Value}"));

        double orToolsCost = Cost(problem, fromOrTools);
        double simplexCost = Cost(problem, fromSimplex);
        double scale = Math.Max(1, Math.Max(Math.Abs(orToolsCost), Math.Abs(simplexCost)));

        Assert.True(Math.Abs(orToolsCost - simplexCost) / scale < 1e-6,
            $"seed {seed}: OR-Tools cost {orToolsCost}, simplex cost {simplexCost}");

        // Both claimed optimality, so the simplex answer must also satisfy every constraint.
        AssertConstraintsSatisfied(problem, fromSimplex, seed);
    }

    private static void AssertConstraintsSatisfied(
        OptimizationProblem problem,
        Dictionary<string, double> values,
        int seed)
    {
        foreach (OptimizationProblem.OptimizationConstraint constraint in problem.Constraints)
        {
            double activity = constraint.Coefficients.Sum(term => term.Value * values[term.Key]);
            double slack = Math.Max(1, Math.Abs(constraint.UpperBound)) * 1e-6;

            Assert.True(activity >= constraint.LowerBound - slack,
                $"seed {seed}: {constraint.Name} activity {activity} below lower bound {constraint.LowerBound}");
            Assert.True(activity <= constraint.UpperBound + slack,
                $"seed {seed}: {constraint.Name} activity {activity} above upper bound {constraint.UpperBound}");
        }
    }

    private static double Cost(OptimizationProblem problem, Dictionary<string, double> values) =>
        problem.Objective.Coefficients.Sum(term => term.Value * values[term.Key]);
}
