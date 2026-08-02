namespace SYT.NPKTools.Nutrients;

/// <summary>
/// The shape of an ordinary source water — the proportions between its ions, not a composition.
/// </summary>
/// <remarks>
/// <para>
/// A grower with an EC meter knows how much is dissolved in their water and nothing about what. A
/// preset supplies the missing half: which ions, in what ratio. <see cref="WaterEstimator"/> then
/// scales the shape until its computed conductivity matches the meter, so the numbers that come out
/// are consistent with a real reading rather than invented.
/// </para>
/// <para>
/// Bicarbonate is not a field. The library infers it from the cation surplus, which is what makes
/// six numbers enough to describe water that is mostly calcium bicarbonate.
/// </para>
/// <para>
/// These are shapes of water classes, not of particular supplies. A grower with a laboratory
/// analysis should enter it; a preset is for the grower who has none.
/// </para>
/// </remarks>
public sealed record WaterPreset
{
    private WaterPreset(
        string id,
        string label,
        double calcium,
        double magnesium,
        double sodium,
        double sulfur,
        double chlorine,
        double nitrogen)
    {
        Id = id;
        Label = label;
        Calcium = calcium;
        Magnesium = magnesium;
        Sodium = sodium;
        Sulfur = sulfur;
        Chlorine = chlorine;
        Nitrogen = nitrogen;
    }

    /// <summary>Gets the stable identifier, safe to persist in a link or a file.</summary>
    public string Id { get; }

    /// <summary>Gets the name to show.</summary>
    public string Label { get; }

    /// <summary>Gets the calcium at nominal scale, in ppm.</summary>
    public double Calcium { get; }

    /// <summary>Gets the magnesium at nominal scale, in ppm.</summary>
    public double Magnesium { get; }

    /// <summary>Gets the sodium at nominal scale, in ppm.</summary>
    public double Sodium { get; }

    /// <summary>Gets the sulfur at nominal scale, in ppm, as elemental S.</summary>
    public double Sulfur { get; }

    /// <summary>Gets the chlorine at nominal scale, in ppm.</summary>
    public double Chlorine { get; }

    /// <summary>Gets the nitrogen at nominal scale, in ppm, as nitrate.</summary>
    public double Nitrogen { get; }

    /// <summary>
    /// Surface water from granite country, and rain-fed reservoirs. Little of anything.
    /// </summary>
    public static WaterPreset SoftLowAlkalinity { get; } =
        new("SoftLowAlkalinity", "Soft, low alkalinity", 18, 4, 8, 4, 9, 1);

    /// <summary>
    /// The commonest municipal supply: hardness and alkalinity both from limestone.
    /// </summary>
    public static WaterPreset CalciumBicarbonateModerate { get; } =
        new("CalciumBicarbonateModerate", "Calcium bicarbonate, moderately hard", 55, 11, 16, 13, 21, 3);

    /// <summary>
    /// Groundwater from chalk or limestone aquifers. Enough alkalinity to move root-zone pH all season.
    /// </summary>
    public static WaterPreset CalciumBicarbonateHard { get; } =
        new("CalciumBicarbonateHard", "Calcium bicarbonate, hard", 105, 22, 28, 32, 38, 5);

    /// <summary>
    /// Water from a domestic ion-exchange softener, which trades calcium for sodium.
    /// </summary>
    /// <remarks>
    /// Conductivity stays high while the calcium is gone, so a meter alone cannot tell this apart from
    /// hard water. Estimated as hard, it would promise calcium that is not there while understating
    /// sodium that is — the most expensive of the ordinary mistakes, and the reason this preset exists.
    /// </remarks>
    public static WaterPreset SodiumExchangeSoftened { get; } =
        new("SodiumExchangeSoftened", "Softened by sodium exchange", 4, 1, 120, 14, 30, 2);

    /// <summary>Gets every preset, in rising order of dissolved content.</summary>
    public static IReadOnlyList<WaterPreset> All { get; } =
    [
        SoftLowAlkalinity,
        CalciumBicarbonateModerate,
        CalciumBicarbonateHard,
        SodiumExchangeSoftened,
    ];

    /// <summary>
    /// Builds the water this shape describes, at a given scale.
    /// </summary>
    /// <param name="scale">
    /// A multiplier on every ion. One is the nominal composition; the estimator solves for the scale
    /// that reproduces a measured conductivity.
    /// </param>
    /// <returns>The scaled analysis.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="scale"/> is negative.</exception>
    public WaterProfile ToProfile(double scale = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(scale);

        return new WaterProfileBuilder()
            .AddCa(Calcium * scale)
            .AddMg(Magnesium * scale)
            .AddNa(Sodium * scale)
            .AddS(Sulfur * scale)
            .AddCl(Chlorine * scale)
            .AddNitrate(Nitrogen * scale)
            .Build();
    }
}
