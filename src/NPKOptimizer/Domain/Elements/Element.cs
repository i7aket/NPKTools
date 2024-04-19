using NPKOptimizer.Common;
using NPKOptimizer.Domain.Elements.ValueObjects;
using NPKOptimizer.Enums;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable NotAccessedPositionalProperty.Global

namespace NPKOptimizer.Domain.Elements;

public record Element
{
    public AtomicName Name { get; } 
    public AtomicSymbol Symbol { get; }
    public AtomicNumber AtomicNumber { get; }
    public AtomicMass AtomicMass { get; }
    public OxidationStates OxidationStates { get; } 
    public Electronegativity Electronegativity { get; }
    public IonizationEnergy IonizationEnergy { get; }
    public ElectronAffinity ElectronAffinity { get; }
    public AtomicRadius AtomicRadius { get; }
    public MeltingPoint MeltingPoint { get; }
    public BoilingPoint BoilingPoint { get; } 
    public ElementType Type { get; }

    public Element(
        AtomicName name,
        AtomicSymbol symbol,
        AtomicNumber atomicNumber,
        AtomicMass atomicMass,
        OxidationStates oxidationStates,
        Electronegativity electronegativity,
        IonizationEnergy ionizationEnergy,
        ElectronAffinity electronAffinity,
        AtomicRadius atomicRadius,
        MeltingPoint meltingPoint,
        BoilingPoint boilingPoint,
        ElementType type
    )
    {
        Validate.NotNull(name);
        Name = name;
        
        Validate.NotNull(symbol);
        Symbol = symbol;

        Validate.NotNull(atomicNumber);
        AtomicNumber = atomicNumber;
        
        Validate.NotNull(atomicMass);
        AtomicMass = atomicMass;
        
        Validate.NotNull(oxidationStates);
        OxidationStates = oxidationStates;
        
        Validate.NotNull(electronegativity);
        Electronegativity = electronegativity;
        
        Validate.NotNull(ionizationEnergy);
        IonizationEnergy = ionizationEnergy;
        
        Validate.NotNull(electronAffinity);
        ElectronAffinity = electronAffinity;

        Validate.NotNull(atomicRadius);
        AtomicRadius = atomicRadius;
        
        Validate.NotNull(meltingPoint);
        MeltingPoint = meltingPoint;

        Validate.NotNull(boilingPoint);
        BoilingPoint = boilingPoint;
        
        Validate.NotDefault(type);
        Type = type;
    }
}