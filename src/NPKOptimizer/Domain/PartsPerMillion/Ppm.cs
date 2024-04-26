using NPKOptimizer.Common;
using NPKOptimizer.Domain.PartsPerMillion.ValueObjects;

namespace NPKOptimizer.Domain.PartsPerMillion;

public class Ppm
{
    public NitrogenPpm Nitrogen { get; set;}
    public PhosphorusPpm Phosphorus { get; set;}
    public PotassiumPpm Potassium { get; set;}
    public CalciumPpm Calcium { get; set;}
    public MagnesiumPpm Magnesium { get; set;}
    public SulfurPpm Sulfur { get; set;}
    public IronPpm Iron { get; set;}
    public CopperPpm Copper { get; set;}
    public ManganesePpm Manganese { get; set;}
    public ZincPpm Zinc { get; set;}
    public BoronPpm Boron { get; set;}
    public MolybdenumPpm Molybdenum { get; set;}
    public ChlorinePpm Chlorine { get; set;}
    public SiliconPpm Silicon { get; set;}
    public SeleniumPpm Selenium { get; set;}
    public SodiumPpm Sodium { get; set;}

    public double Value => Nitrogen.Value + Phosphorus.Value + Potassium.Value + Calcium.Value + Magnesium.Value + Sulfur.Value +
                           Iron.Value + Copper.Value + Manganese.Value + Zinc.Value + Boron.Value + Molybdenum.Value +
                           Chlorine.Value + Sodium.Value + Silicon.Value + Selenium.Value;

    public  Ppm(){}

    public Ppm(NitrogenPpm nitrogen, PhosphorusPpm phosphorus, PotassiumPpm potassium, CalciumPpm calcium, MagnesiumPpm magnesium, SulfurPpm sulfur, IronPpm iron,
        CopperPpm copper, ManganesePpm manganese, ZincPpm zinc, BoronPpm boron, MolybdenumPpm molybdenum, ChlorinePpm chlorine, SiliconPpm silicon,
        SeleniumPpm selenium, SodiumPpm sodium)
    {
        ArgumentNullException.ThrowIfNull(nitrogen);
        ArgumentOutOfRangeException.ThrowIfNegative(nitrogen.Ammonium);
        ArgumentOutOfRangeException.ThrowIfNegative(nitrogen.Amine);
        ArgumentOutOfRangeException.ThrowIfNegative(nitrogen.Nitrate);
        Nitrogen = nitrogen;
        
        ArgumentNullException.ThrowIfNull(phosphorus);
        ArgumentOutOfRangeException.ThrowIfNegative(phosphorus.Value);
        Phosphorus = phosphorus;

        ArgumentNullException.ThrowIfNull(potassium);
        ArgumentOutOfRangeException.ThrowIfNegative(potassium.Value);
        Potassium = potassium;

        ArgumentNullException.ThrowIfNull(calcium);
        ArgumentOutOfRangeException.ThrowIfNegative(calcium.Value);
        Calcium = calcium;

        ArgumentNullException.ThrowIfNull(magnesium);
        ArgumentOutOfRangeException.ThrowIfNegative(magnesium.Value);
        Magnesium = magnesium;

        ArgumentNullException.ThrowIfNull(sulfur);
        ArgumentOutOfRangeException.ThrowIfNegative(sulfur.Value);
        Sulfur = sulfur;

        ArgumentNullException.ThrowIfNull(iron);
        ArgumentOutOfRangeException.ThrowIfNegative(iron.Value);
        Iron = iron;

        ArgumentNullException.ThrowIfNull(copper);
        ArgumentOutOfRangeException.ThrowIfNegative(copper.Value);
        Copper = copper;

        ArgumentNullException.ThrowIfNull(manganese);
        ArgumentOutOfRangeException.ThrowIfNegative(manganese.Value);
        Manganese = manganese;

        ArgumentNullException.ThrowIfNull(zinc);
        ArgumentOutOfRangeException.ThrowIfNegative(zinc.Value);
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