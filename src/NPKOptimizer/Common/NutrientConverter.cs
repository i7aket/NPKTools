// ReSharper disable UnusedMember.Global
namespace NPKOptimizer.Common;

public static class NutrientConverter
{
    public static double P2O5ToP(double p2O5)
    {
        return p2O5 * (Atom.P.AtomicMass.Value * 2 /
                       (Atom.P.AtomicMass.Value * 2 +
                        Atom.O.AtomicMass.Value * 5));
    }

    public static double PToP2O5(double p)
    {
        return p * (Atom.P.AtomicMass.Value * 2 +
                    Atom.O.AtomicMass.Value * 5) /
               (2 * Atom.P.AtomicMass.Value);
    }

    public static double K2OToK(double k2O)
    {
        return k2O * (Atom.K.AtomicMass.Value /
                      (Atom.K.AtomicMass.Value * 2 +
                       Atom.O.AtomicMass.Value));
    }
    
    public static double KToK2O(double k)
    {
        return k * ((Atom.K.AtomicMass.Value * 2 +
                    Atom.O.AtomicMass.Value) /
               (2 * Atom.K.AtomicMass.Value));
    }
    
    public static double MgOToMg(double mgo)
    {
        return mgo * (Atom.Mg.AtomicMass.Value /
                      (Atom.Mg.AtomicMass.Value +
                       Atom.O.AtomicMass.Value));
    }
    
    public static double MgToMgO(double mg)
    {
        return mg * ((Atom.Mg.AtomicMass.Value +
                     Atom.O.AtomicMass.Value) /
               Atom.Mg.AtomicMass.Value);
    }
    
    public static double So3ToS(double so3)
    {
        return so3 * (Atom.S.AtomicMass.Value /
                      (Atom.S.AtomicMass.Value +
                       Atom.O.AtomicMass.Value * 3));
    }
    
    public static double SToSo3(double s)
    {
        return s * ((Atom.S.AtomicMass.Value +
                    Atom.O.AtomicMass.Value * 3) /
               Atom.S.AtomicMass.Value);
    }
    
    public static double CaOToCa(double cao)
    {
        return cao * (Atom.Ca.AtomicMass.Value /
                      (Atom.Ca.AtomicMass.Value +
                       Atom.O.AtomicMass.Value));
    }
    
    public static double CaToCaO(double ca)
    {
        return ca * ((Atom.Ca.AtomicMass.Value +
                     Atom.O.AtomicMass.Value) /
               Atom.Ca.AtomicMass.Value);
    }
    
    public static double PpmToEc500(double value)
    {
        return value / 500;
    }
    
    public static double PpmToEc640(double value)
    {
        return value / 640;
    }
    
    public static double PpmToEc700(double value)
    {
        return value / 700;
    }
    
    public static double EcToPpm500(double ec)
    {
        return ec * 500;
    }
    
    public static double EcToPpm640(double ec)
    {
        return ec * 640;
    }
    
    public static double EcToPpm700(double ec)
    {
        return ec * 700;
    }
}

