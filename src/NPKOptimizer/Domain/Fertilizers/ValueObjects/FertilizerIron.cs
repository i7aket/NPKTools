using NPKOptimizer.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;
/// <summary>
/// Represents the iron content in the fertilizer, detailed by various chelation forms such as EDTA, DTPA, EDDHA, HBED, and ortho part.
/// This record provides a detailed account of the iron available in different forms, useful for precision agriculture and advanced nutrient management.
/// </summary>
public record FertilizerIron 
{
    public double FeNonChelated { get; }
    public double FeEdta { get; }
    public double FeDtpa { get; }
    public double FeEddha { get; }
    public double FeHbed { get; }
    public double FeOrthoPart { get; }
    public double Value => FeNonChelated + FeEdta + FeDtpa + FeEddha + FeHbed;


    public FertilizerIron(double feNonChelated = 0, double feEdta = 0, double feDtpa = 0, double feEddha = 0,
        double feHbed = 0, double feOrthoPart = 0) 
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