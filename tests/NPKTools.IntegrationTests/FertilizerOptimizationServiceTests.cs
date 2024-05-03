using NPKTools.Core.Domain.Collections;
using NPKTools.Core.Domain.PartsPerMillion;
using NPKTools.Core.Domain.PpmTarget;
using NPKTools.Core.Domain.PpmTarget.Builder;
using NPKTools.Optimizer.Components;
using NPKTools.Optimizer.Contracts;
using NPKTools.Optimizer.Preset;
using NPKTools.PPMCalc;
using Xunit;

namespace NPKTools.IntegrationTests;

public class FertilizerOptimizationServiceTests
{
    protected IFertilizerOptimizationService FertilizerOptimizationService;
    protected IPpmCalculationService Calc;

    public FertilizerOptimizationServiceTests()
    {
        IOptimizationProblemSolver solver = new GoogleOrToolsOptimizationSolver();
        IOptimizationProblemMapper mapper = new OptimizationProblemMapper();
        IFertilizerOptimizer optimizer = new FertilizerOptimizationAdapter(solver, mapper);
        IFertilizerBundleRepository bundles = new FertilizerBundleRepository();        
        FertilizerOptimizationService = new FertilizerOptimizationService(optimizer, bundles);
        
        Calc = new PpmCalculationService();
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public void FindMacroSolutions_WithValidTarget_ReturnsSolutions()
    {
        PpmTarget target = new PpmTargetBuilder()
            .AddN(100)
            .AddP(50)
            .AddK(150)
            .AddMg(60)
            .AddCa(60)
            .AddS(80)
            .Build();

        Solutions result = FertilizerOptimizationService.FindMacroSolutions(target);

        const double tolerance = 0.00001;
        
        foreach (Solution solution in result)
        {
            Ppm solutionPpm = Calc.CalculatePpm(solution);
            Assert.InRange(solutionPpm.Nitrogen.Value, target.N.Value - tolerance, target.N.Value + tolerance);
            Assert.InRange(solutionPpm.Phosphorus.Value, target.P.Value - tolerance, target.P.Value + tolerance);
            Assert.InRange(solutionPpm.Potassium.Value, target.K.Value - tolerance, target.K.Value + tolerance);
            Assert.InRange(solutionPpm.Magnesium.Value, target.Mg.Value - tolerance, target.Mg.Value + tolerance);
            Assert.InRange(solutionPpm.Calcium.Value, target.Ca.Value - tolerance, target.Ca.Value + tolerance);
        }
        
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public void FindMicroSolutions_WithValidTarget_ReturnsSolutions()
    {
        PpmTarget target = new PpmTargetBuilder()
            .AddFe(2)
            .AddCu(0.05)
            .AddMn(0.55)
            .AddZn(0.33)
            .AddB(0.28)
            .AddMo(0.05)
            .AddSi(0.01)
            .AddSe(0.01)
            .Build();

        Solutions result = FertilizerOptimizationService.FindMicroSolutions(target);

        const double tolerance = 0.00001;
        
        foreach (Solution solution in result)
        {
            Ppm solutionPpm = Calc.CalculatePpm(solution);
            Assert.InRange(solutionPpm.Iron.Value, target.Fe.Value - tolerance, target.Fe.Value + tolerance);
            Assert.InRange(solutionPpm.Copper.Value, target.Cu.Value - tolerance, target.Cu.Value + tolerance);
            Assert.InRange(solutionPpm.Manganese.Value, target.Mn.Value - tolerance, target.Mn.Value + tolerance);
            Assert.InRange(solutionPpm.Zinc.Value, target.Zn.Value - tolerance, target.Zn.Value + tolerance);
            Assert.InRange(solutionPpm.Boron.Value, target.B.Value - tolerance, target.B.Value + tolerance);
            Assert.InRange(solutionPpm.Molybdenum.Value, target.Mo.Value - tolerance, target.Mo.Value + tolerance);
            Assert.InRange(solutionPpm.Silicon.Value, target.Si.Value - tolerance, target.Si.Value + tolerance);
            Assert.InRange(solutionPpm.Selenium.Value, target.Se.Value - tolerance, target.Se.Value + tolerance);
        }
        
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public void FindSolutions_WithValidTarget_ReturnsSolutions()
    {
        PpmTarget target = new PpmTargetBuilder()
            .AddN(150)
            .AddP(50)
            .AddK(200)
            .AddMg(60)
            .AddCa(100)
            .AddS(100)
            .AddFe(2)
            .AddCu(0.05)
            .AddMn(0.55)
            .AddZn(0.33)
            .AddB(0.28)
            .AddMo(0.05)
            .AddCl(0.01)
            .AddSi(0.01)
            .AddSe(0.01)
            .Build();

        (Solutions Macro, Solutions Micro) result = FertilizerOptimizationService.FindSolutions(target);

        const double tolerance = 0.00001;
        
        foreach (Solution solution in result.Macro)
        {
            Ppm solutionPpm = Calc.CalculatePpm(solution);
            Assert.InRange(solutionPpm.Nitrogen.Value, target.N.Value - tolerance, target.N.Value + tolerance);
            Assert.InRange(solutionPpm.Phosphorus.Value, target.P.Value - tolerance, target.P.Value + tolerance);
            Assert.InRange(solutionPpm.Potassium.Value, target.K.Value - tolerance, target.K.Value + tolerance);
            Assert.InRange(solutionPpm.Magnesium.Value, target.Mg.Value - tolerance, target.Mg.Value + tolerance);
            Assert.InRange(solutionPpm.Calcium.Value, target.Ca.Value - tolerance, target.Ca.Value + tolerance);
            Assert.InRange(solutionPpm.Chlorine.Value, target.Cl.Value - tolerance, target.Cl.Value + tolerance);
        }
        
        foreach (Solution solution in result.Micro)
        {
            Ppm solutionPpm = Calc.CalculatePpm(solution);
            Assert.InRange(solutionPpm.Iron.Value, target.Fe.Value - tolerance, target.Fe.Value + tolerance);
            Assert.InRange(solutionPpm.Copper.Value, target.Cu.Value - tolerance, target.Cu.Value + tolerance);
            Assert.InRange(solutionPpm.Manganese.Value, target.Mn.Value - tolerance, target.Mn.Value + tolerance);
            Assert.InRange(solutionPpm.Zinc.Value, target.Zn.Value - tolerance, target.Zn.Value + tolerance);
            Assert.InRange(solutionPpm.Boron.Value, target.B.Value - tolerance, target.B.Value + tolerance);
            Assert.InRange(solutionPpm.Molybdenum.Value, target.Mo.Value - tolerance, target.Mo.Value + tolerance);
            Assert.InRange(solutionPpm.Silicon.Value, target.Si.Value - tolerance, target.Si.Value + tolerance);
            Assert.InRange(solutionPpm.Selenium.Value, target.Se.Value - tolerance, target.Se.Value + tolerance);
        }
    }
}