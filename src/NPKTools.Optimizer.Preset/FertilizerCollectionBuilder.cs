using NPKTools.Core.Common;
using NPKTools.Core.Domain.Collections;
using NPKTools.Core.Domain.Fertilizers;
using NPKTools.Core.Domain.Fertilizers.Builders;

namespace NPKTools.Optimizer.Preset;

/// <summary>
/// Builds a collection of fertilizer optimization models. This builder allows for the creation of complex
/// fertilizer mixes by adding predefined or custom fertilizers.
/// </summary>
public class FertilizerCollectionBuilder
{
    private readonly HashSet<FertilizerOptimizationModel> _selectedFertilizer = new (new FertilizerAttributesComparer());
    
    /// <summary>
    /// Adds a fertilizer optimization model to the collection.
    /// </summary>
    /// <param name="fertilizer">The fertilizer model to add.</param>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if a duplicate fertilizer is attempted to be added.</exception>
    public FertilizerCollectionBuilder Add(FertilizerOptimizationModel fertilizer)
    {
        ThrowIf.Duplicate(_selectedFertilizer, fertilizer);
        return this;
    }
    
    /// <summary>
    /// Adds a predefined Calcium Nitrate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder CalciumNitrate() => Add(new FertilizerBuilder()
        .AddCaNonChelated(16.972)
        .AddNo3(11.863).Build());

    /// <summary>
    /// Adds a predefined Potassium Nitrate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder K() => Add(new FertilizerBuilder()
        .AddNo3(13.854)
        .AddK(38.672).Build());

    /// <summary>
    /// Adds a predefined Magnesium Sulfate (Epsom Salt) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder Mgs() => Add(new FertilizerBuilder()
        .AddMgNonChelated(9.861)
        .AddS(13.008).Build());

    /// <summary>
    /// Adds a predefined Mono Potassium Phosphate (MKP) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder Mkp() => Add(new FertilizerBuilder()
        .AddP(22.761)
        .AddK(28.731).Build());
    
    /// <summary>
    /// Adds a predefined Calcium Chloride fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder Calc() => Add(new FertilizerBuilder()
        .AddCaNonChelated(18.295)
        .AddCl(32.364).Build());

    /// <summary>
    /// Adds a predefined Sulfate of Potash (SOP) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder Sop() => Add(new FertilizerBuilder()
        .AddK(44.875)
        .AddS(18.399).Build());

    /// <summary>
    /// Adds a predefined Dipotassium Phosphate (DKP) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder Dkp() => Add(new FertilizerBuilder()
        .AddP(17.783)
        .AddK(44.896).Build());

    /// <summary>
    /// Adds a predefined Magnesium Nitrate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder Mag() => Add(new FertilizerBuilder()
        .AddMgNonChelated(9.479)
        .AddNo3(10.926).Build());

    /// <summary>
    /// Adds a predefined Ammonium Nitrate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder AmmoniumNitrate() => Add(new FertilizerBuilder()
        .AddNh4(17.499)
        .AddNo3(17.499).Build());

    /// <summary>
    /// Adds a predefined Urea fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder Urea() => Add(new FertilizerBuilder()
        .AddNh2(46.646).Build());

    /// <summary>
    /// Adds a predefined Urea Phosphate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder UreaPhosphate() => Add(new FertilizerBuilder()
        .AddNh2(17.725)
        .AddP(19.597).Build());

    /// <summary>
    /// Adds a predefined Monoammonium Phosphate (MAP) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder Map() => Add(new FertilizerBuilder()
        .AddNh4(12.177)
        .AddP(26.928).Build());

    /// <summary>
    /// Adds a predefined Muriate of Potash (MOP) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder Mop() => Add(new FertilizerBuilder()
        .AddK(52.447)
        .AddCl(47.553).Build());

    /// <summary>
    /// Adds a predefined Ammonium Chloride fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder AmmoniumChloride() => Add(new FertilizerBuilder()
        .AddNh4(26.187)
        .AddCl(66.275).Build());

    /// <summary>
    /// Adds a predefined Ammonium Sulfate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder AmmoniumSulfate() => Add(new FertilizerBuilder()
        .AddNh4(21.201)
        .AddS(24.263).Build());

    /// <summary>
    /// Adds a predefined Phosphoric Acid fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder PhosphoricAcid() => Add(new FertilizerBuilder()
        .AddP(31.608).Build());

    /// <summary>
    /// Adds a predefined Calcium Monobasic Phosphate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder CalciumMonobasicPhosphate() => Add(new FertilizerBuilder()
        .AddCaNonChelated(15.9)
        .AddP(24.576).Build());

    /// <summary>
    /// Adds a predefined Boric Acid fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder BoricAcid() => Add(new FertilizerBuilder()
        .AddB(17.483).Build());

    /// <summary>
    /// Adds a predefined Sodium Borate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder SodiumBorate() => Add(new FertilizerBuilder()
        .AddB(11.338)
        .AddNa(12.057).Build());

    /// <summary>
    /// Adds a predefined Sodium Molybdate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder SodiumMolybdate() => Add(new FertilizerBuilder()
        .AddMo(39.656)
        .AddNa(19.003).Build());

    /// <summary>
    /// Adds a predefined Sodium Silicate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder SodiumSilicate() => Add(new FertilizerBuilder()
        .AddSi(23.009)
        .AddNa(37.669).Build());

    /// <summary>
    /// Adds a predefined Sodium Selenate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder SodiumSelenate() => Add(new FertilizerBuilder()
        .AddSe(41.795)
        .AddNa(24.335).Build());

    /// <summary>
    /// Adds a predefined Iron Sulfate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder IronSulfate() => Add(new FertilizerBuilder()
        .AddFeNonChelated(20.088)
        .AddS(11.532).Build());

    /// <summary>
    /// Adds a predefined Copper Sulfate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder CopperSulfate() => Add(new FertilizerBuilder()
        .AddCuNonChelated(25.451)
        .AddS(12.841).Build());

    /// <summary>
    /// Adds a predefined Manganese Sulfate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder ManganeseSulfate() => Add(new FertilizerBuilder()
        .AddMnNonChelated(32.506)
        .AddS(18.969).Build());

    /// <summary>
    /// Adds a predefined Zinc Sulfate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder ZincSulfate() => Add(new FertilizerBuilder()
        .AddZnNonChelated(36.433)
        .AddS(17.866).Build());

    /// <summary>
    /// Adds a predefined Copper Nitrate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder CopperNitrate() => Add(new FertilizerBuilder()
        .AddCuNonChelated(21.494)
        .AddNo3(9.476).Build());

    /// <summary>
    /// Adds a predefined Zinc Nitrate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder ZincNitrate() => Add(new FertilizerBuilder()
        .AddZnNonChelated(21.978)
        .AddNo3(9.417).Build());

    /// <summary>
    /// Adds a predefined Iron Nitrate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder IronNitrate() => Add(new FertilizerBuilder()
        .AddFeNonChelated(13.823)
        .AddNo3(10.401).Build());

    /// <summary>
    /// Adds a predefined Manganese Nitrate fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder ManganeseNitrate() => Add(new FertilizerBuilder()
        .AddMnNonChelated(30.701)
        .AddNo3(15.655).Build());

    /// <summary>
    /// Adds a predefined Chelated Copper (EDTA) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder CopperEdta() => Add(new FertilizerBuilder()
        .AddCuEdta(14.65).Build());

    /// <summary>
    /// Adds a predefined Chelated Manganese (EDTA) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder ManganeseEdta() => Add(new FertilizerBuilder()
        .AddMnEdta(12.922).Build());

    /// <summary>
    /// Adds a predefined Chelated Zinc (EDTA) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder ZincEdta() => Add(new FertilizerBuilder()
        .AddZnEdta(15.009).Build());

    /// <summary>
    /// Adds a predefined Chelated Iron (EDTA) fertilizer to the collection.
    /// </summary>
    /// <returns>The same FertilizerCollectionBuilder instance to allow for method chaining.</returns>
    public FertilizerCollectionBuilder IronEdta() => Add(new FertilizerBuilder()
        .AddFeEdta(13.262).Build());
    
    /// <summary>
    /// Finalizes the building process and returns the complete collection of fertilizers.
    /// </summary>
    /// <returns>A list of FertilizerOptimizationModel instances, representing the complete set of fertilizers added to the builder.</returns>
    public IList<FertilizerOptimizationModel> Build() => _selectedFertilizer.ToList();
}