using FluentAssertions;
using NPKTools.Core.Domain.Fertilizers;
using NPKTools.Core.Domain.Fertilizers.Builders;
using NPKTools.Core.Domain.Fertilizers.Enums;
using Xunit;

namespace NPKTools.Core.Tests
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
            string name = "name";
            string formula = "formula";
            ConcentrateType type = ConcentrateType.B;
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
            Fertilizer fertilizerResultModel = builder
                .AddName(name)
                .AddFormula(formula)
                .AddType(type)
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
            Assert.Equal(name, fertilizerResultModel.Name.Value);
            Assert.Equal(formula, fertilizerResultModel.Formula.Value);
            Assert.Equal(type, fertilizerResultModel.Type);
            Assert.Equal(id, fertilizerResultModel.RefId.Value);
            Assert.Equal(weight, fertilizerResultModel.Weight.Value);
            Assert.Equal(price, fertilizerResultModel.Price.Value);
            Assert.Equal(no3+nh4+nh2, fertilizerResultModel.Nitrogen.Value);
            Assert.Equal(no3, fertilizerResultModel.Nitrogen.Nitrate);
            Assert.Equal(nh4, fertilizerResultModel.Nitrogen.Ammonium);
            Assert.Equal(nh2, fertilizerResultModel.Nitrogen.Amine);
            Assert.Equal(p, fertilizerResultModel.Phosphorus.Value);
            Assert.Equal(k, fertilizerResultModel.Potassium.Value);
            Assert.Equal(caNonChelated+caEdta, fertilizerResultModel.Calcium.Value);
            Assert.Equal(caNonChelated, fertilizerResultModel.Calcium.CaNonChelated);
            Assert.Equal(caEdta, fertilizerResultModel.Calcium.CaEdta);
            Assert.Equal(mgNonChelated+mgEdta, fertilizerResultModel.Magnesium.Value);
            Assert.Equal(mgNonChelated, fertilizerResultModel.Magnesium.MgNonChelated);
            Assert.Equal(mgEdta, fertilizerResultModel.Magnesium.MgEdta);
            Assert.Equal(s, fertilizerResultModel.Sulfur.Value);
            Assert.Equal(feNonChelated+feEdta+feDtpa+feEddha+feHbed, fertilizerResultModel.Iron.Value);
            Assert.Equal(feNonChelated, fertilizerResultModel.Iron.FeNonChelated);
            Assert.Equal(feEdta, fertilizerResultModel.Iron.FeEdta);
            Assert.Equal(feDtpa, fertilizerResultModel.Iron.FeDtpa);
            Assert.Equal(feEddha, fertilizerResultModel.Iron.FeEddha);
            Assert.Equal(feHbed, fertilizerResultModel.Iron.FeHbed);
            Assert.Equal(feOrthoPart, fertilizerResultModel.Iron.FeOrthoPart);
            Assert.Equal(cuNonChelated+cuEdta, fertilizerResultModel.Copper.Value);
            Assert.Equal(cuNonChelated, fertilizerResultModel.Copper.CuNonChelated);
            Assert.Equal(cuEdta, fertilizerResultModel.Copper.CuEdta);
            Assert.Equal(mnNonChelated+mnEdta, fertilizerResultModel.Manganese.Value);
            Assert.Equal(mnNonChelated, fertilizerResultModel.Manganese.MnNonChelated);
            Assert.Equal(mnEdta, fertilizerResultModel.Manganese.MnEdta);
            Assert.Equal(znNonChelated+znEdta, fertilizerResultModel.Zinc.Value);
            Assert.Equal(znNonChelated, fertilizerResultModel.Zinc.ZnNonChelated);
            Assert.Equal(znEdta, fertilizerResultModel.Zinc.ZnEdta);
            Assert.Equal(b, fertilizerResultModel.Boron.Value);
            Assert.Equal(mo, fertilizerResultModel.Molybdenum.Value);
            Assert.Equal(cl, fertilizerResultModel.Chlorine.Value);
            Assert.Equal(si, fertilizerResultModel.Silicon.Value);
            Assert.Equal(se, fertilizerResultModel.Selenium.Value);
            Assert.Equal(na, fertilizerResultModel.Sodium.Value);
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