namespace NPKTools.Core.Domain.Fertilizers.Builders;

/// <summary>
/// Fluent builder for <see cref="Fertilizer"/>. Each nutrient may be set at most once;
/// setting the same one twice throws <see cref="InvalidOperationException"/>.
/// </summary>
public sealed class FertilizerBuilder : FertilizerBuilderBase<FertilizerBuilder>;