using AwesomeAssertions;
using SYT.NPKTools.Fertilizers;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers generating optimization bundles from a caller's own salts.
/// </summary>
/// <remarks>
/// The shape being pinned is hold-one-out: everything on the shelf, then the shelf minus each salt. It
/// was chosen by measurement rather than taste — against the hand-written catalogue on three macro
/// targets it returned more distinct recipes — so the tests fix the structure that measurement picked,
/// and the reporting that keeps a short answer from looking like a complete one.
/// </remarks>
public class FertilizerBundleGeneratorTests
{
    private static Fertilizer Salt(
        string name,
        double n = 0,
        double p = 0,
        double k = 0,
        double ca = 0,
        double mg = 0,
        double s = 0,
        double fe = 0,
        double na = 0,
        double cl = 0) =>
        new FertilizerBuilder()
            .AddName(name)
            .AddNo3(n).AddP(p).AddK(k).AddCaNonChelated(ca).AddMgNonChelated(mg).AddS(s)
            .AddFeNonChelated(fe).AddNa(na).AddCl(cl)
            .Build();

    private static Fertilizer[] Shelf() =>
    [
        Salt("Calcium Nitrate", n: 11.86, ca: 16.97),
        Salt("Potassium Nitrate", n: 13.85, k: 38.67),
        Salt("Magnesium Sulfate", mg: 9.86, s: 13.01),
        Salt("Monopotassium Phosphate", p: 22.76, k: 28.73),
    ];

    // ---------------------------------------------------------------- structure

    /// <summary>
    /// The generated shape is one bundle per salt plus one holding everything. Alternatives can only come
    /// from taking a salt away: every superset of the best mix yields that same mix again, so a bundle
    /// list that only grew would be one recipe repeated.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GenerateMacro_ProducesTheFullShelfPlusOneBundlePerSaltHeldOut()
    {
        // Arrange
        Fertilizer[] shelf = Shelf();

        // Act
        GeneratedBundles result = FertilizerBundleGenerator.GenerateMacro(shelf);

        // Assert
        result.Bundles.Should().HaveCount(shelf.Length + 1);
        result.Bundles[0].Should().HaveCount(shelf.Length);
        result.Bundles.Skip(1).Should().AllSatisfy(bundle => bundle.Should().HaveCount(shelf.Length - 1));
    }

    /// <summary>
    /// Every salt must be the one held out exactly once, otherwise some alternative recipe is silently
    /// unavailable. This is also what lets a caller label a bundle "without the MKP" by set difference
    /// against the first.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GenerateMacro_HoldsOutEachSaltExactlyOnce()
    {
        // Arrange
        Fertilizer[] shelf = Shelf();

        // Act
        GeneratedBundles result = FertilizerBundleGenerator.GenerateMacro(shelf);

        // Assert
        IEnumerable<string> heldOut = result.Bundles
            .Skip(1)
            .Select(bundle => result.Bundles[0].Except(bundle).Single().Name.Value);

        heldOut.Should().BeEquivalentTo(shelf.Select(f => f.Name.Value));
    }

    /// <summary>
    /// Order in, order out: a caller who lists their salts differently must get the same bundles, or two
    /// runs of the same shelf would disagree about which recipes exist.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GenerateMacro_IsIndependentOfTheOrderSaltsAreSuppliedIn()
    {
        // Arrange
        Fertilizer[] shelf = Shelf();
        Fertilizer[] reversed = [.. shelf.Reverse()];

        // Act
        GeneratedBundles first = FertilizerBundleGenerator.GenerateMacro(shelf);
        GeneratedBundles second = FertilizerBundleGenerator.GenerateMacro(reversed);

        // Assert
        first.Bundles.Select(b => b.Select(f => f.Name.Value))
            .Should().BeEquivalentTo(
                second.Bundles.Select(b => b.Select(f => f.Name.Value)),
                options => options.WithStrictOrdering());
    }

    /// <summary>
    /// One salt cannot be reduced any further, so it yields the one bundle it is — not an empty second
    /// bundle that could never solve anything.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GenerateMacro_SingleSalt_ProducesOneBundle()
    {
        // Act
        GeneratedBundles result = FertilizerBundleGenerator.GenerateMacro([Salt("Potassium Nitrate", n: 13.85, k: 38.67)]);

        // Assert
        result.Bundles.Should().HaveCount(1);
        result.Bundles[0].Should().HaveCount(1);
    }

    /// <summary>
    /// The same salt listed twice is one salt. Left in, it would become two identical columns in the
    /// linear program and the recipe would list it twice.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GenerateMacro_DuplicateSalts_AreCountedOnce()
    {
        // Arrange
        Fertilizer[] shelf =
        [
            Salt("Potassium Nitrate", n: 13.85, k: 38.67),
            Salt("potassium nitrate", n: 13.85, k: 38.67),
        ];

        // Act
        GeneratedBundles result = FertilizerBundleGenerator.GenerateMacro(shelf);

        // Assert
        result.Bundles.Should().HaveCount(1);
        result.Bundles[0].Should().HaveCount(1);
    }

    // ---------------------------------------------------------------- tier split

    /// <summary>
    /// A salt carrying a micronutrient belongs to the micro tier even when it also carries a macro
    /// element. Iron sulfate's sulfur is incidental: dosing it to meet a sulfur target would mean iron at
    /// a hundred times the intended rate.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Generate_IronSulfate_IsAMicroSaltDespiteItsSulfur()
    {
        // Arrange
        Fertilizer ironSulfate = Salt("Iron Sulfate", fe: 20.09, s: 11.53);
        Fertilizer[] shelf = [.. Shelf(), ironSulfate];

        // Act
        GeneratedBundles macro = FertilizerBundleGenerator.GenerateMacro(shelf);
        GeneratedBundles micro = FertilizerBundleGenerator.GenerateMicro(shelf);

        // Assert
        FertilizerBundleGenerator.IsMicro(ironSulfate).Should().BeTrue();
        macro.Bundles[0].Should().NotContain(ironSulfate);
        micro.Bundles[0].Should().Contain(ironSulfate);
    }

    /// <summary>
    /// A shelf holding nothing this tier can use produces no bundles rather than one empty bundle, which
    /// the optimizer would spend a solve on for nothing.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GenerateMicro_ShelfOfOnlyMacroSalts_ProducesNothing()
    {
        // Act
        GeneratedBundles result = FertilizerBundleGenerator.GenerateMicro(Shelf());

        // Assert
        result.Bundles.Should().BeEmpty();
        result.Should().Be(GeneratedBundles.Empty);
    }

    // ---------------------------------------------------------------- reporting

    /// <summary>
    /// An element nothing on the shelf supplies is the difference between "no solutions" and "you have no
    /// magnesium source". Returning the former and leaving the caller to work out the latter is the
    /// failure this reporting exists to prevent.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GenerateMacro_ElementNoSaltSupplies_IsReportedAsUncovered()
    {
        // Arrange — no magnesium and no sulfur anywhere on the shelf
        Fertilizer[] shelf =
        [
            Salt("Calcium Nitrate", n: 11.86, ca: 16.97),
            Salt("Monopotassium Phosphate", p: 22.76, k: 28.73),
        ];

        // Act
        GeneratedBundles result = FertilizerBundleGenerator.GenerateMacro(shelf);

        // Assert
        result.UncoveredElements.Should().BeEquivalentTo(["Mg", "S"]);
        result.IsComplete.Should().BeFalse();
        result.Bundles.Should().NotBeEmpty("the elements that are covered can still be met");
    }

    /// <summary>
    /// A shelf that covers everything reports nothing outstanding, so <c>IsComplete</c> is usable as the
    /// single check a caller makes.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GenerateMacro_ShelfCoveringEveryElement_IsComplete()
    {
        // Act
        GeneratedBundles result = FertilizerBundleGenerator.GenerateMacro(Shelf());

        // Assert
        result.UncoveredElements.Should().BeEmpty();
        result.BundlesDropped.Should().Be(0);
        result.IsComplete.Should().BeTrue();
    }

    /// <summary>
    /// Hitting the count limit must be visible. A truncated list that reported nothing would read as
    /// "these are all the options" when it was not.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void GenerateMacro_MoreBundlesThanTheLimit_ReportsWhatWasDropped()
    {
        // Arrange
        BundleGenerationSettings settings = new() { MaxBundles = 3 };

        // Act
        GeneratedBundles result = FertilizerBundleGenerator.GenerateMacro(Shelf(), settings);

        // Assert
        result.Bundles.Should().HaveCount(3);
        result.BundlesDropped.Should().Be(2, "four salts yield five bundles, of which three were kept");
        result.IsComplete.Should().BeFalse();
    }

    /// <summary>
    /// A salt of nothing but sodium and chloride is real, but there is no target it helps meet. Left
    /// unreported it would look included and every recipe would read as though it had been considered.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Repository_SaltCarryingNoTargetableElement_IsReportedAsUnusable()
    {
        // Arrange
        Fertilizer[] shelf = [.. Shelf(), Salt("Sodium Chloride", na: 39.34, cl: 60.66)];

        // Act
        CustomFertilizerBundleRepository repository = new(shelf);

        // Assert
        repository.UnusableSalts.Should().BeEquivalentTo(["Sodium Chloride"]);
        repository.Macro()[0].Select(f => f.Name.Value).Should().NotContain("Sodium Chloride");
    }

    // ---------------------------------------------------------------- guards

    /// <summary>
    /// A limit of zero would mean no bundles at all, which is a caller bug rather than a valid request.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [Trait("Category", "Unit")]
    public void GenerateMacro_NonPositiveBundleLimit_Throws(int maxBundles)
    {
        // Arrange
        BundleGenerationSettings settings = new() { MaxBundles = maxBundles };

        // Act
        Action act = () => FertilizerBundleGenerator.GenerateMacro(Shelf(), settings);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName(nameof(BundleGenerationSettings.MaxBundles));
    }

    /// <summary>
    /// Guards the null cases on every public entry point.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Generate_NullArguments_Throw()
    {
        // Act & Assert
        ((Action)(() => FertilizerBundleGenerator.GenerateMacro(null!)))
            .Should().Throw<ArgumentNullException>();
        ((Action)(() => FertilizerBundleGenerator.GenerateMicro(null!)))
            .Should().Throw<ArgumentNullException>();
        ((Action)(() => FertilizerBundleGenerator.IsMicro(null!)))
            .Should().Throw<ArgumentNullException>();
        ((Action)(() => FertilizerBundleGenerator.IsUsable(null!)))
            .Should().Throw<ArgumentNullException>();
        ((Action)(() => new CustomFertilizerBundleRepository(null!)))
            .Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// An empty shelf is not an error — a caller may be building a list up — but it must not produce a
    /// bundle of nothing.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Generate_EmptyShelf_ProducesNothing()
    {
        // Act
        CustomFertilizerBundleRepository repository = new([]);

        // Assert
        repository.Macro().Should().BeEmpty();
        repository.Micro().Should().BeEmpty();
        repository.UnusableSalts.Should().BeEmpty();
    }
}
