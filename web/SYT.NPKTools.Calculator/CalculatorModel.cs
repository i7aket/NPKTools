using System.Globalization;
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
        foreach (Fertilizer salt in Catalogue)
        {
            Selected.Add(salt.Name.Value);
        }

        // Seeded through the setter so the starter target is written once, in the format the parser
        // and the transport share, rather than duplicated as a field initialiser.
        TargetText = "N=150 P=50 K=210 Ca=160 Mg=50 S=65 L=100";
    }

    /// <summary>Every salt the library knows about, macro and micro together.</summary>
    public IReadOnlyList<Fertilizer> Catalogue { get; }

    /// <summary>Names of the salts the grower says they have.</summary>
    public HashSet<string> Selected { get; } = new(StringComparer.Ordinal);

    /// <summary>The target, element by element, in ppm. Keyed by symbol.</summary>
    /// <remarks>
    /// The state, rather than a view of it. A number field cannot hold a malformed value, so the only
    /// way to reach a parse error is the string box — and an error there no longer destroys what is
    /// already entered.
    /// </remarks>
    public Dictionary<string, double> TargetFields { get; } =
        ElementGroups.All.ToDictionary(symbol => symbol, _ => 0d, StringComparer.Ordinal);

    /// <summary>The reservoir volume, in litres.</summary>
    public double Liters { get; set; } = 100;

    /// <summary>
    /// The target as the parser accepts it — a projection of <see cref="TargetFields"/>.
    /// </summary>
    /// <remarks>
    /// Kept because it is the transport format: a link and a file both carry it, so a setup saved by
    /// any version stays readable. Setting it parses, and on failure records the error and leaves the
    /// fields as they were.
    /// </remarks>
    public string TargetText
    {
        get
        {
            IEnumerable<string> pairs = ElementGroups.All
                .Where(symbol => TargetFields[symbol] > 0)
                .Select(symbol => $"{symbol}={Format(TargetFields[symbol])}");

            return string.Join(' ', pairs.Append($"L={Format(Liters)}"));
        }

        set
        {
            PpmTarget parsed;
            try
            {
                parsed = _parser.Parse(value);
            }
            catch (Exception ex) when (ex is ArgumentException or FormatException or InvalidOperationException)
            {
                Error = ex.Message;
                return;
            }

            Error = null;
            Liters = parsed.Liters.Value;
            foreach (string symbol in ElementGroups.All)
            {
                TargetFields[symbol] = ValueOf(parsed, symbol);
            }
        }
    }

    private static string Format(double value) =>
        value.ToString("0.####", CultureInfo.InvariantCulture);

    // PpmTarget names its properties by symbol, so the symbol is the property name. WaterProfile is
    // the opposite — it spells the elements out — so the two are not interchangeable.
    private static double ValueOf(PpmTarget target, string symbol) => symbol switch
    {
        "N" => target.N.Value,
        "P" => target.P.Value,
        "K" => target.K.Value,
        "Ca" => target.Ca.Value,
        "Mg" => target.Mg.Value,
        "S" => target.S.Value,
        "Fe" => target.Fe.Value,
        "Cu" => target.Cu.Value,
        "Mn" => target.Mn.Value,
        "Zn" => target.Zn.Value,
        "B" => target.B.Value,
        "Mo" => target.Mo.Value,
        "Cl" => target.Cl.Value,
        "Si" => target.Si.Value,
        "Se" => target.Se.Value,
        "Na" => target.Na.Value,
        _ => 0,
    };

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

    /// <summary>How the source water is being described.</summary>
    public WaterInputMode Mode { get; set; } = WaterInputMode.Osmosis;

    /// <summary>The chosen water shape, by <see cref="WaterPreset.Id"/>.</summary>
    public string WaterPresetId { get; set; } = WaterPreset.CalciumBicarbonateModerate.Id;

    /// <summary>The meter reading, in whatever scale <see cref="WaterEcUnit"/> names.</summary>
    public double WaterEc { get; set; }

    /// <summary>The scale the meter reading is in.</summary>
    public EcUnit WaterEcUnit { get; set; } = EcUnit.MilliSiemensPerCm;

    /// <summary>General hardness in °dH, or null when not measured.</summary>
    public double? WaterGh { get; set; }

    /// <summary>Carbonate hardness in °dKH, or null when not measured.</summary>
    public double? WaterKh { get; set; }

    /// <summary>Whether the water is being acidified.</summary>
    public bool AcidEnabled { get; set; }

    /// <summary>The chosen acid, by <see cref="Nutrients.Acid.Id"/>.</summary>
    public string AcidId { get; set; } = Nutrients.Acid.Nitric60.Id;

    /// <summary>The pH to bring the solution to.</summary>
    public double TargetPh { get; set; } = 5.8;

    /// <summary>The pH of the untreated water.</summary>
    public double WaterPh { get; set; } = 7.6;

    /// <summary>The currently chosen shape, falling back to the default if the id is unknown.</summary>
    public WaterPreset Preset =>
        WaterPreset.All.FirstOrDefault(p => p.Id == WaterPresetId)
        ?? WaterPreset.CalciumBicarbonateModerate;

    /// <summary>
    /// Puts the chosen water type's own conductivity in the reading, unless a real one is there.
    /// </summary>
    /// <remarks>
    /// Picking a water type otherwise does nothing visible: the reading starts at zero, so the
    /// estimate is all zeros and the card sits dead until a number is typed. Seeding it makes the
    /// choice mean something immediately — here is what this class of water looks like — and the
    /// grower then corrects it to their own meter.
    /// <para>
    /// A reading the grower typed is never overwritten. The test is whether the current value is
    /// still some preset's nominal figure, which is exactly the case where nobody has typed anything.
    /// </para>
    /// </remarks>
    public void SeedReadingFromPreset()
    {
        bool untouched = WaterEc <= 0 || WaterPreset.All.Any(p => IsNominal(p, WaterEc));

        if (untouched)
        {
            WaterEc = FromMicroSiemens(Preset.NominalMicroSiemensPerCm);
        }
    }

    private bool IsNominal(WaterPreset preset, double reading) =>
        Math.Abs(reading - FromMicroSiemens(preset.NominalMicroSiemensPerCm)) < 0.005;

    private double FromMicroSiemens(double microSiemens) => WaterEcUnit switch
    {
        EcUnit.MilliSiemensPerCm => Math.Round(microSiemens / 1000, 2),
        EcUnit.Ppm500 => Math.Round(microSiemens / 1000 * 500),
        EcUnit.Ppm700 => Math.Round(microSiemens / 1000 * 700),
        _ => 0,
    };

    /// <summary>The meter reading converted to µS/cm, whatever scale it was entered in.</summary>
    public double WaterMicroSiemensPerCm => WaterEcUnit switch
    {
        EcUnit.MilliSiemensPerCm => WaterEc * 1000,
        EcUnit.Ppm500 => WaterEc / 500 * 1000,
        EcUnit.Ppm700 => WaterEc / 700 * 1000,
        _ => 0,
    };

    // ---------------------------------------------------------------- results

    public string? Error { get; private set; }
    public PpmTarget? Target { get; private set; }
    public WaterProfile WaterProfile { get; private set; } = WaterProfile.Pure;
    public double Alkalinity { get; private set; }
    public ConductivityEstimate? WaterConductivity { get; private set; }

    /// <summary>The inferred analysis, or null when the water was not estimated.</summary>
    public WaterEstimate? WaterEstimate { get; private set; }

    /// <summary>The acid plan, or null when no acid is needed or wanted.</summary>
    public AcidPlan? Acid { get; private set; }

    /// <summary>
    /// The source water plus whatever the acid contributes.
    /// </summary>
    /// <remarks>
    /// Distinct from <see cref="WaterProfile"/>, which stays the water as it comes out of the tap: the
    /// alkalinity and the water EC shown on screen describe the untreated supply, and folding the acid
    /// into them would make both wrong. This is what the recipe is solved against and what the reported
    /// tank contents are measured from — an acid's nitrogen is in the reservoir exactly as the water's
    /// is, and reporting one without the other would understate the tank by the whole dose.
    /// </remarks>
    private WaterProfile _effectiveWater = WaterProfile.Pure;
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

        Acid = null;
        _effectiveWater = WaterProfile;

        if (AcidEnabled && Alkalinity > 0)
        {
            Nutrients.Acid acid = Nutrients.Acid.All.FirstOrDefault(a => a.Id == AcidId)
                ?? Nutrients.Acid.Nitric60;

            Acid = AcidDose.Calculate(Alkalinity, WaterPh, TargetPh, acid, Liters);
            _effectiveWater = WithNutrient(WaterProfile, Acid.NutrientSymbol, Acid.NutrientPpm);
        }

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
        WaterAdjustedTarget adjusted = target.AdjustFor(_effectiveWater);
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

    /// <summary>Names of every catalogue salt, in display order — the ordering a link's indices mean.</summary>
    public IReadOnlyList<string> CatalogueNames => [.. Catalogue.Select(f => f.Name.Value)];

    /// <summary>Takes a snapshot of everything the person entered.</summary>
    public CalculatorState Capture() => new()
    {
        Target = TargetText,
        Water = new Dictionary<string, double>(Water, StringComparer.Ordinal),
        Salts = [.. Selected],
        ConcentrateLiters = ConcentrateLiters,
        WaterMode = Mode.ToString(),
        WaterPresetId = WaterPresetId,
        WaterEc = WaterEc,
        WaterEcUnit = WaterEcUnit.ToString(),
        WaterGh = WaterGh,
        WaterKh = WaterKh,
        AcidEnabled = AcidEnabled,
        AcidId = AcidId,
        TargetPh = TargetPh,
        WaterPh = WaterPh,
    };

    /// <summary>
    /// Applies a snapshot, ignoring anything it does not carry.
    /// </summary>
    /// <param name="state">The snapshot.</param>
    /// <remarks>
    /// Deliberately tolerant. A snapshot may come from an older version, a hand-edited file or a link
    /// someone truncated in a chat window, and the useful behaviour is to take what is readable rather
    /// than reject the lot. Unknown element keys and unknown salt names are dropped rather than added,
    /// so a stale file cannot introduce a salt the library no longer has.
    /// </remarks>
    public void Apply(CalculatorState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        if (!string.IsNullOrWhiteSpace(state.Target))
        {
            TargetText = state.Target;
        }

        foreach (string element in Water.Keys.ToList())
        {
            Water[element] = state.Water.TryGetValue(element, out double value) && value >= 0 ? value : 0;
        }

        if (state.Salts.Count > 0)
        {
            HashSet<string> known = [.. Catalogue.Select(f => f.Name.Value)];
            Selected.Clear();
            foreach (string name in state.Salts.Where(known.Contains))
            {
                Selected.Add(name);
            }
        }

        ConcentrateLiters = state.ConcentrateLiters;

        // Version 1 carried no mode. What the link itself says is the only honest inference: water
        // values mean somebody typed an analysis, and no water values mean they used osmosis.
        Mode = Enum.TryParse(state.WaterMode, out WaterInputMode mode)
            ? mode
            : Water.Values.Any(value => value > 0) ? WaterInputMode.Analysis : WaterInputMode.Osmosis;

        // An unknown preset or acid is dropped rather than accepted, the same way an unknown salt name
        // is: a stale file must not introduce something this version does not have.
        if (state.WaterPresetId is not null && WaterPreset.All.Any(p => p.Id == state.WaterPresetId))
        {
            WaterPresetId = state.WaterPresetId;
        }

        WaterEc = state.WaterEc ?? 0;
        WaterEcUnit = Enum.TryParse(state.WaterEcUnit, out EcUnit unit) ? unit : EcUnit.MilliSiemensPerCm;
        WaterGh = state.WaterGh;
        WaterKh = state.WaterKh;

        AcidEnabled = state.AcidEnabled ?? false;
        if (state.AcidId is not null && Nutrients.Acid.All.Any(a => a.Id == state.AcidId))
        {
            AcidId = state.AcidId;
        }

        TargetPh = state.TargetPh ?? 5.8;
        WaterPh = state.WaterPh ?? 7.6;
    }

    private WaterProfile BuildWater()
    {
        WaterEstimate = null;

        switch (Mode)
        {
            case WaterInputMode.Osmosis:
                return WaterProfile.Pure;

            case WaterInputMode.Conductivity:
            case WaterInputMode.ConductivityWithTests:
                // Drop tests are read only in the mode that shows them. Values left behind by a
                // previous mode would otherwise silently constrain an estimate nobody asked them to.
                bool withTests = Mode == WaterInputMode.ConductivityWithTests;
                WaterPreset preset = WaterPreset.All.FirstOrDefault(p => p.Id == WaterPresetId)
                    ?? WaterPreset.CalciumBicarbonateModerate;

                WaterEstimate = WaterEstimator.Estimate(
                    preset,
                    WaterMicroSiemensPerCm,
                    withTests ? WaterGh : null,
                    withTests ? WaterKh : null);

                return WaterEstimate.Profile;

            default:
                WaterProfileBuilder builder = new();
                builder.AddNitrate(Water["N"]).AddP(Water["P"]).AddK(Water["K"])
                    .AddCa(Water["Ca"]).AddMg(Water["Mg"]).AddS(Water["S"])
                    .AddFe(Water["Fe"]).AddCu(Water["Cu"]).AddMn(Water["Mn"]).AddZn(Water["Zn"])
                    .AddB(Water["B"]).AddMo(Water["Mo"]).AddCl(Water["Cl"]).AddSi(Water["Si"])
                    .AddSe(Water["Se"]).AddNa(Water["Na"]);
                return builder.Build();
        }
    }

    /// <summary>
    /// The same water with one element raised — how an acid's own nutrient enters the reservoir.
    /// </summary>
    /// <remarks>
    /// Rebuilt rather than mutated: a <see cref="WaterProfile"/> is immutable, and an acid contributes
    /// to the tank exactly as the water does, so it belongs in the same profile rather than in a
    /// separate adjustment every consumer would have to be told about.
    /// </remarks>
    private static WaterProfile WithNutrient(WaterProfile water, string symbol, double ppm)
    {
        WaterProfileBuilder builder = new();
        builder
            .AddNitrate(water.Nitrogen.Nitrate + (symbol == "N" ? ppm : 0))
            .AddAmmonium(water.Nitrogen.Ammonium)
            .AddAmine(water.Nitrogen.Amine)
            .AddP(water.Phosphorus.Value + (symbol == "P" ? ppm : 0))
            .AddK(water.Potassium.Value)
            .AddCa(water.Calcium.Value)
            .AddMg(water.Magnesium.Value)
            .AddS(water.Sulfur.Value + (symbol == "S" ? ppm : 0))
            .AddFe(water.Iron.Value).AddCu(water.Copper.Value).AddMn(water.Manganese.Value)
            .AddZn(water.Zinc.Value).AddB(water.Boron.Value).AddMo(water.Molybdenum.Value)
            .AddCl(water.Chlorine.Value).AddSi(water.Silicon.Value).AddSe(water.Selenium.Value)
            .AddNa(water.Sodium.Value);

        return builder.Build();
    }

    private RecipeView Describe(Solution mix)
    {
        // Measured, not assumed: the ppm comes back through the same calculator the library uses, and
        // the water is added on so the figures describe the reservoir rather than the salts alone.
        Ppm fromSalts = _calculator.CalculatePpm([.. mix], mix.WaterLiters);
        Ppm inTank = fromSalts.Plus(_effectiveWater);

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
