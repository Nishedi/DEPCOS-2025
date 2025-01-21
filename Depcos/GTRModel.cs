using Gurobi;
using System;
using System.Collections.Generic;

public class GTRModel
{
    public double CalculateCostGTRv2(double[,] distanceMatrix, List<Customer> GTR, List<Double> vehicleStartTimes)
    {
        try
        {
            // Inicjalizacja modelu Gurobi
            using (var env = new GRBEnv(true))
            {
                env.Start();
                using (var model = new GRBModel(env))
                {
                    int numCustomers = GTR.Count;
                    int numVehicles = vehicleStartTimes.Count;

                    // Zmienne decyzyjne
                    GRBVar[,] x = new GRBVar[numCustomers, numCustomers];
                    GRBVar[] t = new GRBVar[numCustomers];

                    // Zmienna binarna x[i, j] dla każdej pary klientów
                    for (int i = 0; i < numCustomers; i++)
                    {
                        for (int j = 0; j < numCustomers; j++)
                        {
                            if (i != j)
                            {
                                x[i, j] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, $"x_{i}_{j}");
                            }
                        }
                    }

                    // Zmienna t[i] dla czasów przyjazdu do klienta i
                    for (int i = 0; i < numCustomers; i++)
                    {
                        t[i] = model.AddVar(0.0, GRB.INFINITY, 0.0, GRB.CONTINUOUS, $"t_{i}");
                    }

                    // Ograniczenie: każdy klient musi być odwiedzony dokładnie raz
                    for (int i = 0; i < numCustomers; i++)
                    {
                        GRBLinExpr lhs = 0;
                        for (int j = 0; j < numCustomers; j++)
                        {
                            if (i != j)
                            {
                                lhs += x[i, j];
                            }
                        }
                        model.AddConstr(lhs == 1, $"visit_once_{i}");
                    }

                    // Ograniczenie: każdy pojazd może wyjechać z dokładnie jednego klienta
                    for (int j = 0; j < numCustomers; j++)
                    {
                        GRBLinExpr lhs = 0;
                        for (int i = 0; i < numCustomers; i++)
                        {
                            if (i != j)
                            {
                                lhs += x[i, j];
                            }
                        }
                        model.AddConstr(lhs == 1, $"depart_once_{j}");
                    }

                    // Czas przyjazdu ograniczenie (t[i] + czas przejazdu <= t[j] + M * (1 - x[i, j]))
                    double M = 1000000.0; // duża liczba, na przykład 1e6
                    for (int i = 0; i < numCustomers; i++)
                    {
                        for (int j = 0; j < numCustomers; j++)
                        {
                            if (i != j)
                            {
                                model.AddConstr(t[i] + distanceMatrix[i, j] <= t[j] + M * (1 - x[i, j]), $"time_constraint_{i}_{j}");
                            }
                        }
                    }

                    // Ograniczenia czasów dla każdego pojazdu, biorąc pod uwagę start czasów pojazdów
                    for (int i = 0; i < numCustomers; i++)
                    {
                        model.AddConstr(t[i] >= vehicleStartTimes[i], $"vehicle_start_{i}");
                    }

                    // Funkcja celu: minimalizacja kosztów przejazdów i kar
                    GRBLinExpr objective = 0;
                    for (int i = 0; i < numCustomers; i++)
                    {
                        for (int j = 0; j < numCustomers; j++)
                        {
                            if (i != j)
                            {
                                objective += distanceMatrix[i, j] * x[i, j];
                            }
                        }
                    }

                    // Dodanie funkcji celu do modelu
                    model.SetObjective(objective, GRB.MINIMIZE);

                    // Optymalizacja
                    model.Optimize();

                    // Wyniki
                    if (model.Status == GRB.Status.OPTIMAL)
                    {
                        double totalCost = model.ObjVal;
                        Console.WriteLine("Optymalny koszt: " + totalCost);
                        return totalCost;
                    }
                    else
                    {
                        Console.WriteLine("Brak rozwiązania optymalnego");
                        return double.MaxValue;
                    }
                }
            }
        }
        catch (GRBException e)
        {
            Console.WriteLine("Gurobi error: " + e.Message);
            return double.MaxValue;
        }
    }
}
