using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.Fertilizers.ValueObjects;

// ReSharper disable UnusedMember.Global

namespace NPKOptimizer.Repository.Contracts;

public interface IFertilizerRepository
{
    Task Add(Fertilizer fertilizer);
    Task Remove(FertilizerId id);
    Task Update(Fertilizer updatedFertilizer);
    Task <Fertilizer> GetById(FertilizerId id);
    IQueryable<Fertilizer> Query { get; }
}