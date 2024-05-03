using NPKTools.Core.Common;
using NPKTools.Core.Const;
using NPKTools.Core.Domain.Collections;
using NPKTools.Core.Domain.Fertilizers;
using NPKTools.Core.Domain.Fertilizers.ValueObjects;
using NPKTools.Core.Domain.PpmTarget;
using NPKTools.Core.Domain.SolutionsFinderSettings;
using NPKTools.Optimizer.Contracts;

namespace NPKTools.Optimizer.Components;

/// <summary>
/// Implements the IOptimizationProblemMapper interface to provide concrete mappings
/// between the domain model and the structures required for optimization.
/// This class translates user-defined nutrient targets and fertilizer data
/// into an optimization problem that can be solved algorithmically.
/// </summary>
public class OptimizationProblemMapper : IOptimizationProblemMapper
{
    /// <summary>
    /// Creates an optimization problem from given nutrient targets, a collection of fertilizers,
    /// and solution finder settings. This method sets up the necessary variables,
    /// objectives, and constraints for the optimization problem based on the specified parameters.
    /// </summary>
    /// <param name="target">The target nutrient levels to be achieved.</param>
    /// <param name="sourceCollection">The collection of fertilizers available for use.</param>
    /// <param name="settings">Settings that influence the optimization process, such as tolerance levels and cost considerations.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns>An OptimizationProblem object configured with all necessary variables and constraints.</returns>
    public OptimizationProblem CreateOptimizationProblem(PpmTarget target,
        IList<Fertilizer> sourceCollection, SolutionFinderSettings settings)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(settings);
        ThrowIf.NullOrEmpty(sourceCollection);

        OptimizationProblem problem = new();
        HashSet<Fertilizer> fertilizerSet = new HashSet<Fertilizer>(new FertilizerAttributesComparer());
        HashSet<Guid> idsSet = [];
        foreach (Fertilizer fertilizer in sourceCollection)
        {
            if (!idsSet.Add(fertilizer.RefId.Value))
                throw new InvalidOperationException($"Duplicate fertilizer ID detected: {fertilizer.RefId.Value}.");
            ThrowIf.Duplicate(fertilizerSet, fertilizer);

            string fertilizerIdKey = fertilizer.RefId.Value.ToString();
            problem.Variables[fertilizerIdKey] = 0;
            problem.Objective.Coefficients[fertilizerIdKey] = fertilizer.Price.Value;
        }

        Dictionary<string, (double targetValue, double accuracy)> targetElements = new()
        {
            { Names.N, (target.N.Value / OptimizationSettings.ConversionFactor, settings.Nitrogen.Value) },
            { Names.P, (target.P.Value / OptimizationSettings.ConversionFactor, settings.Phosphorus.Value) },
            { Names.K, (target.K.Value / OptimizationSettings.ConversionFactor, settings.Potassium.Value) },
            { Names.Ca, (target.Ca.Value / OptimizationSettings.ConversionFactor, settings.Calcium.Value) },
            { Names.Mg, (target.Mg.Value / OptimizationSettings.ConversionFactor, settings.Magnesium.Value) },
            { Names.S, (target.S.Value / OptimizationSettings.ConversionFactor, settings.Sulfur.Value) },
            { Names.Fe, (target.Fe.Value / OptimizationSettings.ConversionFactor, settings.Iron.Value) },
            { Names.Cu, (target.Cu.Value / OptimizationSettings.ConversionFactor, settings.Copper.Value) },
            { Names.Mn, (target.Mn.Value / OptimizationSettings.ConversionFactor, settings.Manganese.Value) },
            { Names.Zn, (target.Zn.Value / OptimizationSettings.ConversionFactor, settings.Zinc.Value) },
            { Names.B, (target.B.Value / OptimizationSettings.ConversionFactor, settings.Boron.Value) },
            { Names.Mo, (target.Mo.Value / OptimizationSettings.ConversionFactor, settings.Molybdenum.Value) },
            { Names.Cl, (target.Cl.Value / OptimizationSettings.ConversionFactor, settings.Chlorine.Value) },
            { Names.Si, (target.Si.Value / OptimizationSettings.ConversionFactor, settings.Silicon.Value) },
            { Names.Se, (target.Se.Value / OptimizationSettings.ConversionFactor, settings.Selenium.Value) },
            { Names.Na, (target.Na.Value / OptimizationSettings.ConversionFactor, settings.Sodium.Value) }
        };
        foreach (KeyValuePair<string, (double targetValue, double accuracy)> element in targetElements)
        {
            if (element.Value.accuracy == 0) continue;
            Dictionary<string, double> constraintCoefficients = new();
            foreach (Fertilizer fertilizer in sourceCollection)
            {
                double nutrientValue = element.Key switch
                {
                    Names.N => fertilizer.Nitrogen.Value,
                    Names.P => fertilizer.Phosphorus.Value,
                    Names.K => fertilizer.Potassium.Value,
                    Names.Ca => fertilizer.Calcium.Value,
                    Names.Mg => fertilizer.Magnesium.Value,
                    Names.S => fertilizer.Sulfur.Value,
                    Names.Fe => fertilizer.Iron.Value,
                    Names.Cu => fertilizer.Copper.Value,
                    Names.Mn => fertilizer.Manganese.Value,
                    Names.Zn => fertilizer.Zinc.Value,
                    Names.B => fertilizer.Boron.Value,
                    Names.Mo => fertilizer.Molybdenum.Value,
                    Names.Cl => fertilizer.Chlorine.Value,
                    Names.Si => fertilizer.Silicon.Value,
                    Names.Se => fertilizer.Selenium.Value,
                    Names.Na => fertilizer.Sodium.Value,
                    _ => throw new ArgumentOutOfRangeException()
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

    /// <summary>
    /// Creates a solution based on the optimization results, mapping the calculated values back to
    /// real-world quantities of fertilizers to be used, adjusted by the volume of water specified.
    /// </summary>
    /// <param name="solutionValues">The calculated quantities of each fertilizer from the optimization.</param>
    /// <param name="originalSourceCollection">The original collection of fertilizers used in the optimization.</param>
    /// <param name="waterLiters">The amount of water to be used with the fertilizers, affecting the final solution concentrations.</param>
    /// <returns>A Solution object detailing the types and quantities of fertilizers to be used.</returns>
    public Solution CreateSolution(Dictionary<string, double> solutionValues,
        IList<Fertilizer> originalSourceCollection, double waterLiters = 1)
    {
        ThrowIf.NullOrEmpty(solutionValues);
        ThrowIf.NullOrEmpty(originalSourceCollection);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(waterLiters);
        Solution solutionCollection = new Solution
        {
            WaterLiters = waterLiters
        };
        foreach (KeyValuePair<string, double> item in solutionValues)
        {
            Guid itemId = Guid.Parse(item.Key);

            Fertilizer? fertilizerOptimizationModel =
                originalSourceCollection.FirstOrDefault(f => f.RefId.Value == itemId);

            ArgumentNullException.ThrowIfNull(fertilizerOptimizationModel);
            ArgumentOutOfRangeException.ThrowIfNegative(item.Value);
            if (item.Value == 0) continue;

            FertilizerWeight weight =
                new FertilizerWeight(Math.Round(item.Value, OptimizationSettings.RoundingPrecision));
            Fertilizer fertilizerResultModel =
                fertilizerOptimizationModel.With(new FertilizerWeight(weight.Value * waterLiters));
            solutionCollection.Add(fertilizerResultModel);
        }

        return solutionCollection;
    }
}