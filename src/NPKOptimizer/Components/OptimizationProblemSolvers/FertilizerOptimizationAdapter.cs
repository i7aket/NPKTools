using NPKOptimizer.Common;
using NPKOptimizer.Components.OptimizationProblemSolvers.Contracts;
using NPKOptimizer.Const;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.Fertilizers.ValueObjects;
using NPKOptimizer.Domain.NPK;
using NPKOptimizer.Domain.SolutionsFinderSettings;

namespace NPKOptimizer.Components.OptimizationProblemSolvers;

public class FertilizerOptimizationAdapter : IFertilizerOptimizer
{
    private readonly IOptimizationProblemSolver _optimizationProblemSolver;

    public FertilizerOptimizationAdapter(IOptimizationProblemSolver optimizationProblemSolver)
    {
        Validate.NotNull(optimizationProblemSolver);
        _optimizationProblemSolver = optimizationProblemSolver;
    }

    public ActionResult<FertilizerCollection> Optimize(NpkTarget target, FertilizerCollection collection,
        SolutionFinderSettings settings)
    {
        OptimizationProblem problem = CreateOptimizationProblem(target, collection, settings);
        
        ActionResult<Dictionary<string, double>> result = _optimizationProblemSolver.Solve(problem);

        if (!result.IsSuccess)
            return ActionResult<FertilizerCollection>.Fail("Optimization failed.");

        FertilizerCollection solution = MapSolution(result.Payload, collection);
        return ActionResult<FertilizerCollection>.Success(solution);
    }

private OptimizationProblem CreateOptimizationProblem(NpkTarget target, FertilizerCollection collection, SolutionFinderSettings settings)
{
    OptimizationProblem problem = new ();

    // Define variables for each fertilizer
    foreach (Fertilizer fertilizer in collection)
    {
        problem.Variables[fertilizer.Name.Value] = 0;
        problem.Objective.Coefficients[fertilizer.Name.Value] = fertilizer.FertilizerPrice.Value;
    }
    
    // Prepare target elements based on settings
    Dictionary<string, (double targetValue, double accuracy)> targetElements = new ()
    {
        { ElementName.N, (target.N.Value, settings.N.Accuracy) },
        { ElementName.P, (target.P.Value, settings.P.Accuracy) },
        { ElementName.K, (target.K.Value, settings.K.Accuracy) },
        { ElementName.Ca, (target.Ca.Value, settings.Ca.Accuracy) },
        { ElementName.Mg, (target.Mg.Value, settings.Mg.Accuracy) },
        { ElementName.S, (target.S.Value, settings.S.Accuracy) },
        { ElementName.Fe, (target.Fe.Value, settings.Fe.Accuracy) },
        { ElementName.Cu, (target.Cu.Value, settings.Cu.Accuracy) },
        { ElementName.Mn, (target.Mn.Value, settings.Mn.Accuracy) },
        { ElementName.Zn, (target.Zn.Value, settings.Zn.Accuracy) },
        { ElementName.B, (target.B.Value, settings.B.Accuracy) },
        { ElementName.Mo, (target.Mo.Value, settings.Mo.Accuracy) },
        { ElementName.Cl, (target.Cl.Value, settings.Cl.Accuracy) },
        { ElementName.Si, (target.Si.Value, settings.Si.Accuracy) },
        { ElementName.Se, (target.Se.Value, settings.Se.Accuracy) },
        { ElementName.Na, (target.Na.Value, settings.Na.Accuracy) }
    };
    
    // Build constraints only for elements with non-zero accuracy
    foreach (KeyValuePair<string, (double targetValue, double accuracy)> element in targetElements)
    {
        if (element.Value.accuracy == 0)
            continue; // Skip creating constraints for this element

        Dictionary<string, double> constraintCoefficients = new ();
        foreach (Fertilizer fertilizer in collection)
        {
            double nutrientValue = GetNutrientValue(element.Key, fertilizer);
            constraintCoefficients[fertilizer.Name.Value] = nutrientValue;
        }

        problem.Constraints.Add(new OptimizationConstraint
        {
            Name = element.Key,
            LowerBound = element.Value.targetValue,
            UpperBound = element.Value.targetValue,
            Coefficients = constraintCoefficients
        });
    }

    return problem;
}

private double GetNutrientValue(string nutrient, Fertilizer fertilizer)
{
    return nutrient switch
    {
        ElementName.N => fertilizer.N.Value,
        ElementName.P => fertilizer.P.Value,
        ElementName.K => fertilizer.K.Value,
        ElementName.Ca => fertilizer.Ca.Value,
        ElementName.Mg => fertilizer.Mg.Value,
        ElementName.S => fertilizer.S.Value,
        ElementName.Fe => fertilizer.Fe.Value,
        ElementName.Cu => fertilizer.Cu.Value,
        ElementName.Mn => fertilizer.Mn.Value,
        ElementName.Zn => fertilizer.Zn.Value,
        ElementName.B => fertilizer.B.Value,
        ElementName.Mo => fertilizer.Mo.Value,
        ElementName.Cl => fertilizer.Cl.Value,
        ElementName.Si => fertilizer.Si.Value,
        ElementName.Se => fertilizer.Se.Value,
        ElementName.Na => fertilizer.Na.Value,
        _ => throw new ArgumentException($"Unknown nutrient: {nutrient}")
    };
}
    
    private FertilizerCollection MapSolution(Dictionary<string, double> solutionValues, FertilizerCollection originalCollection)
    {
        FertilizerCollection solutionCollection = new ();
        foreach (KeyValuePair<string, double> item in solutionValues)
        {
            Fertilizer? fertilizer = originalCollection.FirstOrDefault(f => f.Name.Value == item.Key);
            
            if (fertilizer == null || !(item.Value > 0)) continue;
            
            Fertilizer updatedFertilizer = fertilizer with
            {
                Weight = new FertilizerWeight(Math.Round(item.Value, Settings.RoundingPrecision))
            };
            solutionCollection.Add(updatedFertilizer);
        }
        return solutionCollection;
    }

}