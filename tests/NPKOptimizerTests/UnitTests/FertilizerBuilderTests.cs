using FluentAssertions;
using NPKOptimizer.Domain.Fertilizers;
using NPKOptimizer.Domain.Fertilizers.Builders;
using Xunit;

namespace NPKOptimizer.Tests.UnitTests
{
    public class FertilizerBuilderTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void Build_FertilizerWithSetProperties_PropertiesAreSetCorrectly()
        {
            // Arrange
            FertilizerBuilder builder = new FertilizerBuilder();
            Guid id = Guid.NewGuid();
            double weight = 50.0;
            double price = 100.0;
            double no3 = 10.0;
            double nh4 = 5.0;
            double nh2 = 2.5;
            double p = 3.0;
            double k = 7.0;
            double caNonChelated = 1.0;
            double caEdta = 1.5;
            double mgNonChelated = 0.5;
            double mgEdta = 0.3;
            double s = 0.2;
            double feNonChelated = 0.1;
            double feEdta = 0.05;
            double feDtpa = 0.03;
            double feEddha = 0.07;
            double feHbed = 0.04;
            double feOrthoPart = 0.02;
            double cuNonChelated = 0.01;
            double cuEdta = 0.02;
            double mnNonChelated = 0.015;
            double mnEdta = 0.025;
            double znNonChelated = 0.05;
            double znEdta = 0.07;
            double b = 0.009;
            double mo = 0.006;
            double cl = 0.008;
            double si = 0.05;
            double se = 0.004;
            double na = 0.01;

            // Act
            Fertilizer fertilizer = builder
                .AddId(id)
                .AddWeight(weight)
                .AddPrice(price)
                .AddNo3(no3)
                .AddNh4(nh4)
                .AddNh2(nh2)
                .AddP(p)
                .AddK(k)
                .AddCaNonChelated(caNonChelated)
                .AddCaEdta(caEdta)
                .AddMgNonChelated(mgNonChelated)
                .AddMgEdta(mgEdta)
                .AddS(s)
                .AddFeNonChelated(feNonChelated)
                .AddFeEdta(feEdta)
                .AddFeDtpa(feDtpa)
                .AddFeEddha(feEddha)
                .AddFeHbed(feHbed)
                .AddFeOrthoPart(feOrthoPart)
                .AddCuNonChelated(cuNonChelated)
                .AddCuEdta(cuEdta)
                .AddMnNonChelated(mnNonChelated)
                .AddMnEdta(mnEdta)
                .AddZnNonChelated(znNonChelated)
                .AddZnEdta(znEdta)
                .AddB(b)
                .AddMo(mo)
                .AddCl(cl)
                .AddSi(si)
                .AddSe(se)
                .AddNa(na)
                .Build();

            // Assert
            Assert.Equal(id, fertilizer.RefId.Value);
            Assert.Equal(weight, fertilizer.Weight.Value);
            Assert.Equal(price, fertilizer.Price.Value);
            Assert.Equal(no3+nh4+nh2, fertilizer.Nitrogen.Value);
            Assert.Equal(no3, fertilizer.Nitrogen.Nitrate);
            Assert.Equal(nh4, fertilizer.Nitrogen.Ammonium);
            Assert.Equal(nh2, fertilizer.Nitrogen.Amine);
            Assert.Equal(p, fertilizer.Phosphorus.Value);
            Assert.Equal(k, fertilizer.Potassium.Value);
            Assert.Equal(caNonChelated+caEdta, fertilizer.Calcium.Value);
            Assert.Equal(caNonChelated, fertilizer.Calcium.CaNonChelated);
            Assert.Equal(caEdta, fertilizer.Calcium.CaEdta);
            Assert.Equal(mgNonChelated+mgEdta, fertilizer.Magnesium.Value);
            Assert.Equal(mgNonChelated, fertilizer.Magnesium.MgNonChelated);
            Assert.Equal(mgEdta, fertilizer.Magnesium.MgEdta);
            Assert.Equal(s, fertilizer.Sulfur.Value);
            Assert.Equal(feNonChelated+feEdta+feDtpa+feEddha+feHbed, fertilizer.Iron.Value);
            Assert.Equal(feNonChelated, fertilizer.Iron.FeNonChelated);
            Assert.Equal(feEdta, fertilizer.Iron.FeEdta);
            Assert.Equal(feDtpa, fertilizer.Iron.FeDtpa);
            Assert.Equal(feEddha, fertilizer.Iron.FeEddha);
            Assert.Equal(feHbed, fertilizer.Iron.FeHbed);
            Assert.Equal(feOrthoPart, fertilizer.Iron.FeOrthoPart);
            Assert.Equal(cuNonChelated+cuEdta, fertilizer.Copper.Value);
            Assert.Equal(cuNonChelated, fertilizer.Copper.CuNonChelated);
            Assert.Equal(cuEdta, fertilizer.Copper.CuEdta);
            Assert.Equal(mnNonChelated+mnEdta, fertilizer.Manganese.Value);
            Assert.Equal(mnNonChelated, fertilizer.Manganese.MnNonChelated);
            Assert.Equal(mnEdta, fertilizer.Manganese.MnEdta);
            Assert.Equal(znNonChelated+znEdta, fertilizer.Zinc.Value);
            Assert.Equal(znNonChelated, fertilizer.Zinc.ZnNonChelated);
            Assert.Equal(znEdta, fertilizer.Zinc.ZnEdta);
            Assert.Equal(b, fertilizer.Boron.Value);
            Assert.Equal(mo, fertilizer.Molybdenum.Value);
            Assert.Equal(cl, fertilizer.Chlorine.Value);
            Assert.Equal(si, fertilizer.Silicon.Value);
            Assert.Equal(se, fertilizer.Selenium.Value);
            Assert.Equal(na, fertilizer.Sodium.Value);
        }
        
        [Fact]
        [Trait("Category", "Unit")]
        public void SetPropertyTwice_ThrowsInvalidOperationException()
        {
            // Arrange
            FertilizerBuilder builder = new FertilizerBuilder();
            double nitrogenValue = 5.0;

            // Act
            builder.AddNo3(nitrogenValue); // First time setting the nitrogen

            // This second call should throw an exception since nitrogen is already set
            Action act = () => builder.AddNo3(nitrogenValue);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage($"*Property No3 has already been set.*");
        }
    }
}