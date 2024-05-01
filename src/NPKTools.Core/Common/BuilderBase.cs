namespace NPKTools.Core.Common;

/// <summary>
/// Provides a generic base class for implementing the builder pattern in a type-safe manner.
/// This abstract class enforces constraints on derived builder classes, ensuring they provide
/// mechanisms to chain method calls fluently while maintaining state consistency.
/// </summary>
/// <typeparam name="TBuilder">The type of the concrete builder inheriting from this base class.</typeparam>
public abstract class BuilderBase<TBuilder> where TBuilder : BuilderBase<TBuilder>
{
    /// <summary>
    /// Maintains a set of property names that have been set. This helps in ensuring
    /// that each property can only be set once, enforcing immutability after initial set.
    /// </summary>
    protected readonly HashSet<string> PropertiesSet = new();

    /// <summary>
    /// Provides derived classes with access to the concrete builder instance.
    /// This property is crucial for enabling fluent API style in derived builders.
    /// </summary>
    protected abstract TBuilder Self { get; }

    /// <summary>
    /// Sets a value for a property, identified by name, and ensures the property can only be set once.
    /// This method is designed to support fluent interfaces in builder implementations by allowing
    /// chainable set operations that enforce single assignment per property.
    /// </summary>
    /// <typeparam name="T">The type of the property being set.</typeparam>
    /// <param name="field">A reference to the field backing the property.</param>
    /// <param name="value">The value to set for the property.</param>
    /// <param name="propertyName">The name of the property to be set, used to track if it has been set previously.</param>
    /// <returns>The current builder instance to allow for method chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when attempting to set a property that has already been set.</exception>
    protected TBuilder SetValue<T>(ref T field, T value, string propertyName)
    {
        if (PropertiesSet.Add(propertyName))
        {
            field = value;
            return Self;
        }

        throw new InvalidOperationException($"Property {propertyName} has already been set.");
    }

    /// <summary>
    /// When overridden in a derived class, builds the final object of the specified type.
    /// This method should construct and return the object being built, using the state
    /// accumulated in the builder.
    /// </summary>
    /// <returns>The constructed object.</returns>
    public abstract object Build();
}
