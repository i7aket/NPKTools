namespace SYT.NPKTools.Calculator;

/// <summary>How much a grower knows about their source water.</summary>
public enum WaterInputMode
{
    /// <summary>Reverse osmosis, distilled or rain. Nothing dissolved.</summary>
    Osmosis,

    /// <summary>A meter reading and a water type.</summary>
    Conductivity,

    /// <summary>A meter reading, a water type, and hardness drop tests.</summary>
    ConductivityWithTests,

    /// <summary>A full laboratory analysis.</summary>
    Analysis,
}

/// <summary>The scale a conductivity meter prints in.</summary>
/// <remarks>
/// A "ppm" meter shows conductivity multiplied by a fixed factor, and manufacturers do not agree on
/// the factor. Hanna uses 500, Truncheon 700, so the same water reads 225 on one and 315 on the other.
/// </remarks>
public enum EcUnit
{
    /// <summary>Millisiemens per centimetre.</summary>
    MilliSiemensPerCm,

    /// <summary>Parts per million on the 500 scale.</summary>
    Ppm500,

    /// <summary>Parts per million on the 700 scale.</summary>
    Ppm700,
}
