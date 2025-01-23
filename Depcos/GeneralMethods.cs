using Depcos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GeneralMethods
{
    Random random = new Random();
    public List<Customer> createGreedyGTR(double[,] distanceMatrix, List<Customer> Customers, List<double> vehicleStarts, List<Vehicle> Vehicles)
    {
        var InitialGTR = new List<Customer>();
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
                if (nextCustomer == null && current.Id != 0 || vehicleTime >= Customers[0].dv)
                {
                    InitialGTR.Add(Customers[0]); // Powrót do bazy
                    vehicleNumber++;
                    if (vehicleNumber >= Vehicles.Count)
                    {
                        return InitialGTR;
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

                if (current.Id == 0)
                {
                    vehicleTime = Math.Max(nextCustomer.bv - distanceMatrix[current.Id, nextCustomer.Id], 0);
                    vehicleStarts.Add(Math.Max(nextCustomer.bv - distanceMatrix[current.Id, nextCustomer.Id], 0.0));
                }
                vehicleTime += distanceMatrix[current.Id, nextCustomer.Id]; // Przesuń czas o czas podróży
                // Odwiedź wybranego klienta
                double upperTimeLeft = nextCustomer.dv - vehicleTime; // dodaktowy koszt za przekroczenie od gory okna
                double lowerTimeLeft = nextCustomer.bv - vehicleTime; // dodatkowy koszt za przekroczenie od dolu okna
                double penalty = Math.Max(0, nextCustomer.ServiceTime - upperTimeLeft); // naliczenie potencjalnej kary
                penalty += Math.Max(0, Math.Min(lowerTimeLeft, nextCustomer.ServiceTime));
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

        return InitialGTR;
    }

    public List<Customer> createGreedyGTRMutated(double[,] distanceMatrix, List<Customer> Customers, List<double> vehicleStarts, List<Vehicle> Vehicles)
    {
        List<Customer> InitialGTR = createGreedyGTR(distanceMatrix, Customers, vehicleStarts, Vehicles);
        vehicleStarts.Clear();
        int i = random.Next(1, distanceMatrix.GetLength(0));
        int j = i;
        while (i == j)
        {
            j = random.Next(1, distanceMatrix.GetLength(0));
        }
        var toSwap = InitialGTR[i];
        InitialGTR[i] = InitialGTR[j];
        InitialGTR[j] = toSwap;
        for (i = 0; i < InitialGTR.Count()-1; i++)
        {
            var customer = InitialGTR[i];
            var nextCustomer = InitialGTR[i+1];
            if (customer.Id == 0)
            {
                vehicleStarts.Add(Math.Max(nextCustomer.bv - distanceMatrix[customer.Id, nextCustomer.Id], 0.0));
            }
        }
        return InitialGTR;
    }

    public double calculateCostGTRv2(double[,] distanceMatrix, List<Customer> GTR, List<Double> vehicleStartTimes)
    {
        double cost = 0;
        double vehicleTime = 0;
        Customer prevCustomer = null;
        int count = GTR.Count(customer => customer.Id == 0);
        int vehicleId = 0;
        foreach (Customer customer in GTR)
        {
            if (customer.Id == 0)
            {
                if (prevCustomer != null)
                {
                    vehicleTime += distanceMatrix[prevCustomer.Id, GTR[0].Id];
                    cost += vehicleTime - vehicleStartTimes[vehicleId];
                    vehicleId++;
                }
                //Console.WriteLine(vehicleTime); // wyswietlenie kosztu dla trasy pojedynczych pojazdow
                if (vehicleId < vehicleStartTimes.Count)
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
                prevCustomer = customer;
            }
        }
        return cost;
    }
}
