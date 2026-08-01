using SYT.NPKTools.Fertilizers;

namespace SYT.NPKTools.Concentrates;

/// <summary>
/// Turns a working-strength recipe into concentrate tanks.
/// </summary>
public static class ConcentrateExtensions
{
    /// <summary>
    /// Splits a mix into two concentrate tanks, keeping calcium away from sulfate and phosphate.
    /// </summary>
    /// <param name="solution">
    /// The mix as the optimizer produced it, carrying the weights for its own water volume.
    /// </param>
    /// <param name="concentrateLiters">
    /// The volume of each tank. The whole recipe's salt goes into this much water, so a 100-liter recipe
    /// concentrated into 1 liter gives a 1:100 dilution.
    /// </param>
    /// <param name="solubility">
    /// The solubility figures to check each salt's tank strength against. Defaults to
    /// <see cref="SolubilityTable.Default"/>, which covers the preset catalogue; add your own salts with
    /// <see cref="SolubilityTable.With"/>, since a salt with no figure can only be reported as unchecked.
    /// </param>
    /// <returns>The tanks, the dilution ratio, and anything worth checking before mixing.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="solution"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="concentrateLiters"/> is not positive, or is not smaller than the mix's
    /// own water volume — a "concentrate" at or above working volume is not concentrating anything.
    /// </exception>
    /// <remarks>
    /// <para>
    /// Each fertilizer's own <see cref="Fertilizer.Type"/> decides its tank. Where that is
    /// <see cref="ConcentrateType.None"/> — which is what a custom salt gets unless its author says
    /// otherwise — a tank is inferred from the composition and the inference is reported, because
    /// guessing silently is how a custom sulfate ends up next to calcium.
    /// </para>
    /// <para>
    /// The precipitation check is a rule of thumb, not a solubility calculation: it flags calcium and
    /// sulfate or phosphate arriving in one tank from <em>different</em> salts. A single salt carrying
    /// both internally — monocalcium phosphate, for instance — is not flagged, because it is a soluble
    /// compound rather than two reagents that happen to be adjacent. Predicting actual precipitation
    /// needs solubility products, pH and temperature, none of which this library models.
    /// </para>
    /// </remarks>
    public static ConcentratePlan AsConcentrate(
        this Solution solution,
        double concentrateLiters,
        SolubilityTable? solubility = null)
    {
        ArgumentNullException.ThrowIfNull(solution);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(concentrateLiters);

        solubility ??= SolubilityTable.Default;

        if (concentrateLiters >= solution.WaterLiters)
        {
            throw new ArgumentOutOfRangeException(
                nameof(concentrateLiters),
                concentrateLiters,
                $"A concentrate must hold less water than the working solution ({solution.WaterLiters} L).");
        }

        List<ConcentrateWarning> warnings = [];
        List<ConcentrateComponent> tankA = [];
        List<ConcentrateComponent> tankB = [];

        foreach (Fertilizer fertilizer in solution)
        {
            ConcentrateType tank = fertilizer.Type;

            if (tank == ConcentrateType.None)
            {
                tank = Infer(fertilizer);
                warnings.Add(new ConcentrateWarning(
                    ConcentrateWarningKind.TankInferred,
                    tank,
                    [fertilizer.Name.Value],
                    $"'{fertilizer.Name.Value}' has no tank assigned; tank {tank} was inferred from its "
                        + "composition. Set ConcentrateType explicitly to be sure."));
            }

            ConcentrateComponent component = new(
                fertilizer,
                fertilizer.Weight.Value / concentrateLiters,
                solubility.Limit(fertilizer.Name.Value));

            if (tank == ConcentrateType.B)
            {
                tankB.Add(component);
            }
            else
            {
                tankA.Add(component);
            }
        }

        warnings.AddRange(FindPrecipitationRisks(ConcentrateType.A, tankA));
        warnings.AddRange(FindPrecipitationRisks(ConcentrateType.B, tankB));

        ConcentrateTank tankOfA = new(ConcentrateType.A, tankA);
        ConcentrateTank tankOfB = new(ConcentrateType.B, tankB);

        List<string> unknownSolubility = [];
        double? maxRatio = null;

        foreach (ConcentrateTank tank in new[] { tankOfA, tankOfB })
        {
            unknownSolubility.AddRange(tank.Components
                .Where(c => c.SolubilityLimit is null)
                .Select(c => c.Fertilizer.Name.Value));

            warnings.AddRange(tank.Components
                .Where(c => c.ExceedsSolubility)
                .Select(c => new ConcentrateWarning(
                    ConcentrateWarningKind.SolubilityExceeded,
                    tank.Tank,
                    [c.Fertilizer.Name.Value],
                    $"Tank {tank.Tank} needs {c.GramsPerLiter:F1} g/L of '{c.Fertilizer.Name.Value}', "
                        + $"which dissolves to {c.SolubilityLimit:F0} g/L at 20 °C. Use a larger "
                        + "concentrate volume, or a more soluble source of that element.")));

            if (tank.IsSaturated)
            {
                warnings.Add(new ConcentrateWarning(
                    ConcentrateWarningKind.TankSaturated,
                    tank.Tank,
                    [.. tank.Components.Select(c => c.Fertilizer.Name.Value)],
                    $"Tank {tank.Tank} is at {tank.SaturationFraction:P0} of saturation: its salts "
                        + "together need more water than the tank holds, because they compete for the "
                        + "same water. Use a larger concentrate volume."));
            }

            maxRatio = Smaller(maxRatio, CeilingFor(tank, solution.WaterLiters));
        }

        return new ConcentratePlan(
            tankOfA,
            tankOfB,
            concentrateLiters,
            solution.WaterLiters,
            warnings,
            unknownSolubility)
        {
            MaxDilutionRatio = maxRatio
        };
    }

    /// <summary>
    /// Works out how far a tank can be concentrated before it saturates.
    /// </summary>
    /// <remarks>
    /// Saturation rises in proportion to the dilution ratio — twice the ratio is twice the strength — so
    /// the ceiling is where the summed fractions reach 1. Returns null when any salt's limit is unknown, or
    /// when nothing in the tank has a finite limit to bind against.
    /// </remarks>
    private static double? CeilingFor(ConcentrateTank tank, double workingLiters)
    {
        if (tank.IsEmpty || tank.Components.Any(c => c.SolubilityLimit is null))
        {
            return null;
        }

        double saturationPerUnitRatio = tank.Components
            .Where(c => double.IsFinite(c.SolubilityLimit!.Value))
            .Sum(c => c.Grams / (workingLiters * c.SolubilityLimit!.Value));

        return saturationPerUnitRatio > 0 ? 1 / saturationPerUnitRatio : null;
    }

    private static double? Smaller(double? left, double? right) =>
        left is null ? right : right is null ? left : Math.Min(left.Value, right.Value);

    /// <summary>
    /// Picks a tank for a fertilizer that does not declare one, from what it contains.
    /// </summary>
    /// <remarks>
    /// Calcium goes to A and sulfate or phosphate to B, which is the convention the tanks exist to serve.
    /// A salt carrying calcium <em>and</em> phosphate internally goes to B: it is soluble in itself, and B
    /// is where the phosphate it would otherwise meet already is. Anything carrying none of the three —
    /// a nitrate, urea — is chemically welcome in either, and lands in A because that is where the
    /// convention puts the bulk of the nitrogen.
    /// </remarks>
    private static ConcentrateType Infer(Fertilizer fertilizer)
    {
        bool hasSulfateOrPhosphate = fertilizer.Sulfur.Value > 0 || fertilizer.Phosphorus.Value > 0;

        if (hasSulfateOrPhosphate)
        {
            return ConcentrateType.B;
        }

        return ConcentrateType.A;
    }

    /// <summary>
    /// Flags calcium meeting sulfate or phosphate in one tank, from separate salts.
    /// </summary>
    private static IEnumerable<ConcentrateWarning> FindPrecipitationRisks(
        ConcentrateType tank,
        IReadOnlyList<ConcentrateComponent> components)
    {
        List<string> calciumSources = [];
        List<string> sulfateOrPhosphateSources = [];

        foreach (ConcentrateComponent component in components)
        {
            Fertilizer fertilizer = component.Fertilizer;
            bool hasCalcium = fertilizer.Calcium.Value > 0;
            bool hasSulfateOrPhosphate = fertilizer.Sulfur.Value > 0 || fertilizer.Phosphorus.Value > 0;

            // A salt holding both is a compound, not a collision, so it counts as neither source.
            if (hasCalcium && hasSulfateOrPhosphate)
            {
                continue;
            }

            if (hasCalcium)
            {
                calciumSources.Add(fertilizer.Name.Value);
            }
            else if (hasSulfateOrPhosphate)
            {
                sulfateOrPhosphateSources.Add(fertilizer.Name.Value);
            }
        }

        if (calciumSources.Count == 0 || sulfateOrPhosphateSources.Count == 0)
        {
            yield break;
        }

        yield return new ConcentrateWarning(
            ConcentrateWarningKind.PrecipitationRisk,
            tank,
            [.. calciumSources, .. sulfateOrPhosphateSources],
            $"Tank {tank} holds calcium ({string.Join(", ", calciumSources)}) together with sulfate or "
                + $"phosphate ({string.Join(", ", sulfateOrPhosphateSources)}). These precipitate at "
                + "concentrate strength even though they stay dissolved at working strength. Move one "
                + "group to the other tank.");
    }
}
