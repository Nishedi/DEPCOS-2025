public class Program
{
    public static void Main()
    {
        //string filePath = "C101.txt";
        string filePath = "CTEST.txt";
        VRPTW vrptw = new VRPTW(filePath, 101);
        /*vrptw.Solve();*/
        vrptw.createInitialGTR();
        //Console.WriteLine(vrptw.printDistanceMatrix());

        Console.WriteLine(vrptw.printGTR(vrptw.InitialGTR));
        Console.WriteLine(vrptw.calculateCostGTR(vrptw.InitialGTR));
        vrptw.createGreedyGTR();
        //Console.WriteLine(vrptw.printDistanceMatrix());

        Console.WriteLine(vrptw.printGTR(vrptw.InitialGTR));
        //Console.WriteLine(vrptw.printGTRDistances(vrptw.InitialGTR));
        Console.WriteLine(vrptw.calculateCostGTR(vrptw.InitialGTR));
    }
}
