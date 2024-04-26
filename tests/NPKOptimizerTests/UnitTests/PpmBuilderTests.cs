using NPKOptimizer.Domain.PartsPerMillion;
using NPKOptimizer.Domain.PartsPerMillion.Builder;
using Xunit;

namespace NPKOptimizer.Tests.UnitTests
{
    public class PpmBuilderTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void Build_PpmWithSetProperties_PropertiesAreSetCorrectly()
        {
            // Arrange
            PpmBuilder builder = new PpmBuilder();
            double nitrate = 100.0, ammonium = 50.0, amine = 25.0;
            double p = 80.0, k = 120.0, ca = 60.0, mg = 40.0, s = 30.0;
            double fe = 15.0, cu = 7.0, mn = 3.0, zn = 5.0, b = 1.0, mo = 0.5;
            double cl = 10.0, si = 8.0, se = 0.2, na = 2.0;
    
            double expectedTotalValue = nitrate + ammonium + amine + p + k + ca + mg + s + fe +
                                        cu + mn + zn + b + mo + cl + si + se + na;

            // Act
            Ppm ppm = builder
                .AddNitrate(nitrate)
                .AddAmmonium(ammonium)
                .AddAmine(amine)
                .AddP(p)
                .AddK(k)
                .AddCa(ca)
                .AddMg(mg)
                .AddS(s)
                .AddFe(fe)
                .AddCu(cu)
                .AddMn(mn)
                .AddZn(zn)
                .AddB(b)
                .AddMo(mo)
                .AddCl(cl)
                .AddSi(si)
                .AddSe(se)
                .AddNa(na)
                .Build();

            // Assert
            Assert.Equal(nitrate+ammonium+amine, ppm.Nitrogen.Value);
            Assert.Equal(nitrate, ppm.Nitrogen.Nitrate);
            Assert.Equal(ammonium, ppm.Nitrogen.Ammonium);
            Assert.Equal(amine, ppm.Nitrogen.Amine);
            Assert.Equal(p, ppm.Phosphorus.Value);
            Assert.Equal(k, ppm.Potassium.Value);
            Assert.Equal(ca, ppm.Calcium.Value);
            Assert.Equal(mg, ppm.Magnesium.Value);
            Assert.Equal(s, ppm.Sulfur.Value);
            Assert.Equal(fe, ppm.Iron.Value);
            Assert.Equal(cu, ppm.Copper.Value);
            Assert.Equal(mn, ppm.Manganese.Value);
            Assert.Equal(zn, ppm.Zinc.Value);
            Assert.Equal(b, ppm.Boron.Value);
            Assert.Equal(mo, ppm.Molybdenum.Value);
            Assert.Equal(cl, ppm.Chlorine.Value);
            Assert.Equal(si, ppm.Silicon.Value);
            Assert.Equal(se, ppm.Selenium.Value);
            Assert.Equal(na, ppm.Sodium.Value);
            Assert.Equal(expectedTotalValue, ppm.Value); 
        }
    }
}
