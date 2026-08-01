using SYT.NPKTools.Concentrates;
using SYT.NPKTools.Fertilizers;
using SYT.NPKTools.Nutrients;
using SYT.NPKTools.Optimization;

namespace SYT.NPKTools.Calculator;

/// <summary>
/// Everything the page needs, and the whole calculation in one place.
/// </summary>
/// <remarks>
/// The components render this and raise events; none of them computes anything. Keeping the chain
/// here — water, target, salts, recipes, concentrate — means the order the steps depend on each other
/// is visible in one method rather than spread across the UI, which is what makes it possible to see
/// that the water is deducted before the optimizer runs and added back before the ppm is judged.
/// </remarks>
public sealed class CalculatorModel
{
    private readonly IPpmTargetParser _parser = NpkTools.CreateTargetParser();
    private readonly IPpmCalculationService _calculator = NpkTools.CreatePpmCalculator();
    private readonly IFertilizerOptimizer _optimizer = NpkTools.CreateOptimizer();

    public CalculatorModel()
    {
        IFertilizerBundleRepository catalogue = NpkTools.CreateBundleRepository();

        Catalogue =
        [
            .. catalogue.Macro().SelectMany(b => b)
                .Concat(catalogue.Micro().SelectMany(b => b))
                .DistinctBy(f => f.Name.Value)
                .OrderBy(f => f.Name.Value, StringComparer.Ordinal)
        ];

        // A shelf nobody has edited yet is the whole catalogue: the calculator should produce an
        // answer on first load rather than an empty state that has to be configured first.
        Selected = [.. Catalogue.Select(f => f.Name.Value)];
    }

    /// <summary>Every salt the library knows about, macro and micro together.</summary>
    public IReadOnlyList<Fertilizer> Catalogue { get; }

    /// <summary>Names of the salts the grower says they have.</summary>
    public HashSet<string> Selected { get; }

    /// <summary>The target, as the parser accepts it.</summary>
    public string TargetText { get; set; } = "N=150 P=50 K=210 Ca=160 Mg=50 S=65 L=100";

    /// <summary>The source-water analysis, in ppm. Keyed by element symbol.</summary>
    public Dictionary<string, double> Water { get; } = new()
    {
        ["N"] = 0,
        ["P"] = 0,
        ["K"] = 0,
        ["Ca"] = 0,
        ["Mg"] = 0,
        ["S"] = 0,
        ["Fe"] = 0,
        ["Cu"] = 0,
        ["Mn"] = 0,
        ["Zn"] = 0,
        ["B"] = 0,
        ["Mo"] = 0,
        ["Cl"] = 0,
        ["Si"] = 0,
        ["Se"] = 0,
        ["Na"] = 0,
    };

    /// <summary>Litres in each concentrate tank. Null means the caller does not want a concentrate.</summary>
    public double? ConcentrateLiters { get; set; } = 1;

    // ---------------------------------------------------------------- results

    public string? Error { get; private set; }
    public PpmTarget? Target { get; private set; }
    public WaterProfile WaterProfile { get; private set; } = WaterProfile.Pure;
    public double Alkalinity { get; private set; }
    public ConductivityEstimate? WaterConductivity { get; private set; }
    public IReadOnlyList<NutrientExcess> Excesses { get; private set; } = [];
    public IReadOnlyList<string> UncoveredElements { get; private set; } = [];
    public IReadOnlyList<RecipeView> Recipes { get; private set; } = [];
    public bool HasRun { get; private set; }

    /// <summary>
    /// Runs the whole chain. Never throws for bad input — a malformed target lands in
    /// <see cref="Error"/>, because a calculator that crashes on a typo is unusable.
    /// </summary>
    public void Recalculate()
    {
        HasRun = true;
        Error = null;
        Recipes = [];
        Excesses = [];
        UncoveredElements = [];

        WaterProfile = BuildWater();
        Alkalinity = WaterProfile.EstimatedAlkalinity();
        WaterConductivity = WaterProfile.EstimateConductivity();

        PpmTarget target;
        try
        {
            target = _parser.Parse(TargetText);
        }
        catch (Exception ex) when (ex is ArgumentException or FormatException or InvalidOperationException)
        {
            Error = ex.Message;
            Target = null;
            return;
        }

        Target = target;

        Fertilizer[] shelf = [.. Catalogue.Where(f => Selected.Contains(f.Name.Value))];
        if (shelf.Length == 0)
        {
            Error = "No salts selected. Tick at least one.";
            return;
        }

        // The water comes off the target before the optimizer sees it; whatever the water already
        // supplies is not something the salts should supply again.
        WaterAdjustedTarget adjusted = target.AdjustFor(WaterProfile);
        Excesses = adjusted.Excesses;

        CustomFertilizerBundleRepository bundles = NpkTools.CreateBundleRepository(shelf);
        UncoveredElements = bundles.MacroGeneration.UncoveredElements;

        Solutions found = new FertilizerOptimizationService(_optimizer, bundles)
            .FindMacroSolutions(adjusted.Target);

        if (found.Count == 0)
        {
            Error = UncoveredElements.Count > 0
                ? $"No recipe: nothing on the shelf supplies {string.Join(", ", UncoveredElements)}."
                : "No recipe. The selected salts cannot reach this target — try adding more, or relax it.";
            return;
        }

        Recipes = [.. found.Select(Describe)];
    }

    private WaterProfile BuildWater()
    {
        WaterProfileBuilder builder = new();
        builder.AddNitrate(Water["N"]).AddP(Water["P"]).AddK(Water["K"])
            .AddCa(Water["Ca"]).AddMg(Water["Mg"]).AddS(Water["S"])
            .AddFe(Water["Fe"]).AddCu(Water["Cu"]).AddMn(Water["Mn"]).AddZn(Water["Zn"])
            .AddB(Water["B"]).AddMo(Water["Mo"]).AddCl(Water["Cl"]).AddSi(Water["Si"])
            .AddSe(Water["Se"]).AddNa(Water["Na"]);
        return builder.Build();
    }

    private RecipeView Describe(Solution mix)
    {
        // Measured, not assumed: the ppm comes back through the same calculator the library uses, and
        // the water is added on so the figures describe the reservoir rather than the salts alone.
        Ppm fromSalts = _calculator.CalculatePpm([.. mix], mix.WaterLiters);
        Ppm inTank = fromSalts.Plus(WaterProfile);

        ConcentratePlan? plan = null;
        if (ConcentrateLiters is > 0 && ConcentrateLiters < mix.WaterLiters)
        {
            plan = mix.AsConcentrate(ConcentrateLiters.Value);
        }

        return new RecipeView(
            mix,
            inTank,
            inTank.Ratios(),
            inTank.IonBalance(),
            inTank.EstimateConductivity(Alkalinity),
            plan);
    }
}

/// <summary>
/// One recipe, with everything the page shows about it already computed.
/// </summary>
/// <param name="Mix">The salts and their weights.</param>
/// <param name="InTank">What the reservoir holds — the salts plus the source water.</param>
/// <param name="Ratios">The ratios the mix is judged by.</param>
/// <param name="Ions">Charge balance, and the acid the recipe itself contributes.</param>
/// <param name="Conductivity">The EC a meter should read, bicarbonate included.</param>
/// <param name="Concentrate">The A/B split, or null when no concentrate was asked for.</param>
public sealed record RecipeView(
    Solution Mix,
    Ppm InTank,
    NutrientRatios Ratios,
    IonBalance Ions,
    ConductivityEstimate Conductivity,
    ConcentratePlan? Concentrate);
