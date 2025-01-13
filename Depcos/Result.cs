using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Result
{
    public List<Customer> GTR { get; set; }
    public List<double> VehicleStartTimes { get; set; }
    public Result(List<Customer> gtr, List<double> vehicleStartTimes)
    {
        GTR = gtr;
        VehicleStartTimes = vehicleStartTimes;
    }
}
