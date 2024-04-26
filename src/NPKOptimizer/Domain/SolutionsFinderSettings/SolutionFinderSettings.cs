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
        ArgumentOutOfRangeException.ThrowIfNegative(rangeFactor.Value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(rangeFactor.Value, 1);
        RangeFactor = rangeFactor;
        
        ArgumentNullException.ThrowIfNull(nitrogen);
        ArgumentOutOfRangeException.ThrowIfNegative(nitrogen.Value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(nitrogen.Value, 1);
        Nitrogen = nitrogen;

        ArgumentNullException.ThrowIfNull(phosphorus);
        ArgumentOutOfRangeException.ThrowIfNegative(phosphorus.Value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(phosphorus.Value, 1);
        Phosphorus = phosphorus;

        ArgumentNullException.ThrowIfNull(potassium);
        ArgumentOutOfRangeException.ThrowIfNegative(potassium.Value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(potassium.Value, 1);
        Potassium = potassium;

        ArgumentNullException.ThrowIfNull(calcium);
        ArgumentOutOfRangeException.ThrowIfNegative(calcium.Value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(calcium.Value, 1);
        Calcium = calcium;

        ArgumentNullException.ThrowIfNull(magnesium);
        ArgumentOutOfRangeException.ThrowIfNegative(magnesium.Value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(magnesium.Value, 1);
        Magnesium = magnesium;

        ArgumentNullException.ThrowIfNull(sulfur);
        ArgumentOutOfRangeException.ThrowIfNegative(sulfur.Value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(sulfur.Value, 1);
        Sulfur = sulfur;

        ArgumentNullException.ThrowIfNull(iron);
        ArgumentOutOfRangeException.ThrowIfNegative(iron.Value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(iron.Value, 1);
        Iron = iron;

        ArgumentNullException.ThrowIfNull(copper);
        ArgumentOutOfRangeException.ThrowIfNegative(copper.Value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(copper.Value, 1);
        Copper = copper;

        ArgumentNullException.ThrowIfNull(manganese);
        ArgumentOutOfRangeException.ThrowIfNegative(manganese.Value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(manganese.Value, 1);
        Manganese = manganese;

        ArgumentNullException.ThrowIfNull(zinc);
        ArgumentOutOfRangeException.ThrowIfNegative(zinc.Value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(zinc.Value, 1);
        Zinc = zinc;

        ArgumentNullException.ThrowIfNull(boron);
        ArgumentOutOfRangeException.ThrowIfNegative(boron.Value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(boron.Value, 1);
        Boron = boron;

        ArgumentNullException.ThrowIfNull(molybdenum);
        ArgumentOutOfRangeException.ThrowIfNegative(molybdenum.Value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(molybdenum.Value, 1);
        Molybdenum = molybdenum;

        ArgumentNullException.ThrowIfNull(chlorine);
        ArgumentOutOfRangeException.ThrowIfNegative(chlorine.Value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(chlorine.Value, 1);
        Chlorine = chlorine;

        ArgumentNullException.ThrowIfNull(silicon);
        ArgumentOutOfRangeException.ThrowIfNegative(silicon.Value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(silicon.Value, 1);
        Silicon = silicon;

        ArgumentNullException.ThrowIfNull(selenium);
        ArgumentOutOfRangeException.ThrowIfNegative(selenium.Value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(selenium.Value, 1);
        Selenium = selenium;

        ArgumentNullException.ThrowIfNull(sodium);
        ArgumentOutOfRangeException.ThrowIfNegative(sodium.Value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(sodium.Value, 1);
        Sodium = sodium;
    }
}