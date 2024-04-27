using NPKOptimizer.Common;

namespace NPKOptimizer.Domain.Fertilizers.ValueObjects;
/// <summary>
/// Represents the selenium content in the fertilizer, expressed as a single value.
/// </summary>
public record FertilizerSelenium(double Value) : ElementFieldBase (Value);