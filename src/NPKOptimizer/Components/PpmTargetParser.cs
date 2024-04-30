using System.Globalization;
using NPKOptimizer.Const;
using NPKOptimizer.Contracts;
using NPKOptimizer.Domain.PpmTarget;
using NPKOptimizer.Domain.PpmTarget.ValueObjects;

namespace NPKOptimizer.Components;
/// <summary>
/// Parses a string containing elements and their corresponding values in parts per million (ppm)
/// into a PpmTarget object. This parser supports a predefined set of elements and ensures that
/// the input format is correctly followed.
/// </summary>
public class PpmTargetParser : IPpmTargetParser
{
    private const string ErrorParsePair = "Unable to parse '{0}' as an element=value pair.";
    private const string ErrorElementNotRecognized = "The element '{0}' is not recognized as a valid input.";
    private const string ErrorDuplicateElement = "Duplicate element '{0}' found in input.";
    
    private static readonly HashSet<string> ValidElements = new(StringComparer.OrdinalIgnoreCase)
    {
        Names.N, Names.P, Names.K, Names.Ca, Names.Mg, Names.S, Names.Fe, Names.Cu, 
        Names.Mn, Names.Zn, Names.B, Names.Mo, Names.Cl, Names.Si, Names.Se, Names.Liters
    };
    
    /// <summary>
    /// Parses the provided string input into a PpmTarget object. Each pair in the input string
    /// should be in the format "element=value". The method checks for correct formatting, 
    /// validates elements against a set list, and handles duplicates appropriately.
    /// </summary>
    /// <param name="input">The input string containing the element-value pairs.</param>
    /// <returns>A PpmTarget object populated based on the input string.</returns>
    /// <exception cref="ArgumentException">Thrown if the input string is null or whitespace.</exception>
    /// <exception cref="FormatException">Thrown if the input string has incorrect formatting, an unrecognized element, or duplicate elements.</exception>
    public PpmTarget Parse(string input)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(input);

        Dictionary<string, double> values = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        string[] pairs = input.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string pair in pairs)
        {
            string[] parts = pair.Split('=');
            if (parts.Length != 2 || !double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            {
                throw new FormatException(string.Format(ErrorParsePair, pair));
            }

            string elementKey = parts[0].ToUpper();
            if (!ValidElements.Contains(elementKey))
            {
                throw new FormatException(string.Format(ErrorElementNotRecognized, pair));
            }
            
            if (!values.TryAdd(elementKey, value))
            {
                throw new FormatException(string.Format(ErrorDuplicateElement, elementKey));
            }
        }

        PpmTarget ppmTarget = new PpmTarget(
            new NitrogenPpmTarget(values.GetValueOrDefault(Names.N, 0)),
            new PhosphorusPpmTarget(values.GetValueOrDefault(Names.P, 0)),
            new PotassiumPpmTarget(values.GetValueOrDefault(Names.K, 0)),
            new CalciumPpmTarget(values.GetValueOrDefault(Names.Ca, 0)),
            new MagnesiumPpmTarget(values.GetValueOrDefault(Names.Mg, 0)),
            new SulfurPpmTarget(values.GetValueOrDefault(Names.S, 0)),
            new IronPpmTarget(values.GetValueOrDefault(Names.Fe, 0)),
            new CopperPpmTarget(values.GetValueOrDefault(Names.Cu, 0)),
            new ManganesePpmTarget(values.GetValueOrDefault(Names.Mn, 0)),
            new ZincPpmTarget(values.GetValueOrDefault(Names.Zn, 0)),
            new BoronPpmTarget(values.GetValueOrDefault(Names.B, 0)),
            new MolybdenumPpmTarget(values.GetValueOrDefault(Names.Mo, 0)),
            new ChlorinePpmTarget(values.GetValueOrDefault(Names.Cl, 0)),
            new SiliconPpmTarget(values.GetValueOrDefault(Names.Si, 0)),
            new SeleniumPpmTarget(values.GetValueOrDefault(Names.Se, 0)),
            new SodiumPpmTarget(values.GetValueOrDefault(Names.Na, 0)),
            new WaterVolumeLitersPpmTarget(values.GetValueOrDefault(Names.Liters, 1))
        );
            
        return ppmTarget;
    }
}