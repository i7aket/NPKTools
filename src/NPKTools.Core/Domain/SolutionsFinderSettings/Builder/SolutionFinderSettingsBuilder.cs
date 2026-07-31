namespace NPKTools.Core.Domain.SolutionsFinderSettings.Builder;

/// <summary>
/// Fluent builder for <see cref="SolutionFinderSettings"/>. Elements left unset default to a
/// precision of zero, which leaves them unconstrained during optimization.
/// </summary>
public sealed class SolutionFinderSettingsBuilder : SolutionFinderSettingsBuilderBase<SolutionFinderSettingsBuilder>;
