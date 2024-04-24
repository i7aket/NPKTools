using NPKOptimizer.Const;
using NPKOptimizer.Contracts;
using NPKOptimizer.Domain.Collections;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.Fertilizers.ValueObjects;
using NPKOptimizer.Domain.PpmTarget;
using NPKOptimizer.Domain.SolutionsFinderSettings;

namespace NPKOptimizer.Components;

public class OptimizationProblemMapper : IOptimizationProblemMapper
{
    public OptimizationProblem CreateOptimizationProblem(PpmTarget target,
        IList<FertilizerOptimizationModel> sourceCollection, SolutionFinderSettings settings)
    {
        OptimizationProblem problem = new();
        HashSet<FertilizerOptimizationModel> fertilizerSet = new HashSet<FertilizerOptimizationModel>(new FertilizerAttributesComparer());  
        HashSet<Guid> idsSet = new HashSet<Guid>();
        foreach (FertilizerOptimizationModel fertilizer in sourceCollection)
        {
            if (!idsSet.Add(fertilizer.RefId.Value))
                throw new InvalidOperationException($"Duplicate fertilizer ID detected: {fertilizer.RefId.Value}.");
            if (!fertilizerSet.Add(fertilizer))
                throw new InvalidOperationException($"Duplicate fertilizer attributes detected for ID: {fertilizer.RefId.Value}.");
            string fertilizerIdKey = fertilizer.RefId.Value.ToString();
            problem.Variables[fertilizerIdKey] = 0;
            problem.Objective.Coefficients[fertilizerIdKey] = fertilizer.Price.Value;
        }
        Dictionary<string, (double targetValue, double accuracy)> targetElements = new()
        {
            { ElementName.N, (target.N.Value / OptimizationSettings.ConversionFactor, settings.Nitrogen.Value) },
            { ElementName.P, (target.P.Value / OptimizationSettings.ConversionFactor, settings.Phosphorus.Value) },
            { ElementName.K, (target.K.Value / OptimizationSettings.ConversionFactor, settings.Potassium.Value) },
            { ElementName.Ca, (target.Ca.Value / OptimizationSettings.ConversionFactor, settings.Calcium.Value) },
            { ElementName.Mg, (target.Mg.Value / OptimizationSettings.ConversionFactor, settings.Magnesium.Value) },
            { ElementName.S, (target.S.Value / OptimizationSettings.ConversionFactor, settings.Sulfur.Value) },
            { ElementName.Fe, (target.Fe.Value / OptimizationSettings.ConversionFactor, settings.Iron.Value) },
            { ElementName.Cu, (target.Cu.Value / OptimizationSettings.ConversionFactor, settings.Copper.Value) },
            { ElementName.Mn, (target.Mn.Value / OptimizationSettings.ConversionFactor, settings.Manganese.Value) },
            { ElementName.Zn, (target.Zn.Value / OptimizationSettings.ConversionFactor, settings.Zinc.Value) },
            { ElementName.B, (target.B.Value / OptimizationSettings.ConversionFactor, settings.Boron.Value) },
            { ElementName.Mo, (target.Mo.Value / OptimizationSettings.ConversionFactor, settings.Molybdenum.Value) },
            { ElementName.Cl, (target.Cl.Value / OptimizationSettings.ConversionFactor, settings.Chlorine.Value) },
            { ElementName.Si, (target.Si.Value / OptimizationSettings.ConversionFactor, settings.Silicon.Value) },
            { ElementName.Se, (target.Se.Value / OptimizationSettings.ConversionFactor, settings.Selenium.Value) },
            { ElementName.Na, (target.Na.Value / OptimizationSettings.ConversionFactor, settings.Sodium.Value) }
        };
        foreach (KeyValuePair<string, (double targetValue, double accuracy)> element in targetElements)
        {
            if (element.Value.accuracy == 0) continue;
            Dictionary<string, double> constraintCoefficients = new();
            foreach (FertilizerOptimizationModel fertilizer in sourceCollection)
            {
                double nutrientValue = element.Key switch
                {
                    ElementName.N => fertilizer.Nitrogen.Value,
                    ElementName.P => fertilizer.Phosphorus.Value,
                    ElementName.K => fertilizer.Potassium.Value,
                    ElementName.Ca => fertilizer.Calcium.Value,
                    ElementName.Mg => fertilizer.Magnesium.Value,
                    ElementName.S => fertilizer.Sulfur.Value,
                    ElementName.Fe => fertilizer.Iron.Value,
                    ElementName.Cu => fertilizer.Copper.Value,
                    ElementName.Mn => fertilizer.Manganese.Value,
                    ElementName.Zn => fertilizer.Zinc.Value,
                    ElementName.B => fertilizer.Boron.Value,
                    ElementName.Mo => fertilizer.Molybdenum.Value,
                    ElementName.Cl => fertilizer.Chlorine.Value,
                    ElementName.Si => fertilizer.Silicon.Value,
                    ElementName.Se => fertilizer.Selenium.Value,
                    ElementName.Na => fertilizer.Sodium.Value,
                    _ => throw new ArgumentException($"Unknown nutrient: {element.Key}")
                };
                constraintCoefficients[fertilizer.RefId.Value.ToString()] = nutrientValue;
            }
            double rangeFactor = element.Value.targetValue *
                                 (1 - Math.Min(settings.RangeFactor.Value, element.Value.accuracy));
            problem.Constraints.Add(new OptimizationProblem.OptimizationConstraint
            {
                Name = element.Key,
                LowerBound = element.Value.targetValue - rangeFactor,
                UpperBound = element.Value.targetValue + rangeFactor,
                Coefficients = constraintCoefficients
            });
        }
        return problem;
    }
    public IList<Fertilizer> MapSolution(Dictionary<string, double> solutionValues,
        IList<FertilizerOptimizationModel> originalSourceCollection)
    {
        IList<Fertilizer> solutionCollection = new List<Fertilizer>();
        foreach (KeyValuePair<string, double> item in solutionValues)
        {
            Guid itemId = Guid.Parse(item.Key);
            FertilizerOptimizationModel? fertilizerOptimizationModel =
                originalSourceCollection.FirstOrDefault(f => f.RefId.Value == itemId);
            if (fertilizerOptimizationModel == null)
                throw new InvalidOperationException($"No matching fertilizer found for ID: {item.Key}");
            if (item.Value < 0)
                throw new ArgumentException("Weight cannot be negative.", nameof(item.Value));
            if (item.Value == 0) continue;
            FertilizerWeight weight = new FertilizerWeight(Math.Round(item.Value, OptimizationSettings.RoundingPrecision));
            Fertilizer fertilizer = new Fertilizer(
                fertilizerOptimizationModel.RefId,
                weight,
                fertilizerOptimizationModel.Price,
                fertilizerOptimizationModel.Nitrogen,
                fertilizerOptimizationModel.Phosphorus,
                fertilizerOptimizationModel.Potassium,
                fertilizerOptimizationModel.Calcium,
                fertilizerOptimizationModel.Magnesium,
                fertilizerOptimizationModel.Sulfur,
                fertilizerOptimizationModel.Iron,
                fertilizerOptimizationModel.Copper,
                fertilizerOptimizationModel.Manganese,
                fertilizerOptimizationModel.Zinc,
                fertilizerOptimizationModel.Boron,
                fertilizerOptimizationModel.Molybdenum,
                fertilizerOptimizationModel.Chlorine,
                fertilizerOptimizationModel.Silicon,
                fertilizerOptimizationModel.Selenium,
                fertilizerOptimizationModel.Sodium
            );
            solutionCollection.Add(fertilizer);
        }
        return solutionCollection;
    }
}