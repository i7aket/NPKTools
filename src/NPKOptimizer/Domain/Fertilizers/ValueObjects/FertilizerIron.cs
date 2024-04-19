using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerIron : FieldBase
{
    public double FeNonChelated { get; }
    public double FeEdta { get; }
    public double FeDtpa { get; }
    public double FeEddha { get; }
    public double FeHbed { get; }
    public double FeOrthoPart { get; }

    public FertilizerIron(double feNonChelated = 0, double feEdta = 0, double feDtpa = 0, double feEddha = 0,
        double feHbed = 0, double feOrthoPart = 0) : base(feNonChelated + feEdta + feDtpa + feEddha + feHbed)
    {
        Validate.Positive(feNonChelated);
        FeNonChelated = feNonChelated;

        Validate.Positive(feEdta);
        FeEdta = feEdta;

        Validate.Positive(feDtpa);
        FeDtpa = feDtpa;

        Validate.Positive(feEddha);
        FeEddha = feEddha;

        Validate.Positive(feHbed);
        FeHbed = feHbed;

        Validate.Positive(feOrthoPart);
        FeOrthoPart = feOrthoPart;
    }
}