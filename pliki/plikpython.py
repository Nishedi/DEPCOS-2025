import os

def remove_first_five_lines_from_files(directory):
    """
    Removes the first five lines from all files in the specified directory.
    
    :param directory: Path to the directory containing files.
    """
    for filename in os.listdir(directory):
        file_path = os.path.join(directory, filename)
        
        # Sprawdzenie, czy jest to plik
        if os.path.isfile(file_path):
            try:
                # Wczytanie zawartości pliku
                with open(file_path, 'r', encoding='utf-8') as file:
                    lines = file.readlines()
                
                # Pomijanie pierwszych 5 linii
                updated_lines = lines[5:]
                
                # Nadpisanie pliku
                with open(file_path, 'w', encoding='utf-8') as file:
                    file.writelines(updated_lines)
                
                print(f"Pierwsze 5 linii usunięto z pliku: {filename}")
            
            except Exception as e:
                print(f"Błąd podczas przetwarzania pliku {filename}: {e}")

# Ścieżka do katalogu (zmień ją na odpowiednią)
directory_path = "200 lokacji"

# Uruchomienie funkcji
remove_first_five_lines_from_files(directory_path)
