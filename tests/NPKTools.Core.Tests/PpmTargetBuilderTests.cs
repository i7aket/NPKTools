using NPKTools.Core.Domain.PpmTarget;
using NPKTools.Core.Domain.PpmTarget.Builder;
using Xunit;

namespace NPKTools.Core.Tests
{
    public class PpmTargetBuilderTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void Build_PpmTargetWithSetProperties_PropertiesAreSetCorrectly()
        {
            // Arrange
            PpmTargetBuilder builder = new PpmTargetBuilder();
            double n = 150.0, p = 100.0, k = 200.0;
            double ca = 50.0, mg = 30.0, s = 20.0;
            double fe = 10.0, cu = 5.0, mn = 3.0;
            double zn = 7.0, b = 0.5, mo = 0.3;
            double cl = 2.0, si = 1.5, se = 0.1, na = 0.2;
            double liters = 1;

            // Act
            PpmTarget ppmTarget = builder
                .AddN(n)
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
                .AddLitters(liters)
                .Build();

            // Assert
            Assert.Equal(n, ppmTarget.N.Value);
            Assert.Equal(p, ppmTarget.P.Value);
            Assert.Equal(k, ppmTarget.K.Value);
            Assert.Equal(ca, ppmTarget.Ca.Value);
            Assert.Equal(mg, ppmTarget.Mg.Value);
            Assert.Equal(s, ppmTarget.S.Value);
            Assert.Equal(fe, ppmTarget.Fe.Value);
            Assert.Equal(cu, ppmTarget.Cu.Value);
            Assert.Equal(mn, ppmTarget.Mn.Value);
            Assert.Equal(zn, ppmTarget.Zn.Value);
            Assert.Equal(b, ppmTarget.B.Value);
            Assert.Equal(mo, ppmTarget.Mo.Value);
            Assert.Equal(cl, ppmTarget.Cl.Value);
            Assert.Equal(si, ppmTarget.Si.Value);
            Assert.Equal(se, ppmTarget.Se.Value);
            Assert.Equal(na, ppmTarget.Na.Value);
            Assert.Equal(liters, ppmTarget.Liters.Value);
        }
    }
}