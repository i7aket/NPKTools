using NPKOptimizer.Domain.Elements;
using NPKOptimizer.Domain.Fertilizers.ValueObjects;

namespace NPKOptimizer.Components.Builders;

public class FertilizerCompositionBuilder 
{
    private readonly Dictionary<Element, int> _atoms = new ();
    
    public FertilizerComposition Build()
    {
        return new FertilizerComposition
        {
            Atoms = new Dictionary<Element, int>(_atoms)
        };
    }
    
    public FertilizerCompositionBuilder Add(Element element, int count = 1)
    {
        if (!_atoms.TryAdd(element, count))
        {
            throw new InvalidOperationException($"Element {element.Symbol} has already been added.");
        }

        return this;
    }

}