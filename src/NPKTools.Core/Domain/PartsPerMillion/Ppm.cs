using System.Text;
using NPKTools.Core.Common;
using NPKTools.Core.Constants;
using NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

namespace NPKTools.Core.Domain.PartsPerMillion;

/// <summary>
/// Represents the concentration of various nutrients in parts per million (ppm),
/// along with the total volume of water in liters in which these nutrients are intended to be dissolved.
/// </summary>
public sealed class Ppm
{
    /// <summary>
    /// Gets or sets the ppm value for nitrogen.
    /// </summary>
    public NitrogenPpm Nitrogen { get; }

    /// <summary>
    /// Gets or sets the ppm value for phosphorus.
    /// </summary>
    public PhosphorusPpm Phosphorus { get; }

    /// <summary>
    /// Gets or sets the ppm value for potassium.
    /// </summary>
    public PotassiumPpm Potassium { get; }

    /// <summary>
    /// Gets or sets the ppm value for calcium.
    /// </summary>
    public CalciumPpm Calcium { get; }

    /// <summary>
    /// Gets or sets the ppm value for magnesium.
    /// </summary>
    public MagnesiumPpm Magnesium { get; }

    /// <summary>
    /// Gets or sets the ppm value for sulfur.
    /// </summary>
    public SulfurPpm Sulfur { get; }

    /// <summary>
    /// Gets or sets the ppm value for iron.
    /// </summary>
    public IronPpm Iron { get; }

    /// <summary>
    /// Gets or sets the ppm value for copper.
    /// </summary>
    public CopperPpm Copper { get; }

    /// <summary>
    /// Gets or sets the ppm value for manganese.
    /// </summary>
    public ManganesePpm Manganese { get; }

    /// <summary>
    /// Gets or sets the ppm value for zinc.
    /// </summary>
    public ZincPpm Zinc { get; }

    /// <summary>
    /// Gets or sets the ppm value for boron.
    /// </summary>
    public BoronPpm Boron { get; }

    /// <summary>
    /// Gets or sets the ppm value for molybdenum.
    /// </summary>
    public MolybdenumPpm Molybdenum { get; }

    /// <summary>
    /// Gets or sets the ppm value for chlorine.
    /// </summary>
    public ChlorinePpm Chlorine { get; }

    /// <summary>
    /// Gets or sets the ppm value for silicon.
    /// </summary>
    public SiliconPpm Silicon { get; }

    /// <summary>
    /// Gets or sets the ppm value for selenium.
    /// </summary>
    public SeleniumPpm Selenium { get; }

    /// <summary>
    /// Gets or sets the ppm value for sodium.
    /// </summary>
    public SodiumPpm Sodium { get; }

    /// <summary>
    /// Gets or sets the volume of water in liters intended for dissolving the nutrients.
    /// </summary>
    public WaterVolumeLitersPpm Liters { get; }

    /// <summary>
    /// Calculates the combined ppm value of all the nutrients.
    /// </summary>
    public double Value => Nitrogen.Value + Phosphorus.Value + Potassium.Value + Calcium.Value + Magnesium.Value +
                           Sulfur.Value +
                           Iron.Value + Copper.Value + Manganese.Value + Zinc.Value + Boron.Value + Molybdenum.Value +
                           Chlorine.Value + Sodium.Value + Silicon.Value + Selenium.Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Ppm"/> class.
    /// </summary>
    /// <param name="nitrogen">Nitrogen (N) concentration in ppm.</param>
    /// <param name="phosphorus">Phosphorus (P) concentration in ppm.</param>
    /// <param name="potassium">Potassium (K) concentration in ppm.</param>
    /// <param name="calcium">Calcium (Ca) concentration in ppm.</param>
    /// <param name="magnesium">Magnesium (Mg) concentration in ppm.</param>
    /// <param name="sulfur">Sulfur (S) concentration in ppm.</param>
    /// <param name="iron">Iron (Fe) concentration in ppm.</param>
    /// <param name="copper">Copper (Cu) concentration in ppm.</param>
    /// <param name="manganese">Manganese (Mn) concentration in ppm.</param>
    /// <param name="zinc">Zinc (Zn) concentration in ppm.</param>
    /// <param name="boron">Boron (B) concentration in ppm.</param>
    /// <param name="molybdenum">Molybdenum (Mo) concentration in ppm.</param>
    /// <param name="chlorine">Chlorine (Cl) concentration in ppm.</param>
    /// <param name="silicon">Silicon (Si) concentration in ppm.</param>
    /// <param name="selenium">Selenium (Se) concentration in ppm.</param>
    /// <param name="sodium">Sodium (Na) concentration in ppm.</param>
    /// <param name="liters">The volume of water, in liters, the concentrations apply to.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    public Ppm(NitrogenPpm nitrogen, PhosphorusPpm phosphorus, PotassiumPpm potassium, CalciumPpm calcium,
        MagnesiumPpm magnesium, SulfurPpm sulfur, IronPpm iron,
        CopperPpm copper, ManganesePpm manganese, ZincPpm zinc, BoronPpm boron, MolybdenumPpm molybdenum,
        ChlorinePpm chlorine, SiliconPpm silicon,
        SeleniumPpm selenium, SodiumPpm sodium, WaterVolumeLitersPpm liters)
    {
        ArgumentNullException.ThrowIfNull(nitrogen);
        Nitrogen = nitrogen;

        ArgumentNullException.ThrowIfNull(phosphorus);
        Phosphorus = phosphorus;

        ArgumentNullException.ThrowIfNull(potassium);
        Potassium = potassium;

        ArgumentNullException.ThrowIfNull(calcium);
        Calcium = calcium;

        ArgumentNullException.ThrowIfNull(magnesium);
        Magnesium = magnesium;

        ArgumentNullException.ThrowIfNull(sulfur);
        Sulfur = sulfur;

        ArgumentNullException.ThrowIfNull(iron);
        Iron = iron;

        ArgumentNullException.ThrowIfNull(copper);
        Copper = copper;

        ArgumentNullException.ThrowIfNull(manganese);
        Manganese = manganese;

        ArgumentNullException.ThrowIfNull(zinc);
        Zinc = zinc;

        ArgumentNullException.ThrowIfNull(boron);
        Boron = boron;

        ArgumentNullException.ThrowIfNull(molybdenum);
        Molybdenum = molybdenum;

        ArgumentNullException.ThrowIfNull(chlorine);
        Chlorine = chlorine;

        ArgumentNullException.ThrowIfNull(silicon);
        Silicon = silicon;

        ArgumentNullException.ThrowIfNull(selenium);
        Selenium = selenium;

        ArgumentNullException.ThrowIfNull(sodium);
        Sodium = sodium;

        ArgumentNullException.ThrowIfNull(liters);
        Liters = liters;
    }

    /// <summary>
    /// Renders a multi-line report of the total ppm and every non-zero nutrient, with nitrogen
    /// broken down by chemical form. Uses the invariant culture, so output is reproducible.
    /// </summary>
    /// <returns>A multi-line report string.</returns>
    public string Report()
    {
        StringBuilder responseBuilder = new StringBuilder();

        responseBuilder.AppendLine($"{Labels.PpmReport}");
        ReportFormatter.AppendLineIfNonZero(responseBuilder, Labels.TotalPpm, Value);
        ReportFormatter.AppendLineIfNonZero(responseBuilder, Labels.Nitrogen, Nitrogen.Value);
        ReportFormatter.AppendLineIfNonZero(responseBuilder, $"{Labels.SubItemPrefix}{Labels.NitrateNo3}",
            Nitrogen.Nitrate);
        ReportFormatter.AppendLineIfNonZero(responseBuilder, $"{Labels.SubItemPrefix}{Labels.AmmoniumNh4}",
            Nitrogen.Ammonium);
        ReportFormatter.AppendLineIfNonZero(responseBuilder, $"{Labels.SubItemPrefix}{Labels.AmineNh2}", Nitrogen.Amine);
        ReportFormatter.AppendLineIfNonZero(responseBuilder, Labels.Phosphorus, Phosphorus.Value);
        ReportFormatter.AppendLineIfNonZero(responseBuilder, Labels.Potassium, Potassium.Value);
        ReportFormatter.AppendLineIfNonZero(responseBuilder, Labels.Magnesium, Magnesium.Value);
        ReportFormatter.AppendLineIfNonZero(responseBuilder, Labels.Sulfur, Sulfur.Value);
        ReportFormatter.AppendLineIfNonZero(responseBuilder, Labels.Calcium, Calcium.Value);
        ReportFormatter.AppendLineIfNonZero(responseBuilder, Labels.Iron, Iron.Value);
        ReportFormatter.AppendLineIfNonZero(responseBuilder, Labels.Copper, Copper.Value);
        ReportFormatter.AppendLineIfNonZero(responseBuilder, Labels.Manganese, Manganese.Value);
        ReportFormatter.AppendLineIfNonZero(responseBuilder, Labels.Zinc, Zinc.Value);
        ReportFormatter.AppendLineIfNonZero(responseBuilder, Labels.Boron, Boron.Value);
        ReportFormatter.AppendLineIfNonZero(responseBuilder, Labels.Molybdenum, Molybdenum.Value);
        ReportFormatter.AppendLineIfNonZero(responseBuilder, Labels.Chlorine, Chlorine.Value);
        ReportFormatter.AppendLineIfNonZero(responseBuilder, Labels.Silicon, Silicon.Value);
        ReportFormatter.AppendLineIfNonZero(responseBuilder, Labels.Selenium, Selenium.Value);
        ReportFormatter.AppendLineIfNonZero(responseBuilder, Labels.Sodium, Sodium.Value);

        return responseBuilder.ToString();
    }
}
