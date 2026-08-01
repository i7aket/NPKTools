using AwesomeAssertions;
using SYT.NPKTools.Nutrients;
using Xunit;

namespace SYT.NPKTools.Tests;

/// <summary>
/// Pins the physical constants the library computes from, against published values.
/// </summary>
/// <remarks>
/// <para>
/// The constants themselves are internal, so these tests reach them the way a caller would — through the
/// conversion that uses them. One atomic weight in ppm is one millimole per litre by definition, and one
/// millimole of an ion times its molar conductivity is that ion's µS/cm, so each constant can be read back
/// out of a single-element profile and compared with its reference value.
/// </para>
/// <para>
/// Worth pinning because these are the numbers no test would otherwise notice being wrong. A mistaken atomic
/// weight shifts every mM and meq figure by a few percent and nothing fails; a mistaken molar conductivity
/// moves EC in a way that looks like ordinary model error. All were verified against published tables — the
/// IUPAC standard atomic weights, and the CRC "Ionic Conductivity and Diffusion at Infinite Dilution" table
/// cross-checked against a second compilation. H₂PO₄⁻ is the one with real spread in the literature (31.8 to
/// 36 depending on the source), which matters little: phosphate is about 2% of a feed's conductivity, so the
/// whole spread moves total EC by under half a percent.
/// </para>
/// </remarks>
public class ReferenceDataTests
{
    // ---------------------------------------------------------------- atomic weights

    /// <summary>
    /// One atomic weight in ppm is one millimole per litre. Reading the constant back out this way pins it
    /// against the IUPAC standard atomic weights without exposing the internal table.
    /// </summary>
    [Theory]
    [InlineData("N", 14.007)]
    [InlineData("P", 30.974)]
    [InlineData("K", 39.098)]
    [InlineData("Ca", 40.078)]
    [InlineData("Mg", 24.305)]
    [InlineData("S", 32.06)]
    [InlineData("Fe", 55.845)]
    [InlineData("Cu", 63.546)]
    [InlineData("Mn", 54.938)]
    [InlineData("Zn", 65.38)]
    [InlineData("B", 10.81)]
    [InlineData("Mo", 95.95)]
    [InlineData("Cl", 35.45)]
    [InlineData("Si", 28.085)]
    [InlineData("Se", 78.971)]
    [InlineData("Na", 22.990)]
    [Trait("Category", "Unit")]
    public void AsMillimolar_OneAtomicWeightInPpm_IsExactlyOneMillimolar(string element, double atomicWeight)
    {
        // Arrange
        PpmBuilder builder = new();
        builder = element switch
        {
            "N" => builder.AddNitrate(atomicWeight),
            "P" => builder.AddP(atomicWeight),
            "K" => builder.AddK(atomicWeight),
            "Ca" => builder.AddCa(atomicWeight),
            "Mg" => builder.AddMg(atomicWeight),
            "S" => builder.AddS(atomicWeight),
            "Fe" => builder.AddFe(atomicWeight),
            "Cu" => builder.AddCu(atomicWeight),
            "Mn" => builder.AddMn(atomicWeight),
            "Zn" => builder.AddZn(atomicWeight),
            "B" => builder.AddB(atomicWeight),
            "Mo" => builder.AddMo(atomicWeight),
            "Cl" => builder.AddCl(atomicWeight),
            "Si" => builder.AddSi(atomicWeight),
            "Se" => builder.AddSe(atomicWeight),
            _ => builder.AddNa(atomicWeight),
        };

        MolarProfile molar = builder.AddLiters(100).Build().AsMillimolar();

        // Act
        double millimolar = element switch
        {
            "N" => molar.Nitrate,
            "P" => molar.Phosphorus,
            "K" => molar.Potassium,
            "Ca" => molar.Calcium,
            "Mg" => molar.Magnesium,
            "S" => molar.Sulfur,
            "Fe" => molar.Iron,
            "Cu" => molar.Copper,
            "Mn" => molar.Manganese,
            "Zn" => molar.Zinc,
            "B" => molar.Boron,
            "Mo" => molar.Molybdenum,
            "Cl" => molar.Chlorine,
            "Si" => molar.Silicon,
            "Se" => molar.Selenium,
            _ => molar.Sodium,
        };

        // Assert
        millimolar.Should().BeApproximately(1.0, 1e-12);
    }

    // ---------------------------------------------------------------- molar conductivities

    /// <summary>
    /// One millimole of an ion contributes its molar conductivity in µS/cm, so the constants read back out of
    /// a single-ion profile. Compared against the CRC infinite-dilution table.
    /// </summary>
    [Theory]
    [InlineData("K", 39.098, 73.5)]
    [InlineData("Na", 22.990, 50.1)]
    [InlineData("Ca", 40.078, 119.0)]
    [InlineData("Mg", 24.305, 106.0)]
    [InlineData("Cl", 35.45, 76.3)]
    [InlineData("S", 32.06, 160.0)]
    [InlineData("P", 30.974, 36.0)]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_OneMillimolarIon_ContributesItsMolarConductivity(
        string element,
        double atomicWeight,
        double expectedMicroSiemens)
    {
        // Arrange
        PpmBuilder builder = new();
        builder = element switch
        {
            "K" => builder.AddK(atomicWeight),
            "Na" => builder.AddNa(atomicWeight),
            "Ca" => builder.AddCa(atomicWeight),
            "Mg" => builder.AddMg(atomicWeight),
            "Cl" => builder.AddCl(atomicWeight),
            "S" => builder.AddS(atomicWeight),
            _ => builder.AddP(atomicWeight),
        };

        ConductivityEstimate estimate = builder.AddLiters(100).Build().EstimateConductivity();

        // Act
        double contribution = element switch
        {
            "K" => estimate.Potassium,
            "Na" => estimate.Sodium,
            "Ca" => estimate.Calcium,
            "Mg" => estimate.Magnesium,
            "Cl" => estimate.Chloride,
            "S" => estimate.Sulfate,
            _ => estimate.Phosphate,
        };

        // Assert
        contribution.Should().BeApproximately(expectedMicroSiemens, 1e-9);
    }

    /// <summary>
    /// Ammonium and bicarbonate have no element of their own to pin through, so they are read from the ion
    /// they belong to. NH₄⁺ shares potassium's mobility almost exactly, which is why they carry the same
    /// figure.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_AmmoniumAndBicarbonate_MatchTheirReferenceValues()
    {
        // Arrange
        Ppm ammonium = new PpmBuilder().AddAmmonium(14.007).AddLiters(100).Build();

        // Act & Assert
        ammonium.EstimateConductivity().Ammonium.Should().BeApproximately(73.5, 1e-9);

        Ppm blank = new PpmBuilder().AddLiters(100).Build();
        blank.EstimateConductivity(bicarbonateMeqPerLitre: 1).Bicarbonate
            .Should().BeApproximately(44.5, 1e-9);
    }

    /// <summary>
    /// The divalent conductivities are per mole of ion, not per equivalent — the distinction that halves or
    /// doubles a figure depending on which convention a table uses. Calcium's 119.0 is 2 × 59.5 and sulfate's
    /// 160.0 is 2 × 80.0, so each divalent ion must come out above every monovalent one here.
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_DivalentConductivities_ArePerMoleNotPerEquivalent()
    {
        // Arrange — one millimole of each
        Ppm profile = new PpmBuilder()
            .AddCa(40.078).AddMg(24.305).AddS(32.06).AddK(39.098)
            .AddLiters(100)
            .Build();

        // Act
        ConductivityEstimate result = profile.EstimateConductivity();

        // Assert
        result.Calcium.Should().BeApproximately(2 * 59.5, 0.01);
        result.Sulfate.Should().BeApproximately(2 * 80.0, 0.01);
        result.Magnesium.Should().BeGreaterThan(result.Potassium);
    }

    // ---------------------------------------------------------------- the external anchor

    /// <summary>
    /// The certified KCl conductivity standards, which are what a meter is calibrated against and the only
    /// point where this library can be checked against an external authority rather than against itself.
    /// </summary>
    [Theory]
    [InlineData(0.001, 147.0, 0.005)]
    [InlineData(0.01, 1412.0, 0.005)]
    [InlineData(0.1, 12880.0, 0.05)]
    [Trait("Category", "Unit")]
    public void EstimateConductivity_AgainstTheCertifiedKclStandards_StaysWithinItsStatedError(
        double molarity,
        double certifiedMicroSiemens,
        double tolerance)
    {
        // Arrange
        Ppm kcl = new PpmBuilder()
            .AddK(molarity * 39098).AddCl(molarity * 35450)
            .AddLiters(100)
            .Build();

        // Act
        double estimate = kcl.EstimateConductivity().MicroSiemensPerCm;

        // Assert
        Math.Abs(estimate / certifiedMicroSiemens - 1).Should().BeLessThan(tolerance);
    }
}
