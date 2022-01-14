using Google.OrTools.Algorithms;
using Google.OrTools.LinearSolver;

namespace MiniTools.HostApp.Services;

internal class OrToolService
{
    public static void Example()
    {
        // Example of linear optimization.
        // Given a function  "3x + y"
        // Find the max values for x and y such that following conditions (contraints) are met:
        // 0 <= x <= 1
        // 0 <= y <= 2
        //  x + y <= 2

        // Create the linear solver with the GLOP backend.
        Solver solver = Solver.CreateSolver("GLOP");

        // Create the variables x and y.
        Variable x = solver.MakeNumVar(0.0, 1.0, "x"); // 0	<= x <=	1
        Variable y = solver.MakeNumVar(0.0, 2.0, "y"); // 0 <= y <= 2

        Console.WriteLine("Number of variables = " + solver.NumVariables());

        // Create a linear constraint, 0 <= x + y <= 2.
        Constraint ct = solver.MakeConstraint(0.0, 2.0, "ct");
        ct.SetCoefficient(x, 1);
        ct.SetCoefficient(y, 1);

        Console.WriteLine("Number of constraints = " + solver.NumConstraints());

        // Create the objective function, 3 * x + y.
        Objective objective = solver.Objective();
        objective.SetCoefficient(x, 3);
        objective.SetCoefficient(y, 1);
        objective.SetMaximization();

        solver.Solve();

        Console.WriteLine("Solution:");
        Console.WriteLine("Objective value = " + solver.Objective().Value());
        Console.WriteLine("x = " + x.SolutionValue());
        Console.WriteLine("y = " + y.SolutionValue());

    }

    public static void ExampleKnapsack()
    {
        // In the knapsack problem, you need to pack a set of items, with given values and sizes(such as weights or volumes),
        // into a container with a maximum capacity.
        // If the total size of the items exceeds the capacity, you can't pack them all.
        // In that case, the problem is to choose a subset of the items of maximum total value that will fit in the container.

        long[] values = { 360, 83, 59, 130, 431, 67,  230, 52,  93,  125, 670, 892, 600, 38,  48,  147, 78,
                  256, 63, 17, 120, 164, 432, 35,  92,  110, 22,  42,  50,  323, 514, 28,  87,  73,
                  78,  15, 26, 78,  210, 36,  85,  189, 274, 43,  33,  10,  19,  389, 276, 312 };

        long[,] weights = { { 7,  0,  30, 22, 80, 94, 11, 81, 70, 64, 59, 18, 0,  36, 3,  8,  15,
                      42, 9,  0,  42, 47, 52, 32, 26, 48, 55, 6,  29, 84, 2,  4,  18, 56,
                      7,  29, 93, 44, 71, 3,  86, 66, 31, 65, 0,  79, 20, 65, 52, 13 } };

        long[] capacities = { 850 };

        KnapsackSolver solver = new KnapsackSolver(
            KnapsackSolver.SolverType.KNAPSACK_MULTIDIMENSION_BRANCH_AND_BOUND_SOLVER, "KnapsackExample");

        solver.Init(values, weights, capacities);
        long computedValue = solver.Solve();
        Console.WriteLine("Optimal Value = " + computedValue);

        for (int i = 0; i < weights.Length; i++)
        {
            if (solver.BestSolutionContains(i))
            {
                Console.Write(i.ToString() + ",");
            }
        }
    }

    public static void ExampleMultipleKnapsack()
    {
        // you start with a collection of items of varying weights and values.
        // The problem is to pack a subset of the items into five bins, each of which has a maximum capacity of 100, so that the total packed value is a maximum.

        double[] Weights = { 48, 30, 42, 36, 36, 48, 42, 42, 36, 24, 30, 30, 42, 36, 36 };
        double[] Values = { 10, 30, 25, 50, 35, 30, 15, 40, 30, 35, 45, 10, 20, 30, 25 };

        int NumItems = Weights.Length;
        int[] allItems = Enumerable.Range(0, NumItems).ToArray();

        double[] BinCapacities = { 100, 100, 100, 100, 100 };
        int NumBins = BinCapacities.Length;
        int[] allBins = Enumerable.Range(0, NumBins).ToArray();

        // Create the linear solver with the SCIP backend.
        Solver solver = Solver.CreateSolver("SCIP");

        // Each x[(i, j)] is a 0-1 variable, where i is an item and j is a bin.
        // In the solution, x[(i, j)] will be 1 if item i is placed in bin j, and 0 otherwise

        Variable[,] x = new Variable[NumItems, NumBins];
        foreach (int i in allItems)
        {
            foreach (int b in allBins)
            {
                x[i, b] = solver.MakeBoolVar($"x_{i}_{b}");
            }
        }

        // Constraints

        // Each item can be placed in at most one bin. This constraint is set by requiring the sum of x[i][j] over all bins j to be less than or equal to 1.
        // The total weight packed in each bin can't exceed its capacity. This constraint is set by requiring the sum of the weights of items placed in bin j to be less than or equal to the capacity of the bin.


        // Each item is assigned to at most one bin.
        foreach (int i in allItems)
        {
            Constraint constraint = solver.MakeConstraint(0, 1, "");
            foreach (int b in allBins)
            {
                constraint.SetCoefficient(x[i, b], 1);
            }
        }

        // The amount packed in each bin cannot exceed its capacity.
        foreach (int b in allBins)
        {
            Constraint constraint = solver.MakeConstraint(0, BinCapacities[b], "");
            foreach (int i in allItems)
            {
                constraint.SetCoefficient(x[i, b], Weights[i]);
            }
        }

        Objective objective = solver.Objective();
        foreach (int i in allItems)
        {
            foreach (int b in allBins)
            {
                objective.SetCoefficient(x[i, b], Values[i]);
            }
        }
        objective.SetMaximization();

        Solver.ResultStatus resultStatus = solver.Solve();

        // Check that the problem has an optimal solution.
        if (resultStatus == Solver.ResultStatus.OPTIMAL)
        {
            Console.WriteLine($"Total packed value: {solver.Objective().Value()}");
            double TotalWeight = 0.0;
            foreach (int b in allBins)
            {
                double BinWeight = 0.0;
                double BinValue = 0.0;
                Console.WriteLine("Bin " + b);
                foreach (int i in allItems)
                {
                    if (x[i, b].SolutionValue() == 1)
                    {
                        Console.WriteLine($"Item {i} weight: {Weights[i]} values: {Values[i]}");
                        BinWeight += Weights[i];
                        BinValue += Values[i];
                    }
                }
                Console.WriteLine("Packed bin weight: " + BinWeight);
                Console.WriteLine("Packed bin value: " + BinValue);
                TotalWeight += BinWeight;
            }
            Console.WriteLine("Total packed weight: " + TotalWeight);
        }
        else
        {
            Console.WriteLine("The problem does not have an optimal solution!");
        }

    }
}
