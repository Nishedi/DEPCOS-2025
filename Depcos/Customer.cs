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
}
