import os
import pandas as pd
import math

class FileStructure:
    def __init__(self, fileName, location_type, window_type, gurobi_time, greedy_time, ts_time):
        self.fileName = fileName
        if(location_type == 0):
            self.location_type = "Clustered"
        if(location_type == 1):
            self.location_type = "Random"
        if(location_type == 2):
            self.location_type = "Random-clustered"

        if(window_type == 0):
            self.window_type = "Narrow"
        if(window_type == 1):
            self.window_type = "Wide"

        self.gurobi_time = round(gurobi_time)
        self.greedy_time = round(greedy_time)
        self.ts_time = round(ts_time)

def createLatexChartNoGurobi(chartName, description, data, window_filter = None, location_filter = None):
    file = open(chartName, "w")
    file.write("\\begin{table}[t]\n")
    file.write("\centering\n")
    file.write("\caption{\n")
    file.write(description)
    file.write("}\n")
    file.write("\\begin{tabular*}{0.8\linewidth}{@{\extracolsep{\\fill}}cccccc}\n")
    file.write("\\toprule\n")
    file.write("$file$ $name$ & $distribution$ & $time$ $window$ & $Greedy$ & $TS$ & $adventage[\\%]$ \\\\ \midrule\n")
    if(location_filter == None and window_filter == None):
        for i in range(len(data)):
            file.write(data[i].fileName.split('.')[0] + " & " + data[i].location_type + " & " + data[i].window_type + " & " + str(
                data[i].greedy_time) + " & " + str(data[i].ts_time) + " & " + str(
                round((data[i].greedy_time / data[i].ts_time - 1) * 100)) + "\\\\\n")
    elif(location_filter != None and window_filter == None):
        for i in range(len(data)):
            if(data[i].location_type == location_filter):
                file.write(data[i].fileName.split('.')[0] + " & " + data[i].location_type + " & " + data[i].window_type + " & " + str(
                    data[i].greedy_time) + " & " + str(data[i].ts_time) + " & " + str(
                    round((data[i].greedy_time / data[i].ts_time - 1) * 100)) + "\\\\\n")
    elif(location_filter == None and window_filter != None):
        for i in range(len(data)):
            if(data[i].window_type == window_filter):
                file.write(data[i].fileName.split('.')[0] + " & " + data[i].location_type + " & " + data[i].window_type + " & " + str(
                    data[i].greedy_time) + " & " + str(data[i].ts_time) + " & " + str(
                    round((data[i].greedy_time / data[i].ts_time - 1) * 100)) + "\\\\\n")
    else:
        for i in range(len(data)):
            if(data[i].window_type == window_filter and data[i].location_type == location_filter):
                file.write(data[i].fileName.split('.')[0] + " & " + data[i].location_type + " & " + data[i].window_type + " & " + str(
                    data[i].greedy_time) + " & " + str(data[i].ts_time) + " & " + str(
                    round((data[i].greedy_time / data[i].ts_time - 1) * 100)) + "\\\\\n")


    # file.write("\phantom{0}$1000$ & 3.84 & 3.21 & 2.73 & 3.20 \\\\\n")
    file.write("\end{tabular*}")
    file.write("\end{table}")
    file.close()

def createLatexChartGurobi(chartName, description, data, window_filter = None, location_filter = None):
    file = open(chartName, "w")
    file.write("\\begin{table}[t]\n")
    file.write("\centering\n")
    file.write("\caption{\n")
    file.write(description)
    file.write("}\n")
    file.write("\\begin{tabular*}{0.8\linewidth}{@{\extracolsep{\\fill}}cccccc}\n")
    file.write("\\toprule\n")
    file.write("$file$ $name$ & $distribution$ & $time$ $window$ & $Greedy$ & $TS$ & $Solver$ \\\\ \midrule\n")
    if(location_filter == None and window_filter == None):
        for i in range(len(data)):
            file.write(data[i].fileName.split('.')[0] + " & " + data[i].location_type + " & " + data[i].window_type + " & " + str(
                data[i].greedy_time) + " & " + str(data[i].ts_time) + " & " + str(data[i].gurobi_time) + "\\\\\n")
    elif(location_filter != None and window_filter == None):
        for i in range(len(data)):
            if(data[i].location_type == location_filter):
                file.write(data[i].fileName.split('.')[0] + " & " + data[i].location_type + " & " + data[i].window_type + " & " + str(
                    data[i].greedy_time) + " & " + str(data[i].ts_time) + " & " + str(data[i].gurobi_time) + "\\\\\n")
    elif(location_filter == None and window_filter != None):
        for i in range(len(data)):
            if(data[i].window_type == window_filter):
                file.write(data[i].fileName.split('.')[0] + " & " + data[i].location_type + " & " + data[i].window_type + " & " + str(
                    data[i].greedy_time) + " & " + str(data[i].ts_time) + " & " + str(data[i].gurobi_time) + "\\\\\n")
    else:
        for i in range(len(data)):
            if(data[i].window_type == window_filter and data[i].location_type == location_filter):
                file.write(data[i].fileName.split('.')[0] + " & " + data[i].location_type + " & " + data[i].window_type + " & " + str(
                    data[i].greedy_time) + " & " + str(data[i].ts_time) + " & " + str(data[i].gurobi_time) + "\\\\\n")


    # file.write("\phantom{0}$1000$ & 3.84 & 3.21 & 2.73 & 3.20 \\\\\n")
    file.write("\end{tabular*}")
    file.write("\end{table}")
    file.close()

def merge_csv_files(directory, output_file):
    all_files = [f for f in os.listdir(directory) if f.endswith('.csv')]

    if not all_files:
        print("Brak plików CSV w katalogu.")
        return

    merged_df = pd.concat(
        (pd.read_csv(os.path.join(directory, f)) for f in all_files),
        ignore_index=True
    )

    merged_df.to_csv(output_file, index=False)
    print(f"Połączono {len(all_files)} plików do {output_file}")


def load_csv_to_objects(file_path):
    df = pd.read_csv(file_path)
    objects_list = [FileStructure(row["file_name"], row["location_type"], row["window_type"],
                                  row["gurobi_result"], row["greedy_result"], row["tabu_result"])
                    for _, row in df.iterrows()]

    return objects_list


data = load_csv_to_objects("AA_concated_100.csv")

# no filter
description = "Aggregated results of Greedy and Tabu Search algorithms for 100 locations"
chartName = "table_test.tex"
createLatexChartNoGurobi(chartName, description, data)

# location clustered, window no filter
description = "Aggregated results of Greedy and Tabu Search algorithms for 100 locations, clustered"
chartName = "table_test_clustered.tex"
createLatexChartNoGurobi(chartName, description, data, location_filter="Clustered")

# location random, window no filter
description = "Aggregated results of Greedy and Tabu Search algorithms for 100 locations, random"
chartName = "table_test_random.tex"
createLatexChartNoGurobi(chartName, description, data, location_filter="Random")

# location random-clustered, window no filter
description = "Aggregated results of Greedy and Tabu Search algorithms for 100 locations, random-clustered"
chartName = "table_test_random_clustered.tex"
createLatexChartNoGurobi(chartName, description, data, location_filter="Random-clustered")

# location no filter, window narrow
description = "Aggregated results of Greedy and Tabu Search algorithms for 100 locations, narrow window"
chartName = "table_test_narrow.tex"
createLatexChartNoGurobi(chartName, description, data, window_filter="Narrow")

# location clustered, window narrow
description = "Aggregated results of Greedy and Tabu Search algorithms for 100 locations, narrow window, clustered"
chartName = "table_test_clustered_narrow.tex"
createLatexChartNoGurobi(chartName, description, data, window_filter="Narrow", location_filter="Clustered")

# location random, window narrow
description = "Aggregated results of Greedy and Tabu Search algorithms for 100 locations, narrow window, random"
chartName = "table_test_random_narrow.tex"
createLatexChartNoGurobi(chartName, description, data, window_filter="Narrow", location_filter="Random")

# location random-clustered, window narrow
description = "Aggregated results of Greedy and Tabu Search algorithms for 100 locations, narrow window, random-clustered"
chartName = "table_test_random_clustered_narrow.tex"
createLatexChartNoGurobi(chartName, description, data, window_filter="Narrow", location_filter="Random-clustered")

# location no filter, window wide
description = "Aggregated results of Greedy and Tabu Search algorithms for 100 locations, wide window"
chartName = "table_test_wide.tex"
createLatexChartNoGurobi(chartName, description, data, window_filter="Wide")

# location clustered, window wide
description = "Aggregated results of Greedy and Tabu Search algorithms for 100 locations, wide window, clustered"
chartName = "table_test_clustered_wide.tex"
createLatexChartNoGurobi(chartName, description, data, window_filter="Wide", location_filter="Clustered")

# location random, window wide
description = "Aggregated results of Greedy and Tabu Search algorithms for 100 locations, wide window, random"
chartName = "table_test_random_wide.tex"
createLatexChartNoGurobi(chartName, description, data, window_filter="Wide", location_filter="Random")

# location random-clustered, window wide
description = "Aggregated results of Greedy and Tabu Search algorithms for 100 locations, wide window, random-clustered"
chartName = "table_test_random_clustered_wide.tex"
createLatexChartNoGurobi(chartName, description, data, window_filter="Wide", location_filter="Random-clustered")


description = "Gurobi results for 100 locations"
chartName = "gurobitest.tex"
createLatexChartGurobi(chartName, description, data, window_filter="Wide", location_filter="Random-clustered")









