using System.Reflection;
using System.Text.RegularExpressions;

public class Program
{
    static string[] fileNames = { 
        "100 lokacji/C101.txt", "100 lokacji/C102.txt", "100 lokacji/C103.txt", 
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
        /*"200 lokacji/C1_10_1_o.TXT", "200 lokacji/C1_2_10_o.TXT", "200 lokacji/C1_2_2_o.TXT", "200 lokacji/C1_2_3_o.TXT", 
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
        "200 lokacji/RC2_2_5_o.TXT", "200 lokacji/RC2_2_6_o.TXT", "200 lokacji/RC2_2_7_o.TXT", "200 lokacji/RC2_2_8_o.TXT", "200 lokacji/RC2_2_9_o.TXT"*/
    };
    static string GetDescription(string key)
    {
        return key switch
        {
            "C1" => "0,0",
            "C2" => "0,1",
            "R1" => "1,0",
            "R2" => "1,1",
            "RC1" => "2,0",
            "RC2" => "2,1",
            _ => "3,3"
        };
    }
    static void SaveToCsv(string fileName, string header, string values, bool isGurobi)
    {
        
        string directoryPath = "results";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        
        string fullFilePath = Path.Combine(directoryPath, fileName.Split(".")[0] + "_results.csv");
       
        if (File.Exists(fullFilePath) && isGurobi)
        {
            File.Delete(fullFilePath);
        }
        bool fileExists = File.Exists(fullFilePath);
        

        using (StreamWriter writer = new StreamWriter(fullFilePath, append: true))
        {
            if (!fileExists) // Jeśli plik nie istnieje, zapisz nagłówek
            {
                writer.WriteLine(header);
            }
            writer.Write(values);
        }

        Console.WriteLine($"Zapisano do pliku: {fullFilePath}");
    }

    public static void runGurobi(VRPTW vrptw, string filePath)
    {
        string fileName = filePath.Split("/")[1];
        Regex regex = new Regex(@"(RC|R|C)(\d+)", RegexOptions.Compiled);


        Match match = regex.Match(fileName);
        if (match.Success)
        {
            string key = match.Groups[1].Value + match.Groups[2].Value[0]; // Pobiera np. "RC1", "R2", "C2"
            TextWriter originalConsoleError = Console.Error;
            Console.SetError(TextWriter.Null);
            GurobiVRP gurobi = new GurobiVRP();
            var gurobiResult = gurobi.gurobi_test(vrptw);
            Console.SetError(originalConsoleError);
            /* Console.WriteLine("funkcja celu: ");
             Console.WriteLine(gurobiResult.Item1);
             Console.WriteLine("czas obliczen w sekundach: ");
             Console.WriteLine(gurobiResult.Item2);*/

            String header = "file_name,location_type,window_type,gurobi_result,greedy_result,tabu_result";
            String values = fileName + "," + GetDescription(key) + "," + gurobiResult.Item1 + ",";
            SaveToCsv(fileName, header, values, true);
        }

        //gurobi1.printSubSets();
    }

    public static void runTabuSearch(VRPTW vrptw, string filePath)
    {
        string fileName = filePath.Split("/")[1];
        GeneralMethods gm = new GeneralMethods();
        TS tabuSearch = new TS();

        Result rs = tabuSearch.TabuSearch(vrptw.distanceMatrix, vrptw.Customers, vrptw.Vehicles, 10, 60, 1);
        /*Console.WriteLine(gm.calculateCostGTRv2(vrptw.distanceMatrix, rs.GTR, rs.VehicleStartTimes));
        Console.WriteLine(vrptw.printGTR(rs.GTR));
        Console.WriteLine();*/
        String values = rs.GreedyResult + "," + gm.calculateCostGTRv2(vrptw.distanceMatrix, rs.GTR, rs.VehicleStartTimes);
        SaveToCsv(fileName, "", values, false);

    }

    public static void Main()
    {


        /*string filePath = "100 lokacji/C101.txt";
        int numOfvehicles = 60;
        VRPTW vrptw = new VRPTW(filePath, numOfvehicles/2);

        runGurobi(vrptw, filePath);


        runTabuSearch(vrptw, filePath);*/
        int numOfvehicles = 10;
        for (int i = 0; i < fileNames.Length; i++)
        {
            string filePath = fileNames[i];
            VRPTW vrptw = new VRPTW(filePath, numOfvehicles);

            runGurobi(vrptw, filePath);

        }


        Parallel.ForEach(fileNames, (filePath) =>
        {
            VRPTW vrptw = new VRPTW(filePath, numOfvehicles);
            runTabuSearch(vrptw, filePath);
        });

       /* for (int i = 0; i < fileNames.Length; i++)
        {
            string filePath = fileNames[i];
            VRPTW vrptw = new VRPTW(filePath, numOfvehicles);

            runTabuSearch(vrptw, filePath);

        }*/
    }
}
