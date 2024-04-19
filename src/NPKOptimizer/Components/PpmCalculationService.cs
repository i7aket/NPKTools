using NPKOptimizer.Common;
using NPKOptimizer.Const;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.PartsPerMillion;
using NPKOptimizer.Domain.PartsPerMillion.ValueObjects;

namespace NPKOptimizer.Components;

public interface IPpmCalculationService
{
    ActionResult<Ppm> CalculatePpm(FertilizerCollection collection, double waterLiters = 1);
}


public class PpmCalculationService : IPpmCalculationService
{
    public ActionResult<Ppm> CalculatePpm(FertilizerCollection collection, double waterLiters = 1)
    {
        double totalMass = collection.Sum(f => f.Weight.Value);

        double totalNo3 = 0, totalNh4 = 0, totalNh2 = 0, totalP = 0, totalK = 0, totalMg = 0, totalS = 0, totalCa = 0;
        double totalFe = 0, totalCu = 0, totalMn = 0, totalZn = 0, totalB = 0, totalMo = 0, totalCl = 0;
        double totalSi = 0, totalSe = 0, totalNa = 0;

        foreach (Fertilizer fertilizer in collection)
        {
            totalNo3 += fertilizer.N.Nitrate * fertilizer.Weight.Value;
            totalNh4 += fertilizer.N.Ammonium * fertilizer.Weight.Value;
            totalNh2 += fertilizer.N.Amine * fertilizer.Weight.Value;
            totalP += fertilizer.P.Value * fertilizer.Weight.Value;
            totalK += fertilizer.K.Value * fertilizer.Weight.Value;
            totalMg += fertilizer.Mg.Value * fertilizer.Weight.Value;
            totalS += fertilizer.S.Value * fertilizer.Weight.Value;
            totalCa += fertilizer.Ca.Value * fertilizer.Weight.Value;
            totalFe += fertilizer.Fe.Value * fertilizer.Weight.Value;
            totalCu += fertilizer.Cu.Value * fertilizer.Weight.Value;
            totalMn += fertilizer.Mn.Value * fertilizer.Weight.Value;
            totalZn += fertilizer.Zn.Value * fertilizer.Weight.Value;
            totalB += fertilizer.B.Value * fertilizer.Weight.Value;
            totalMo += fertilizer.Mo.Value * fertilizer.Weight.Value;
            totalCl += fertilizer.Cl.Value * fertilizer.Weight.Value;
            totalSi += fertilizer.Si.Value * fertilizer.Weight.Value;
            totalSe += fertilizer.Se.Value * fertilizer.Weight.Value;
            totalNa += fertilizer.Na.Value * fertilizer.Weight.Value;
        }

        Ppm ppm = new (
            n: new NitrogenPpm(
                nitrate: totalNo3 / waterLiters * Settings.ConversionFactor,
                ammonium: totalNh4 / waterLiters * Settings.ConversionFactor,
                amine: totalNh2 / waterLiters * Settings.ConversionFactor),
            p: new PhosphorusPpm(totalP / waterLiters * Settings.ConversionFactor),
            k: new PotassiumPpm(totalK / waterLiters * Settings.ConversionFactor),
            ca: new CalciumPpm(totalCa / waterLiters * Settings.ConversionFactor),
            mg: new MagnesiumPpm(totalMg / waterLiters * Settings.ConversionFactor),
            s: new SulfurPpm(totalS / waterLiters * Settings.ConversionFactor),
            fe: new IronPpm(totalFe / waterLiters * Settings.ConversionFactor),
            cu: new CopperPpm(totalCu / waterLiters * Settings.ConversionFactor),
            mn: new ManganesePpm(totalMn / waterLiters * Settings.ConversionFactor),
            zn: new ZincPpm(totalZn / waterLiters * Settings.ConversionFactor),
            b: new BoronPpm(totalB / waterLiters * Settings.ConversionFactor),
            mo: new MolybdenumPpm(totalMo / waterLiters * Settings.ConversionFactor),
            cl: new ChlorinePpm(totalCl / waterLiters * Settings.ConversionFactor),
            si: new SiliconPpm(totalSi / waterLiters * Settings.ConversionFactor),
            se: new SeleniumPpm(totalSe / waterLiters * Settings.ConversionFactor),
            na: new SodiumPpm(totalNa / waterLiters * Settings.ConversionFactor),
            totalMassPpm: new TotalMassPpm(totalMass));


        return ActionResult<Ppm>.Success(ppm);
    }
}