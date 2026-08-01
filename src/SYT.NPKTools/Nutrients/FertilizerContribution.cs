using SYT.NPKTools.Fertilizers;

namespace SYT.NPKTools.Nutrients;

/// <summary>
/// What one fertilizer in a mix contributes to the finished solution.
/// </summary>
/// <param name="Fertilizer">The fertilizer, carrying the weight prescribed for it.</param>
/// <param name="Contribution">
/// The concentrations this fertilizer alone produces, in the same volume of water as the whole mix.
/// </param>
/// <remarks>
/// Answers the question a bare recipe cannot: which salt is responsible for the sulfur you did not ask
/// for. Every salt brings a counter-ion along with the nutrient you wanted, so an unwanted element is
/// almost never a mistake in the mix — it is the price of the element next to it. Seeing which salt
/// carries it is what makes a recipe adjustable rather than merely acceptable.
/// </remarks>
public sealed record FertilizerContribution(Fertilizer Fertilizer, Ppm Contribution);
