namespace NPKTools.Core.Domain.PpmTarget.Builder;

/// <summary>
/// Fluent builder for <see cref="PpmTarget"/>. Each element may be set at most once.
/// Elements left unset default to zero, and the water volume defaults to one liter.
/// </summary>
public sealed class PpmTargetBuilder : PpmTargetBuilderBase<PpmTargetBuilder>;