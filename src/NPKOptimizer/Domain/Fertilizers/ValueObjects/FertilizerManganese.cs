using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerManganese : FieldBase
{
    public double MnNonChelated { get; }
    public double MnEdta { get; }

    public FertilizerManganese(double mnNonChelated = 0, double mnEdta = 0) : base(mnNonChelated + mnEdta)
    {
        Validate.Positive(mnNonChelated);
        MnNonChelated = mnNonChelated;

        Validate.Positive(mnEdta);
        MnEdta = mnEdta;
    }
}