namespace SYT.NPKTools.Nutrients;

/// <summary>
/// An analysis inferred from a meter reading, with the evidence for judging it.
/// </summary>
/// <remarks>
/// An estimate that cannot be checked is a guess. This carries the conductivity the estimate actually
/// has alongside the one that was asked for, so a caller can show the agreement rather than assert it.
/// </remarks>
public sealed record WaterEstimate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WaterEstimate"/> record.
    /// </summary>
    /// <param name="profile">The inferred analysis.</param>
    /// <param name="microSiemensPerCm">The conductivity the inferred analysis has.</param>
    /// <param name="requestedMicroSiemensPerCm">The conductivity that was asked for.</param>
    /// <param name="feasible">Whether the readings could be reconciled.</param>
    internal WaterEstimate(
        WaterProfile profile,
        double microSiemensPerCm,
        double requestedMicroSiemensPerCm,
        bool feasible)
    {
        Profile = profile;
        MicroSiemensPerCm = microSiemensPerCm;
        RequestedMicroSiemensPerCm = requestedMicroSiemensPerCm;
        Feasible = feasible;
    }

    /// <summary>Gets the inferred analysis, ready to subtract from a target.</summary>
    public WaterProfile Profile { get; }

    /// <summary>Gets the conductivity the inferred analysis has, in µS/cm.</summary>
    public double MicroSiemensPerCm { get; }

    /// <summary>Gets the conductivity that was asked for, in µS/cm.</summary>
    public double RequestedMicroSiemensPerCm { get; }

    /// <summary>
    /// Gets a value indicating whether the readings describe the same water.
    /// </summary>
    /// <remarks>
    /// False when a hardness reading already accounts for more conductivity than the meter showed.
    /// The profile is still returned — it is the closest water consistent with the drop tests — but
    /// it does not match the meter, and presenting it without saying so would be dishonest.
    /// </remarks>
    public bool Feasible { get; }

    /// <summary>Gets how far the estimate lands from the reading, as a fraction of it.</summary>
    public double RelativeError => RequestedMicroSiemensPerCm > 0
        ? Math.Abs(MicroSiemensPerCm - RequestedMicroSiemensPerCm) / RequestedMicroSiemensPerCm
        : 0;
}
