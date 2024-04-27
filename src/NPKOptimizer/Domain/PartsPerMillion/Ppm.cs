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
    public WaterVolumeLitersPpm Liters { get; set;}

    public double Value => Nitrogen.Value + Phosphorus.Value + Potassium.Value + Calcium.Value + Magnesium.Value + Sulfur.Value +
                           Iron.Value + Copper.Value + Manganese.Value + Zinc.Value + Boron.Value + Molybdenum.Value +
                           Chlorine.Value + Sodium.Value + Silicon.Value + Selenium.Value;

    public  Ppm(){}

    public Ppm(NitrogenPpm nitrogen, PhosphorusPpm phosphorus, PotassiumPpm potassium, CalciumPpm calcium, MagnesiumPpm magnesium, SulfurPpm sulfur, IronPpm iron,
        CopperPpm copper, ManganesePpm manganese, ZincPpm zinc, BoronPpm boron, MolybdenumPpm molybdenum, ChlorinePpm chlorine, SiliconPpm silicon,
        SeleniumPpm selenium, SodiumPpm sodium, WaterVolumeLitersPpm liters)
    {
        ArgumentNullException.ThrowIfNull(nitrogen);
        Nitrogen = nitrogen;
        
        ArgumentNullException.ThrowIfNull(phosphorus);
        Phosphorus = phosphorus;

        ArgumentNullException.ThrowIfNull(potassium);
        Potassium = potassium;

        ArgumentNullException.ThrowIfNull(calcium);
        Calcium = calcium;

        ArgumentNullException.ThrowIfNull(magnesium);
        Magnesium = magnesium;

        ArgumentNullException.ThrowIfNull(sulfur);
        Sulfur = sulfur;

        ArgumentNullException.ThrowIfNull(iron);
        Iron = iron;

        ArgumentNullException.ThrowIfNull(copper);
        Copper = copper;

        ArgumentNullException.ThrowIfNull(manganese);
        Manganese = manganese;

        ArgumentNullException.ThrowIfNull(zinc);
        Zinc = zinc;

        ArgumentNullException.ThrowIfNull(boron);
        Boron = boron;

        ArgumentNullException.ThrowIfNull(molybdenum);
        Molybdenum = molybdenum;

        ArgumentNullException.ThrowIfNull(chlorine);
        Chlorine = chlorine;

        ArgumentNullException.ThrowIfNull(silicon);
        Silicon = silicon;

        ArgumentNullException.ThrowIfNull(selenium);
        Selenium = selenium;

        ArgumentNullException.ThrowIfNull(sodium);
        Sodium = sodium;
        
        ArgumentNullException.ThrowIfNull(liters);
        Liters = liters;
    }
}