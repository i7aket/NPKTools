using SYT.NPKTools.Internal;

namespace SYT.NPKTools.Nutrients;

/// <summary>
/// The acids used to neutralise the alkalinity of source water.
/// </summary>
public enum AcidKind
{
    /// <summary>Nitric acid, HNO₃. Contributes nitrate.</summary>
    Nitric,

    /// <summary>Phosphoric acid, H₃PO₄. Contributes phosphorus.</summary>
    Phosphoric,

    /// <summary>Sulfuric acid, H₂SO₄. Contributes sulfur.</summary>
    Sulfuric,
}

/// <summary>
/// A bottle of acid: what it is, how strong, and what it brings to the solution besides acidity.
/// </summary>
/// <remarks>
/// <para>
/// Strength is derived, never stored. Equivalents per litre follow from the percentage by weight, the
/// density and the protons the molecule can actually give up at working pH, so a mistyped density
/// changes the dose rather than quietly disagreeing with it.
/// </para>
/// <para>
/// The protons that count are the ones available above pH 5.5. Nitric gives one. Phosphoric gives one:
/// its second dissociation is at pKa 7.20, out of reach of a solution run at 5.8. Sulfuric gives two,
/// its second at pKa 1.99 being fully dissociated. Counting phosphoric as three-protic — an easy
/// mistake — would understate the dose threefold.
/// </para>
/// </remarks>
public sealed record Acid
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Acid"/> record.
    /// </summary>
    /// <param name="kind">Which acid it is.</param>
    /// <param name="percentByWeight">Concentration, as a percentage by weight.</param>
    /// <param name="densityGramsPerMillilitre">Density of the liquid, in g/mL.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the percentage is outside 0–100, or the density is not positive.
    /// </exception>
    public Acid(AcidKind kind, double percentByWeight, double densityGramsPerMillilitre)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(percentByWeight);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(percentByWeight, 100);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(densityGramsPerMillilitre);

        Kind = kind;
        PercentByWeight = percentByWeight;
        DensityGramsPerMillilitre = densityGramsPerMillilitre;
        Id = string.Create(
            System.Globalization.CultureInfo.InvariantCulture,
            $"{kind}{percentByWeight:0.##}");
        Label = string.Create(
            System.Globalization.CultureInfo.InvariantCulture,
            $"{kind} acid {percentByWeight:0.##}%");
    }

    private Acid(AcidKind kind, double percentByWeight, double density, string id, string label)
        : this(kind, percentByWeight, density)
    {
        Id = id;
        Label = label;
    }

    /// <summary>Gets a stable identifier, safe to persist in a link or a file.</summary>
    public string Id { get; }

    /// <summary>Gets the name to show.</summary>
    public string Label { get; }

    /// <summary>Gets which acid it is.</summary>
    public AcidKind Kind { get; }

    /// <summary>Gets the concentration, as a percentage by weight.</summary>
    public double PercentByWeight { get; }

    /// <summary>Gets the density of the liquid, in g/mL.</summary>
    public double DensityGramsPerMillilitre { get; }

    /// <summary>Gets the weight of acid that supplies one mole of usable protons, in g/mol.</summary>
    public double EquivalentWeight => Kind switch
    {
        AcidKind.Nitric => 63.012,
        AcidKind.Phosphoric => 97.994,
        AcidKind.Sulfuric => 98.079 / 2,
        _ => throw new InvalidOperationException($"Unknown acid kind '{Kind}'."),
    };

    /// <summary>Gets the element symbol the acid contributes to the solution.</summary>
    public string NutrientSymbol => Kind switch
    {
        AcidKind.Nitric => Names.N,
        AcidKind.Phosphoric => Names.P,
        AcidKind.Sulfuric => Names.S,
        _ => throw new InvalidOperationException($"Unknown acid kind '{Kind}'."),
    };

    /// <summary>Gets the nutrient delivered per milliequivalent of acid, in milligrams.</summary>
    public double MilligramsOfNutrientPerMilliequivalent => Kind switch
    {
        AcidKind.Nitric => AtomicMasses.N,
        AcidKind.Phosphoric => AtomicMasses.P,
        AcidKind.Sulfuric => AtomicMasses.S / 2,
        _ => throw new InvalidOperationException($"Unknown acid kind '{Kind}'."),
    };

    /// <summary>Gets the usable protons per litre of the liquid, in equivalents.</summary>
    public double EquivalentsPerLitre =>
        PercentByWeight / 100 * DensityGramsPerMillilitre * 1000 / EquivalentWeight;

    /// <summary>Technical-grade nitric acid, 60%.</summary>
    public static Acid Nitric60 { get; } =
        new(AcidKind.Nitric, 60, 1.367, "Nitric60", "Nitric acid 60%");

    /// <summary>Dilute nitric acid, 38%.</summary>
    public static Acid Nitric38 { get; } =
        new(AcidKind.Nitric, 38, 1.234, "Nitric38", "Nitric acid 38%");

    /// <summary>Concentrated orthophosphoric acid, 85%.</summary>
    public static Acid Phosphoric85 { get; } =
        new(AcidKind.Phosphoric, 85, 1.685, "Phosphoric85", "Phosphoric acid 85%");

    /// <summary>Orthophosphoric acid, 75%.</summary>
    public static Acid Phosphoric75 { get; } =
        new(AcidKind.Phosphoric, 75, 1.579, "Phosphoric75", "Phosphoric acid 75%");

    /// <summary>Concentrated sulfuric acid, 98%.</summary>
    public static Acid Sulfuric98 { get; } =
        new(AcidKind.Sulfuric, 98, 1.836, "Sulfuric98", "Sulfuric acid 98%");

    /// <summary>Battery-strength sulfuric acid, 37%.</summary>
    public static Acid Sulfuric37 { get; } =
        new(AcidKind.Sulfuric, 37, 1.276, "Sulfuric37", "Sulfuric acid 37%");

    /// <summary>Gets every built-in acid.</summary>
    public static IReadOnlyList<Acid> All { get; } =
        [Nitric60, Nitric38, Phosphoric85, Phosphoric75, Sulfuric98, Sulfuric37];
}
