namespace SYT.NPKTools.Concentrates;

/// <summary>
/// A recipe split into concentrate tanks, ready to mix and store.
/// </summary>
/// <param name="TankA">
/// Tank A, conventionally the calcium-bearing salts.
/// </param>
/// <param name="TankB">
/// Tank B, conventionally the sulfates and phosphates that would precipitate calcium if stored
/// alongside it.
/// </param>
/// <param name="ConcentrateLiters">The volume of each tank.</param>
/// <param name="WorkingLiters">The volume of working solution the original recipe was for.</param>
/// <param name="Warnings">Anything worth checking before mixing. Empty is the good case.</param>
/// <param name="UnknownSolubility">
/// Names of salts the solubility table had no figure for, so nothing could be checked. Reported rather
/// than passed over: "we do not know whether this dissolves" must not read as "this dissolves".
/// </param>
/// <remarks>
/// <para>
/// A concentrate exists so that mixing happens once a month instead of every watering. The catch is
/// chemical: calcium sulfate and the higher calcium phosphates are barely soluble, so at working
/// strength they stay dissolved while at concentrate strength they fall out of solution. Keeping calcium
/// apart from sulfate and phosphate until the moment of final dilution is what the two tanks are for.
/// </para>
/// <para>
/// The weights are the same as the working recipe called for — concentrating changes the volume of
/// water, not the amount of salt.
/// </para>
/// </remarks>
public sealed record ConcentratePlan(
    ConcentrateTank TankA,
    ConcentrateTank TankB,
    double ConcentrateLiters,
    double WorkingLiters,
    IReadOnlyList<ConcentrateWarning> Warnings,
    IReadOnlyList<string> UnknownSolubility)
{
    /// <summary>
    /// Gets how many parts of water each part of concentrate is diluted into.
    /// </summary>
    /// <remarks>
    /// A ratio of 100 means 10 ml of tank A and 10 ml of tank B per liter of finished solution. Both
    /// tanks share the same ratio, which is what lets them be dosed together.
    /// </remarks>
    public double DilutionRatio => WorkingLiters / ConcentrateLiters;

    /// <summary>
    /// Gets the millilitres of each tank to add per liter of finished solution.
    /// </summary>
    public double MillilitresPerLiter => 1000d / DilutionRatio;

    /// <summary>
    /// Gets both tanks.
    /// </summary>
    public IReadOnlyList<ConcentrateTank> Tanks => [TankA, TankB];

    /// <summary>
    /// Gets a value indicating whether anything in this plan needs checking before mixing.
    /// </summary>
    public bool HasWarnings => Warnings.Count > 0;

    /// <summary>
    /// Gets a value indicating whether any tank is expected to precipitate.
    /// </summary>
    /// <remarks>
    /// The one flag worth blocking on. Other warnings are advisory; this one means the batch is likely to
    /// be wasted.
    /// </remarks>
    public bool HasPrecipitationRisk =>
        Warnings.Any(w => w.Kind == ConcentrateWarningKind.PrecipitationRisk);

    /// <summary>
    /// Gets a value indicating whether any salt is asked to dissolve past its solubility.
    /// </summary>
    /// <remarks>
    /// The other flag worth blocking on, and the more certain of the two: precipitation is a prediction,
    /// whereas this is arithmetic against a published figure. Dilute the concentrate — a larger tank at a
    /// smaller ratio — or pick a more soluble source of the element.
    /// </remarks>
    public bool ExceedsSolubility =>
        Warnings.Any(w => w.Kind is ConcentrateWarningKind.SolubilityExceeded
                                 or ConcentrateWarningKind.TankSaturated);

    /// <summary>
    /// Gets the largest dilution ratio the known solubility figures allow, or null when no salt in either
    /// tank has a finite known limit.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The actionable number when <see cref="ExceedsSolubility"/> is true: a 1:100 concentrate that cannot
    /// dissolve may well work at 1:40, and this says where the ceiling is. It shares the caveats of the
    /// table it comes from — the true ceiling in a mixture is lower, so leave headroom.
    /// </para>
    /// <para>
    /// Salts named in <see cref="UnknownSolubility"/> are not part of this bound, because there is no figure
    /// to bound them with. That makes the ceiling an optimistic one whenever that list is non-empty: the real
    /// limit may be lower, and it is the caller's job to read the two together.
    /// </para>
    /// </remarks>
    public double? MaxDilutionRatio { get; init; }
}
