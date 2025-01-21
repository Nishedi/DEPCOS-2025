using System.Reflection;

public class Program
{
    public static void Main()
    {
        string filePath = "C101.txt";
        //string filePath = "CTEST.txt";
        VRPTW vrptw = null;
 

        TS tabuSearch = new TS();
        Genetic genetic = new Genetic();
        Result rs = null;

        GeneralMethods gm = new GeneralMethods();
        vrptw = new VRPTW(filePath, 100);
        /*rs = tabuSearch.TabuSearch(vrptw.distanceMatrix, vrptw.Customers, vrptw.Vehicles, 100, 100, 1);
        
        Console.WriteLine(gm.calculateCostGTRv2(vrptw.distanceMatrix, rs.GTR, rs.VehicleStartTimes));
        Console.WriteLine(vrptw.printGTR(rs.GTR));
        Console.WriteLine();*/

        rs = genetic.GeneticSolve(vrptw.distanceMatrix, vrptw.Customers, vrptw.Vehicles, 10);
        Console.WriteLine(gm.calculateCostGTRv2(vrptw.distanceMatrix, rs.GTR, rs.VehicleStartTimes));
        Console.WriteLine(vrptw.printGTR(rs.GTR));
        Console.WriteLine();


        /*GurobiVRP gurobi1 = new GurobiVRP();        
        double fCelu = gurobi1.gurobi_test(vrptw);
        Console.WriteLine("funkcja celu: ");
        Console.WriteLine(fCelu);*/
        //gurobi1.printSubSets();
    }
}
