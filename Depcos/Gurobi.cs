using Gurobi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GurobiVRP
{
    private static GRBEnv env = new GRBEnv();

    public void gurobi_test()
    {
        // Create a new model
        int size = 5;
        GRBModel model = new GRBModel(env);
        GRBVar[, , ] x = new GRBVar[size, size, size]; // Tu zmienić na ilości lokacji i pojazdow

        // Create variables
        for(int v = 0; v < size; v++)
        {
            for(int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    x[v, i, j] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, "x_" + v + "_" + i + "_" + j);
                }
            }
        }

        model.Update();

        // Set objective
        GRBLinExpr expr = 0.0;
        for (int v = 0; v < size; v++)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    expr += x[v, i, j] * j;
                }
            }
        }
        model.SetObjective(expr, GRB.MINIMIZE);

        // Add constraint
        
        for (int v = 0; v < size; v++)
        {
            GRBLinExpr sum = 0.0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    sum += x[v, i, j];
                }
            }
            //Console.WriteLine(int(sum));
            model.AddConstr(sum, GRB.GREATER_EQUAL, 5.0, "c0");
        }
   

        // Optimize model
        model.Optimize();
        model.Write("model.lp");
        // Dispose of model and environment
        model.Dispose();
        env.Dispose();
    }   
}
