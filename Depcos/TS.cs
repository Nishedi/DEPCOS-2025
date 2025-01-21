using Depcos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

public class TS
{
    private void Reverse(List<Customer> list, int start, int end)
    {
        while (start < end)
        {
            (list[start], list[end]) = (list[end], list[start]);
            start++;
            end--;
        }
    }

    public List<Neighbor> GenerateNeighbor1_X(double[,] distances, List<Customer> GTR, int numberOfMaxMoves, int seed)
    {
        var random = new Random(seed);
        var neighborhood = new List<Neighbor>();
        //&& iter < 1000000

        for (int iter = 0; iter < numberOfMaxMoves ; iter++)
        {
            var vehicleStartTimes = new List<double>();
            int i = random.Next(GTR.Count-2)+1;
            int j;

            do
            {
                j = random.Next(GTR.Count - 2) + 1;
            } while (i == j);

            Customer valueToMove = GTR[i];
            var neighbor = new List<Customer>(GTR);
            neighbor.RemoveAt(i);
            neighbor.Insert(j, valueToMove);
            for(int x = 0; x < neighbor.Count;x++)
            {
                if (neighbor[x].Id == 0 && x+1<neighbor.Count)
                {
                    vehicleStartTimes.Add(Math.Max(neighbor[x + 1].bv - distances[neighbor[x].Id, neighbor[x + 1].Id],0));
                }
            }
            neighborhood.Add(new Neighbor(neighbor,vehicleStartTimes, i, j));
        }

        return neighborhood;
    }
    public List<Neighbor> GenerateNeighborInsert(double[,] distances, List<Customer> GTR, int numberOfMaxMoves, int seed)
    {
        var random = new Random(seed);
        var neighborhood = new List<Neighbor>();
        //&& iter < 1000000

        for (int i = 1; i < GTR.Count - 1; i++)
        {
            for(int j = 1; j < GTR.Count-1; j++)
            {
                if (i == j) continue;
                var vehicleStartTimes = new List<double>();
                Customer valueToMove = GTR[i];
                var neighbor = new List<Customer>(GTR);
                neighbor.RemoveAt(i);
                neighbor.Insert(j, valueToMove);
                for (int x = 0; x < neighbor.Count; x++)
                {
                    if (neighbor[x].Id == 0 && x + 1 < neighbor.Count)
                    {
                        vehicleStartTimes.Add(Math.Max(neighbor[x + 1].bv - distances[neighbor[x].Id, neighbor[x + 1].Id], 0));
                    }
                }
                neighborhood.Add(new Neighbor(neighbor, vehicleStartTimes, i, j));
            }
        }

        return neighborhood;
    }
    public List<Neighbor> GenerateNeighborSwap(double[,] distances, List<Customer> GTR, int numberOfMaxMoves, int seed)
    {
        var neighborhood = new List<Neighbor>();
        //&& iter < 1000000

        for (int i = 1; i < GTR.Count - 1; i++)
        {
            for (int j = 1; j < GTR.Count - 1; j++)
            {

                if (i == j) continue;
                var vehicleStartTimes = new List<double>();
                var neighbor = new List<Customer>(GTR);
                (neighbor[i], neighbor[j]) = (neighbor[j], neighbor[i]);
                for (int x = 0; x < neighbor.Count; x++)
                {
                    if (neighbor[x].Id == 0 && x + 1 < neighbor.Count)
                    {
                        vehicleStartTimes.Add(Math.Max(neighbor[x + 1].bv - distances[neighbor[x].Id, neighbor[x + 1].Id], 0));
                    }
                }
                neighborhood.Add(new Neighbor(neighbor, vehicleStartTimes, i, j));

            }
        }
        return neighborhood;
    }

    public List<Neighbor> GenerateNeighborReverse(double[,] distances, List<Customer> GTR, int numberOfMaxMoves, int seed)
    {
        var neighborhood = new List<Neighbor>();
        //&& iter < 1000000

        for (int i = 1; i < GTR.Count - 1; i++)
        {
            for (int j = 1; j < GTR.Count - 1; j++)
            {
                if (i == j) continue;
                var vehicleStartTimes = new List<double>();
                var neighbor = new List<Customer>(GTR);
                Reverse(neighbor, i, j);
                for (int x = 0; x < neighbor.Count; x++)
                {
                    if (neighbor[x].Id == 0 && x + 1 < neighbor.Count)
                    {
                        vehicleStartTimes.Add(Math.Max(neighbor[x + 1].bv - distances[neighbor[x].Id, neighbor[x + 1].Id], 0));
                    }
                }
                neighborhood.Add(new Neighbor(neighbor, vehicleStartTimes, i, j));
            }
        }
        return neighborhood;
    }

   
    public Result TabuSearch(double[,] distances, List<Customer> customers, List<Vehicle> vehicles, int tabuSize, int maxTime,  int neighborType)
    {
        var bestSolution = new List<Customer>();
        double bestSolutionLength = int.MaxValue;
        var gm = new GeneralMethods();
        var currentSolution = new List<Customer>();
        List<double> vehicleStarts = new List<double>();
        List<double> bestVehicleStarts = new List<double>();
        currentSolution = gm.createGreedyGTR(distances, customers, vehicleStarts, vehicles);
        double currentSolutionLength = gm.calculateCostGTRv2(distances, currentSolution, vehicleStarts);
        Console.WriteLine(currentSolutionLength);
        if (bestSolution == null || currentSolutionLength < bestSolutionLength)
        {
            bestSolution = currentSolution;
            bestSolutionLength = currentSolutionLength;
            bestVehicleStarts = vehicleStarts;
        }

        maxTime *= 1000;
        var tabuList = new List<Neighbor>();
        var stopwatch = Stopwatch.StartNew();
        currentSolution = bestSolution;
        while (stopwatch.ElapsedMilliseconds <= maxTime)
        {
            List<Neighbor> neighborhood;
            if (neighborType == 1)
                neighborhood = GenerateNeighborInsert(distances, currentSolution, currentSolution.Count, 1000);
            else if (neighborType == 2)
                neighborhood = GenerateNeighborSwap(distances, currentSolution, currentSolution.Count, 1000);
            else
                neighborhood = GenerateNeighborReverse(distances, currentSolution, currentSolution.Count,1000);

            neighborhood.Sort((a, b) => gm.calculateCostGTRv2(distances,a.Path,a.VehicleStarts).CompareTo(gm.calculateCostGTRv2(distances,b.Path,b.VehicleStarts)));
            foreach (var neighbor in neighborhood)
            {
                if (!tabuList.Contains(neighbor) || gm.calculateCostGTRv2(distances, neighbor.Path, neighbor.VehicleStarts) < bestSolutionLength)
                {
                    
                    currentSolution = neighbor.Path;
                    vehicleStarts = neighbor.VehicleStarts;
                    tabuList.Add(neighbor);
                    if (tabuList.Count > tabuSize)
                    {
                        tabuList.RemoveAt(0);
                    }
                    break;
                }
            }

            List<double> vehicleStartsTry = new List<double>();
            foreach(double d in vehicleStarts){
                vehicleStartsTry.Add(d);
            }
            for(int i = 0; i < vehicleStarts.Count; i++)
            {
                double bestValue = 0;
                for (int j = 0; j < 20; j++)
                {
                    double value = vehicleStarts[i];
                    double diffrence = value/10;
                    if (value < 10) diffrence = 1;
                    vehicleStartsTry[i] = diffrence * j;
                    
                    currentSolutionLength = gm.calculateCostGTRv2(distances, currentSolution, vehicleStartsTry);

                    if (currentSolutionLength < bestSolutionLength)
                    {
                        bestSolution = currentSolution;
                        bestSolutionLength = currentSolutionLength;
                        bestVehicleStarts.Clear();
                        foreach (double d in vehicleStartsTry)
                        {
                            bestVehicleStarts.Add(d);
                        }
                        bestValue = diffrence * j;
                    }
                }
                vehicleStartsTry[i] = bestValue;
            }
        }
        return new Result(bestSolution, bestVehicleStarts);
    }
}

public class Neighbor
{
    public int I { get; set; }
    public int J { get; set; }
    public List<Customer> Path { get; set; }
    public List<Double> VehicleStarts { get; set; }

    public Neighbor(List<Customer> path, List<Double> vehicleStarts, int i, int j)
    {
        Path = path;
        VehicleStarts = vehicleStarts;
        I = i;
        J = j;
    }

    public override bool Equals(object obj)
    {
        return obj is Neighbor neighbor &&
               I == neighbor.I &&
               J == neighbor.J ;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(I, J, Path);
    }
}
