// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace NPKOptimizer.Domain.Fertilizers;

public sealed record Solutions
{
    public FertilizerCollectionSet Macro { get; init; } = new ();
    public FertilizerCollectionSet Micro { get; init; } = new ();
}