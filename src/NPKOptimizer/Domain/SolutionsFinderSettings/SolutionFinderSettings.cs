using NPKOptimizer.Common;
using NPKOptimizer.Domain.SolutionsFinderSettings.ValueObjects;

// ReSharper disable UnusedMember.Global

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
        ThrowIf.NotInRange(rangeFactor.Value, 0, 1);
        RangeFactor = rangeFactor;
        
        ArgumentNullException.ThrowIfNull(nitrogen);
        ThrowIf.NotInRange(nitrogen.Value, 0, 1);
        Nitrogen = nitrogen;

        ArgumentNullException.ThrowIfNull(phosphorus);
        ThrowIf.NotInRange(phosphorus.Value, 0, 1);
        Phosphorus = phosphorus;

        ArgumentNullException.ThrowIfNull(potassium);
        ThrowIf.NotInRange(potassium.Value, 0, 1);
        Potassium = potassium;

        ArgumentNullException.ThrowIfNull(calcium);
        ThrowIf.NotInRange(calcium.Value, 0, 1);
        Calcium = calcium;

        ArgumentNullException.ThrowIfNull(magnesium);
        ThrowIf.NotInRange(magnesium.Value, 0, 1);
        Magnesium = magnesium;

        ArgumentNullException.ThrowIfNull(sulfur);
        ThrowIf.NotInRange(sulfur.Value, 0, 1);
        Sulfur = sulfur;

        ArgumentNullException.ThrowIfNull(iron);
        ThrowIf.NotInRange(iron.Value, 0, 1);
        Iron = iron;

        ArgumentNullException.ThrowIfNull(copper);
        ThrowIf.NotInRange(copper.Value, 0, 1);
        Copper = copper;

        ArgumentNullException.ThrowIfNull(manganese);
        ThrowIf.NotInRange(manganese.Value, 0, 1);
        Manganese = manganese;

        ArgumentNullException.ThrowIfNull(zinc);
        ThrowIf.NotInRange(zinc.Value, 0, 1);
        Zinc = zinc;

        ArgumentNullException.ThrowIfNull(boron);
        ThrowIf.NotInRange(boron.Value, 0, 1);
        Boron = boron;

        ArgumentNullException.ThrowIfNull(molybdenum);
        ThrowIf.NotInRange(molybdenum.Value, 0, 1);
        Molybdenum = molybdenum;

        ArgumentNullException.ThrowIfNull(chlorine);
        ThrowIf.NotInRange(chlorine.Value, 0, 1);
        Chlorine = chlorine;

        ArgumentNullException.ThrowIfNull(silicon);
        ThrowIf.NotInRange(silicon.Value, 0, 1);
        Silicon = silicon;

        ArgumentNullException.ThrowIfNull(selenium);
        ThrowIf.NotInRange(selenium.Value, 0, 1);
        Selenium = selenium;

        ArgumentNullException.ThrowIfNull(sodium);
        ThrowIf.NotInRange(sodium.Value, 0, 1);
        Sodium = sodium;
    }
}