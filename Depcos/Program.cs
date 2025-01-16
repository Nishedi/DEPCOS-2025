public class Program
{
    public static void Main()
    {
        string filePath = "C101.txt";
        //string filePath = "CTEST.txt";
        VRPTW vrptw = null;
 

        TS tabuSearch = new TS();
        Result rs = null;


        //vrptw = new VRPTW(filePath, 101);
        //rs = tabuSearch.TabuSearch(vrptw.distanceMatrix, vrptw.Customers, vrptw.Vehicles, 100, 120, 1);
        //GeneralMethods gm = new GeneralMethods();
        //Console.WriteLine(gm.calculateCostGTRv2(vrptw.distanceMatrix, rs.GTR, rs.VehicleStartTimes));
        //Console.WriteLine(vrptw.printGTR(rs.GTR));
        //Console.WriteLine();

        GurobiVRP gurobi1 = new GurobiVRP();
        gurobi1.gurobi_test();
    }
}
