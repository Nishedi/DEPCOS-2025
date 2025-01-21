using System.Reflection;

public class Program
{
    public static void Main()
    {
        string filePath = "CTEST.txt";
        //string filePath = "CTEST.txt";
        VRPTW vrptw = null;
 

        TS tabuSearch = new TS();
        Result rs = null;


        vrptw = new VRPTW(filePath, 10);
        //rs = tabuSearch.TabuSearch(vrptw.distanceMatrix, vrptw.Customers, vrptw.Vehicles, 100, 120, 1);
        //GeneralMethods gm = new GeneralMethods();
        //Console.WriteLine(gm.calculateCostGTRv2(vrptw.distanceMatrix, rs.GTR, rs.VehicleStartTimes));
        //Console.WriteLine(vrptw.printGTR(rs.GTR));
        //Console.WriteLine();


        GurobiVRP gurobi1 = new GurobiVRP();        
        double fCelu = gurobi1.gurobi_test(vrptw);
        Console.WriteLine("funkcja celu: ");
        Console.WriteLine(fCelu);
        //gurobi1.printSubSets();
    }
}
