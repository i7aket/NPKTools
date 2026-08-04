using System.Globalization;
using System.Text;
using SYT.NPKTools.Internal;

namespace SYT.NPKTools.Fertilizers;

/// <summary>
/// A chemical formula read into the elements it contains.
/// </summary>
/// <remarks>
/// <para>
/// Exists so a grower can describe a fertilizer the catalogue does not carry by writing what is on
/// the bag rather than working out percentages by hand. Deriving them from the formula is both less
/// work and less error-prone: fertilizer labels quote phosphorus and potassium as their oxides,
/// P₂O₅ and K₂O, and a figure copied straight off a label overstates phosphorus by a factor of 2.3.
/// </para>
/// <para>
/// Nitrogen is reported by group as well as in total, because the library treats nitrate, ammonium
/// and amide differently — they carry different charges and different acid-base character, and urea
/// carries no charge at all.
/// </para>
/// </remarks>
public sealed class ChemicalFormula
{
    private static readonly Dictionary<string, double> Masses = new(StringComparer.Ordinal)
    {
        [Names.N] = AtomicMasses.N,
        [Names.P] = AtomicMasses.P,
        [Names.K] = AtomicMasses.K,
        [Names.Ca] = AtomicMasses.Ca,
        [Names.Mg] = AtomicMasses.Mg,
        [Names.S] = AtomicMasses.S,
        [Names.Fe] = AtomicMasses.Fe,
        [Names.Cu] = AtomicMasses.Cu,
        [Names.Mn] = AtomicMasses.Mn,
        [Names.Zn] = AtomicMasses.Zn,
        [Names.B] = AtomicMasses.B,
        [Names.Mo] = AtomicMasses.Mo,
        [Names.Cl] = AtomicMasses.Cl,
        [Names.Si] = AtomicMasses.Si,
        [Names.Se] = AtomicMasses.Se,
        [Names.Na] = AtomicMasses.Na,
        ["H"] = 1.008,
        ["O"] = 15.999,
        ["C"] = 12.011,
    };

    private readonly double _nitrateAtoms;
    private readonly double _ammoniumAtoms;
    private readonly double _amideAtoms;

    private ChemicalFormula(
        Dictionary<string, int> atoms,
        double nitrateAtoms,
        double ammoniumAtoms,
        double amideAtoms)
    {
        Atoms = atoms;
        MolarMass = atoms.Sum(pair => Masses[pair.Key] * pair.Value);
        _nitrateAtoms = nitrateAtoms;
        _ammoniumAtoms = ammoniumAtoms;
        _amideAtoms = amideAtoms;
    }

    /// <summary>Gets how many atoms of each element the formula contains.</summary>
    public IReadOnlyDictionary<string, int> Atoms { get; }

    /// <summary>Gets the molar mass, in grams per mole.</summary>
    public double MolarMass { get; }

    /// <summary>Gets the nitrate nitrogen, as a percentage of the whole by weight.</summary>
    public double NitratePercent => Share(_nitrateAtoms, Names.N);

    /// <summary>Gets the ammonium nitrogen, as a percentage of the whole by weight.</summary>
    public double AmmoniumPercent => Share(_ammoniumAtoms, Names.N);

    /// <summary>Gets the amide nitrogen, as a percentage of the whole by weight.</summary>
    public double AmidePercent => Share(_amideAtoms, Names.N);

    /// <summary>
    /// The share of the total weight one element accounts for.
    /// </summary>
    /// <param name="symbol">The element symbol.</param>
    /// <returns>A percentage, or zero when the formula does not contain the element.</returns>
    public double PercentOf(string symbol) =>
        Atoms.TryGetValue(symbol, out int count) ? Share(count, symbol) : 0;

    private double Share(double atoms, string symbol) =>
        MolarMass > 0 ? 100 * Masses[symbol] * atoms / MolarMass : 0;

    /// <summary>
    /// Reads a formula, reporting why rather than throwing when it cannot.
    /// </summary>
    /// <param name="text">The formula, for example <c>Ca(NO₃)2*4H₂O</c>.</param>
    /// <param name="formula">The parsed formula, or null when it could not be read.</param>
    /// <param name="problem">What went wrong and where, or null on success.</param>
    /// <returns><see langword="true"/> when the formula was read.</returns>
    /// <remarks>
    /// Accepts element symbols, plain and Unicode subscripts — the catalogue mixes them within a
    /// single formula — bracketed groups with a multiplier, and hydrates joined by <c>*</c> or
    /// <c>·</c>. Anything else is refused: guessing would produce a fertilizer that is confidently
    /// wrong, which is worse than one that is rejected.
    /// </remarks>
    public static bool TryParse(string? text, out ChemicalFormula? formula, out FormulaProblem? problem)
    {
        formula = null;
        problem = null;

        if (string.IsNullOrWhiteSpace(text))
        {
            problem = new(FormulaProblemKind.Empty, "The formula is empty.");
            return false;
        }

        string[] parts = Normalise(text).Split('*', StringSplitOptions.RemoveEmptyEntries);
        Dictionary<string, int> atoms = new(StringComparer.Ordinal);
        double nitrate = 0;
        double ammonium = 0;
        double amide = 0;

        for (int part = 0; part < parts.Length; part++)
        {
            string body = parts[part];
            int multiplier = 1;

            // Only a hydrate carries its count in front, as in *4H2O. A leading digit anywhere else
            // is a formula nobody wrote on a bag.
            int digits = 0;
            while (digits < body.Length && char.IsAsciiDigit(body[digits]))
            {
                digits++;
            }

            if (digits > 0)
            {
                if (part == 0)
                {
                    problem = new(
                        FormulaProblemKind.StartsWithNumber,
                        "A formula cannot start with a number.");
                    return false;
                }

                multiplier = int.Parse(body[..digits], CultureInfo.InvariantCulture);
                body = body[digits..];
            }

            if (!ParseGroup(body, multiplier, atoms, ref nitrate, ref ammonium, ref amide, out problem))
            {
                return false;
            }
        }

        if (atoms.Count == 0)
        {
            problem = new(FormulaProblemKind.NoElements, "The formula names no elements.");
            return false;
        }

        formula = new ChemicalFormula(atoms, nitrate, ammonium, amide);
        return true;
    }

    private static string Normalise(string text)
    {
        StringBuilder builder = new(text.Length);
        foreach (char c in text.Trim())
        {
            builder.Append(c switch
            {
                '·' or '×' or '.' => '*',
                >= '₀' and <= '₉' => (char)('0' + (c - '₀')),
                '[' => '(',
                ']' => ')',
                _ => c,
            });
        }

        return builder.ToString();
    }

    private static bool ParseGroup(
        string body,
        int multiplier,
        Dictionary<string, int> atoms,
        ref double nitrate,
        ref double ammonium,
        ref double amide,
        out FormulaProblem? problem)
    {
        problem = null;
        int index = 0;

        while (index < body.Length)
        {
            if (body[index] == ')')
            {
                problem = new(
                    FormulaProblemKind.UnmatchedClosingBracket,
                    "A closing bracket has nothing to close.");
                return false;
            }

            if (body[index] == '(')
            {
                int depth = 1;
                int close = index + 1;
                while (close < body.Length && depth > 0)
                {
                    depth += body[close] switch { '(' => 1, ')' => -1, _ => 0 };
                    close++;
                }

                if (depth != 0)
                {
                    problem = new(FormulaProblemKind.UnclosedBracket, "A bracket is not closed.");
                    return false;
                }

                string inner = body[(index + 1)..(close - 1)];
                index = close;
                int repeats = ReadNumber(body, ref index) * multiplier;

                Dictionary<string, int> group = new(StringComparer.Ordinal);
                double groupNitrate = 0;
                double groupAmmonium = 0;
                double groupAmide = 0;
                if (!ParseGroup(inner, 1, group, ref groupNitrate, ref groupAmmonium, ref groupAmide, out problem))
                {
                    return false;
                }

                foreach ((string symbol, int count) in group)
                {
                    atoms[symbol] = atoms.GetValueOrDefault(symbol) + (count * repeats);
                }

                // A group the inner pass already explained keeps its own attribution; one it did not
                // — NO₃ and NH₄ written without a nested bracket — is classified from its shape.
                double explained = groupNitrate + groupAmmonium + groupAmide;
                if (explained > 0)
                {
                    nitrate += groupNitrate * repeats;
                    ammonium += groupAmmonium * repeats;
                    amide += groupAmide * repeats;
                }
                else
                {
                    Classify(group, repeats, ref nitrate, ref ammonium, ref amide);
                }

                continue;
            }

            if (!char.IsAsciiLetterUpper(body[index]))
            {
                problem = new(
                    FormulaProblemKind.UnexpectedCharacter,
                    $"Unexpected '{body[index]}' at position {index + 1}.",
                    body[index].ToString(),
                    index + 1);
                return false;
            }

            int start = index;
            index++;
            while (index < body.Length && char.IsAsciiLetterLower(body[index]))
            {
                index++;
            }

            string element = body[start..index];
            if (!Masses.ContainsKey(element))
            {
                problem = new(
                    FormulaProblemKind.UnknownElement,
                    $"'{element}' is not an element this calculator knows.",
                    element);
                return false;
            }

            int howMany = ReadNumber(body, ref index) * multiplier;
            atoms[element] = atoms.GetValueOrDefault(element) + howMany;

            if (element == Names.N)
            {
                ClassifyRun(body, index, howMany, ref nitrate, ref ammonium, ref amide);
            }
        }

        return true;
    }

    private static int ReadNumber(string body, ref int index)
    {
        int start = index;
        while (index < body.Length && char.IsAsciiDigit(body[index]))
        {
            index++;
        }

        return index == start ? 1 : int.Parse(body[start..index], CultureInfo.InvariantCulture);
    }

    /// <summary>Attributes a bracketed group's nitrogen, as in Ca(NO3)2 or (NH4)2SO4.</summary>
    private static void Classify(
        Dictionary<string, int> group,
        int repeats,
        ref double nitrate,
        ref double ammonium,
        ref double amide)
    {
        if (group.Count != 2 || !group.TryGetValue(Names.N, out int n) || n != 1)
        {
            return;
        }

        if (group.TryGetValue("O", out int oxygen) && oxygen == 3)
        {
            nitrate += repeats;
        }
        else if (group.TryGetValue("H", out int hydrogen))
        {
            if (hydrogen == 4)
            {
                ammonium += repeats;
            }
            else if (hydrogen == 2)
            {
                amide += repeats;
            }
        }
    }

    /// <summary>Attributes nitrogen written without brackets, as in KNO3 or NH4Cl.</summary>
    private static void ClassifyRun(
        string body,
        int after,
        int atoms,
        ref double nitrate,
        ref double ammonium,
        ref double amide)
    {
        string rest = body[after..];
        if (rest.StartsWith("O3", StringComparison.Ordinal))
        {
            nitrate += atoms;
        }
        else if (rest.StartsWith("H4", StringComparison.Ordinal))
        {
            ammonium += atoms;
        }
        else if (rest.StartsWith("H2", StringComparison.Ordinal))
        {
            amide += atoms;
        }
    }
}
