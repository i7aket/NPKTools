using Google.OrTools.LinearSolver;
using NPKOptimizer.Const;
using NPKOptimizer.Contracts;

namespace NPKOptimizer.Components;

public class GoogleOrToolsOptimizationSolver : IOptimizationProblemSolver
{
    public Dictionary<string, double> Solve(OptimizationProblem problem)
    {
        Solver solver = Solver.CreateSolver("GLOP");
        
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

        foreach (OptimizationProblem.OptimizationConstraint constraint in problem.Constraints)
        {
            Constraint solverConstraint = solver.MakeConstraint(constraint.LowerBound, constraint.UpperBound, constraint.Name);
            foreach (KeyValuePair<string, double> coefficient in constraint.Coefficients) 
            {
                solverConstraint.SetCoefficient(variables[coefficient.Key], coefficient.Value);
            }
        }

        Solver.ResultStatus resultStatus = solver.Solve();

        if (resultStatus != Solver.ResultStatus.OPTIMAL)
        {
            throw new InvalidOperationException(OptimizationSettings.SolverOptimalSolutionNotFoundMessage);
        }

        return variables.ToDictionary(variable => variable.Key, variable => variable.Value.SolutionValue());
    }
}
