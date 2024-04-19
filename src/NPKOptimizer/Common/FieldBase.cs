namespace NPKOptimizer.Common;

public abstract record FieldBase
{
    public double Value { get; }

    protected FieldBase(double value)
    {
        Validate.Positive(value);
        Value = value;
    }
}