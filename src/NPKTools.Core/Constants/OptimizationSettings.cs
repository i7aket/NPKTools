namespace NPKTools.Core.Constants;

/// <summary>
/// Numeric constants governing the optimization. <see cref="ConversionFactor"/> converts between
/// percent-by-weight fertilizer composition and ppm; <see cref="RoundingPrecision"/> is the number
/// of decimal places solver output is rounded to.
/// </summary>
public static class OptimizationSettings
{
    public const double ConversionFactor = 10.0;
    public const int RoundingPrecision = 8;
}
