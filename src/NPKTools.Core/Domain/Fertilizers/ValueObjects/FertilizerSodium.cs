using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.Fertilizers.ValueObjects;
/// <summary>
/// Represents the sodium content in the fertilizer, expressed as a single value.
/// </summary>
public record FertilizerSodium(double Value) : ElementFieldBase (Value);