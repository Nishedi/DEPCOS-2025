using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using Gurobi;

public class VRPTW
{
    public List<Customer> Customers { get; private set; }
    public int VehicleCapacity { get; private set; }
    public int NumberOfVehicles { get; private set; }

    public VRPTW(string filePath)
    {
        Customers = new List<Customer>();
        ParseSolomonFile(filePath);
        createDistanceMatrix();
    }

    private void ParseSolomonFileWithDetails(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);
        bool customerSection = false;
        bool vechicleSection = false;

        foreach (var line in lines)//plik solomona z dodatkowymi danymi - liczba pojazdow - moze sie przyda
        {
            string trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine)) continue;

            if (trimmedLine.StartsWith("VEHICLE"))
            {
                continue;
            }
            else if (trimmedLine.StartsWith("NUMBER"))
            {
                vechicleSection = true;
                continue;
            }
            else if (vechicleSection)
            {
                NumberOfVehicles = int.Parse(trimmedLine);
                vechicleSection=false;
            }
            else if (trimmedLine.StartsWith("CUSTOMER"))
            {
                customerSection = true;
                continue;
            }
            else if (customerSection)
            {
                var parts = trimmedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 7) continue;

                try
                {
                    Customers.Add(new Customer
                    {
                        Id = int.Parse(parts[0]),
                        X = double.Parse(parts[1], CultureInfo.InvariantCulture),
                        Y = double.Parse(parts[2], CultureInfo.InvariantCulture),
                        Demand = double.Parse(parts[3], CultureInfo.InvariantCulture),
                        bv = double.Parse(parts[4], CultureInfo.InvariantCulture),
                        dv = double.Parse(parts[5], CultureInfo.InvariantCulture),
                        ServiceTime = double.Parse(parts[6], CultureInfo.InvariantCulture)
                    });
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"Błąd parsowania linii: {trimmedLine}. Szczegóły: {ex.Message}");
                }
            }
        }
    }

    private void ParseSolomonFile(string filePath)//typowy plik solomona
    {
        string[] lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            string trimmedLine = line.Trim();
            var parts = trimmedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 7) continue;

            try
            {
                Customers.Add(new Customer
                {
                    Id = int.Parse(parts[0]),
                    X = double.Parse(parts[1], CultureInfo.InvariantCulture),
                    Y = double.Parse(parts[2], CultureInfo.InvariantCulture),
                    Demand = double.Parse(parts[3], CultureInfo.InvariantCulture),
                    bv = double.Parse(parts[4], CultureInfo.InvariantCulture),
                    dv = double.Parse(parts[5], CultureInfo.InvariantCulture),
                    ServiceTime = double.Parse(parts[6], CultureInfo.InvariantCulture)
                });
                
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Błąd parsowania linii: {trimmedLine}. Szczegóły: {ex.Message}");
            }
        }
    }

    public double[,] createDistanceMatrix() // funkcja generująca macierz odleglosci
    {
        double[,] distanceMatrix = new double[Customers.Count, Customers.Count];
        for(int i = 0; i < Customers.Count; i++)
        {
            for(int j = 0; j < Customers.Count; j++)
            {
                double deltaX = Customers[j].X - Customers[i].X;
                double deltaY = Customers[j].Y - Customers[i].Y;
                double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY); 
                distanceMatrix[i, j] = distance;
            }
        }
        return distanceMatrix;
    }

    public void Solve() // raczej do usuniecia
    {
        try
        {
            double M = 10000;

            using (GRBEnv env = new GRBEnv(true))
            {
                env.Start();
                using (GRBModel model = new GRBModel(env))
                {
                    int n = Customers.Count;
                    int r = 10; 

                    GRBVar[,,] x = new GRBVar[NumberOfVehicles, r, r]; 
                    GRBVar[,] y = new GRBVar[NumberOfVehicles, r];

                    for (int v = 0; v < NumberOfVehicles; v++)
                    {
                        for (int l = 0; l < r; l++)
                        {
                            for (int k = 0; k < r; k++)
                            {
                                if (l != k)
                                    x[v, l, k] = model.AddVar(0, 1, 0, GRB.BINARY, $"x_{v}_{l}_{k}");
                            }
                        }
                    }

                    for (int v = 0; v < NumberOfVehicles; v++)
                    {
                        for (int k = 0; k < r; k++)
                        {
                            y[v, k] = model.AddVar(0, 1, 0, GRB.BINARY, $"y_{v}_{k}");
                        }
                    }

                    GRBLinExpr objective = 0;
                    for (int v = 0; v < NumberOfVehicles; v++)
                    {
                        for (int l = 0; l < r; l++)
                        {
                            for (int k = 0; k < r; k++)
                            {
                                if (l != k)
                                {
                                    double distance = Math.Sqrt(Math.Pow(Customers[l].X - Customers[k].X, 2) + Math.Pow(Customers[l].Y - Customers[k].Y, 2));
                                    objective += x[v, l, k] * distance;
                                }
                            }
                        }
                    }

                    for (int v = 0; v < NumberOfVehicles; v++)
                    {
                        for (int k = 0; k < r; k++)
                        {
                            double penalty = Math.Max(Customers[k].dv - Customers[v].Demand, 0);
                            objective += y[v, k] * penalty;
                        }
                    }

                    model.SetObjective(objective, GRB.MINIMIZE);

                   
                    for (int v = 0; v < NumberOfVehicles; v++)
                    {
                        for (int k = 0; k < r; k++)
                        {
                            GRBLinExpr constraint1 = 0;
                            for (int l = 0; l < r; l++)
                            {
                                if (l != k)
                                    constraint1 += x[v, l, k];
                            }
                            model.AddConstr(constraint1 + y[v, k] <= 1, $"c1_{v}_{k}");
                        }
                    }

                    for (int k = 0; k < r; k++)
                    {
                        GRBLinExpr sumY = 0;
                        for (int v = 0; v < NumberOfVehicles; v++)
                        {
                            sumY += y[v, k];
                        }
                        model.AddConstr(sumY == 1, $"c2_{k}");
                    }

                    model.Optimize();

                    if (model.Status == GRB.Status.INFEASIBLE)
                    {
                        model.ComputeIIS();
                        model.Write("model.ilp");
                        Console.WriteLine("Model jest niespełnialny. Szczegóły w model.ilp");
                    }
                    else if (model.Status == GRB.Status.OPTIMAL)
                    {
                        Console.WriteLine("Optymalne trasy:");
                        for (int v = 0; v < NumberOfVehicles; v++)
                        {
                            for (int l = 0; l < r; l++)
                            {
                                for (int k = 0; k < r; k++)
                                {
                                    if (l != k && x[v, l, k].X > 0.5)
                                    {
                                        Console.WriteLine($"Trasa: {l} -> {k} (Vehicle {v})");
                                    }
                                }
                            }
                        }
                        Console.WriteLine($"Całkowity koszt: {model.ObjVal}");
                    }
                }
            }
        }
        catch (GRBException e)
        {
            Console.WriteLine($"Błąd Gurobi: {e.Message}");
        }
    }
}
