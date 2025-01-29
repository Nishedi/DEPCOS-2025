using Depcos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Result
{
    public List<Customer> GTR { get; set; }
    public List<double> VehicleStartTimes { get; set; }
    public double GreedyResult = int.MaxValue;
    public Result(List<Customer> gtr, List<double> vehicleStartTimes)
    {
        GTR = gtr;
        VehicleStartTimes = vehicleStartTimes;
    }
    public Result(List<Customer> gtr, List<double> vehicleStartTimes, double greedyResult)
    {
        GTR = gtr;
        VehicleStartTimes = vehicleStartTimes;
        GreedyResult = greedyResult;
    }
    public void generateGreedyTimes(double[,] distanceMatrix)
    {
        VehicleStartTimes = new List<double>();
        for (int i = 0; i < GTR.Count() - 1; i++)
        {
            var customer = GTR[i];
            var nextCustomer = GTR[i + 1];
            if (customer.Id == 0)
            {
                VehicleStartTimes.Add(Math.Max(nextCustomer.bv - distanceMatrix[customer.Id, nextCustomer.Id], 0.0));
            }
        }
    }
}
