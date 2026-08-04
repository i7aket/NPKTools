using SYT.NPKTools.Internal;

namespace SYT.NPKTools.Fertilizers;

/// <summary>
/// Builds a fertilizer from a chemical formula.
/// </summary>
/// <remarks>
/// Everything the optimizer needs is derivable from the formula, so a grower adding a salt the
/// catalogue lacks writes what is on the bag and the percentages follow. The result is an ordinary
/// <see cref="Fertilizer"/>: the bundle generator, the optimizer and the concentrate split cannot
/// tell it from a built-in one, and none of them need to.
/// </remarks>
public static class FormulaComposition
{
    /// <summary>
    /// Builds a fertilizer from a formula, reporting why rather than throwing when it cannot.
    /// </summary>
    /// <param name="name">The name to show. Must be unique among the salts on offer.</param>
    /// <param name="formula">The chemical formula.</param>
    /// <param name="type">Which concentrate tank the salt belongs in.</param>
    /// <param name="fertilizer">The result, or null when the formula could not be read.</param>
    /// <param name="problem">What went wrong, or null on success.</param>
    /// <returns><see langword="true"/> when the fertilizer was built.</returns>
    /// <remarks>
    /// Metals are recorded as non-chelated. A formula spells out atoms, and a chelate is defined by
    /// the agent holding the metal rather than by its composition — see <see cref="LooksChelated"/>
    /// for why a chelate entered this way would be wrong rather than merely imprecise.
    /// </remarks>
    public static bool TryCreate(
        string name,
        string formula,
        ConcentrateType type,
        out Fertilizer? fertilizer,
        out FormulaProblem? problem)
    {
        fertilizer = null;
        problem = null;

        if (string.IsNullOrWhiteSpace(name))
        {
            problem = new(FormulaProblemKind.NameMissing, "The salt needs a name.");
            return false;
        }

        if (!ChemicalFormula.TryParse(formula, out ChemicalFormula? parsed, out problem))
        {
            return false;
        }

        FertilizerBuilder builder = new FertilizerBuilder()
            .AddName(name.Trim())
            .AddFormula(formula.Trim())
            .AddType(type);

        Set(builder.AddNo3, parsed!.NitratePercent);
        Set(builder.AddNh4, parsed.AmmoniumPercent);
        Set(builder.AddNh2, parsed.AmidePercent);
        Set(builder.AddP, parsed.PercentOf(Names.P));
        Set(builder.AddK, parsed.PercentOf(Names.K));
        Set(builder.AddCaNonChelated, parsed.PercentOf(Names.Ca));
        Set(builder.AddMgNonChelated, parsed.PercentOf(Names.Mg));
        Set(builder.AddS, parsed.PercentOf(Names.S));
        Set(builder.AddFeNonChelated, parsed.PercentOf(Names.Fe));
        Set(builder.AddCuNonChelated, parsed.PercentOf(Names.Cu));
        Set(builder.AddMnNonChelated, parsed.PercentOf(Names.Mn));
        Set(builder.AddZnNonChelated, parsed.PercentOf(Names.Zn));
        Set(builder.AddB, parsed.PercentOf(Names.B));
        Set(builder.AddMo, parsed.PercentOf(Names.Mo));
        Set(builder.AddCl, parsed.PercentOf(Names.Cl));
        Set(builder.AddSi, parsed.PercentOf(Names.Si));
        Set(builder.AddSe, parsed.PercentOf(Names.Se));
        Set(builder.AddNa, parsed.PercentOf(Names.Na));

        fertilizer = builder.Build();
        return true;
    }

    /// <summary>
    /// Suggests which concentrate tank a salt belongs in.
    /// </summary>
    /// <param name="formula">The parsed formula.</param>
    /// <returns>Tank A or tank B.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="formula"/> is null.</exception>
    /// <remarks>
    /// The rule the concentrate split already enforces: calcium precipitates with sulfate and with
    /// phosphate, so the two must not share a tank. Calcium therefore suggests A, and sulfur or
    /// phosphorus without calcium suggests B. A suggestion only — a salt carrying both is a real
    /// product, and the caller should be able to say where it goes.
    /// </remarks>
    public static ConcentrateType SuggestTank(ChemicalFormula formula)
    {
        ArgumentNullException.ThrowIfNull(formula);

        if (formula.PercentOf(Names.Ca) > 0)
        {
            return ConcentrateType.A;
        }

        bool precipitates = formula.PercentOf(Names.S) > 0 || formula.PercentOf(Names.P) > 0;
        return precipitates ? ConcentrateType.B : ConcentrateType.A;
    }

    /// <summary>
    /// Whether a formula looks like a chelate, and so should not be entered as one.
    /// </summary>
    /// <param name="formula">The parsed formula.</param>
    /// <returns><see langword="true"/> when the formula carries an organic ligand and a metal.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="formula"/> is null.</exception>
    /// <remarks>
    /// <para>
    /// A chelating agent spelled out as atoms parses perfectly well and gives an answer that is
    /// confidently wrong. EDTA contains nitrogen, but that nitrogen holds the metal rather than
    /// feeding the plant, and a fertilizer built from the formula would offer it as nutrient
    /// nitrogen. The built-in catalogue declares zero nitrogen for its EDTA salts for exactly this
    /// reason.
    /// </para>
    /// <para>
    /// Carbon alongside a chelatable metal is the signal. It is a heuristic, and it is meant to
    /// prompt rather than to refuse: the caller should steer the grower to entering percentages,
    /// where the chelating agent is named explicitly.
    /// </para>
    /// </remarks>
    public static bool LooksChelated(ChemicalFormula formula)
    {
        ArgumentNullException.ThrowIfNull(formula);

        bool organic = formula.Atoms.ContainsKey("C");
        bool metal = formula.PercentOf(Names.Fe) > 0 || formula.PercentOf(Names.Cu) > 0
            || formula.PercentOf(Names.Mn) > 0 || formula.PercentOf(Names.Zn) > 0;

        return organic && metal;
    }

    // Builder setters throw when called twice and reject values outside their range, so only real
    // content is passed through.
    private static void Set(Func<double, FertilizerBuilder> add, double percent)
    {
        if (percent > 0)
        {
            add(percent);
        }
    }
}
