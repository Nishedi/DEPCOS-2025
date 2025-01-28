using System.Reflection;

public class Program
{
    static string[] fileNames = { "100 lokacji/C101.txt", "100 lokacji/C102.txt", "100 lokacji/C103.txt", 
        "100 lokacji/C104.txt", "100 lokacji/C105.txt", "100 lokacji/C106.txt", "100 lokacji/C107.txt", 
        "100 lokacji/C108.txt", "100 lokacji/C109.txt", "100 lokacji/C201.txt", "100 lokacji/C202.txt", 
        "100 lokacji/C203.txt", "100 lokacji/C204.txt", "100 lokacji/C205.txt", "100 lokacji/C206.txt", 
        "100 lokacji/C207.txt", "100 lokacji/C208.txt", "100 lokacji/R101.txt", "100 lokacji/R102.txt", 
        "100 lokacji/R103.txt", "100 lokacji/R104.txt", "100 lokacji/R105.txt", "100 lokacji/R106.txt", 
        "100 lokacji/R107.txt", "100 lokacji/R108.txt", "100 lokacji/R109.txt", "100 lokacji/R110.txt", 
        "100 lokacji/R111.txt", "100 lokacji/R112.txt", "100 lokacji/R201.txt", "100 lokacji/R202.txt", 
        "100 lokacji/R203.txt", "100 lokacji/R204.txt", "100 lokacji/R205.txt", "100 lokacji/R206.txt", 
        "100 lokacji/R207.txt", "100 lokacji/R208.txt", "100 lokacji/R209.txt", "100 lokacji/R210.txt", 
        "100 lokacji/R211.txt", "100 lokacji/RC101.txt", "100 lokacji/RC102.txt", "100 lokacji/RC103.txt", 
        "100 lokacji/RC104.txt", "100 lokacji/RC105.txt", "100 lokacji/RC106.txt", "100 lokacji/RC107.txt", 
        "100 lokacji/RC108.txt", "100 lokacji/RC201.txt", "100 lokacji/RC202.txt", "100 lokacji/RC203.txt", 
        "100 lokacji/RC204.txt", "100 lokacji/RC205.txt", "100 lokacji/RC206.txt", "100 lokacji/RC207.txt", "100 lokacji/RC208.txt",
        "200 lokacji/C1_10_1_o.TXT", "200 lokacji/C1_2_10_o.TXT", "200 lokacji/C1_2_2_o.TXT", "200 lokacji/C1_2_3_o.TXT", 
        "200 lokacji/C1_2_4_o.TXT", "200 lokacji/C1_2_5_o.TXT", "200 lokacji/C1_2_6_o.TXT", "200 lokacji/C1_2_7_o.TXT", 
        "200 lokacji/C1_2_8_o.TXT", "200 lokacji/C1_2_9_o.TXT", "200 lokacji/C2_10_1_o.TXT", "200 lokacji/C2_2_10_o.TXT", 
        "200 lokacji/C2_2_1_o.TXT", "200 lokacji/C2_2_2_o.TXT", "200 lokacji/C2_2_3_o.TXT", "200 lokacji/C2_2_4_o.TXT", 
        "200 lokacji/C2_2_5_o.TXT", "200 lokacji/C2_2_6_o.TXT", "200 lokacji/C2_2_7_o.TXT", "200 lokacji/C2_2_8_o.TXT", 
        "200 lokacji/C2_2_9_o.TXT", "200 lokacji/R1_10_1_o.TXT", "200 lokacji/R1_2_10_o.TXT", "200 lokacji/R1_2_1_o.TXT",
        "200 lokacji/R1_2_2_o.TXT", "200 lokacji/R1_2_3_o.TXT", "200 lokacji/R1_2_4_o.TXT", "200 lokacji/R1_2_5_o.TXT", 
        "200 lokacji/R1_2_6_o.TXT", "200 lokacji/R1_2_7_o.TXT", "200 lokacji/R1_2_8_o.TXT", "200 lokacji/R1_2_9_o.TXT", 
        "200 lokacji/R2_10_1_o.TXT", "200 lokacji/R2_2_10_o.TXT", "200 lokacji/R2_2_1_o.TXT", "200 lokacji/R2_2_2_o.TXT", 
        "200 lokacji/R2_2_3_o.TXT", "200 lokacji/R2_2_4_o.TXT", "200 lokacji/R2_2_5_o.TXT", "200 lokacji/R2_2_6_o.TXT", 
        "200 lokacji/R2_2_7_o.TXT", "200 lokacji/R2_2_8_o.TXT", "200 lokacji/R2_2_9_o.TXT", "200 lokacji/RC1_10_1_o.TXT", 
        "200 lokacji/RC1_2_10_o.TXT", "200 lokacji/RC1_2_1_o.TXT", "200 lokacji/RC1_2_2_o.TXT", "200 lokacji/RC1_2_3_o.TXT",
        "200 lokacji/RC1_2_4_o.TXT", "200 lokacji/RC1_2_5_o.TXT", "200 lokacji/RC1_2_6_o.TXT", "200 lokacji/RC1_2_7_o.TXT", 
        "200 lokacji/RC1_2_8_o.TXT", "200 lokacji/RC1_2_9_o.TXT", "200 lokacji/RC2_10_1_o.TXT", "200 lokacji/RC2_2_10_o.TXT", 
        "200 lokacji/RC2_2_1_o.TXT", "200 lokacji/RC2_2_2_o.TXT", "200 lokacji/RC2_2_3_o.TXT", "200 lokacji/RC2_2_4_o.TXT", 
        "200 lokacji/RC2_2_5_o.TXT", "200 lokacji/RC2_2_6_o.TXT", "200 lokacji/RC2_2_7_o.TXT", "200 lokacji/RC2_2_8_o.TXT", "200 lokacji/RC2_2_9_o.TXT"
    };

    public static void Main()
    {
        //string filePath = "C101.txt";
        //string filePath = "CTEST.txt";
        string filePath = fileNames[0];
        VRPTW vrptw = null;

        
        TS tabuSearch = new TS();
        Genetic genetic = new Genetic();
        Result rs = null;
       
        GeneralMethods gm = new GeneralMethods();
        vrptw = new VRPTW(filePath, 5);

        rs = tabuSearch.TabuSearch(vrptw.distanceMatrix, vrptw.Customers, vrptw.Vehicles, 10, 10, 1);
        Console.WriteLine(gm.calculateCostGTRv2(vrptw.distanceMatrix, rs.GTR, rs.VehicleStartTimes));
        Console.WriteLine(vrptw.printGTR(rs.GTR));
        Console.WriteLine();

        GurobiVRP gurobi1 = new GurobiVRP();
        double fCelu = gurobi1.gurobi_test(vrptw);
        Console.WriteLine("funkcja celu: ");
        Console.WriteLine(fCelu);
        //gurobi1.printSubSets();
    }
}
