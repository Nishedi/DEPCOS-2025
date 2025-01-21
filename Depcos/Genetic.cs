using Depcos;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Genetic
{
    private int distancesSize;
    private int populationSize = 10000;
    private GeneralMethods gm = new GeneralMethods();
    Random generator = new Random();


    private List<Result> InitializePopulation(double[,] distancesMatrix, List<Customer> customers, List<Vehicle> vehicles)
    {
        var population = new List<Result>(this.populationSize);

        List<double> vehicleStarts = new List<double>();
        var GTR = gm.createGreedyGTR(distancesMatrix, customers, vehicleStarts, vehicles);
        population.Add(new Result(GTR, vehicleStarts));


        for (int i = distancesSize; i < this.populationSize; i++)
        {
            vehicleStarts = new List<double>();
            GTR = gm.createGreedyGTRMutated(distancesMatrix, customers, vehicleStarts, vehicles);
            population.Add(new Result(GTR, vehicleStarts));
        }
        return population;
    }

    private Result SelectParent(double[,] distancesMatrix, List<Result> population)
    {
        int tournamentSize = population[0].GTR.Count / 2; // pierwotnie 2
        var tournament = new List<Result>();

        for (int i = 0; i < tournamentSize; i++)
        {
            int randomIndex = generator.Next(population.Count);
            tournament.Add(population[randomIndex]);
        }

        int bestTourIndex = 0;
        double bestTourDistance = gm.calculateCostGTRv2(distancesMatrix, tournament[0].GTR, tournament[0].VehicleStartTimes);

        for (int i = 1; i < tournamentSize; i++)
        {
            double distance = gm.calculateCostGTRv2(distancesMatrix, tournament[i].GTR, tournament[i].VehicleStartTimes);

            if (distance < bestTourDistance)
            {
                bestTourDistance = distance;
                bestTourIndex = i;
            }
        }

        return tournament[bestTourIndex];
    }

    public Result GeneticSolve(double[,] distances, List<Customer> customers, List<Vehicle> vehicles, int maxTime)
    {
        maxTime *= 1000;
        
        double bestDistance = int.MaxValue;
        
        
        var bestTour = new Result(null, null);

        Stopwatch stopwatch = Stopwatch.StartNew();
        var population = InitializePopulation(distances, customers, vehicles);


        while (stopwatch.ElapsedMilliseconds <= maxTime)
        {
            var newPopulation = new List<Result>(populationSize);

            for (int i = 0; i < populationSize; i++)
            {
                var first = SelectParent(distances, population);
                var second = SelectParent(distances, population);

                /*int iter = 0;
                while (iter++ < 100 && first.SequenceEqual(second))
                {
                    second = SelectParent(population);
                }

                List<int> child;
                if (first.SequenceEqual(second))
                {
                    child = cs.PMXCrossOver(first, second, distancesSize, 0);
                }
                else
                {
                    child = crossoverType switch
                    {
                        0 => cs.PMXCrossOver(first, second, distancesSize, crossoverRate),
                        1 => cs.OrderCrossover(first, second, distancesSize, crossoverRate),
                        _ => throw new ArgumentException("Invalid crossover type")
                    };
                }

                if (mutationType == 0) mt.InsertionMutate(child, distancesSize, mutationRate);
                if (mutationType == 1) mt.SwapMutate(child, distancesSize, mutationRate);*/
                var child = first;

                newPopulation.Add(child);
            }

            double lastBest = bestDistance;
            foreach (var individual in newPopulation)
            {
                double distance = gm.calculateCostGTRv2(distances, individual.GTR, individual.VehicleStartTimes);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestTour = individual;
                }

                if (distance < 2.5 * lastBest)
                {
                    int randomIndex = generator.Next(population.Count);
                    population[randomIndex] = individual;
                }
            }
        }

        return bestTour;
    }
}
