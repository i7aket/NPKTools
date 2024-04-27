using NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

namespace NPKOptimizer.Domain.SolutionsFinderSettings;

public class SolutionFinderSettings
{
    public RangeFactorSettings RangeFactor { get; set; }
    public NitrogenSettings Nitrogen { get; set; }
    public PhosphorusSettings Phosphorus { get; set; }
    public PotassiumSettings Potassium { get;  set;}
    public CalciumSettings Calcium { get;  set;}
    public MagnesiumSettings Magnesium { get;  set;}
    public SulfurSettings Sulfur { get;  set;}
    public ChlorineSettings Chlorine { get;  set;}
    public IronSettings Iron { get;  set;}
    public CopperSettings Copper { get;  set;}
    public ManganeseSettings Manganese { get;  set;}
    public ZincSettings Zinc { get;  set;}
    public BoronSettings Boron { get; set; }
    public MolybdenumSettings Molybdenum { get;  set;}
    public SiliconSettings Silicon { get;  set;}
    public SeleniumSettings Selenium { get;  set;}
    public SodiumSettings Sodium { get; set; }
    public SolutionFinderSettings(){} 
    public SolutionFinderSettings(RangeFactorSettings rangeFactor, NitrogenSettings nitrogen, PhosphorusSettings phosphorus, PotassiumSettings potassium, CalciumSettings calcium,
        MagnesiumSettings magnesium, SulfurSettings sulfur, ChlorineSettings chlorine, IronSettings iron, CopperSettings copper,
        ManganeseSettings manganese, ZincSettings zinc, BoronSettings boron, MolybdenumSettings molybdenum, SiliconSettings silicon,
        SeleniumSettings selenium, SodiumSettings sodium)
    {
        ArgumentNullException.ThrowIfNull(rangeFactor);
        RangeFactor = rangeFactor;
        
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
    }
}