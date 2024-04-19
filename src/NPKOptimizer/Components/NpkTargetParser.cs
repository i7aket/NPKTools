using System.Globalization;
using NPKOptimizer.Common;
using NPKOptimizer.Const;
using NPKOptimizer.Domain.NPK;
using NPKOptimizer.Domain.NPK.ValueObjects;

namespace NPKOptimizer.Components;

public interface INpkTargetParser
{
    // ReSharper disable once UnusedMember.Global
    ActionResult<NpkTarget> Parse(string input);
}
public class NpkTargetParser : INpkTargetParser
{
    private const string ErrorInputNullOrWhitespace = "Input string cannot be null or whitespace.";
    private const string ErrorParsePair = "Unable to parse '{0}' as an element=value pair.";
    private const string ErrorElementNotRecognized = "The element '{0}' is not recognized as a valid input.";
    
    private static readonly HashSet<string> ValidElements = new(StringComparer.OrdinalIgnoreCase)
    {
        ElementName.N, ElementName.P, ElementName.K, ElementName.Ca, ElementName.Mg, ElementName.S, ElementName.Fe, ElementName.Cu, 
        ElementName.Mn, ElementName.Zn, ElementName.B, ElementName.Mo, ElementName.Cl, ElementName.Si, ElementName.Se
    };
    
    public ActionResult<NpkTarget> Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return ActionResult<NpkTarget>.Fail(ErrorInputNullOrWhitespace);
        }

        Dictionary<string, double> values = new (StringComparer.OrdinalIgnoreCase);
        string[] pairs = input.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string pair in pairs)
        {
            string[] parts = pair.Split('=');
            if (parts.Length != 2 || !double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            {
                return ActionResult<NpkTarget>.Fail(string.Format(ErrorParsePair, pair));
            }

            string elementKey = parts[0].ToUpper();
            if (!ValidElements.Contains(elementKey))
            {
                return ActionResult<NpkTarget>.Fail(string.Format(ErrorElementNotRecognized, elementKey));
            }

            values[elementKey] = value * Settings.PpmToPercentConversionFactor;
        }

        NpkTarget npkTarget = new (
            new NitrogenNpkTarget(values.GetValueOrDefault(ElementName.N, 0)),
            new PhosphorusNpkTarget(values.GetValueOrDefault(ElementName.P, 0)),
            new PotassiumNpkTarget(values.GetValueOrDefault(ElementName.K, 0)),
            new CalciumNpkTarget(values.GetValueOrDefault(ElementName.Ca, 0)),
            new MagnesiumNpkTarget(values.GetValueOrDefault(ElementName.Mg, 0)),
            new SulfurNpkTarget(values.GetValueOrDefault(ElementName.S, 0)),
            new IronNpkTarget(values.GetValueOrDefault(ElementName.Fe, 0)),
            new CopperNpkTarget(values.GetValueOrDefault(ElementName.Cu, 0)),
            new ManganeseNpkTarget(values.GetValueOrDefault(ElementName.Mn, 0)),
            new ZincNpkTarget(values.GetValueOrDefault(ElementName.Zn, 0)),
            new BoronNpkTarget(values.GetValueOrDefault(ElementName.B, 0)),
            new MolybdenumNpkTarget(values.GetValueOrDefault(ElementName.Mo, 0)),
            new ChlorineNpkTarget(values.GetValueOrDefault(ElementName.Cl, 0)),
            new SiliconNpkTarget(values.GetValueOrDefault(ElementName.Si, 0)),
            new SeleniumNpkTarget(values.GetValueOrDefault(ElementName.Se, 0)),
            new SodiumNpkTarget(values.GetValueOrDefault(ElementName.Na, 0))
        );
            
        return ActionResult<NpkTarget>.Success(npkTarget);
    }
}