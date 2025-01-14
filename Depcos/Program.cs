public class Program
{
    public static void Main()
    {
        string filePath = "C101.txt";
        //string filePath = "CTEST.txt";
        VRPTW vrptw = new VRPTW(filePath, 101);
 

        TS tabuSearch = new TS();
        Result rs = tabuSearch.TabuSearch(vrptw.distanceMatrix, vrptw.Customers, vrptw.Vehicles, 100, 200, 1);
        GeneralMethods gm = new GeneralMethods();
        Console.WriteLine(gm.calculateCostGTRv2(vrptw.distanceMatrix, rs.GTR, rs.VehicleStartTimes));
        /*rs = tabuSearch.TabuSearch(vrptw.distanceMatrix, vrptw.Customers, vrptw.Vehicles, 100, 20, 2);
        Console.WriteLine(gm.calculateCostGTRv2(vrptw.distanceMatrix, rs.GTR, rs.VehicleStartTimes));
        rs = tabuSearch.TabuSearch(vrptw.distanceMatrix, vrptw.Customers, vrptw.Vehicles, 100, 20, 3);
        Console.WriteLine(gm.calculateCostGTRv2(vrptw.distanceMatrix, rs.GTR, rs.VehicleStartTimes));*/
        Console.WriteLine(vrptw.printGTR(rs.GTR));
    }
}
