using NPKTools.Core.Common;
using NPKTools.Core.Const;
using NPKTools.Core.Domain.Fertilizers;
using NPKTools.Core.Domain.PartsPerMillion;
using NPKTools.Core.Domain.PartsPerMillion.ValueObjects;

namespace NPKTools.PPMCalc;

/// <summary>
/// Provides services to calculate parts per million (ppm) concentrations of various nutrients
/// based on a collection of fertilizers and a specified amount of water.
/// </summary>
public class PpmCalculationService : IPpmCalculationService
{
    /// <summary>
    /// Calculates ppm concentrations for a collection of fertilizers diluted in a specified amount of water.
    /// </summary>
    /// <param name="collection">A list of fertilizers to calculate ppm values from.</param>
    /// <param name="waterLiters">The volume of water in liters used for dilution. Must be greater than 0.</param>
    /// <returns>A <see cref="Ppm"/> object containing the ppm values for all relevant nutrients.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="collection"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="collection"/> is empty or if <paramref name="waterLiters"/> is less than or equal to zero.</exception>
    public Ppm CalculatePpm(IList<Fertilizer> collection, double waterLiters = 1)
    {
        ThrowIf.NullOrEmpty(collection);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(waterLiters);
        
        double totalNo3 = 0, totalNh4 = 0, totalNh2 = 0, totalP = 0, totalK = 0, totalMg = 0, totalS = 0, totalCa = 0;
        double totalFe = 0, totalCu = 0, totalMn = 0, totalZn = 0, totalB = 0, totalMo = 0, totalCl = 0;
        double totalSi = 0, totalSe = 0, totalNa = 0;

        foreach (Fertilizer fertilizer in collection)
        {
            totalNo3 += fertilizer.Nitrogen.Nitrate * fertilizer.Weight.Value;
            totalNh4 += fertilizer.Nitrogen.Ammonium * fertilizer.Weight.Value;
            totalNh2 += fertilizer.Nitrogen.Amine * fertilizer.Weight.Value;
            totalP += fertilizer.Phosphorus.Value * fertilizer.Weight.Value;
            totalK += fertilizer.Potassium.Value * fertilizer.Weight.Value;
            totalMg += fertilizer.Magnesium.Value * fertilizer.Weight.Value;
            totalS += fertilizer.Sulfur.Value * fertilizer.Weight.Value;
            totalCa += fertilizer.Calcium.Value * fertilizer.Weight.Value;
            totalFe += fertilizer.Iron.Value * fertilizer.Weight.Value;
            totalCu += fertilizer.Copper.Value * fertilizer.Weight.Value;
            totalMn += fertilizer.Manganese.Value * fertilizer.Weight.Value;
            totalZn += fertilizer.Zinc.Value * fertilizer.Weight.Value;
            totalB += fertilizer.Boron.Value * fertilizer.Weight.Value;
            totalMo += fertilizer.Molybdenum.Value * fertilizer.Weight.Value;
            totalCl += fertilizer.Chlorine.Value * fertilizer.Weight.Value;
            totalSi += fertilizer.Silicon.Value * fertilizer.Weight.Value;
            totalSe += fertilizer.Selenium.Value * fertilizer.Weight.Value;
            totalNa += fertilizer.Sodium.Value * fertilizer.Weight.Value;
        }

        return new Ppm(
            nitrogen: new NitrogenPpm(
                nitrate: totalNo3 / waterLiters * OptimizationSettings.ConversionFactor,
                ammonium: totalNh4 / waterLiters * OptimizationSettings.ConversionFactor,
                amine: totalNh2 / waterLiters * OptimizationSettings.ConversionFactor),
            phosphorus: new PhosphorusPpm(totalP / waterLiters * OptimizationSettings.ConversionFactor),
            potassium: new PotassiumPpm(totalK / waterLiters * OptimizationSettings.ConversionFactor),
            calcium: new CalciumPpm(totalCa / waterLiters * OptimizationSettings.ConversionFactor),
            magnesium: new MagnesiumPpm(totalMg / waterLiters * OptimizationSettings.ConversionFactor),
            sulfur: new SulfurPpm(totalS / waterLiters * OptimizationSettings.ConversionFactor),
            iron: new IronPpm(totalFe / waterLiters * OptimizationSettings.ConversionFactor),
            copper: new CopperPpm(totalCu / waterLiters * OptimizationSettings.ConversionFactor),
            manganese: new ManganesePpm(totalMn / waterLiters * OptimizationSettings.ConversionFactor),
            zinc: new ZincPpm(totalZn / waterLiters * OptimizationSettings.ConversionFactor),
            boron: new BoronPpm(totalB / waterLiters * OptimizationSettings.ConversionFactor),
            molybdenum: new MolybdenumPpm(totalMo / waterLiters * OptimizationSettings.ConversionFactor),
            chlorine: new ChlorinePpm(totalCl / waterLiters * OptimizationSettings.ConversionFactor),
            silicon: new SiliconPpm(totalSi / waterLiters * OptimizationSettings.ConversionFactor),
            selenium: new SeleniumPpm(totalSe / waterLiters * OptimizationSettings.ConversionFactor),
            sodium: new SodiumPpm(totalNa / waterLiters * OptimizationSettings.ConversionFactor),
            liters: new WaterVolumeLitersPpm(waterLiters)
        );
    }
}