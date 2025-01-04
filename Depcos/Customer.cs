using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using Gurobi;

public class Customer
{
    public int Id { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Demand { get; set; }
    public double bv { get; set; }
    public double dv { get; set; }
    public double ServiceTime { get; set; }

    public override string ToString()
    {
        return $"Customer [Id={Id}, X={X.ToString(CultureInfo.InvariantCulture)}, Y={Y.ToString(CultureInfo.InvariantCulture)}, " +
               $"Demand={Demand.ToString(CultureInfo.InvariantCulture)}, bv={bv.ToString(CultureInfo.InvariantCulture)}, " +
               $"dv={dv.ToString(CultureInfo.InvariantCulture)}, ServiceTime={ServiceTime.ToString(CultureInfo.InvariantCulture)}]";
    }
}
