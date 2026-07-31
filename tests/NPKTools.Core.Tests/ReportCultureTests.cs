using System.Globalization;
using System.Text;
using NPKTools.Core.Common;
using NPKTools.Core.Domain.Fertilizers;
using NPKTools.Core.Domain.Fertilizers.Builders;
using NPKTools.Core.Domain.Fertilizers.Extensions;
using Xunit;

namespace NPKTools.Core.Tests;

/// <summary>
/// Regression tests for the culture bug fixed in 2.0.0. <see cref="ReportFormatter"/> formatted
/// its decimal branch with the ambient culture while the integer branch used the invariant one,
/// so the same fertilizer rendered as "100,000" on a machine with a comma decimal separator and
/// "100.000" elsewhere. Reports must not depend on the host's regional settings.
/// </summary>
public class ReportCultureTests
{
    // de-DE, ru-RU and fr-FR use a comma as the decimal separator; en-US uses a dot.
    [Theory]
    [InlineData("de-DE")]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    [InlineData("fr-FR")]
    [Trait("Category", "Unit")]
    public void AppendLineIfNonZero_UsesInvariantDecimalSeparator(string culture)
    {
        RunInCulture(culture, () =>
        {
            StringBuilder builder = new();

            ReportFormatter.AppendLineIfNonZero(builder, "Weight", 100.5);

            Assert.Contains("100.500", builder.ToString(), StringComparison.Ordinal);
            Assert.DoesNotContain(",", builder.ToString(), StringComparison.Ordinal);
        });
    }

    [Theory]
    [InlineData("de-DE")]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    [InlineData("fr-FR")]
    [Trait("Category", "Unit")]
    public void FertilizerReport_IsIdenticalAcrossCultures(string culture)
    {
        string underInvariant = RunInCulture("en-US", () => BuildFertilizer().Report());
        string underCulture = RunInCulture(culture, () => BuildFertilizer().Report());

        Assert.Equal(underInvariant, underCulture);
    }

    [Theory]
    [InlineData("de-DE")]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    [InlineData("fr-FR")]
    [Trait("Category", "Unit")]
    public void NutrientSummary_IsIdenticalAcrossCultures(string culture)
    {
        string underInvariant = RunInCulture("en-US", () => BuildFertilizer().GetNutrientSummary());
        string underCulture = RunInCulture(culture, () => BuildFertilizer().GetNutrientSummary());

        Assert.Equal(underInvariant, underCulture);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AppendLineIfNonZero_NullBuilder_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ReportFormatter.AppendLineIfNonZero(null!, "Weight", 1));
    }

    private static Fertilizer BuildFertilizer() => new FertilizerBuilder()
        .AddName("Calcium Nitrate Tetrahydrate")
        .AddFormula("Ca(NO₃)2*4H₂O")
        .AddWeight(100.5)
        .AddNo3(11.863)
        .AddCaNonChelated(16.972)
        .Build();

    private static void RunInCulture(string culture, Action action) =>
        RunInCulture(culture, () =>
        {
            action();
            return 0;
        });

    private static T RunInCulture<T>(string culture, Func<T> action)
    {
        CultureInfo original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            return action();
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }
}
