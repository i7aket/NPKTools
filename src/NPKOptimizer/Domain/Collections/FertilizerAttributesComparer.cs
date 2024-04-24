using NPKOptimizer.Domain.Fertilizers;

namespace NPKOptimizer.Domain.Collections;

public class FertilizerAttributesComparer : IEqualityComparer<FertilizerAttributes>
{
    public bool Equals(FertilizerAttributes? x, FertilizerAttributes? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x == null || y == null) return false;

        return Equals(x.Price, y.Price) &&
               Equals(x.Nitrogen, y.Nitrogen) &&
               Equals(x.Phosphorus, y.Phosphorus) &&
               Equals(x.Potassium, y.Potassium) &&
               Equals(x.Calcium, y.Calcium) &&
               Equals(x.Magnesium, y.Magnesium) &&
               Equals(x.Sulfur, y.Sulfur) &&
               Equals(x.Iron, y.Iron) &&
               Equals(x.Copper, y.Copper) &&
               Equals(x.Manganese, y.Manganese) &&
               Equals(x.Zinc, y.Zinc) &&
               Equals(x.Boron, y.Boron) &&
               Equals(x.Molybdenum, y.Molybdenum) &&
               Equals(x.Chlorine, y.Chlorine) &&
               Equals(x.Silicon, y.Silicon) &&
               Equals(x.Selenium, y.Selenium) &&
               Equals(x.Sodium, y.Sodium);
    }

    public int GetHashCode(FertilizerAttributes obj)
    {
        unchecked 
        {
            int hash = 19;
            hash = hash * 31 + obj.Price.GetHashCode();
            hash = hash * 31 + obj.Nitrogen.GetHashCode();
            hash = hash * 31 + obj.Phosphorus.GetHashCode();
            hash = hash * 31 + obj.Potassium.GetHashCode();
            hash = hash * 31 + obj.Calcium.GetHashCode();
            hash = hash * 31 + obj.Magnesium.GetHashCode();
            hash = hash * 31 + obj.Sulfur.GetHashCode();
            hash = hash * 31 + obj.Iron.GetHashCode();
            hash = hash * 31 + obj.Copper.GetHashCode();
            hash = hash * 31 + obj.Manganese.GetHashCode();
            hash = hash * 31 + obj.Zinc.GetHashCode();
            hash = hash * 31 + obj.Boron.GetHashCode();
            hash = hash * 31 + obj.Molybdenum.GetHashCode();
            hash = hash * 31 + obj.Chlorine.GetHashCode();
            hash = hash * 31 + obj.Silicon.GetHashCode();
            hash = hash * 31 + obj.Selenium.GetHashCode();
            hash = hash * 31 + obj.Sodium.GetHashCode();
            return hash;
        }
    }
}
