using System.Collections.Concurrent;
using NPKOptimizer.Common;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.Fertilizers.ValueObjects;
using NPKOptimizer.Repository.Contracts;

// ReSharper disable UnusedMethodReturnValue.Local

namespace NPKOptimizer.Repository;

public class InMemoryFertilizerRepository : IFertilizerRepository
{
    private readonly ConcurrentDictionary<FertilizerId, Fertilizer> _fertilizers = new();

    public InMemoryFertilizerRepository(IFertilizerRepositoryInitializer fertilizerRepositoryInitializer)
    {
        Validate.NotNull(fertilizerRepositoryInitializer);
        
        foreach (Fertilizer fertilizer in fertilizerRepositoryInitializer.InitializeFertilizers())
        {
            _fertilizers.TryAdd(fertilizer.Id, fertilizer);
        }
    }

    public IQueryable<Fertilizer> Query => _fertilizers.Values.AsQueryable();

    public Task Add(Fertilizer fertilizer)
    {
        if (!_fertilizers.TryAdd(fertilizer.Id, fertilizer))
        {
            throw new InvalidOperationException($"Fertilizer with ID {fertilizer.Id} already exists.");
        }
        return Task.CompletedTask;
    }

    public Task Remove(FertilizerId id)
    {
        if (!_fertilizers.TryRemove(id, out _))
        {
            throw new KeyNotFoundException($"Fertilizer with ID {id} not found.");
        }
        return Task.CompletedTask;
    }

    public Task Update(Fertilizer updatedFertilizer)
    {
        if (_fertilizers.TryGetValue(updatedFertilizer.Id, out _))
        {
            _fertilizers[updatedFertilizer.Id] = updatedFertilizer;
            return Task.CompletedTask;
        }
        throw new KeyNotFoundException($"Fertilizer with ID {updatedFertilizer.Id} not found to update.");
    }

    public Task<Fertilizer> GetById(FertilizerId id)
    {
        if (_fertilizers.TryGetValue(id, out Fertilizer? fertilizer))
        {
            return Task.FromResult(fertilizer);
        }
        throw new KeyNotFoundException($"Fertilizer with ID {id} not found.");
    }
}