namespace NPKOptimizer.Common;

public abstract class BuilderBase<TBuilder> where TBuilder : BuilderBase<TBuilder>
{
    protected readonly HashSet<string> PropertiesSet = new();

    protected abstract TBuilder Self { get; }

    protected TBuilder SetValue<T>(ref T field, T value, string propertyName)
    {
        if (PropertiesSet.Add(propertyName))
        {
            field = value;
            return Self;
        }

        throw new InvalidOperationException($"Property {propertyName} has already been set.");
    }

    public abstract object Build();
}