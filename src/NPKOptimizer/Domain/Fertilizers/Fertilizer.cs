using System.Text;
using NPKOptimizer.Common;
using NPKOptimizer.Const;
using NPKOptimizer.Domain.Fertilizers.ValueObjects;
using NPKOptimizer.Enums;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NPKOptimizer.Domain.Fertilizers;

/// <summary>
/// Represents a fertilizer with detailed information on its nutrient content and chemical composition.
/// It includes various elements crucial for plant growth, along with their specific forms and concentrations.
/// </summary>
public record Fertilizer
{
    public FertilizerId Id { get; }
    public FertilizerName Name { get; }
    public FertilizerFormula Formula { get; }
    public FertilizerPrice FertilizerPrice { get; }
    public FertilizerWeight Weight { get; init; }
    public ConcentrateType ConcentrateType { get; }
    public FertilizerNitrogen N { get; }
    public FertilizerPhosphorus P { get; }
    public FertilizerPotassium K { get; }
    public FertilizerCalcium Ca { get; }
    public FertilizerMagnesium Mg { get; }
    public FertilizerSulfur S { get; }
    public FertilizerIron Fe { get; }
    public FertilizerCopper Cu { get; }
    public FertilizerManganese Mn { get; }
    public FertilizerZinc Zn { get; }
    public FertilizerBoron B { get; }
    public FertilizerMolybdenum Mo { get; }
    public FertilizerChlorine Cl { get; }
    public FertilizerSilicon Si { get; }
    public FertilizerSelenium Se { get; }
    public FertilizerSodium Na { get; }
    public FertilizerComposition? Composition {get; }

    
    public Fertilizer(
        FertilizerId id,
        FertilizerName name,
        FertilizerFormula formula,
        FertilizerPrice fertilizerPrice,
        FertilizerWeight weight,
        ConcentrateType concentrateType,
        FertilizerNitrogen n,
        FertilizerPhosphorus p,
        FertilizerPotassium k,
        FertilizerCalcium ca,
        FertilizerMagnesium mg,
        FertilizerSulfur s,
        FertilizerIron fe,
        FertilizerCopper cu,
        FertilizerManganese mn,
        FertilizerZinc zn,
        FertilizerBoron b,
        FertilizerMolybdenum mo,
        FertilizerChlorine cl,
        FertilizerSilicon si,
        FertilizerSelenium se,
        FertilizerSodium na,
        FertilizerComposition? composition = null)
    {
        Validate.NotNull(id);
        Id = id;
        Validate.NotNull(name);
        Name = name;
        Validate.NotNull(formula);
        Formula = formula;
        Validate.NotNull(fertilizerPrice);
        FertilizerPrice = fertilizerPrice;
        Validate.NotNull(weight);
        Weight = weight;
        ConcentrateType = concentrateType;

        Validate.NotNull(n);
        N = n;
        Validate.NotNull(p);
        P = p;
        Validate.NotNull(k);
        K = k;
        Validate.NotNull(ca);
        Ca = ca;
        Validate.NotNull(mg);
        Mg = mg;
        Validate.NotNull(s);
        S = s;

        Validate.NotNull(fe);
        Fe = fe;
        Validate.NotNull(cu);
        Cu = cu;
        Validate.NotNull(mn);
        Mn = mn;
        Validate.NotNull(zn);
        Zn = zn;
        Validate.NotNull(b);
        B = b;
        Validate.NotNull(mo);
        Mo = mo;
        Validate.NotNull(cl);
        Cl = cl;
        Validate.NotNull(si);
        Si = si;
        Validate.NotNull(se);
        Se = se;
        Validate.NotNull(na);
        Na = na;
        Composition = composition;
    }
        
    public override string ToString()
    {
        StringBuilder stringBuilder = new ();
        stringBuilder.AppendLine($"{Labels.Name}: {Name.Value}");
        stringBuilder.AppendLine($"{Labels.Formula}: {Formula.Value}");
        stringBuilder.AppendLine($"{Labels.ConcentrateType}: {ConcentrateType}");

        AppendLineIfNonZero(Labels.Nitrogen, N.Value);
        AppendLineIfNonZero($"{Labels.SubItemPrefix}{Labels.NitrateNo3}", N.Nitrate);
        AppendLineIfNonZero($"{Labels.SubItemPrefix}{Labels.AmmoniumNh4}", N.Ammonium);
        AppendLineIfNonZero($"{Labels.SubItemPrefix}{Labels.AmineNh2}", N.Amine);

        AppendLineIfNonZero(Labels.Phosphorus, P.Value);
        AppendLineIfNonZero(Labels.Potassium, K.Value);

        AppendLineIfNonZero(Labels.Calcium, Ca.Value);
        AppendLineIfNonZero($"{Labels.SubItemPrefix}{Labels.Edta}", Ca.CaEdta);

        AppendLineIfNonZero(Labels.Magnesium, Mg.Value);
        AppendLineIfNonZero($"{Labels.SubItemPrefix}{Labels.Edta}", Mg.MgEdta);

        AppendLineIfNonZero(Labels.Sulfur, S.Value);
        AppendLineIfNonZero(Labels.Chlorine, Cl.Value);

        AppendLineIfNonZero(Labels.Iron, Fe.Value);
        AppendLineIfNonZero($"{Labels.SubItemPrefix}{Labels.Edta}", Fe.FeEdta);
        AppendLineIfNonZero($"{Labels.SubItemPrefix}{Labels.Dtpa}", Fe.FeDtpa);
        AppendLineIfNonZero($"{Labels.SubItemPrefix}{Labels.Eddha}", Fe.FeEddha);
        AppendLineIfNonZero($"{Labels.SubItemPrefix}{Labels.Hbed}", Fe.FeHbed);
        AppendLineIfNonZero($"{Labels.SubItemPrefix}{Labels.OrthoOrtho}", Fe.FeOrthoPart);

        AppendLineIfNonZero(Labels.Copper, Cu.Value);
        AppendLineIfNonZero($"{Labels.SubItemPrefix}{Labels.Edta}", Cu.CuEdta);

        AppendLineIfNonZero(Labels.Manganese, Mn.Value);
        AppendLineIfNonZero($"{Labels.SubItemPrefix}{Labels.Edta}", Mn.MnEdta);

        AppendLineIfNonZero(Labels.Zinc, Zn.Value);
        AppendLineIfNonZero($"{Labels.SubItemPrefix}{Labels.Edta}", Zn.ZnEdta);

        AppendLineIfNonZero(Labels.Boron, B.Value);
        AppendLineIfNonZero(Labels.Molybdenum, Mo.Value);
        AppendLineIfNonZero(Labels.Silicon, Si.Value);
        AppendLineIfNonZero(Labels.Selenium, Se.Value);
        AppendLineIfNonZero(Labels.Sodium, Na.Value);

        return stringBuilder.ToString();

        void AppendLineIfNonZero(string label, double value)
        {
            if (value > 0)
            {
                stringBuilder.AppendLine($"{label}: {Math.Round(value, Settings.DecimalPlaces)}");
            }
        }
    }
}