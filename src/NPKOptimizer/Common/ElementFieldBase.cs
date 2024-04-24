namespace NPKOptimizer.Common;

public abstract record ElementFieldBase
{
    public double Value { get; }

    protected ElementFieldBase(double value)
    {
        ThrowIf.LowerThan(value, 0);
        Value = value;
    }
}