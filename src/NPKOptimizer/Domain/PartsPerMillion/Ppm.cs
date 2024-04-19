using System.Text;
using NPKOptimizer.Common;
using NPKOptimizer.Const;
using NPKOptimizer.Domain.PartsPerMillion.ValueObjects;

// ReSharper disable MemberCanBePrivate.Global

namespace NPKOptimizer.Domain.PartsPerMillion;

public sealed record Ppm
{
    public NitrogenPpm N { get; }
    public PhosphorusPpm P { get; }
    public PotassiumPpm K { get; }
    public CalciumPpm Ca { get; }
    public MagnesiumPpm Mg { get; }
    public SulfurPpm S { get; }
    public IronPpm Fe { get; }
    public CopperPpm Cu { get; }
    public ManganesePpm Mn { get; }
    public ZincPpm Zn { get; }
    public BoronPpm B { get; }
    public MolybdenumPpm Mo { get; }
    public ChlorinePpm Cl { get; }
    public SiliconPpm Si { get; }
    public SeleniumPpm Se { get; }
    public SodiumPpm Na { get; }
    // ReSharper disable once MemberCanBePrivate.Global
    public TotalMassPpm TotalWeight { get; }

    public double Value => N.Value + P.Value + K.Value + Ca.Value + Mg.Value + S.Value +
                              Fe.Value + Cu.Value + Mn.Value + Zn.Value + B.Value + Mo.Value +
                              Cl.Value + Na.Value + Si.Value + Se.Value;

    // ReSharper disable once MemberCanBePrivate.Global
    public ElectricalConductivityPpm Ec => new (NutrientConverter.PpmToEc500(Value));
    public Ppm(NitrogenPpm n, PhosphorusPpm p, PotassiumPpm k, CalciumPpm ca, MagnesiumPpm mg, SulfurPpm s, IronPpm fe, CopperPpm cu,
        ManganesePpm mn, ZincPpm zn, BoronPpm b, MolybdenumPpm mo, ChlorinePpm cl, SiliconPpm si, SeleniumPpm se, SodiumPpm na,
        TotalMassPpm totalMassPpm)
    {
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
        Validate.NotNull(totalMassPpm);
        TotalWeight = totalMassPpm;
    }
    
    public override string ToString()
    {
        StringBuilder responseBuilder = new ();

        responseBuilder.AppendLine($"{Labels.PpmReport}");
        AppendLineIfNonZero($"{Labels.TotalPpm}", Value);
        AppendLineIfNonZero($"{Labels.Ec}", Ec.Value);
        AppendLineIfNonZero($"{Labels.TotalMass}", TotalWeight.Value);
        AppendLineIfNonZero(Labels.Nitrogen, N.Value);
        AppendLineIfNonZero($"{Labels.SubItemPrefix}{Labels.NitrateNo3}", N.Nitrate);
        AppendLineIfNonZero($"{Labels.SubItemPrefix}{Labels.AmmoniumNh4}", N.Ammonium);
        AppendLineIfNonZero($"{Labels.SubItemPrefix}{Labels.AmineNh2}", N.Amine);
        AppendLineIfNonZero(Labels.Phosphorus, P.Value);
        AppendLineIfNonZero(Labels.Potassium, K.Value);
        AppendLineIfNonZero(Labels.Magnesium, Mg.Value);
        AppendLineIfNonZero(Labels.Sulfur, S.Value);
        AppendLineIfNonZero(Labels.Calcium, Ca.Value);
        responseBuilder.AppendLine();
        AppendLineIfNonZero(Labels.Iron, Fe.Value);
        AppendLineIfNonZero(Labels.Copper, Cu.Value);
        AppendLineIfNonZero(Labels.Manganese, Mn.Value);
        AppendLineIfNonZero(Labels.Zinc, Zn.Value);
        AppendLineIfNonZero(Labels.Boron, B.Value);
        AppendLineIfNonZero(Labels.Molybdenum, Mo.Value);
        AppendLineIfNonZero(Labels.Chlorine, Cl.Value);
        AppendLineIfNonZero(Labels.Sodium, Na.Value);
        AppendLineIfNonZero(Labels.Silicon, Si.Value);
        AppendLineIfNonZero(Labels.Selenium, Se.Value);

        return responseBuilder.ToString();

        void AppendLineIfNonZero(string label, double value)
        {
            if (value > 0)
            {
                responseBuilder.AppendLine($"{label}: {Math.Round(value, Settings.Precision)}");
            }
        }
    }
}

