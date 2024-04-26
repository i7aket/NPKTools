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
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price.Value);
        Price = price;

        ArgumentNullException.ThrowIfNull(nitrogen);
        ArgumentOutOfRangeException.ThrowIfNegative(nitrogen.Amine);
        ArgumentOutOfRangeException.ThrowIfNegative(nitrogen.Ammonium);
        ArgumentOutOfRangeException.ThrowIfNegative(nitrogen.Nitrate);
        Nitrogen = nitrogen;

        ArgumentNullException.ThrowIfNull(phosphorus);
        ArgumentOutOfRangeException.ThrowIfNegative(phosphorus.Value);
        Phosphorus = phosphorus;

        ArgumentNullException.ThrowIfNull(potassium);
        ArgumentOutOfRangeException.ThrowIfNegative(potassium.Value);
        Potassium = potassium;

        ArgumentNullException.ThrowIfNull(calcium);
        ArgumentOutOfRangeException.ThrowIfNegative(calcium.CaEdta);
        ArgumentOutOfRangeException.ThrowIfNegative(calcium.CaNonChelated);
        Calcium = calcium;

        ArgumentNullException.ThrowIfNull(magnesium);
        ArgumentOutOfRangeException.ThrowIfNegative(magnesium.MgEdta);
        ArgumentOutOfRangeException.ThrowIfNegative(magnesium.MgNonChelated);
        Magnesium = magnesium;

        ArgumentNullException.ThrowIfNull(sulfur);
        ArgumentOutOfRangeException.ThrowIfNegative(sulfur.Value);
        Sulfur = sulfur;

        ArgumentNullException.ThrowIfNull(iron);
        ArgumentOutOfRangeException.ThrowIfNegative(iron.FeNonChelated);
        ArgumentOutOfRangeException.ThrowIfNegative(iron.FeEdta);
        ArgumentOutOfRangeException.ThrowIfNegative(iron.FeDtpa);
        ArgumentOutOfRangeException.ThrowIfNegative(iron.FeEddha);
        ArgumentOutOfRangeException.ThrowIfNegative(iron.FeHbed);
        ArgumentOutOfRangeException.ThrowIfNegative(iron.FeOrthoPart);
        Iron = iron;

        ArgumentNullException.ThrowIfNull(copper);
        ArgumentOutOfRangeException.ThrowIfNegative(copper.CuNonChelated);
        ArgumentOutOfRangeException.ThrowIfNegative(copper.CuEdta);
        Copper = copper;

        ArgumentNullException.ThrowIfNull(manganese);
        ArgumentOutOfRangeException.ThrowIfNegative(manganese.MnEdta);
        ArgumentOutOfRangeException.ThrowIfNegative(manganese.MnNonChelated);
        Manganese = manganese;

        ArgumentNullException.ThrowIfNull(zinc);
        ArgumentOutOfRangeException.ThrowIfNegative(zinc.ZnNonChelated);
        ArgumentOutOfRangeException.ThrowIfNegative(zinc.ZnEdta);
        Zinc = zinc;

        ArgumentNullException.ThrowIfNull(boron);
        ArgumentOutOfRangeException.ThrowIfNegative(boron.Value);
        Boron = boron;

        ArgumentNullException.ThrowIfNull(molybdenum);
        ArgumentOutOfRangeException.ThrowIfNegative(molybdenum.Value);
        Molybdenum = molybdenum;

        ArgumentNullException.ThrowIfNull(chlorine);
        ArgumentOutOfRangeException.ThrowIfNegative(chlorine.Value);
        Chlorine = chlorine;

        ArgumentNullException.ThrowIfNull(silicon);
        ArgumentOutOfRangeException.ThrowIfNegative(silicon.Value);
        Silicon = silicon;

        ArgumentNullException.ThrowIfNull(selenium);
        ArgumentOutOfRangeException.ThrowIfNegative(selenium.Value);
        Selenium = selenium;

        ArgumentNullException.ThrowIfNull(sodium);
        ArgumentOutOfRangeException.ThrowIfNegative(sodium.Value);
        Sodium = sodium;
    }
}