using NPKTools.Core.Common;

namespace NPKTools.Core.Domain.Fertilizers.ValueObjects;
/// <summary>
/// Represents the silicon content in the fertilizer, expressed as a single value.
/// </summary>
public record FertilizerSilicon(double Value) : ElementFieldBase (Value);