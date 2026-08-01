namespace SYT.NPKTools.Concentrates;

/// <summary>
/// How much of each salt will dissolve in a litre of water, in grams, at 20 °C.
/// </summary>
/// <remarks>
/// <para>
/// A concentrate is the one place in this library where a recipe can be arithmetically perfect and
/// physically impossible. <see cref="ConcentrateComponent.GramsPerLiter"/> says how strong a salt ends up
/// in its tank; this says how strong it can get. Without the second number the first is just a figure to
/// look at.
/// </para>
/// <para>
/// <b>These are screening values, not guarantees.</b> Three things make them approximate. Solubility rises
/// steeply with temperature, and 20 °C is a cool garage rather than a warm one. In a mixture the practical
/// limit for each salt is <em>lower</em> than in pure water, because salts sharing an ion crowd each other
/// out — two potassium salts in one tank will bind sooner than either alone. And a hydrate's figure
/// depends on which hydrate was weighed. So exceeding a limit here means the tank will certainly not
/// dissolve; staying under it means only that this particular obstacle is clear.
/// </para>
/// <para>
/// <b>Salts deliberately absent.</b> Calcium chloride hexahydrate, urea phosphate, manganese sulfate
/// monohydrate, the nitrate micro salts, the EDTA chelates, sodium silicate and sodium selenate carry no
/// entry, because the figures in circulation for them disagree by more than the check would be worth — a
/// chelate's solubility depends on the formulation, and a deliquescent hydrate's on which hydrate it
/// actually is. Guessing would be worse than declining: a wrong limit either blocks a tank that would have
/// mixed or passes one that will not. A salt with no entry is reported in
/// <see cref="ConcentratePlan.UnknownSolubility"/> rather than assumed to be fine, which is the whole
/// reason <see cref="Limit"/> returns null instead of a default.
/// </para>
/// </remarks>
public sealed class SolubilityTable
{
    private readonly Dictionary<string, double> _gramsPerLitre;

    /// <summary>
    /// Initializes a new instance of the <see cref="SolubilityTable"/> class.
    /// </summary>
    /// <param name="gramsPerLitreAt20C">
    /// Solubility in grams per litre of water at 20 °C, keyed by fertilizer name. Names are matched
    /// case-insensitively. Use <see cref="double.PositiveInfinity"/> for a salt that is miscible.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="gramsPerLitreAt20C"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any value is zero, negative or NaN.</exception>
    public SolubilityTable(IEnumerable<KeyValuePair<string, double>> gramsPerLitreAt20C)
    {
        ArgumentNullException.ThrowIfNull(gramsPerLitreAt20C);

        _gramsPerLitre = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        foreach ((string name, double limit) in gramsPerLitreAt20C)
        {
            if (double.IsNaN(limit) || limit <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(gramsPerLitreAt20C),
                    limit,
                    $"'{name}' has a solubility of {limit}, which is not a quantity that can dissolve.");
            }

            _gramsPerLitre[name] = limit;
        }
    }

    /// <summary>
    /// Gets the built-in table, covering the salts in the preset catalogue whose published figures agree.
    /// </summary>
    /// <remarks>
    /// Values are the ordinary handbook solubilities at 20 °C for the hydrate the catalogue names. The two
    /// worth knowing by heart are the low ones, because they are what a concentrate actually runs into:
    /// monocalcium phosphate at 18 g/L and boric acid at 49 g/L will bind long before potassium nitrate's
    /// 316 does.
    /// </remarks>
    public static SolubilityTable Default { get; } = new(new Dictionary<string, double>
    {
        // Macro salts.
        ["Calcium Nitrate Tetrahydrate"] = 1290,
        ["Potassium Nitrate"] = 316,
        ["Magnesium Sulfate Heptahydrate (MGS)"] = 710,
        ["Magnesium Nitrate Hexahydrate (MAG)"] = 1250,
        ["Potassium Dihydrogen Phosphate (MKP)"] = 226,
        ["Potassium Dibasic Phosphate"] = 1490,
        ["Monoammonium Phosphate"] = 383,
        ["Calcium Monobasic Phosphate"] = 18,
        ["Potassium Sulfate (SOP)"] = 111,
        ["Ammonium Sulfate"] = 754,
        ["Ammonium Nitrate"] = 1500,
        ["Ammonium Chloride"] = 372,
        ["Potassium Chloride"] = 344,
        ["Urea"] = 1080,
        ["Phosphoric Acid"] = double.PositiveInfinity,

        // Micro salts. Boric acid and borax matter most: they are barely soluble and boron is dosed high
        // enough for the limit to bind at concentrate strength.
        ["Boric Acid"] = 49,
        ["Sodium Borate Decahydrate"] = 51,
        ["Iron(II) Sulfate Heptahydrate"] = 256,
        ["Copper Sulfate Pentahydrate"] = 320,
        ["Zinc Sulfate Monohydrate"] = 600,
        ["Sodium Molybdate Dihydrate"] = 840,
    });

    /// <summary>
    /// Gets an empty table, for a caller who would rather supply every figure themselves.
    /// </summary>
    public static SolubilityTable Empty { get; } = new(new Dictionary<string, double>());

    /// <summary>
    /// Gets the number of salts the table knows about.
    /// </summary>
    public int Count => _gramsPerLitre.Count;

    /// <summary>
    /// Returns a copy of this table with one salt's limit added or replaced.
    /// </summary>
    /// <param name="fertilizerName">The fertilizer's name, matched case-insensitively.</param>
    /// <param name="gramsPerLitreAt20C">
    /// How much dissolves in a litre of water at 20 °C. A bag label or supplier datasheet is the right
    /// source for a salt of your own.
    /// </param>
    /// <returns>A new table; this one is unchanged.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fertilizerName"/> is null or blank.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="gramsPerLitreAt20C"/> is zero, negative or NaN.
    /// </exception>
    public SolubilityTable With(string fertilizerName, double gramsPerLitreAt20C)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fertilizerName);

        Dictionary<string, double> merged = new(_gramsPerLitre, StringComparer.OrdinalIgnoreCase)
        {
            [fertilizerName] = gramsPerLitreAt20C
        };

        return new SolubilityTable(merged);
    }

    /// <summary>
    /// Gets a salt's solubility, or null when the table has no figure for it.
    /// </summary>
    /// <param name="fertilizerName">The fertilizer's name, matched case-insensitively.</param>
    /// <returns>Grams per litre at 20 °C, or null when unknown.</returns>
    /// <remarks>
    /// Null rather than a default, so that "we do not know" cannot be mistaken for "this is fine". A custom
    /// salt gets null unless its author supplies a figure through <see cref="With"/>.
    /// </remarks>
    public double? Limit(string fertilizerName) =>
        fertilizerName is not null && _gramsPerLitre.TryGetValue(fertilizerName, out double limit)
            ? limit
            : null;
}
