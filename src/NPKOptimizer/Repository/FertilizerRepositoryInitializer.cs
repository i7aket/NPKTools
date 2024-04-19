using NPKOptimizer.Common;
using NPKOptimizer.Components.Builders;
using NPKOptimizer.Const;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.Fertilizers.ValueObjects;
using NPKOptimizer.Enums;
using NPKOptimizer.Repository.Contracts;

namespace NPKOptimizer.Repository;

public class FertilizerRepositoryInitializer : IFertilizerRepositoryInitializer
{
    private const double BasePrice = 1;

    public FertilizerCollection InitializeFertilizers()
    {
        FertilizerCollection fertilizers = new ();

        // Calcium Nitrate
        FertilizerComposition calciumNitrateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Ca)
            .Add(Atom.H, 8)
            .Add(Atom.N, 2)
            .Add(Atom.O, 10)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.CalciumNitrate)
            .Name("Calcium Nitrate Tetrahydrate")
            .Formula("Ca(NO3)2*4H2O")
            .No3(calciumNitrateComposition.ElementPercent(Atom.N))
            .CaNonChelated(calciumNitrateComposition.ElementPercent(Atom.Ca))
            .Composition(calciumNitrateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.A)
            .Build());

        // Potassium Nitrate
        FertilizerComposition potassiumNitrateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.K)
            .Add(Atom.N)
            .Add(Atom.O, 3)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.PotassiumNitrate)
            .Name("Potassium Nitrate")
            .Formula("KNO3")
            .No3(potassiumNitrateComposition.ElementPercent(Atom.N))
            .K(potassiumNitrateComposition.ElementPercent(Atom.K))
            .Composition(potassiumNitrateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.A)
            .Build());

        // Potassium Dihydrogen Phosphate (MKP)
        FertilizerComposition potassiumDihydrogenPhosphateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.K)
            .Add(Atom.H, 2)
            .Add(Atom.P)
            .Add(Atom.O, 4)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.PotassiumPhosphate)
            .Name("Potassium Dihydrogen Phosphate (MKP)")
            .Formula("KH2PO4")
            .P(potassiumDihydrogenPhosphateComposition.ElementPercent(Atom.P))
            .K(potassiumDihydrogenPhosphateComposition.ElementPercent(Atom.K))
            .Composition(potassiumDihydrogenPhosphateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Magnesium Sulfate
        FertilizerComposition magnesiumSulfateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Mg)
            .Add(Atom.S)
            .Add(Atom.O, 11)
            .Add(Atom.H, 14)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.MagnesiumSulfate)
            .Name("Magnesium Sulfate Heptahydrate (MGS)")
            .Formula("MgSO4*7H2O")
            .MgNonChelated(magnesiumSulfateComposition.ElementPercent(Atom.Mg))
            .S(magnesiumSulfateComposition.ElementPercent(Atom.S))
            .Composition(magnesiumSulfateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Calcium Chloride
        FertilizerComposition calciumChlorideComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Ca)
            .Add(Atom.Cl, 2)
            .Add(Atom.H, 12)
            .Add(Atom.O, 6)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.CalciumChloride)
            .Name("Calcium Chloride Hexahydrate (CALC)")
            .Formula("CaCl2*6H2O")
            .CaNonChelated(calciumChlorideComposition.ElementPercent(Atom.Ca))
            .Cl(calciumChlorideComposition.ElementPercent(Atom.Cl))
            .Composition(calciumChlorideComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.A)
            .Build());

        // Potassium Sulfate
        FertilizerComposition potassiumSulfateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.K, 2)
            .Add(Atom.S)
            .Add(Atom.O, 4)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.PotassiumSulfate)
            .Name("Potassium Sulfate (SOP)")
            .Formula("K2SO4")
            .K(potassiumSulfateComposition.ElementPercent(Atom.K))
            .S(potassiumSulfateComposition.ElementPercent(Atom.S))
            .Composition(potassiumSulfateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Magnesium Nitrate
        FertilizerComposition magnesiumNitrateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Mg)
            .Add(Atom.N, 2)
            .Add(Atom.O, 12)
            .Add(Atom.H, 12)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.MagnesiumNitrate)
            .Name("Magnesium Nitrate Hexahydrate (MAG)")
            .Formula("Mg(NO3)2*6H2O")
            .No3(magnesiumNitrateComposition.ElementPercent(Atom.N))
            .MgNonChelated(magnesiumNitrateComposition.ElementPercent(Atom.Mg))
            .Composition(magnesiumNitrateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.A)
            .Build());
        // Ammonium Nitrate
        FertilizerComposition ammoniumNitrateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.N, 2)
            .Add(Atom.H, 4)
            .Add(Atom.O, 3)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.AmmoniumNitrate)
            .Name("Ammonium Nitrate")
            .Formula("NH4NO3")
            .No3(ammoniumNitrateComposition.ElementPercent(Atom.N) / 2)
            .Nh4(ammoniumNitrateComposition.ElementPercent(Atom.N) / 2)
            .Composition(ammoniumNitrateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.A)
            .Build());

        // Carbamide (Urea)
        FertilizerComposition carbamideComposition = new FertilizerCompositionBuilder()
            .Add(Atom.C)
            .Add(Atom.O)
            .Add(Atom.N, 2)
            .Add(Atom.H, 4)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.Urea)
            .Name("Urea")
            .Formula("CO(NH2)2")
            .Nh2(carbamideComposition.ElementPercent(Atom.N))
            .Composition(carbamideComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.A)
            .Build());

        // Urea Phosphate
        FertilizerComposition ureaPhosphateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.C)
            .Add(Atom.O, 5)
            .Add(Atom.H, 7)
            .Add(Atom.P)
            .Add(Atom.N, 2)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.UreaPhosphate)
            .Name("Urea Phosphate (UP)")
            .Formula("CO(NH2)2*H3PO4")
            .Nh2(ureaPhosphateComposition.ElementPercent(Atom.N))
            .P(ureaPhosphateComposition.ElementPercent(Atom.P))
            .Composition(ureaPhosphateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Ammonium Dihydrogen Phosphate
        FertilizerComposition ammoniumDihydrogenPhosphateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.N)
            .Add(Atom.H, 6)
            .Add(Atom.P)
            .Add(Atom.O, 4)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.MonoammoniumPhosphate)
            .Name("Monoammonium Phosphate")
            .Formula("NH4H2PO4")
            .Nh4(ammoniumDihydrogenPhosphateComposition.ElementPercent(Atom.N))
            .P(ammoniumDihydrogenPhosphateComposition.ElementPercent(Atom.P))
            .Composition(ammoniumDihydrogenPhosphateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Potassium Salt (Potassium Chloride)
        FertilizerComposition potassiumSaltComposition = new FertilizerCompositionBuilder()
            .Add(Atom.K)
            .Add(Atom.Cl)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.PotassiumChloride)
            .Name("Potassium Chloride")
            .Formula("KCl")
            .K(potassiumSaltComposition.ElementPercent(Atom.K))
            .Cl(potassiumSaltComposition.ElementPercent(Atom.Cl))
            .Composition(potassiumSaltComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.A)
            .Build());

        // Ammonium Chloride
        FertilizerComposition ammoniumChlorideComposition = new FertilizerCompositionBuilder()
            .Add(Atom.N)
            .Add(Atom.H, 4)
            .Add(Atom.Cl)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.AmmoniumChloride)
            .Name("Ammonium Chloride")
            .Formula("NH4Cl")
            .Nh4(ammoniumChlorideComposition.ElementPercent(Atom.N))
            .Cl(ammoniumChlorideComposition.ElementPercent(Atom.Cl))
            .Composition(ammoniumChlorideComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.A)
            .Build());
        // Ammonium Sulfate
        FertilizerComposition ammoniumSulfateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.N, 2)
            .Add(Atom.H, 8)
            .Add(Atom.S)
            .Add(Atom.O, 4)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.AmmoniumSulfate)
            .Name("Ammonium Sulfate")
            .Formula("(NH4)2SO4")
            .Nh4(ammoniumSulfateComposition.ElementPercent(Atom.N))
            .S(ammoniumSulfateComposition.ElementPercent(Atom.S))
            .Composition(ammoniumSulfateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Orthophosphoric Acid
        FertilizerComposition orthoPhosphoricAcidComposition = new FertilizerCompositionBuilder()
            .Add(Atom.H, 3)
            .Add(Atom.P)
            .Add(Atom.O, 4)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.PhosphoricAcid)
            .Name("Phosphoric Acid")
            .Formula("H3PO4")
            .P(orthoPhosphoricAcidComposition.ElementPercent(Atom.P))
            .Composition(orthoPhosphoricAcidComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Calcium Monobasic Phosphate
        FertilizerComposition calciumPhosphateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Ca)
            .Add(Atom.H, 6)
            .Add(Atom.P, 2)
            .Add(Atom.O, 9)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.CalciumPhosphate)
            .Name("Calcium Monobasic Phosphate")
            .Formula("Ca(H2PO4)2*H2O")
            .P(calciumPhosphateComposition.ElementPercent(Atom.P))
            .CaNonChelated(calciumPhosphateComposition.ElementPercent(Atom.Ca))
            .Composition(calciumPhosphateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Potassium Dibasic Phosphate
        FertilizerComposition diPotassiumPhosphateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.K, 2)
            .Add(Atom.H)
            .Add(Atom.P)
            .Add(Atom.O, 4)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.DiPotassiumPhosphate)
            .Name("Potassium Dibasic Phosphate")
            .Formula("K2HPO4")
            .P(diPotassiumPhosphateComposition.ElementPercent(Atom.P))
            .K(diPotassiumPhosphateComposition.ElementPercent(Atom.K))
            .Composition(diPotassiumPhosphateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Boric Acid
        FertilizerComposition hydrogenBorateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.H, 3)
            .Add(Atom.B)
            .Add(Atom.O, 3)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.BoricAcid)
            .Name("Boric Acid")
            .Formula("H3BO3")
            .B(hydrogenBorateComposition.ElementPercent(Atom.B))
            .Composition(hydrogenBorateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Sodium Borate
        FertilizerComposition sodiumBorateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Na, 2)
            .Add(Atom.B, 4)
            .Add(Atom.O, 17)
            .Add(Atom.H, 20)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.SodiumBorate)
            .Name("Sodium Borate Decahydrate")
            .Formula("Na2B4O7*10H2O")
            .B(sodiumBorateComposition.ElementPercent(Atom.B))
            .Na(sodiumBorateComposition.ElementPercent(Atom.Na))
            .Composition(sodiumBorateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Sodium Molybdate
        FertilizerComposition sodiumMolybdateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Na, 2)
            .Add(Atom.Mo)
            .Add(Atom.O, 6)
            .Add(Atom.H, 4)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.SodiumMolybdate)
            .Name("Sodium Molybdate Dihydrate")
            .Formula("Na2MoO4*2H2O")
            .Mo(sodiumMolybdateComposition.ElementPercent(Atom.Mo))
            .Na(sodiumMolybdateComposition.ElementPercent(Atom.Na))
            .Composition(sodiumMolybdateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Sodium Silicate
        FertilizerComposition silicateSodiumComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Na, 2)
            .Add(Atom.Si)
            .Add(Atom.O, 3)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.SodiumSilicate)
            .Name("Sodium Silicate")
            .Formula("Na2SiO3")
            .Si(silicateSodiumComposition.ElementPercent(Atom.Si))
            .Na(silicateSodiumComposition.ElementPercent(Atom.Na))
            .Composition(silicateSodiumComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Sodium Selenate
        FertilizerComposition selenateSodiumComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Na, 2)
            .Add(Atom.Se)
            .Add(Atom.O, 4)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.SodiumSelenate)
            .Name("Sodium Selenate")
            .Formula("Na2SeO4")
            .Se(selenateSodiumComposition.ElementPercent(Atom.Se))
            .Na(selenateSodiumComposition.ElementPercent(Atom.Na))
            .Composition(selenateSodiumComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Iron(II) Sulfate
        FertilizerComposition ironSulfateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Fe)
            .Add(Atom.S)
            .Add(Atom.O, 11)
            .Add(Atom.H, 14)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.IronSulfate)
            .Name("Iron(II) Sulfate Heptahydrate")
            .Formula("FeSO4*7H2O")
            .FeNonChelated(ironSulfateComposition.ElementPercent(Atom.Fe))
            .S(ironSulfateComposition.ElementPercent(Atom.S))
            .Composition(ironSulfateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Copper Sulfate
        FertilizerComposition copperSulfateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Cu)
            .Add(Atom.S)
            .Add(Atom.O, 9)
            .Add(Atom.H, 10)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.CopperSulfate)
            .Name("Copper Sulfate Pentahydrate")
            .Formula("CuSO4*5H2O")
            .CuNonChelated(copperSulfateComposition.ElementPercent(Atom.Cu))
            .S(copperSulfateComposition.ElementPercent(Atom.S))
            .Composition(copperSulfateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Manganese Sulfate
        FertilizerComposition manganeseSulfateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Mn)
            .Add(Atom.S)
            .Add(Atom.O, 5)
            .Add(Atom.H, 2)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.ManganeseSulfate)
            .Name("Manganese Sulfate Monohydrate")
            .Formula("MnSO4*H2O")
            .MnNonChelated(manganeseSulfateComposition.ElementPercent(Atom.Mn))
            .S(manganeseSulfateComposition.ElementPercent(Atom.S))
            .Composition(manganeseSulfateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Zinc Sulfate
        FertilizerComposition zincSulfateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Zn)
            .Add(Atom.S)
            .Add(Atom.O, 5)
            .Add(Atom.H, 2)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.ZincSulfate)
            .Name("Zinc Sulfate Monohydrate")
            .Formula("ZnSO4*H2O")
            .ZnNonChelated(zincSulfateComposition.ElementPercent(Atom.Zn))
            .S(zincSulfateComposition.ElementPercent(Atom.S))
            .Composition(zincSulfateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());
        // Copper Nitrate
        FertilizerComposition copperNitrateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Cu)
            .Add(Atom.N, 2)
            .Add(Atom.O, 12)
            .Add(Atom.H, 12)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.CopperNitrate)
            .Name("Copper Nitrate Hexahydrate")
            .Formula("Cu(NO3)2*6H2O")
            .CuNonChelated(copperNitrateComposition.ElementPercent(Atom.Cu))
            .No3(copperNitrateComposition.ElementPercent(Atom.N))
            .Composition(copperNitrateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.A)
            .Build());

        // Zinc Nitrate
        FertilizerComposition zincNitrateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Zn)
            .Add(Atom.N, 2)
            .Add(Atom.O, 12)
            .Add(Atom.H, 12)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.ZincNitrate)
            .Name("Zinc Nitrate Hexahydrate")
            .Formula("Zn(NO3)2*6H2O")
            .ZnNonChelated(zincNitrateComposition.ElementPercent(Atom.Zn))
            .No3(zincNitrateComposition.ElementPercent(Atom.N))
            .Composition(zincNitrateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.A)
            .Build());

        // Iron(III) Nitrate
        FertilizerComposition ironNitrateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Fe)
            .Add(Atom.N, 3)
            .Add(Atom.O, 18)
            .Add(Atom.H, 18)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.IronNitrate)
            .Name("Iron(III) Nitrate Nonahydrate")
            .Formula("Fe(NO3)3*9H2O")
            .FeNonChelated(ironNitrateComposition.ElementPercent(Atom.Fe))
            .No3(ironNitrateComposition.ElementPercent(Atom.N))
            .Composition(ironNitrateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.A)
            .Build());

        // Manganese Nitrate
        FertilizerComposition nitrateManganeseComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Mn)
            .Add(Atom.N, 2)
            .Add(Atom.O, 6)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.ManganeseNitrate)
            .Name("Manganese Nitrate")
            .Formula("Mn(NO3)2")
            .MnNonChelated(nitrateManganeseComposition.ElementPercent(Atom.Mn))
            .No3(nitrateManganeseComposition.ElementPercent(Atom.N))
            .Composition(nitrateManganeseComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.A)
            .Build());

        // Copper EDTA
        FertilizerComposition copperChelateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Cu)
            .Add(Atom.C, 10)
            .Add(Atom.H, 16)
            .Add(Atom.N, 2)
            .Add(Atom.O, 10)
            .Add(Atom.Na, 2)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.CopperEdta)
            .Name("Ethylenediaminetetraacetic acid cupric disodium complex EDTA Cu15%")
            .Formula("C10H12CuN2Na2O8*2H2O")
            .CuEdta(copperChelateComposition.ElementPercent(Atom.Cu))
            .Composition(copperChelateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Manganese EDTA
        FertilizerComposition manganeseChelateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Mn)
            .Add(Atom.C, 10)
            .Add(Atom.H, 16)
            .Add(Atom.N, 2)
            .Add(Atom.O, 10)
            .Add(Atom.Na, 2)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.ManganeseEdta)
            .Name("Ethylenediaminetetraacetic acid manganic disodium complex EDTA Mn13%")
            .Formula("C10H12N2O8Na2Mn*2H2O")
            .MnEdta(manganeseChelateComposition.ElementPercent(Atom.Mn))
            .Composition(manganeseChelateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Zinc EDTA
        FertilizerComposition zincChelateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Zn)
            .Add(Atom.C, 10)
            .Add(Atom.H, 16)
            .Add(Atom.N, 2)
            .Add(Atom.O, 10)
            .Add(Atom.Na, 2)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.ZincEdta)
            .Name("Ethylenediaminetetraacetic acid zinc disodium complex EDTA Zn15%")
            .Formula("C10H12N2O8Na2Zn*2H2O")
            .ZnEdta(zincChelateComposition.ElementPercent(Atom.Zn))
            .Composition(zincChelateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        // Iron EDTA
        FertilizerComposition ironChelateComposition = new FertilizerCompositionBuilder()
            .Add(Atom.Fe)
            .Add(Atom.C, 10)
            .Add(Atom.H, 18)
            .Add(Atom.N, 2)
            .Add(Atom.O, 11)
            .Add(Atom.Na)
            .Build();
        fertilizers.Add(new FertilizerBuilder()
            .Id(FertilizerIds.IronEdta)
            .Name("Ethylenediaminetetraacetic acid ferric-sodium complex EDTA Fe13%")
            .Formula("C10H12N2O8NaFe*3H2O")
            .FeEdta(ironChelateComposition.ElementPercent(Atom.Fe))
            .Composition(ironChelateComposition)
            .Price(BasePrice)
            .ConcentrateType(ConcentrateType.B)
            .Build());

        return fertilizers;
    }
}