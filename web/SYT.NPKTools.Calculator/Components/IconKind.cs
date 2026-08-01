namespace SYT.NPKTools.Calculator;

/// <summary>
/// Which glyph <c>Icon</c> draws.
/// </summary>
public enum IconKind
{
    /// <summary>Neutral information.</summary>
    Info,

    /// <summary>Something worth checking, but the result is still usable.</summary>
    Warning,

    /// <summary>Something that stops the calculation or would ruin a batch.</summary>
    Error,
}
