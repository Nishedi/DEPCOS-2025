using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depcos
{
    public class Vehicle
    {
        public int id {get; set;}
        // public double bv { get; set; } //vehicle starting time - Deprecated
        public double wv { get; set; } //vehicle working time
        public double time { get; set; } // stores total time of routes and services
    }
}
