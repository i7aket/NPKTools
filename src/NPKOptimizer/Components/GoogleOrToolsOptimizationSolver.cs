using Google.OrTools.LinearSolver;
using NPKOptimizer.Common;
using NPKOptimizer.Const;
using NPKOptimizer.Contracts;

namespace NPKOptimizer.Components;

/// <summary>
/// Provides an implementation of the <see cref="IOptimizationProblemSolver"/> using Google's OR-Tools.
/// </summary>
public class GoogleOrToolsOptimizationSolver : IOptimizationProblemSolver
{
    /// <summary>
    /// Solves the given optimization problem using the linear solver from Google OR-Tools.
    /// </summary>
    /// <param name="problem">The optimization problem to solve, containing variables, constraints, and an objective.</param>
    /// <returns>A dictionary where keys are variable names and values are their optimized numerical values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any critical component of the problem (such as the problem itself, its variables, constraints, or objective) is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the solver does not find an optimal solution.</exception>
    public Dictionary<string, double>? Solve(OptimizationProblem problem)
    {
        ArgumentNullException.ThrowIfNull(problem);
        ArgumentNullException.ThrowIfNull(problem.Objective);
        ThrowIf.NullOrEmpty(problem.Objective.Coefficients);
        ThrowIf.NullOrEmpty(problem.Variables);
        ThrowIf.NullOrEmpty(problem.Constraints);

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
            return default;

        return variables.ToDictionary(variable => variable.Key, variable => variable.Value.SolutionValue());
    }
}
