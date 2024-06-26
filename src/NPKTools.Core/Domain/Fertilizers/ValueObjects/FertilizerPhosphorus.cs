using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.Fertilizers.ValueObjects;
/// <summary>
/// Represents the phosphorus content in the fertilizer, expressed as a single value.
/// </summary>
public record FertilizerPhosphorus(double Value) : ElementFieldBase (Value);