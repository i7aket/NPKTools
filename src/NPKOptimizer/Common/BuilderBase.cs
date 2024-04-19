namespace NPKOptimizer.Common;

public abstract class BuilderBase<TBuilder> where TBuilder : BuilderBase<TBuilder>
{
    private readonly HashSet<string> _addedProperties = new();

    protected TBuilder SetElementValue<TField>(ref TField field, TField value, string propertyName)
    {
        if (!_addedProperties.Add(propertyName))
        {
            throw new InvalidOperationException($"Property {propertyName} has already been set.");
        }

        field = value;
        return (TBuilder)this; 
    }
}