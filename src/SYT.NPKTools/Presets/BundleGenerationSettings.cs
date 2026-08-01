namespace SYT.NPKTools;

/// <summary>
/// Bounds on how many bundles are generated from a list of salts.
/// </summary>
/// <remarks>
/// Generation produces one bundle per salt plus one holding everything, so the count tracks the size of
/// the shelf and only needs bounding for an unusually long list. Whatever the limit discards is reported
/// in <see cref="GeneratedBundles.BundlesDropped"/> rather than dropped quietly.
/// </remarks>
public sealed record BundleGenerationSettings
{
    /// <summary>
    /// Gets the settings used when a caller does not supply any.
    /// </summary>
    public static BundleGenerationSettings Default { get; } = new();

    /// <summary>
    /// Gets how many bundles may be returned. Defaults to 64.
    /// </summary>
    /// <remarks>
    /// A guard against a runaway list rather than a curation limit: every bundle costs a solve, and the
    /// default only binds on a shelf of more than 63 salts. The built-in catalogue draws on 17.
    /// </remarks>
    public int MaxBundles { get; init; } = 64;
}
