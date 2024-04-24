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
        ThrowIf.LowerThan(nitrogen.Ammonium,0);
        ThrowIf.LowerThan(nitrogen.Amine,0);
        ThrowIf.LowerThan(nitrogen.Nitrate,0);
        Nitrogen = nitrogen;
        
        ArgumentNullException.ThrowIfNull(phosphorus);
        ThrowIf.LowerThan(phosphorus.Value, 0);
        Phosphorus = phosphorus;

        ArgumentNullException.ThrowIfNull(potassium);
        ThrowIf.LowerThan(potassium.Value, 0);
        Potassium = potassium;

        ArgumentNullException.ThrowIfNull(calcium);
        ThrowIf.LowerThan(calcium.Value, 0);
        Calcium = calcium;

        ArgumentNullException.ThrowIfNull(magnesium);
        ThrowIf.LowerThan(magnesium.Value, 0);
        Magnesium = magnesium;

        ArgumentNullException.ThrowIfNull(sulfur);
        ThrowIf.LowerThan(sulfur.Value, 0);
        Sulfur = sulfur;

        ArgumentNullException.ThrowIfNull(iron);
        ThrowIf.LowerThan(iron.Value, 0);
        Iron = iron;

        ArgumentNullException.ThrowIfNull(copper);
        ThrowIf.LowerThan(copper.Value, 0);
        Copper = copper;

        ArgumentNullException.ThrowIfNull(manganese);
        ThrowIf.LowerThan(manganese.Value, 0);
        Manganese = manganese;

        ArgumentNullException.ThrowIfNull(zinc);
        ThrowIf.LowerThan(zinc.Value, 0);
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