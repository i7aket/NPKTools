using NPKTools.Core.Common;
using NPKTools.Core.Domain.Collections;
using NPKTools.Core.Domain.Fertilizers;
using NPKTools.Core.Domain.Fertilizers.Builders;
using NPKTools.Core.Domain.Fertilizers.Enums;

namespace NPKTools.Optimizer.Preset;

/// <summary>
/// Builds a collection of fertilizers. This builder allows for the creation of complex
/// fertilizer mixes by adding predefined or custom fertilizers.
/// </summary>
public class FertilizerCollectionBuilder
{
    private readonly HashSet<Fertilizer> _selectedFertilizer = new (new FertilizerAttributesComparer());
    
    /// <summary>
    /// Adds a fertilizer optimization model to the collection.
    /// </summary>
    /// <param name="fertilizer">The fertilizer model to add.</param>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if a duplicate fertilizer is attempted to be added.</exception>
    public FertilizerCollectionBuilder Add(Fertilizer fertilizer)
    {
        ThrowIf.Duplicate(_selectedFertilizer, fertilizer);
        return this;
    }
    
    /// <summary>
    /// Adds a predefined Calcium Nitrate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder CalciumNitrate() => Add(new FertilizerBuilder()
        .AddName("Calcium Nitrate Tetrahydrate")
        .AddFormula("Ca(NO₃)2*4H₂O")
        .AddType(ConcentrateType.A)
        .AddCaNonChelated(16.972)
        .AddNo3(11.863).Build());

    /// <summary>
    /// Adds a predefined Potassium Nitrate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder K() => Add(new FertilizerBuilder()
        .AddName("Potassium Nitrate")
        .AddFormula("KNO₃")
        .AddType(ConcentrateType.A)
        .AddNo3(13.854)
        .AddK(38.672).Build());

    /// <summary>
    /// Adds a predefined Magnesium Sulfate (Epsom Salt) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder Mgs() => Add(new FertilizerBuilder()
        .AddName("Magnesium Sulfate Heptahydrate (MGS)")
        .AddFormula("MgSO₄*7H₂O")
        .AddType(ConcentrateType.B)
        .AddMgNonChelated(9.861)
        .AddS(13.008).Build());

    /// <summary>
    /// Adds a predefined Mono Potassium Phosphate (MKP) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder Mkp() => Add(new FertilizerBuilder()
        .AddName("Potassium Dihydrogen Phosphate (MKP)")
        .AddFormula("KH₂PO₄")
        .AddType(ConcentrateType.B)
        .AddP(22.761)
        .AddK(28.731).Build());
    
    /// <summary>
    /// Adds a predefined Calcium Chloride fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder Calc() => Add(new FertilizerBuilder()
        .AddName("Calcium Chloride Hexahydrate (CALC)")
        .AddFormula("CaCl₂*6H₂O")
        .AddType(ConcentrateType.A)
        .AddCaNonChelated(18.295)
        .AddCl(32.364).Build());

    /// <summary>
    /// Adds a predefined Sulfate of Potash (SOP) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder Sop() => Add(new FertilizerBuilder()
        .AddName("Potassium Sulfate (SOP)")
        .AddFormula("K₂SO₄")
        .AddType(ConcentrateType.B)
        .AddK(44.875)
        .AddS(18.399).Build());

    /// <summary>
    /// Adds a predefined Dipotassium Phosphate (DKP) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder Dkp() => Add(new FertilizerBuilder()
        .AddName("Potassium Dibasic Phosphate")
        .AddFormula("K₂HPO₄")
        .AddType(ConcentrateType.B)
        .AddP(17.783)
        .AddK(44.896).Build());

    /// <summary>
    /// Adds a predefined Magnesium Nitrate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder Mag() => Add(new FertilizerBuilder()
        .AddName("Magnesium Nitrate Hexahydrate (MAG)")
        .AddFormula("Mg(NO₃)2*6H₂O")
        .AddType(ConcentrateType.A)
        .AddMgNonChelated(9.479)
        .AddNo3(10.926).Build());

    /// <summary>
    /// Adds a predefined Ammonium Nitrate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder AmmoniumNitrate() => Add(new FertilizerBuilder()
        .AddName("Ammonium Nitrate")
        .AddFormula("NH₄NO₃")
        .AddType(ConcentrateType.A)
        .AddNh4(17.499)
        .AddNo3(17.499).Build());

    /// <summary>
    /// Adds a predefined Urea fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder Urea() => Add(new FertilizerBuilder()
        .AddName("Urea")
        .AddFormula("CO(NH₂)2")
        .AddType(ConcentrateType.A)
        .AddNh2(46.646).Build());

    /// <summary>
    /// Adds a predefined Urea Phosphate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder UreaPhosphate() => Add(new FertilizerBuilder()
        .AddName("Urea Phosphate (UP)")
        .AddFormula("CO(NH₂)2*H₃PO₄")
        .AddType(ConcentrateType.B)
        .AddNh2(17.725)
        .AddP(19.597).Build());

    /// <summary>
    /// Adds a predefined Monoammonium Phosphate (MAP) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder Map() => Add(new FertilizerBuilder()
        .AddName("Monoammonium Phosphate")
        .AddFormula("NH₄H₂PO₄")
        .AddType(ConcentrateType.B)
        .AddNh4(12.177)
        .AddP(26.928).Build());

    /// <summary>
    /// Adds a predefined Muriate of Potash (MOP) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder Mop() => Add(new FertilizerBuilder()
        .AddName("Potassium Chloride")
        .AddFormula("KCl")
        .AddType(ConcentrateType.A)
        .AddK(52.447)
        .AddCl(47.553).Build());

    /// <summary>
    /// Adds a predefined Ammonium Chloride fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder AmmoniumChloride() => Add(new FertilizerBuilder()
        .AddName("Ammonium Chloride")
        .AddFormula("NH₄Cl")
        .AddType(ConcentrateType.A)
        .AddNh4(26.187)
        .AddCl(66.275).Build());

    /// <summary>
    /// Adds a predefined Ammonium Sulfate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder AmmoniumSulfate() => Add(new FertilizerBuilder()
        .AddName("Ammonium Sulfate")
        .AddFormula("(NH₄)2SO₄")
        .AddType(ConcentrateType.B)
        .AddNh4(21.201)
        .AddS(24.263).Build());

    /// <summary>
    /// Adds a predefined Phosphoric Acid fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder PhosphoricAcid() => Add(new FertilizerBuilder()
        .AddName("Phosphoric Acid")
        .AddFormula("H₃PO₄")
        .AddType(ConcentrateType.B)
        .AddP(31.608).Build());

    /// <summary>
    /// Adds a predefined Calcium Monobasic Phosphate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder CalciumMonobasicPhosphate() => Add(new FertilizerBuilder()
        .AddName("Calcium Monobasic Phosphate")
        .AddFormula("Ca(H₂PO₄)2*H₂O")
        .AddType(ConcentrateType.B)
        .AddCaNonChelated(15.9)
        .AddP(24.576).Build());

    /// <summary>
    /// Adds a predefined Boric Acid fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder BoricAcid() => Add(new FertilizerBuilder()
        .AddName("Boric Acid")
        .AddFormula("H₃BO₃")
        .AddType(ConcentrateType.B)
        .AddB(17.483).Build());

    /// <summary>
    /// Adds a predefined Sodium Borate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder SodiumBorate() => Add(new FertilizerBuilder()
        .AddName("Sodium Borate Decahydrate")
        .AddFormula("Na₂B₄O₇*10H₂O")
        .AddType(ConcentrateType.B)
        .AddB(11.338)
        .AddNa(12.057).Build());

    /// <summary>
    /// Adds a predefined Sodium Molybdate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder SodiumMolybdate() => Add(new FertilizerBuilder()
        .AddName("Sodium Molybdate Dihydrate")
        .AddFormula("Na₂MoO₄*2H₂O")
        .AddType(ConcentrateType.B)
        .AddMo(39.656)
        .AddNa(19.003).Build());

    /// <summary>
    /// Adds a predefined Sodium Silicate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder SodiumSilicate() => Add(new FertilizerBuilder()
        .AddName("Sodium Silicate")
        .AddFormula("Na₂SiO₃")
        .AddType(ConcentrateType.B)
        .AddSi(23.009)
        .AddNa(37.669).Build());

    /// <summary>
    /// Adds a predefined Sodium Selenate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder SodiumSelenate() => Add(new FertilizerBuilder()
        .AddName("Sodium Selenate")
        .AddFormula("Na2SeO4")
        .AddType(ConcentrateType.B)
        .AddSe(41.795)
        .AddNa(24.335).Build());

    /// <summary>
    /// Adds a predefined Iron Sulfate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder IronSulfate() => Add(new FertilizerBuilder()
        .AddName("Iron(II) Sulfate Heptahydrate")
        .AddFormula("FeSO₄*7H₂O")
        .AddType(ConcentrateType.B)
        .AddFeNonChelated(20.088)
        .AddS(11.532).Build());

    /// <summary>
    /// Adds a predefined Copper Sulfate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder CopperSulfate() => Add(new FertilizerBuilder()
        .AddName("Copper Sulfate Pentahydrate")
        .AddFormula("CuSO₄*5H₂O")
        .AddType(ConcentrateType.B)
        .AddCuNonChelated(25.451)
        .AddS(12.841).Build());

    /// <summary>
    /// Adds a predefined Manganese Sulfate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder ManganeseSulfate() => Add(new FertilizerBuilder()
        .AddName("Manganese Sulfate Monohydrate")
        .AddFormula("MnSO₄*H₂O")
        .AddType(ConcentrateType.B)
        .AddMnNonChelated(32.506)
        .AddS(18.969).Build());

    /// <summary>
    /// Adds a predefined Zinc Sulfate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder ZincSulfate() => Add(new FertilizerBuilder()
        .AddName("Zinc Sulfate Monohydrate")
        .AddFormula("ZnSO₄*H₂O")
        .AddType(ConcentrateType.B)
        .AddZnNonChelated(36.433)
        .AddS(17.866).Build());

    /// <summary>
    /// Adds a predefined Copper Nitrate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder CopperNitrate() => Add(new FertilizerBuilder()
        .AddName("Copper Nitrate Hexahydrate")
        .AddFormula("Cu(NO₃)2*6H₂O")
        .AddType(ConcentrateType.A)
        .AddCuNonChelated(21.494)
        .AddNo3(9.476).Build());

    /// <summary>
    /// Adds a predefined Zinc Nitrate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder ZincNitrate() => Add(new FertilizerBuilder()
        .AddName("Zinc Nitrate Hexahydrate")
        .AddFormula("Zn(NO₃)2*6H₂O")
        .AddType(ConcentrateType.A)
        .AddZnNonChelated(21.978)
        .AddNo3(9.417).Build());

    /// <summary>
    /// Adds a predefined Iron Nitrate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder IronNitrate() => Add(new FertilizerBuilder()
        .AddName("Iron(III) Nitrate Nonahydrate")
        .AddFormula("Fe(NO₃)3*9H₂O")
        .AddType(ConcentrateType.A)
        .AddFeNonChelated(13.823)
        .AddNo3(10.401).Build());

    /// <summary>
    /// Adds a predefined Manganese Nitrate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder ManganeseNitrate() => Add(new FertilizerBuilder()
        .AddName("Manganese Nitrate")
        .AddFormula("Mn(NO₃)2")
        .AddType(ConcentrateType.A)
        .AddMnNonChelated(30.701)
        .AddNo3(15.655).Build());

    /// <summary>
    /// Adds a predefined Chelated Copper (EDTA) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder CopperEdta() => Add(new FertilizerBuilder()
        .AddName("EDTA Cu15%")
        .AddFormula("C₁₀H₁₂CuN₂Na₂O₈*2H₂O")
        .AddType(ConcentrateType.B)
        .AddCuEdta(14.65).Build());

    /// <summary>
    /// Adds a predefined Chelated Manganese (EDTA) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder ManganeseEdta() => Add(new FertilizerBuilder()
        .AddName("EDTA Mn13%")
        .AddFormula("C₁₀H₁₂N₂O₈Na₂Mn*2H₂O")
        .AddType(ConcentrateType.B)
        .AddMnEdta(12.922).Build());

    /// <summary>
    /// Adds a predefined Chelated Zinc (EDTA) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder ZincEdta() => Add(new FertilizerBuilder()
        .AddName("EDTA Zn15%")
        .AddFormula("C₁₀H₁₂N₂O₈Na₂Zn*2H₂O")
        .AddType(ConcentrateType.B)
        .AddZnEdta(15.009).Build());

    /// <summary>
    /// Adds a predefined Chelated Iron (EDTA) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder IronEdta() => Add(new FertilizerBuilder()
        .AddName("EDTA Fe13%")
        .AddFormula("C₁₀H₁₂N₂O₈NaFe*3H₂O")
        .AddType(ConcentrateType.B)
        .AddFeEdta(13.262).Build());
    
    /// <summary>
    /// Finalizes the building process and returns the complete collection of fertilizers.
    /// </summary>
    /// <returns>A list of FertilizerOptimizationModel instances, representing the complete set of fertilizers added to the builder.</returns>
    public IList<Fertilizer> Build() => _selectedFertilizer.ToList();
}