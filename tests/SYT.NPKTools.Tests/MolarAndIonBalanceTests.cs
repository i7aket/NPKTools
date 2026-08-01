using AwesomeAssertions;
using SYT.NPKTools.Fertilizers;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Covers millimolar conversion and the milliequivalent charge balance.
/// </summary>
/// <remarks>
/// Two claims are being pinned. The first is arithmetic: ppm divided by atomic weight, which matters
/// because every published formulation is stated in mM and a recipe read off a paper table has to be
/// converted before it can be used. The second is chemical and less obvious — the gap between cations and
/// anions is not an error but the acid or base the recipe itself contributes, so the tests assert it comes
/// out at zero for neutral salts and at exactly one proton per phosphorus for the acid and basic ones.
/// </remarks>
public class MolarAndIonBalanceTests
{
    private const double Precision = 1e-6;

    // ---------------------------------------------------------------- millimolar

    /// <summary>
    /// One atomic weight in ppm is one millimole per litre, by definition. Checking three elements of
    /// very different weight pins the table as well as the arithmetic.
    /// </summary>
    [Theory]
    [InlineData(39.098, 1.0)]
    [InlineData(78.196, 2.0)]
    [InlineData(19.549, 0.5)]
    [Trait("Category", "Unit")]
    public void AsMillimolar_PotassiumAtItsAtomicWeight_IsOneMillimolePerAtomicWeight(
        double ppmValue,
        double expectedMillimolar)
    {
        // Arrange
        Ppm ppm = new PpmBuilder().AddK(ppmValue).AddLiters(100).Build();

        // Act
        MolarProfile result = ppm.AsMillimolar();

        // Assert
        result.Potassium.Should().BeApproximately(expectedMillimolar, 1e-4);
    }

    /// <summary>
    /// The reason mM exists in this library: equal ppm of calcium and magnesium are not equal amounts of
    /// anything. Magnesium's lower atomic weight means two-thirds again as many ions, and a grower reading
    /// only ppm cannot see that.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsMillimolar_EqualPpmOfCalciumAndMagnesium_AreNotEqualMillimolar()
    {
        // Arrange
        Ppm ppm = new PpmBuilder().AddCa(40).AddMg(40).AddLiters(100).Build();

        // Act
        MolarProfile result = ppm.AsMillimolar();

        // Assert
        result.Calcium.Should().BeApproximately(0.998, 0.001);
        result.Magnesium.Should().BeApproximately(1.646, 0.001);
    }

    /// <summary>
    /// The nitrogen forms convert separately and still sum to the total, because urea and nitrate behave
    /// differently and the difference is lost if only the total is carried across.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AsMillimolar_KeepsTheThreeNitrogenFormsAndSumsThem()
    {
        // Arrange
        Ppm ppm = new PpmBuilder().AddNitrate(140).AddAmmonium(14.007).AddAmine(28.014).AddLiters(100).Build();

        // Act
        MolarProfile result = ppm.AsMillimolar();

        // Assert
        result.Ammonium.Should().BeApproximately(1.0, 1e-4);
        result.Amine.Should().BeApproximately(2.0, 1e-4);
        result.Nitrogen.Should().BeApproximately(
            result.Nitrate + result.Ammonium + result.Amine, Precision);
    }

    // ---------------------------------------------------------------- milliequivalents

    /// <summary>
    /// Milliequivalents are millimoles times charge, so a divalent ion counts twice. This is why a
    /// solution table in meq looks nothing like the same table in mM.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void IonBalance_DivalentIonsCountTwice()
    {
        // Arrange — one millimole each of a monovalent and two divalent cations
        Ppm ppm = new PpmBuilder()
            .AddK(39.098).AddCa(40.078).AddMg(24.305)
            .AddLiters(100)
            .Build();

        // Act
        IonBalance result = ppm.IonBalance();

        // Assert
        result.Potassium.Should().BeApproximately(1.0, 1e-4);
        result.Calcium.Should().BeApproximately(2.0, 1e-4);
        result.Magnesium.Should().BeApproximately(2.0, 1e-4);
    }

    /// <summary>
    /// Urea is a neutral molecule. It supplies nitrogen, so it must show up in mM, and it carries no
    /// charge, so it must not show up in meq — which is also the reason it does nothing for a plant's
    /// cation-anion uptake balance.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void IonBalance_Urea_CountsAsNitrogenButNotAsCharge()
    {
        // Arrange
        Ppm ppm = new PpmBuilder().AddAmine(46.6).AddLiters(100).Build();

        // Act
        MolarProfile molar = ppm.AsMillimolar();
        IonBalance ions = ppm.IonBalance();

        // Assert
        molar.Amine.Should().BeGreaterThan(0);
        ions.Total.Should().Be(0);
        ions.AcidEquivalents.Should().Be(0);
    }

    /// <summary>
    /// Boron and silicon are undissociated acids at nutrient-solution pH, so they carry no charge at all.
    /// Micronutrients are excluded for a softer reason — their speciation is ambiguous and their
    /// contribution is under 0.1 meq/L — but the effect is the same and both are asserted here so a later
    /// change to include them cannot happen silently.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void IonBalance_MicronutrientsBoronAndSilicon_ContributeNoCharge()
    {
        // Arrange
        Ppm ppm = new PpmBuilder()
            .AddB(0.5).AddSi(10).AddFe(3).AddCu(0.05).AddMn(0.5).AddZn(0.15).AddMo(0.05).AddSe(0.01)
            .AddLiters(100)
            .Build();

        // Act
        IonBalance result = ppm.IonBalance();

        // Assert
        result.Total.Should().Be(0);
    }

    /// <summary>
    /// An empty profile must not divide by zero. A caller building a target up from nothing hits this.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void IonBalance_EmptyProfile_IsNeutralWithoutDividingByZero()
    {
        // Arrange
        Ppm ppm = new PpmBuilder().AddLiters(100).Build();

        // Act
        IonBalance result = ppm.IonBalance();

        // Assert
        result.RelativeDifference.Should().Be(0);
        result.IsChargeNeutral.Should().BeTrue();
    }

    // ---------------------------------------------------------------- acid-base character

    /// <summary>
    /// Neutral salts balance exactly, and the profile has to be derived from real salts for the claim to
    /// mean anything — an invented set of round ppm figures corresponds to no salt combination at all and
    /// would balance or not by accident. Monopotassium phosphate is the case worth naming: K⁺ against
    /// H₂PO₄⁻, one charge each, which is why it is the phosphorus source that does not move pH.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void IonBalance_ProfileMixedFromNeutralSalts_BalancesExactly()
    {
        // Arrange
        Fertilizer[] neutralSalts =
        [
            new FertilizerBuilder().AddName("Calcium Nitrate Tetrahydrate")
                .AddNo3(11.86).AddCaNonChelated(16.97).AddWeight(88).Build(),
            new FertilizerBuilder().AddName("Potassium Nitrate")
                .AddNo3(13.85).AddK(38.67).AddWeight(38).Build(),
            new FertilizerBuilder().AddName("Magnesium Sulfate Heptahydrate")
                .AddMgNonChelated(9.86).AddS(13.01).AddWeight(51).Build(),
            new FertilizerBuilder().AddName("Monopotassium Phosphate")
                .AddP(22.76).AddK(28.73).AddWeight(22).Build(),
        ];

        // Act
        Ppm ppm = NpkTools.CreatePpmCalculator().CalculatePpm(neutralSalts, waterLiters: 100);
        IonBalance result = ppm.IonBalance();

        // Assert
        result.Cations.Should().BeGreaterThan(10, "the mix is a realistic working strength");
        result.AcidEquivalents.Should().BeApproximately(0, 0.01);
        result.IsChargeNeutral.Should().BeTrue();
    }

    /// <summary>
    /// Phosphoric acid supplies H₂PO₄⁻ with a proton rather than a metal, so it shows an anion surplus of
    /// exactly one per phosphorus — which is the H⁺ it releases. This is the number that says how much
    /// less pH-down a recipe needs.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void IonBalance_PhosphoricAcid_ReleasesOneProtonPerPhosphorus()
    {
        // Arrange
        Ppm ppm = new PpmBuilder().AddP(31.0).AddLiters(100).Build();
        double phosphorusMillimolar = ppm.AsMillimolar().Phosphorus;

        // Act
        IonBalance result = ppm.IonBalance();

        // Assert
        result.Cations.Should().Be(0);
        result.AcidEquivalents.Should().BeApproximately(phosphorusMillimolar, Precision);
        result.IsChargeNeutral.Should().BeFalse();
    }

    /// <summary>
    /// Dipotassium phosphate is the mirror image: two K⁺ against an HPO₄²⁻ that takes up a proton on its
    /// way to H₂PO₄⁻ at working pH. The cation surplus of one per phosphorus is the H⁺ it consumes, so it
    /// pushes pH up rather than down.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void IonBalance_DipotassiumPhosphate_ConsumesOneProtonPerPhosphorus()
    {
        // Arrange — K₂HPO₄ is 17.8% P and 44.9% K, so 100 ppm of the salt in these proportions
        Ppm ppm = new PpmBuilder().AddP(17.8).AddK(44.9).AddLiters(100).Build();
        double phosphorusMillimolar = ppm.AsMillimolar().Phosphorus;

        // Act
        IonBalance result = ppm.IonBalance();

        // Assert — negative acidity: the salt is a base
        result.AcidEquivalents.Should().BeApproximately(-phosphorusMillimolar, 0.01);
        result.IsChargeNeutral.Should().BeFalse();
    }

    /// <summary>
    /// The relative figure is what makes the same threshold meaningful for a seedling feed and a
    /// full-strength recipe, so it must scale with concentration while the flag does not move.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void IonBalance_RelativeDifference_IsIndependentOfStrength()
    {
        // Arrange
        Ppm weak = new PpmBuilder().AddP(17.8).AddK(44.9).AddLiters(100).Build();
        Ppm strong = new PpmBuilder().AddP(35.6).AddK(89.8).AddLiters(100).Build();

        // Act
        IonBalance weakBalance = weak.IonBalance();
        IonBalance strongBalance = strong.IonBalance();

        // Assert
        strongBalance.Total.Should().BeApproximately(weakBalance.Total * 2, 1e-6);
        strongBalance.RelativeDifference.Should().BeApproximately(weakBalance.RelativeDifference, 1e-9);
    }

    // ---------------------------------------------------------------- guards

    /// <summary>
    /// Guards both extension methods against a null receiver.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void Conversions_NullProfile_Throw()
    {
        // Act & Assert
        ((Action)(() => PpmExtensions.AsMillimolar(null!))).Should().Throw<ArgumentNullException>();
        ((Action)(() => PpmExtensions.IonBalance(null!))).Should().Throw<ArgumentNullException>();
    }
}
