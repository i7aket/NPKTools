using NPKOptimizer.Domain.SolutionsFinderSettings;
using NPKOptimizer.Domain.SolutionsFinderSettings.Builder;
using Xunit;

namespace NPKOptimizer.Tests.UnitTests
{
    public class SolutionFinderSettingsBuilderTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void Build_SolutionFinderSettingsWithSetProperties_PropertiesAreSetCorrectly()
        {
            // Arrange
            SolutionFinderSettingsBuilder builder = new SolutionFinderSettingsBuilder();
            double rangeFactor = 0.5;
            double nSettings = 0.8;
            double pSettings = 0.7;
            double kSettings = 0.6;
            double caSettings =0.5;
            double mgSettings =0.4;
            double sSettings = 0.3;
            double clSettings =0.2;
            double feSettings =0.3;
            double cuSettings =0.2;
            double mnSettings =0.1;
            double znSettings =0.1;
            double bSettings = 0.05;
            double moSettings =0.02;
            double siSettings =0.01;
            double seSettings =0.005;
            double naSettings =0.1;

            // Act
            SolutionFinderSettings solutionFinderSettings = builder
                .AddRangeFactor(rangeFactor)
                .AddN(nSettings)
                .AddP(pSettings)
                .AddK(kSettings)
                .AddCa(caSettings)
                .AddMg(mgSettings)
                .AddS(sSettings)
                .AddCl(clSettings)
                .AddFe(feSettings)
                .AddCu(cuSettings)
                .AddMn(mnSettings)
                .AddZn(znSettings)
                .AddB(bSettings)
                .AddMo(moSettings)
                .AddSi(siSettings)
                .AddSe(seSettings)
                .AddNa(naSettings)
                .Build();

            // Assert
            Assert.Equal(rangeFactor, solutionFinderSettings.RangeFactor.Value);
            Assert.Equal(nSettings, solutionFinderSettings.Nitrogen.Value);
            Assert.Equal(pSettings, solutionFinderSettings.Phosphorus.Value);
            Assert.Equal(kSettings, solutionFinderSettings.Potassium.Value);
            Assert.Equal(caSettings, solutionFinderSettings.Calcium.Value);
            Assert.Equal(mgSettings, solutionFinderSettings.Magnesium.Value);
            Assert.Equal(sSettings, solutionFinderSettings.Sulfur.Value);
            Assert.Equal(clSettings, solutionFinderSettings.Chlorine.Value);
            Assert.Equal(feSettings, solutionFinderSettings.Iron.Value);
            Assert.Equal(cuSettings, solutionFinderSettings.Copper.Value);
            Assert.Equal(mnSettings, solutionFinderSettings.Manganese.Value);
            Assert.Equal(znSettings, solutionFinderSettings.Zinc.Value);
            Assert.Equal(bSettings, solutionFinderSettings.Boron.Value);
            Assert.Equal(moSettings, solutionFinderSettings.Molybdenum.Value);
            Assert.Equal(siSettings, solutionFinderSettings.Silicon.Value);
            Assert.Equal(seSettings, solutionFinderSettings.Selenium.Value);
            Assert.Equal(naSettings, solutionFinderSettings.Sodium.Value);
        }
    }
}
