using System.Text.Json.Serialization;
using SYT.NPKTools.Fertilizers;

namespace SYT.NPKTools.Calculator;

/// <summary>
/// A fertilizer the grower owns and the catalogue does not carry.
/// </summary>
/// <remarks>
/// Stored as what was entered rather than as a built <see cref="Fertilizer"/>, so a saved file stays
/// readable and a formula can be re-derived if the library's atomic masses are ever corrected.
/// </remarks>
public sealed class CustomSalt
{
    /// <summary>Every nutrient form a salt can be described by, in the order the form shows them.</summary>
    public static IReadOnlyList<string> Forms { get; } =
    [
        "No3", "Nh4", "Nh2", "P", "K", "Ca", "CaEdta", "Mg", "MgEdta", "S",
        "Fe", "FeEdta", "FeDtpa", "FeEddha", "FeHbed", "Cu", "CuEdta",
        "Mn", "MnEdta", "Zn", "ZnEdta", "B", "Mo", "Cl", "Si", "Se", "Na",
    ];

    /// <summary>The name to show. Unique among every salt on offer.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>The chemical formula, or null when the salt was described by percentages.</summary>
    [JsonPropertyName("formula")]
    public string? Formula { get; set; }

    /// <summary>Which concentrate tank the salt belongs in.</summary>
    [JsonPropertyName("tank")]
    public ConcentrateType Tank { get; set; } = ConcentrateType.A;

    /// <summary>Percentages by weight, keyed by nutrient form. Used when there is no formula.</summary>
    [JsonPropertyName("percentages")]
    public Dictionary<string, double> Percentages { get; init; } = new(StringComparer.Ordinal);

    /// <summary>How much dissolves in a litre, when known.</summary>
    [JsonPropertyName("solubility")]
    public double? SolubilityGramsPerLitre { get; set; }

    /// <summary>
    /// Builds the fertilizer this describes.
    /// </summary>
    /// <param name="fertilizer">The result, or null when the description is unusable.</param>
    /// <param name="error">What is wrong with the description, or null on success.</param>
    /// <returns><see langword="true"/> when a fertilizer was built.</returns>
    public bool TryMaterialise(out Fertilizer? fertilizer, out string? error)
    {
        fertilizer = null;

        if (string.IsNullOrWhiteSpace(Name))
        {
            error = "The salt needs a name.";
            return false;
        }

        // The formula wins when both are present: it is the more precise description, and it is the
        // one the form fills in for itself.
        if (!string.IsNullOrWhiteSpace(Formula))
        {
            return FormulaComposition.TryCreate(Name, Formula, Tank, out fertilizer, out error);
        }

        double total = Percentages.Values.Sum();
        if (total <= 0)
        {
            error = "The salt carries no nutrients, so no target can use it.";
            return false;
        }

        if (total > 100)
        {
            error = $"The percentages add up to {total:F1}, which is more than 100.";
            return false;
        }

        FertilizerBuilder builder = new FertilizerBuilder()
            .AddName(Name.Trim())
            .AddFormula("—")
            .AddType(Tank);

        foreach ((string form, double percent) in Percentages.Where(p => p.Value > 0))
        {
            if (!Apply(builder, form, percent))
            {
                error = $"'{form}' is not a nutrient form this calculator knows.";
                return false;
            }
        }

        fertilizer = builder.Build();
        error = null;
        return true;
    }

    private static bool Apply(FertilizerBuilder builder, string form, double percent)
    {
        switch (form)
        {
            case "No3": builder.AddNo3(percent); return true;
            case "Nh4": builder.AddNh4(percent); return true;
            case "Nh2": builder.AddNh2(percent); return true;
            case "P": builder.AddP(percent); return true;
            case "K": builder.AddK(percent); return true;
            case "Ca": builder.AddCaNonChelated(percent); return true;
            case "CaEdta": builder.AddCaEdta(percent); return true;
            case "Mg": builder.AddMgNonChelated(percent); return true;
            case "MgEdta": builder.AddMgEdta(percent); return true;
            case "S": builder.AddS(percent); return true;
            case "Fe": builder.AddFeNonChelated(percent); return true;
            case "FeEdta": builder.AddFeEdta(percent); return true;
            case "FeDtpa": builder.AddFeDtpa(percent); return true;
            case "FeEddha": builder.AddFeEddha(percent); return true;
            case "FeHbed": builder.AddFeHbed(percent); return true;
            case "Cu": builder.AddCuNonChelated(percent); return true;
            case "CuEdta": builder.AddCuEdta(percent); return true;
            case "Mn": builder.AddMnNonChelated(percent); return true;
            case "MnEdta": builder.AddMnEdta(percent); return true;
            case "Zn": builder.AddZnNonChelated(percent); return true;
            case "ZnEdta": builder.AddZnEdta(percent); return true;
            case "B": builder.AddB(percent); return true;
            case "Mo": builder.AddMo(percent); return true;
            case "Cl": builder.AddCl(percent); return true;
            case "Si": builder.AddSi(percent); return true;
            case "Se": builder.AddSe(percent); return true;
            case "Na": builder.AddNa(percent); return true;
            default: return false;
        }
    }
}
