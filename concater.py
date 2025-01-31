import os
import pandas as pd

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

# Użycie
katalog = "100"
plik_wyjściowy = "100/AA_concated_100.csv"
merge_csv_files(katalog, plik_wyjściowy)
