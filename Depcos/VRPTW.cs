using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using Gurobi;
using Depcos;

public class VRPTW
{
    public List<Customer> Customers { get; private set; }
    public List<Vehicle> Vehicles { get; private set; }
    public List<Customer> InitialGTR { get; private set; }
    public int VehicleCapacity { get; private set; }
    public int NumberOfVehicles { get; private set; }
    public double[,] distanceMatrix { get; private set; }

    public List<double> vehicleStarts { get; private set; }

    public VRPTW(string filePath, int vehicleNumber)
    {
        Customers = new List<Customer>();
        Vehicles = new List<Vehicle>();
        ParseSolomonFile(filePath);
        // Adding set of vehicles
        for (int i = 0; i < vehicleNumber; i++)
        {
            Vehicles.Add(new Vehicle
            {
                id = i,
                wv = Customers[0].dv,
                time = 0
            });
        }
        // Adding central depot to customer list

        
        distanceMatrix = createDistanceMatrix();
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
                        Id = int.Parse(parts[0])-1,
                        X = double.Parse(parts[1], CultureInfo.InvariantCulture),
                        Y = double.Parse(parts[2], CultureInfo.InvariantCulture),
                        Demand = double.Parse(parts[3], CultureInfo.InvariantCulture),
                        bv = double.Parse(parts[4], CultureInfo.InvariantCulture),
                        dv = double.Parse(parts[5], CultureInfo.InvariantCulture),
                        ServiceTime = double.Parse(parts[6], CultureInfo.InvariantCulture),
                        penalty = 2
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
                    Id = int.Parse(parts[0])-1,
                    X = double.Parse(parts[1], CultureInfo.InvariantCulture),
                    Y = double.Parse(parts[2], CultureInfo.InvariantCulture),
                    Demand = double.Parse(parts[3], CultureInfo.InvariantCulture),
                    bv = double.Parse(parts[4], CultureInfo.InvariantCulture),
                    dv = double.Parse(parts[5], CultureInfo.InvariantCulture),
                    ServiceTime = double.Parse(parts[6], CultureInfo.InvariantCulture),
                    penalty = 2
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

    public void createInitialGTR()
    {
        InitialGTR = new List<Customer>();
        int vehicleNumber = 0;
        double vehicleTimeLimit = 0;
        double returnTime = 0; 
        double arrivalTime = 0;
        double windowExceed = 0;

        
        foreach (var customer in Customers)
        {
            if (customer.Id != 0)
            {
                vehicleTimeLimit = Vehicles[vehicleNumber].wv;
                returnTime = distanceMatrix[customer.Id, 0];
                arrivalTime = Vehicles[vehicleNumber].time + distanceMatrix[customer.Id - 1, customer.Id];
                windowExceed = new[] {customer.bv - arrivalTime, 0, arrivalTime - customer.dv + customer.ServiceTime}.Max();
                if (arrivalTime + customer.ServiceTime + returnTime + customer.penalty * windowExceed <= vehicleTimeLimit)
                {
                    if (InitialGTR.Count == 0)
                        InitialGTR.Add(Customers[0]);
                    InitialGTR.Add(customer);
                    Vehicles[vehicleNumber].time = arrivalTime + customer.ServiceTime + customer.penalty * windowExceed;
                    
                }
                else
                {
                    InitialGTR.Add(Customers[0]);
                    vehicleNumber++;
                    InitialGTR.Add(customer);
                    Vehicles[vehicleNumber].time = Vehicles[vehicleNumber].time + distanceMatrix[0, customer.Id] + customer.ServiceTime + customer.penalty * windowExceed;
                }
            }

        }
        InitialGTR.Add(Customers[0]);

    }

    public void createGreedyGTR()
    {
        InitialGTR = new List<Customer>();
        vehicleStarts = new List<double>();
        bool[] visited = new bool[Customers.Count];
        int vehicleNumber = 0;
        double vehicleTime = 0;

        // Każdy pojazd zaczyna z bazy
        InitialGTR.Add(Customers[0]);

        while (visited.Contains(false))
        {
            Customer current = Customers[0]; // Start z bazy
            visited[current.Id] = true;
            vehicleTime = 0;

            while (true)
            {
                Customer nextCustomer = null;
                double minDistance = double.MaxValue;

                // Szukaj najbliższego klienta, który spełnia warunki
                foreach (var customer in Customers)
                {
                    if (!visited[customer.Id] && customer.Id != 0)
                    {
                        double distance = distanceMatrix[current.Id, customer.Id]; // koszt przejazdu do punktu
                        double estimatedUpperTimeLeft = customer.dv - vehicleTime; // koszt przekroczenia okna z gory
                        double estimatedLowerTimeLeft = customer.bv - vehicleTime; // koszt przekroczenia okna z dolu
                        double estimatedPenalty = Math.Max(0, customer.ServiceTime - estimatedUpperTimeLeft);
                        estimatedPenalty += Math.Max(0, Math.Min(estimatedLowerTimeLeft, customer.ServiceTime));
                        distance += estimatedPenalty;
                        // Pomyslec o rozbudowaniu warunku krytycznego
                        if (distance < minDistance && distance + vehicleTime < Customers[0].dv)
                        {
                            minDistance = distance;
                            nextCustomer = customer;
                        }
                    }
                }

                // Jeśli nie ma dostępnych klientów, wróć do bazy
                if (nextCustomer == null && current.Id != 0||vehicleTime>= Customers[0].dv)
                {
                    InitialGTR.Add(Customers[0]); // Powrót do bazy
                    vehicleNumber++;
                    if (vehicleNumber >= Vehicles.Count)
                    {
                        return;
                    }
                    break;
                }
                else if (nextCustomer == null && current.Id == 0) // Jeżeli ścieżka jest pusta i nie znaleziono klienta który spełnia wymagania
                                                                  // np. nie da się zdążyć do końca okna czasowego
                                                                  // dodaj pierwsze nieodwiedzone miasto
                {
                    foreach (var customer in Customers)
                    {
                        if (!visited[customer.Id])
                        {
                            nextCustomer = customer;
                        }
                    }
                }

                if(current.Id == 0)
                {
                    vehicleTime = Math.Max(nextCustomer.bv - distanceMatrix[current.Id, nextCustomer.Id], 0);
                    vehicleStarts.Add(Math.Max(nextCustomer.bv - distanceMatrix[current.Id, nextCustomer.Id], 0.0));
                }
                vehicleTime += distanceMatrix[current.Id, nextCustomer.Id]; // Przesuń czas o czas podróży
                // Odwiedź wybranego klienta
                double upperTimeLeft = nextCustomer.dv - vehicleTime; // dodaktowy koszt za przekroczenie od gory okna
                double lowerTimeLeft = nextCustomer.bv - vehicleTime; // dodatkowy koszt za przekroczenie od dolu okna
                double penalty = Math.Max(0,nextCustomer.ServiceTime - upperTimeLeft); // naliczenie potencjalnej kary
                penalty += Math.Max(0,Math.Min(lowerTimeLeft,nextCustomer.ServiceTime));
                vehicleTime += nextCustomer.ServiceTime; // Wykonaj serwis
                vehicleTime += penalty;
                InitialGTR.Add(nextCustomer); // Odznacz punkt
                visited[nextCustomer.Id] = true;
                //Console.WriteLine(nextCustomer.Id + " " + vehicleTime);
                current = nextCustomer;
            }
        }

        // Zakończ trasę powrotem do bazy (jeśli ostatni pojazd jest w trasie)
        if (InitialGTR[InitialGTR.Count - 1].Id != 0)
        {
            InitialGTR.Add(Customers[0]);
        }
    }

    public double calculateCostGTRv2(List<Customer> GTR, List<Double> vehicleStartTimes)
    {
        double cost = 0;
        double vehicleTime = 0;
        Customer prevCustomer = null;
        int count = GTR.Count(customer => customer.Id == 0);
        int vehicleId = 0;
        foreach (Customer customer in GTR) {
            if (customer.Id == 0)
            {
                if(prevCustomer!= null)
                {
                    vehicleTime += distanceMatrix[prevCustomer.Id, GTR[0].Id];
                    cost += vehicleTime - vehicleStartTimes[vehicleId];
                    Console.WriteLine(vehicleTime - vehicleStartTimes[vehicleId]);
                    vehicleId++;
                }
                //Console.WriteLine(vehicleTime); // wyswietlenie kosztu dla trasy pojedynczych pojazdow
                
                
                
                if(vehicleId < vehicleStartTimes.Count)
                    vehicleTime = vehicleStartTimes[vehicleId];
                prevCustomer = customer;
            }
            else
            {
                vehicleTime += distanceMatrix[prevCustomer.Id, customer.Id];
                double upperTimeLeft = customer.dv - vehicleTime; // dodaktowy koszt za przekroczenie od gory okna
                double lowerTimeLeft = customer.bv - vehicleTime; // dodatkowy koszt za przekroczenie od dolu okna
                double penalty = Math.Max(0, customer.ServiceTime - upperTimeLeft); // naliczenie potencjalnej kary
                penalty += Math.Max(0, Math.Min(lowerTimeLeft, customer.ServiceTime));
                vehicleTime += customer.ServiceTime; // Wykonaj serwis
                vehicleTime += penalty;
                prevCustomer= customer;
            }
        }
        return cost;
    }

    public string printGTRDistances(List<Customer> GTR)
    {
        string GTRString = "";
        for (int i = 0; i < GTR.Count - 1; i++)
        {
            GTRString += distanceMatrix[GTR[i].Id, GTR[i + 1].Id].ToString() + "->";
        }
        return GTRString;
    }

    public string printGTR(List<Customer> GTR)
    {
        string GTRString = "";
        foreach (var node in GTR)
        {
            GTRString += node.Id.ToString() + " ";

        }
        return GTRString;

    }

    public string printDistanceMatrix()
    {
        string matrixString = "";
        for (int i = 0; i < distanceMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < distanceMatrix.GetLength(0); j++)
            {
                matrixString += distanceMatrix[i, j] + " ";
            }
            matrixString += "\n";
        }
        return matrixString;
    }


    public double calculateCostGTR(List<Customer> GTR)
    {
        double cost = 0;
        double travelTime = 0;
        double time = 0;
        double windowExceed = 0;
        for (int i = 1; i < GTR.Count; i++)
        { 
            travelTime = distanceMatrix[GTR[i - 1].Id, GTR[i].Id];
            time += travelTime;
            if (GTR[i].Id == 0)
            {
                cost = cost + time;
                time = 0;
            }
            else
            {
                windowExceed = new[] { GTR[i].bv - time, 0, time - GTR[i].dv + GTR[i].ServiceTime }.Max();
                time += GTR[i].penalty * windowExceed;
                time += GTR[i].ServiceTime;
            }
        }
        return cost;

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
