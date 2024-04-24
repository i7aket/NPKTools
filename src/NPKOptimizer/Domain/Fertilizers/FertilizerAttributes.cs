using NPKOptimizer.Common;
using NPKOptimizer.Domain.Fertilizers.ValueObjects;
namespace NPKOptimizer.Domain.Fertilizers;

public class FertilizerAttributes 
{
    public FertilizerPrice Price { get; set; }
    public FertilizerNitrogen Nitrogen { get; set;}
    public FertilizerPhosphorus Phosphorus { get; set;}
    public FertilizerPotassium Potassium { get; set;}
    public FertilizerCalcium Calcium { get; set;}
    public FertilizerMagnesium Magnesium { get; set;}
    public FertilizerSulfur Sulfur { get; set;}
    public FertilizerIron Iron { get; set;}
    public FertilizerCopper Copper { get; set;}
    public FertilizerManganese Manganese { get; set;}
    public FertilizerZinc Zinc { get; set;}
    public FertilizerBoron Boron { get; set;}
    public FertilizerMolybdenum Molybdenum { get; set;}
    public FertilizerChlorine Chlorine { get; set;}
    public FertilizerSilicon Silicon { get; set;}
    public FertilizerSelenium Selenium { get; set;}
    public FertilizerSodium Sodium { get; set;}
    
    public FertilizerAttributes(){}
    
    public FertilizerAttributes(
        FertilizerPrice price,
        FertilizerNitrogen nitrogen,
        FertilizerPhosphorus phosphorus,
        FertilizerPotassium potassium,
        FertilizerCalcium calcium,
        FertilizerMagnesium magnesium,
        FertilizerSulfur sulfur,
        FertilizerIron iron,
        FertilizerCopper copper,
        FertilizerManganese manganese,
        FertilizerZinc zinc,
        FertilizerBoron boron,
        FertilizerMolybdenum molybdenum,
        FertilizerChlorine chlorine,
        FertilizerSilicon silicon,
        FertilizerSelenium selenium,
        FertilizerSodium sodium)
    {
        ArgumentNullException.ThrowIfNull(price);
        ThrowIf.LowerThan(price.Value, 0);
        Price = price;

        ArgumentNullException.ThrowIfNull(nitrogen);
        ThrowIf.LowerThan(nitrogen.Amine, 0);
        ThrowIf.LowerThan(nitrogen.Ammonium, 0);
        ThrowIf.LowerThan(nitrogen.Nitrate, 0);
        Nitrogen = nitrogen;

        ArgumentNullException.ThrowIfNull(phosphorus);
        ThrowIf.LowerThan(phosphorus.Value, 0);
        Phosphorus = phosphorus;

        ArgumentNullException.ThrowIfNull(potassium);
        ThrowIf.LowerThan(potassium.Value, 0);
        Potassium = potassium;

        ArgumentNullException.ThrowIfNull(calcium);
        ThrowIf.LowerThan(calcium.CaEdta, 0);
        ThrowIf.LowerThan(calcium.CaNonChelated, 0);
        Calcium = calcium;

        ArgumentNullException.ThrowIfNull(magnesium);
        ThrowIf.LowerThan(magnesium.MgEdta, 0);
        ThrowIf.LowerThan(magnesium.MgNonChelated, 0);
        Magnesium = magnesium;

        ArgumentNullException.ThrowIfNull(sulfur);
        ThrowIf.LowerThan(sulfur.Value, 0);
        Sulfur = sulfur;

        ArgumentNullException.ThrowIfNull(iron);
        ThrowIf.LowerThan(iron.FeNonChelated, 0);
        ThrowIf.LowerThan(iron.FeEdta, 0);
        ThrowIf.LowerThan(iron.FeDtpa, 0);
        ThrowIf.LowerThan(iron.FeEddha, 0);
        ThrowIf.LowerThan(iron.FeHbed, 0);
        ThrowIf.LowerThan(iron.FeOrthoPart, 0);
        Iron = iron;

        ArgumentNullException.ThrowIfNull(copper);
        ThrowIf.LowerThan(copper.CuNonChelated, 0);
        ThrowIf.LowerThan(copper.CuEdta, 0);
        Copper = copper;

        ArgumentNullException.ThrowIfNull(manganese);
        ThrowIf.LowerThan(manganese.MnEdta, 0);
        ThrowIf.LowerThan(manganese.MnNonChelated, 0);
        Manganese = manganese;

        ArgumentNullException.ThrowIfNull(zinc);
        ThrowIf.LowerThan(zinc.ZnNonChelated, 0);
        ThrowIf.LowerThan(zinc.ZnEdta, 0);
        Zinc = zinc;

        ArgumentNullException.ThrowIfNull(boron);
        ThrowIf.LowerThan(boron.Value, 0);
        Boron = boron;

        ArgumentNullException.ThrowIfNull(molybdenum);
        ThrowIf.LowerThan(molybdenum.Value, 0);
        Molybdenum = molybdenum;

        ArgumentNullException.ThrowIfNull(chlorine);
        ThrowIf.LowerThan(chlorine.Value, 0);
        Chlorine = chlorine;

        ArgumentNullException.ThrowIfNull(silicon);
        ThrowIf.LowerThan(silicon.Value, 0);
        Silicon = silicon;

        ArgumentNullException.ThrowIfNull(selenium);
        ThrowIf.LowerThan(selenium.Value, 0);
        Selenium = selenium;

        ArgumentNullException.ThrowIfNull(sodium);
        ThrowIf.LowerThan(sodium.Value, 0);
        Sodium = sodium;
    }
}