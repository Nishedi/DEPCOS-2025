using System;
using System.Collections.Generic;
using System.Linq;

public class Test
{
    Random rd = new Random();
    public List<int> Crossover(List<int> parent1, List<int> parent2)
    {
        var childClients = new List<int>();
        for (int x = 0; x < parent1.Count; x++)
        {
            childClients.Add(-1);
        }
        int i = rd.Next(parent1.Count);
        int j = rd.Next(parent2.Count);
        while (j == i)
        {
            j = rd.Next(parent1.Count);
        }
        if (i > j)
        {
            int swap = i;
            i = j;
            j = swap;
        }

        for (; i<=j;i++)
        {
            childClients[i] = parent1[i];
        }
        int currentIndex = 1;
        childClients[0] = 0;

        foreach (var element in parent2)
        {
            if (!childClients.Contains(element))
            {
                while (childClients[currentIndex] != -1)
                {
                    currentIndex++;
                }
                childClients[currentIndex] = element;
            }
        }
        for (int x = 0; x < childClients.Count; x++)
        {
            if (childClients[x] == -1)
            {
                childClients[x] = 0;
                i = rd.Next(childClients.Count);
                while (childClients[i] == -1)
                {
                    i = rd.Next(childClients.Count);
                }
                int swap = childClients[i];
                childClients[i] = childClients[x];
                childClients[x] = swap;
                
            }
        }
        return childClients;
    }

    

    public void printGTR(List<int> solution)
    {
        for(int i = 0; i < solution.Count; i++)
        {
            Console.Write(solution[i]+" ");
        }
        Console.WriteLine();
    }

    public void test()
    {
        int num = 10;
        for(int iter = 0; iter < num;iter++) {
            List<int> parent1 = new List<int> { 0, 1, 5, 0, 7, 4, 0, 2, 8, 3, 0, 6, 0, 9, 10, 11, 12, 13, 14, 15 };
            List<int> parent2 = new List<int> { 0, 2, 3, 8, 0, 6, 0, 1, 7, 5, 0, 0, 4, 15, 9, 10, 12, 11,13, 14 };

            List<int> child = Crossover(parent1, parent2);

            if (num == 10)
            {
                printGTR(parent1);
                printGTR(parent2);
                printGTR(child);
            }
            if (parent1.Count != parent2.Count && parent1.Count != child.Count) Console.WriteLine("Error");
            for (int i = 1; i <= 15; i++)
            {
                if (child.Contains(i) == false) Console.WriteLine("Error type 2");
                if (child.Count(x => x == i) > 1) Console.WriteLine("Error type 3");
            }
            for (int i = 0; i <= child.Count; i++)
            {
                if (child.Contains(-1)) Console.WriteLine("Error type 4");
            }
            Console.WriteLine();

            /* 
            Console.WriteLine(parent1.Count);
            Console.WriteLine(parent2.Count);
            Console.WriteLine(child.Count);*/
        }
    }
}
