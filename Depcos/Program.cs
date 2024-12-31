public class Program
{
    public static void Main()
    {
        string filePath = "C101.txt"; 
        VRPTW vrptw = new VRPTW(filePath);
        vrptw.Solve();
    }
}
