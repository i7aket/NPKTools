using Google.OrTools.LinearSolver;
using NPKOptimizer.Common;

namespace NPKOptimizer.Components.OptimizationProblemSolvers;

public class GoogleOrToolsOptimizationSolver : Contracts.IOptimizationProblemSolver
{
    private const string SolverCreationFailureMessage = "Failed to create solver instance.";
    private const string SolverOptimalSolutionNotFoundMessage = "The solver did not find an optimal solution.";

    public ActionResult<Dictionary<string, double>> Solve(Contracts.OptimizationProblem problem)
    {
        Solver solver = Solver.CreateSolver("GLOP");
        if (solver == null)
        {
            return ActionResult<Dictionary<string, double>>.Fail(SolverCreationFailureMessage);
        }

        Dictionary<string, Variable> variables = problem.Variables.ToDictionary(
            name => name.Key,
            name => solver.MakeNumVar(0, double.PositiveInfinity, name.Key)
        );

        Objective costObjective = solver.Objective();
        foreach (KeyValuePair<string, double> coefficientPair in problem.Objective.Coefficients)
        {
            costObjective.SetCoefficient(variables[coefficientPair.Key], coefficientPair.Value);
        }

        if (problem.Objective.IsMinimization)
        {
            costObjective.SetMinimization();
        }
        else
        {
            costObjective.SetMaximization();
        }

        foreach (Contracts.OptimizationConstraint constraint in problem.Constraints)
        {
            Constraint solverConstraint = solver.MakeConstraint(constraint.LowerBound, constraint.UpperBound, constraint.Name);
            foreach (KeyValuePair<string, double> coefficient in constraint.Coefficients) //Coefficients это количество содержания нутриента в удобрении например кальция в кальциевой селитре 
            {
                solverConstraint.SetCoefficient(variables[coefficient.Key], coefficient.Value);
            }
        }

        Solver.ResultStatus resultStatus = solver.Solve();

        if (resultStatus != Solver.ResultStatus.OPTIMAL)
        {
            return ActionResult<Dictionary<string, double>>.Fail(SolverOptimalSolutionNotFoundMessage);
        }

        return ActionResult<Dictionary<string, double>>.Success(
            variables.ToDictionary(variable => variable.Key, variable => variable.Value.SolutionValue())
        );
    }
}
