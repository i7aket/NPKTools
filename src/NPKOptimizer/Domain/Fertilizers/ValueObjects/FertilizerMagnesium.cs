using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerMagnesium : FieldBase
{
    public double MgNonChelated { get; }
    public double MgEdta { get; }

    public FertilizerMagnesium(double mgNonChelated = 0, double mgEdta = 0) : base(mgNonChelated + mgEdta)
    {
        Validate.Positive(mgNonChelated);
        Validate.Positive(mgEdta);

        MgNonChelated = mgNonChelated;
        MgEdta = mgEdta;
    }
}