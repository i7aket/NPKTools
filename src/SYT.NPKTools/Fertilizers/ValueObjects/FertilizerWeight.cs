namespace SYT.NPKTools.Fertilizers;
/// <summary>
/// Represents the physical weight of the fertilizer, in grams.
/// </summary>
/// <remarks>
/// Grams is not a convention but an arithmetic requirement. The ppm calculation is
/// <c>percent × weight ÷ liters × 10</c>, which yields 1000 ppm for one gram of a pure nutrient
/// dissolved in one liter — correct only if the weight is grams. Passing kilograms overstates every
/// result by a factor of 1000.
/// </remarks>
public record FertilizerWeight(double Value = 0) : ElementFieldBase(Value);


