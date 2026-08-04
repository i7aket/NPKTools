using SYT.NPKTools.Fertilizers;

namespace SYT.NPKTools.Calculator;

/// <summary>
/// Why a salt cannot be used, as something the interface can say in any language.
/// </summary>
/// <param name="Key">The translation key of the sentence to show.</param>
/// <param name="Values">What its placeholders take, already formatted for display.</param>
/// <remarks>
/// A key and its values rather than a sentence. The library describes these failures in English prose
/// written for a developer reading a log, and this is a notice shown to somebody who has just mistyped
/// a formula — the worst moment to hand them a language they do not read.
/// </remarks>
public sealed record SaltProblem(string Key, IReadOnlyList<string> Values)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaltProblem"/> class with no values.
    /// </summary>
    /// <param name="key">The translation key.</param>
    public SaltProblem(string key)
        : this(key, [])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaltProblem"/> class with one value.
    /// </summary>
    /// <param name="key">The translation key.</param>
    /// <param name="value">What the placeholder takes.</param>
    public SaltProblem(string key, string value)
        : this(key, [value])
    {
    }

    /// <summary>
    /// The same failure the library reported, as a key the interface can look up.
    /// </summary>
    /// <param name="problem">What the library said went wrong.</param>
    /// <returns>The key and the values its sentence needs.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="problem"/> is null.</exception>
    /// <remarks>
    /// Keyed off <see cref="FormulaProblemKind"/> by name, so a kind added to the library and not to
    /// the resource files shows its key rather than nothing — and a test walks the enum to catch that
    /// before a grower does.
    /// </remarks>
    public static SaltProblem From(FormulaProblem problem)
    {
        ArgumentNullException.ThrowIfNull(problem);

        string key = $"salt.error.{problem.Kind}";

        return problem switch
        {
            { Value: { } value, Position: { } position } => new(
                key,
                [value, position.ToString(System.Globalization.CultureInfo.InvariantCulture)]),
            { Value: { } value } => new(key, value),
            _ => new(key),
        };
    }
}
