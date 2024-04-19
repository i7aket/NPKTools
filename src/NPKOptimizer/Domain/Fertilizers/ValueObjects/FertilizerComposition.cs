using NPKOptimizer.Domain.Elements;

// ReSharper disable MemberCanBePrivate.Global

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public sealed record FertilizerComposition
{
    public Dictionary<Element, int> Atoms { get; init; } = new();

    public double CalculateMolarMass()
    {
        return Atoms.Sum(atomEntry => atomEntry.Key.AtomicMass.Value * atomEntry.Value);
    }

    public double ElementPercent(Element element)
    {
        if (!Atoms.TryGetValue(element, out int atomCount)) return 0;
        double totalMass = CalculateMolarMass();
        double elementMass = element.AtomicMass.Value * atomCount;
        return elementMass / totalMass * 100;
    }
}