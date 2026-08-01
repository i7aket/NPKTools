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
    TankInferred
}

/// <summary>
/// Something worth knowing before mixing a concentrate.
/// </summary>
/// <param name="Kind">What sort of problem this is.</param>
/// <param name="Tank">The tank it concerns.</param>
/// <param name="Fertilizers">The fertilizers involved, so a caller can point at them.</param>
/// <param name="Message">A description suitable for showing to a person.</param>
public sealed record ConcentrateWarning(
    ConcentrateWarningKind Kind,
    ConcentrateType Tank,
    IReadOnlyList<string> Fertilizers,
    string Message);
