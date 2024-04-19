using NPKOptimizer.Common;
using NPKOptimizer.Const;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.Fertilizers.ValueObjects;
using NPKOptimizer.Repository.Contracts;

// ReSharper disable MemberCanBePrivate.Global

namespace NPKOptimizer.Components.Builders.FertilizerCollectionBuilder;

public sealed class FertilizerCollectionBuilder : BuilderBase <FertilizerCollectionBuilder>
{
    private readonly IFertilizerRepository _fertilizerRepository;
    private readonly HashSet<FertilizerId> _selectedFertilizerIds = new();

    public FertilizerCollectionBuilder(IFertilizerRepository fertilizerRepository)
    {
        Validate.NotNull(fertilizerRepository);
        _fertilizerRepository = fertilizerRepository; 
    }

    public FertilizerCollectionBuilder Add(FertilizerId id)
    {
            _selectedFertilizerIds.Add(id);
            return this;
    }

    public FertilizerCollectionBuilder CalciumNitrate() => Add(new FertilizerId(FertilizerIds.CalciumNitrate));
    public FertilizerCollectionBuilder K() => Add(new FertilizerId(FertilizerIds.PotassiumNitrate));
    public FertilizerCollectionBuilder Mgs() => Add(new FertilizerId(FertilizerIds.MagnesiumSulfate));
    public FertilizerCollectionBuilder Mkp() => Add(new FertilizerId(FertilizerIds.PotassiumPhosphate));
    public FertilizerCollectionBuilder Calc() => Add(new FertilizerId(FertilizerIds.CalciumChloride));
    public FertilizerCollectionBuilder Sop() => Add(new FertilizerId(FertilizerIds.PotassiumSulfate));
    public FertilizerCollectionBuilder Dkp() => Add(new FertilizerId(FertilizerIds.DiPotassiumPhosphate));
    public FertilizerCollectionBuilder Mag() => Add(new FertilizerId(FertilizerIds.MagnesiumNitrate));
    public FertilizerCollectionBuilder AmmoniumNitrate() => Add(new FertilizerId(FertilizerIds.AmmoniumNitrate));
    public FertilizerCollectionBuilder Urea() => Add(new FertilizerId(FertilizerIds.Urea));
    public FertilizerCollectionBuilder Up() => Add(new FertilizerId(FertilizerIds.UreaPhosphate));
    public FertilizerCollectionBuilder Map() => Add(new FertilizerId(FertilizerIds.MonoammoniumPhosphate));
    public FertilizerCollectionBuilder Mop() => Add(new FertilizerId(FertilizerIds.PotassiumChloride));
    public FertilizerCollectionBuilder AmmoniumChloride() => Add(new FertilizerId(FertilizerIds.AmmoniumChloride));
    public FertilizerCollectionBuilder AmmoniumSulfate() => Add(new FertilizerId(FertilizerIds.AmmoniumSulfate));
    public FertilizerCollectionBuilder PhosphoricAcid() => Add(new FertilizerId(FertilizerIds.PhosphoricAcid));
    public FertilizerCollectionBuilder CalciumMonobasicPhosphate() => Add(new FertilizerId(FertilizerIds.CalciumPhosphate));
    public FertilizerCollectionBuilder BoricAcid() => Add(new FertilizerId(FertilizerIds.BoricAcid));
    public FertilizerCollectionBuilder SodiumBorate() => Add(new FertilizerId(FertilizerIds.SodiumBorate));
    public FertilizerCollectionBuilder SodiumMolybdate() => Add(new FertilizerId(FertilizerIds.SodiumMolybdate));
    public FertilizerCollectionBuilder SodiumSilicate() => Add(new FertilizerId(FertilizerIds.SodiumSilicate));
    public FertilizerCollectionBuilder SodiumSelenate() => Add(new FertilizerId(FertilizerIds.SodiumSelenate));
    public FertilizerCollectionBuilder IronSulfate() => Add(new FertilizerId(FertilizerIds.IronSulfate));
    public FertilizerCollectionBuilder CopperSulfate() => Add(new FertilizerId(FertilizerIds.CopperSulfate));
    public FertilizerCollectionBuilder ManganeseSulfate() => Add(new FertilizerId(FertilizerIds.ManganeseSulfate));
    public FertilizerCollectionBuilder ZincSulfate() => Add(new FertilizerId(FertilizerIds.ZincSulfate));
    public FertilizerCollectionBuilder CopperNitrate() => Add(new FertilizerId(FertilizerIds.CopperNitrate));
    public FertilizerCollectionBuilder ZincNitrate() => Add(new FertilizerId(FertilizerIds.ZincNitrate));
    public FertilizerCollectionBuilder IronNitrate() => Add(new FertilizerId(FertilizerIds.IronNitrate));
    public FertilizerCollectionBuilder ManganeseNitrate() => Add(new FertilizerId(FertilizerIds.ManganeseNitrate));
    public FertilizerCollectionBuilder CopperEdta() => Add(new FertilizerId(FertilizerIds.CopperEdta));
    public FertilizerCollectionBuilder ManganeseEdta() => Add(new FertilizerId(FertilizerIds.ManganeseEdta));
    public FertilizerCollectionBuilder ZincEdta() => Add(new FertilizerId(FertilizerIds.ZincEdta));
    public FertilizerCollectionBuilder IronEdta() => Add(new FertilizerId(FertilizerIds.IronEdta));

    public FertilizerCollectionBuilder BaseMacroGroup() => 
            CalciumNitrate()
            .K()
            .Mgs()
            .Calc();
    public FertilizerCollectionBuilder BaseMicroGroup() => 
            BoricAcid()
            .SodiumBorate()
            .SodiumMolybdate()
            .SodiumSilicate()
            .SodiumSelenate();
    public FertilizerCollectionBuilder SulfateMicroGroup() => 
            IronSulfate()
            .CopperSulfate()
            .ManganeseSulfate()
            .ZincSulfate();
    public FertilizerCollectionBuilder ExtendedMacroGroup() => 
            Urea()
            .Up()
            .Map()
            .Mop()
            .AmmoniumChloride()
            .AmmoniumSulfate()
            .PhosphoricAcid()
            .CalciumMonobasicPhosphate();
    public FertilizerCollectionBuilder NitrateMicroGroup() => 
            CopperNitrate()
            .ZincNitrate()
            .IronNitrate()
            .ManganeseNitrate();
    public FertilizerCollectionBuilder ChelateMicroGroup() =>
            CopperEdta()
            .ManganeseEdta()
            .ZincEdta()
            .IronEdta();

    public async Task<FertilizerCollection> Build()
    {
            FertilizerCollection collection = new ();
            foreach (FertilizerId id in _selectedFertilizerIds)
            {
                    Fertilizer? fertilizer = await _fertilizerRepository.GetById(id);
                    if (fertilizer == null)
                    {
                            throw new InvalidOperationException($"Fertilizer not found for ID: {id}");
                    }
                    collection.Add(fertilizer);
            }

            Clear();
            return collection;
    }
    
    private void Clear()
    {
            _selectedFertilizerIds.Clear();
    }
}


