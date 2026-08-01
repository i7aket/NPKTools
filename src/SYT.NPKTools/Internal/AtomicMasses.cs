namespace SYT.NPKTools.Internal;

/// <summary>
/// Standard atomic weights in grams per mole, from the IUPAC 2021 table.
/// </summary>
/// <remarks>
/// These convert ppm to millimoles per litre, and only the ratio matters, so the four significant
/// figures given here are far finer than any scale a grower owns.
/// </remarks>
internal static class AtomicMasses
{
    public const double N = 14.007;
    public const double P = 30.974;
    public const double K = 39.098;
    public const double Ca = 40.078;
    public const double Mg = 24.305;
    public const double S = 32.06;
    public const double Fe = 55.845;
    public const double Cu = 63.546;
    public const double Mn = 54.938;
    public const double Zn = 65.38;
    public const double B = 10.81;
    public const double Mo = 95.95;
    public const double Cl = 35.45;
    public const double Si = 28.085;
    public const double Se = 78.971;
    public const double Na = 22.990;
}
