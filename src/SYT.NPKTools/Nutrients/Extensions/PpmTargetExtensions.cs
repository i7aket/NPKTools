using SYT.NPKTools.Internal;

namespace SYT.NPKTools.Nutrients;

/// <summary>
/// Operations on <see cref="PpmTarget"/> that account for what the source water already contains.
/// </summary>
public static class PpmTargetExtensions
{
    /// <summary>
    /// Deducts what the source water already supplies, producing the target the fertilizers must
    /// actually meet.
    /// </summary>
    /// <param name="target">The nutrient profile you want in the finished solution.</param>
    /// <param name="water">
    /// What the source water contains. Use <see cref="WaterProfile.Pure"/> for reverse osmosis or
    /// distilled water, which leaves the target unchanged.
    /// </param>
    /// <returns>
    /// The adjusted target, together with any elements the water oversupplies. Pass
    /// <see cref="WaterAdjustedTarget.Target"/> to the optimizer.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="target"/> or <paramref name="water"/> is null.
    /// </exception>
    /// <remarks>
    /// <para>
    /// Skipping this step is the most common way to get a mix that is right on paper and wrong in the
    /// tank. Municipal water routinely carries tens of ppm of calcium, magnesium and sulfur; whatever it
    /// carries is added on top of everything the fertilizers contribute.
    /// </para>
    /// <para>
    /// An element the water already oversupplies is clamped to zero and reported in
    /// <see cref="WaterAdjustedTarget.Excesses"/> rather than silently truncated, because the caller
    /// needs to know: fertilizer only adds, so that element will overshoot no matter what is mixed.
    /// </para>
    /// <para>
    /// The water volume comes from <paramref name="target"/> and is carried through untouched — a water
    /// analysis is a concentration and does not depend on how much water is used.
    /// </para>
    /// </remarks>
    public static WaterAdjustedTarget AdjustFor(this PpmTarget target, WaterProfile water)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(water);

        List<NutrientExcess> excesses = [];

        double n = Deduct(Names.N, target.N.Value, water.Nitrogen.Value, excesses);
        double p = Deduct(Names.P, target.P.Value, water.Phosphorus.Value, excesses);
        double k = Deduct(Names.K, target.K.Value, water.Potassium.Value, excesses);
        double ca = Deduct(Names.Ca, target.Ca.Value, water.Calcium.Value, excesses);
        double mg = Deduct(Names.Mg, target.Mg.Value, water.Magnesium.Value, excesses);
        double s = Deduct(Names.S, target.S.Value, water.Sulfur.Value, excesses);
        double fe = Deduct(Names.Fe, target.Fe.Value, water.Iron.Value, excesses);
        double cu = Deduct(Names.Cu, target.Cu.Value, water.Copper.Value, excesses);
        double mn = Deduct(Names.Mn, target.Mn.Value, water.Manganese.Value, excesses);
        double zn = Deduct(Names.Zn, target.Zn.Value, water.Zinc.Value, excesses);
        double b = Deduct(Names.B, target.B.Value, water.Boron.Value, excesses);
        double mo = Deduct(Names.Mo, target.Mo.Value, water.Molybdenum.Value, excesses);
        double cl = Deduct(Names.Cl, target.Cl.Value, water.Chlorine.Value, excesses);
        double si = Deduct(Names.Si, target.Si.Value, water.Silicon.Value, excesses);
        double se = Deduct(Names.Se, target.Se.Value, water.Selenium.Value, excesses);
        double na = Deduct(Names.Na, target.Na.Value, water.Sodium.Value, excesses);

        PpmTarget adjusted = new PpmTargetBuilder()
            .AddN(n)
            .AddP(p)
            .AddK(k)
            .AddCa(ca)
            .AddMg(mg)
            .AddS(s)
            .AddFe(fe)
            .AddCu(cu)
            .AddMn(mn)
            .AddZn(zn)
            .AddB(b)
            .AddMo(mo)
            .AddCl(cl)
            .AddSi(si)
            .AddSe(se)
            .AddNa(na)
            .AddLiters(target.Liters.Value)
            .Build();

        return new WaterAdjustedTarget(adjusted, excesses);
    }

    /// <summary>
    /// Subtracts the water's contribution from one element, recording an excess when the water supplies
    /// more than was asked for.
    /// </summary>
    private static double Deduct(string element, double target, double inWater, List<NutrientExcess> excesses)
    {
        double remaining = target - inWater;

        if (remaining >= 0)
        {
            return remaining;
        }

        // Only worth reporting when something was actually asked for. Water carrying sodium against a
        // target that never mentioned sodium is normal and not the caller's problem to solve.
        if (target > 0)
        {
            excesses.Add(new NutrientExcess(element, inWater, target));
        }

        return 0;
    }
}
