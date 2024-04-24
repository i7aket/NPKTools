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
        ThrowIf.LowerThan(feNonChelated,0);
        FeNonChelated = feNonChelated;

        ThrowIf.LowerThan(feEdta,0);
        FeEdta = feEdta;

        ThrowIf.LowerThan(feDtpa,0);
        FeDtpa = feDtpa;

        ThrowIf.LowerThan(feEddha,0);
        FeEddha = feEddha;

        ThrowIf.LowerThan(feHbed,0);
        FeHbed = feHbed;

        ThrowIf.LowerThan(feOrthoPart,0);
        FeOrthoPart = feOrthoPart;
    }
}