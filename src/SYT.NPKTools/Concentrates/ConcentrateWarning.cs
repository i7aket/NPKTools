using SYT.NPKTools.Fertilizers;

namespace SYT.NPKTools.Concentrates;

/// <summary>
/// What kind of problem a <see cref="ConcentrateWarning"/> describes.
/// </summary>
public enum ConcentrateWarningKind
{
    /// <summary>
    /// Salts in one tank that will precipitate each other once concentrated. This is the warning that
    /// ruins a batch: the tank goes cloudy, solids settle out, and the nutrients they carried never
    /// reach the plants even though the arithmetic was correct.
    /// </summary>
    PrecipitationRisk,

    /// <summary>
    /// A fertilizer carried no tank assignment, so one was inferred from its composition. Worth
    /// checking rather than trusting — the inference is a rule of thumb, not chemistry.
    /// </summary>
    TankInferred,

    /// <summary>
    /// A salt is asked to dissolve past its solubility, so the tank cannot be mixed as specified. The
    /// most certain of the three: precipitation is a prediction and an inferred tank is a guess, but this
    /// is arithmetic against a published figure. Dilute the concentrate or change the source of that
    /// element.
    /// </summary>
    SolubilityExceeded,

    /// <summary>
    /// The salts in one tank together need more water than the tank holds, even though none of them
    /// individually exceeds its own limit. They compete for the same water, so four salts each at 40% of
    /// their solubility will not dissolve. Less certain than
    /// <see cref="SolubilityExceeded"/> — it is a first-order screen rather than a single published
    /// figure — but it is the check that catches an over-concentrated tank of otherwise soluble salts.
    /// </summary>
    TankSaturated
}

/// <summary>
/// Something worth knowing before mixing a concentrate.
/// </summary>
/// <param name="Kind">What sort of problem this is.</param>
/// <param name="Tank">The tank it concerns.</param>
/// <param name="Fertilizers">The fertilizers involved, so a caller can point at them.</param>
/// <param name="Message">English prose, for a developer reading a log.</param>
/// <param name="Actual">
/// The figure that is wrong, for the kinds that have one: grams per litre needed for
/// <see cref="ConcentrateWarningKind.SolubilityExceeded"/>, the fraction of saturation for
/// <see cref="ConcentrateWarningKind.TankSaturated"/>. Null for the kinds that carry no number.
/// </param>
/// <param name="Allowed">
/// What that figure may be — grams per litre that dissolve, or 1 for a full tank. Null when
/// <paramref name="Actual"/> is.
/// </param>
/// <remarks>
/// <see cref="Message"/> is prose and cannot be translated by whoever shows it. The kind, the tank, the
/// fertilizers and the two figures are the same information as data, so an application can write the
/// sentence in the language of the person reading it — which for these warnings is somebody deciding
/// whether their concentrate will physically dissolve.
/// </remarks>
public sealed record ConcentrateWarning(
    ConcentrateWarningKind Kind,
    ConcentrateType Tank,
    IReadOnlyList<string> Fertilizers,
    string Message,
    double? Actual = null,
    double? Allowed = null);
