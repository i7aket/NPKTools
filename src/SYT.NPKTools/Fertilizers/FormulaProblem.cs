namespace SYT.NPKTools.Fertilizers;

/// <summary>
/// Why a chemical formula could not be read.
/// </summary>
/// <remarks>
/// Named rather than only described. <see cref="FormulaProblem.Message"/> is English prose written for
/// a developer reading a log; an application showing the failure to somebody who has just mistyped a
/// formula needs to write its own sentence, in their language, and cannot do that from prose.
/// </remarks>
public enum FormulaProblemKind
{
    /// <summary>Nothing was entered.</summary>
    Empty,

    /// <summary>A digit came first, which only a hydrate count may do, and only after a <c>*</c>.</summary>
    StartsWithNumber,

    /// <summary>It parsed, but named no elements.</summary>
    NoElements,

    /// <summary>A closing bracket appeared with nothing open.</summary>
    UnmatchedClosingBracket,

    /// <summary>A bracket was opened and never closed.</summary>
    UnclosedBracket,

    /// <summary>A character that cannot appear in a formula. <see cref="FormulaProblem.Position"/> says where.</summary>
    UnexpectedCharacter,

    /// <summary>A symbol that is not an element this calculator knows.</summary>
    UnknownElement,

    /// <summary>
    /// The salt was given no name. Reported by <see cref="FormulaComposition.TryCreate"/>, which needs
    /// a name as well as a formula, and kept here so a caller has one thing to switch on.
    /// </summary>
    NameMissing,
}

/// <summary>
/// What went wrong in a formula, and where.
/// </summary>
/// <param name="Kind">Which failure this is.</param>
/// <param name="Message">English prose, for a developer.</param>
/// <param name="Value">The offending text, for the failures that have one — a character or a symbol.</param>
/// <param name="Position">Where it is, counting from one, for the failures that have a position.</param>
public sealed record FormulaProblem(
    FormulaProblemKind Kind,
    string Message,
    string? Value = null,
    int? Position = null);
