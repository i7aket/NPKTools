using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;

public record FertilizerIron : ElementFieldBase
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
        ArgumentOutOfRangeException.ThrowIfNegative(feNonChelated);
        FeNonChelated = feNonChelated;

        ArgumentOutOfRangeException.ThrowIfNegative(feEdta);
        FeEdta = feEdta;

        ArgumentOutOfRangeException.ThrowIfNegative(feDtpa);
        FeDtpa = feDtpa;

        ArgumentOutOfRangeException.ThrowIfNegative(feEddha);
        FeEddha = feEddha;

        ArgumentOutOfRangeException.ThrowIfNegative(feHbed);
        FeHbed = feHbed;

        ArgumentOutOfRangeException.ThrowIfNegative(feOrthoPart);
        FeOrthoPart = feOrthoPart;
    }
}