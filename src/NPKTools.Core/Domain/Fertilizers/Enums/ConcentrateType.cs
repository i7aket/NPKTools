namespace NPKTools.Core.Domain.Fertilizers.Enums;

/// <summary>
/// Identifies which concentrate tank a fertilizer belongs in. Calcium and sulfate or phosphate
/// salts precipitate when mixed, so they are kept in separate stock solutions (A and B) and only
/// combined at final dilution.
/// </summary>
public enum ConcentrateType
{
    /// <summary>
    /// No tank assigned. The default for fertilizers built without an explicit type.
    /// </summary>
    None,

    /// <summary>
    /// Tank A, conventionally holding the calcium-bearing fertilizers.
    /// </summary>
    A,

    /// <summary>
    /// Tank B, conventionally holding the sulfates and phosphates that would precipitate
    /// calcium if stored alongside it.
    /// </summary>
    B
}