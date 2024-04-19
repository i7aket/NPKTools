using NPKOptimizer.Common;
using NPKOptimizer.Domain.Elements;
using NPKOptimizer.Domain.Elements.ValueObjects;
using NPKOptimizer.Enums;

namespace NPKOptimizer.Components.Builders;

public class AtomBuilder : BuilderBase<AtomBuilder>
{
    private string _name = null!;
    private string _symbol = null!;
    private int _atomicNumber;
    private double _atomicMass;
    private List<int> _oxidationStates = new ();
    private double? _electronegativity;
    private double _ionizationEnergy;
    private double? _electronAffinity;
    private double _atomicRadius;
    private double? _meltingPoint;
    private double _boilingPoint;
    private ElementType _type;

    public AtomBuilder Name(string value)
    {
        _name = value;
        return this;
    }

    public AtomBuilder Symbol(string value)
    {
        _symbol = value;
        return this;
    }

    public AtomBuilder AtomicNumber(int value)
    {
        _atomicNumber = value;
        return this;
    }

    public AtomBuilder AtomicMass(double value)
    {
        _atomicMass = value;
        return this;
    }

    public AtomBuilder OxidationStates(params int[] values)
    {
        _oxidationStates = new List<int>(values);
        return this;
    }

    public AtomBuilder Electronegativity(double? value)
    {
        _electronegativity = value;
        return this;
    }

    public AtomBuilder IonizationEnergy(double value)
    {
        _ionizationEnergy = value;
        return this;
    }

    public AtomBuilder ElectronAffinity(double value)
    {
        _electronAffinity = value;
        return this;
    }

    public AtomBuilder AtomicRadius(double value)
    {
        _atomicRadius = value;
        return this;
    }

    public AtomBuilder MeltingPoint(double? value)
    {
        _meltingPoint = value;
        return this;
    }

    public AtomBuilder BoilingPoint(double value)
    {
        _boilingPoint = value;
        return this;
    }

    public AtomBuilder Type(ElementType type)
    {
        _type = type;
        return this;
    }

    public Element Build()
    {
        return new Element(
            new AtomicName(_name),
            new AtomicSymbol(_symbol),
            new AtomicNumber(_atomicNumber),
            new AtomicMass(_atomicMass),
            new OxidationStates(_oxidationStates),
            new Electronegativity(_electronegativity),
            new IonizationEnergy(_ionizationEnergy),
            new ElectronAffinity(_electronAffinity),
            new AtomicRadius(_atomicRadius),
            new MeltingPoint(_meltingPoint),
            new BoilingPoint(_boilingPoint),
            _type
        );
    }
}
