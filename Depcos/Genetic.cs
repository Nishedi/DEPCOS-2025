using Depcos;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Genetic
{
    private int distancesSize;
    private int populationSize = 1000;
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
            GTR = gm.createGreedyGTRMutated(distancesMatrix, customers, vehicleStarts, vehicles);
            var x = new Result(GTR, vehicleStarts);
            x.generateGreedyTimes(distancesMatrix);
            population.Add(x);
        }
        return population;
    }

    private Result SelectParent(double[,] distancesMatrix, List<Result> population)
    {
        int tournamentSize = population[0].GTR.Count / 10; // pierwotnie 2
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

    public List<Customer> Swap(List<Customer> child)
    {
        int i = generator.Next(child.Count-2)+1;
        int j = generator.Next(child.Count-2)+1;
        if(i==j) j = generator.Next(child.Count-2)+1;
        var customer = child[i];
        child[i] = child[j];
        child[j] = customer;
        return child;
    }

    public List<Customer> Crossover(List<Customer> parent1, List<Customer> parent2)
    {
        var childClients = new List<Customer>();
        for (int x = 0; x < parent1.Count; x++)
        {
            childClients.Add(null);
        }
        int i = generator.Next(parent1.Count);
        int j = generator.Next(parent2.Count);
        while (j == i)
        {
            j = generator.Next(parent1.Count);
        }
        if (i > j)
        {
            int swap = i;
            i = j;
            j = swap;
        }

        for (; i <= j; i++)
        {
            childClients[i] = parent1[i];
        }
        int currentIndex = 1;
        childClients[0] = parent1[0];
        childClients[childClients.Count-1] = parent1[parent1.Count-1];

        for( i = 1; i < parent1.Count-1; i++)
        {
            if (parent1[i].Id == 0)
            {
                childClients[i] = parent1[i];
            }
        }

        foreach (var element in parent2)
        {
            if (!childClients.Contains(element))
            {
                while (childClients[currentIndex] != null)
                {
                    currentIndex++;
                }
                childClients[currentIndex] = element;
            }
        }
        for (int x = 0; x < childClients.Count; x++)
        {
            if (childClients[x] == null)
            {
                childClients[x] = parent1[0];
                i = generator.Next(childClients.Count);
                while (childClients[i] == null)
                {
                    i = generator.Next(childClients.Count);
                }
                Customer swap = childClients[i];
                childClients[i] = childClients[x];
                childClients[x] = swap;

            }
        }
        return childClients;
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
                bool isSame = true;
                for(int x = 0; x < first.GTR.Count; x++)
                {
                    if (first.GTR[x] != second.GTR[x])
                    {
                        isSame= false;
                        break;
                    }   
                }
                if (isSame)
                {
                    newPopulation.Add(first);
                    continue;
                }

                var childGTR = Crossover(first.GTR, second.GTR);
                childGTR = Swap(childGTR);
                var child = new Result(childGTR, null);
                child.generateGreedyTimes(distances);
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
                    Console.WriteLine(distance);
                }


                if (distance <  lastBest)
                {
                    
                    int randomIndex = generator.Next(population.Count);
                    population[randomIndex] = individual;
                }

            }
        }
        return bestTour;
    }
}
