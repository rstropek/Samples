import csv
from converter import dms_to_dd

lat_sum: float = 0
long_sum: float = 0

with open('space_debris_positions.csv', 'r') as file:
    reader = csv.DictReader(file)
    for row in reader:
        lat_decimal = dms_to_dd(row['Lat'])
        long_decimal = dms_to_dd(row['Long'])
        
        lat_sum += lat_decimal
        long_sum += long_decimal

result = lat_sum * long_sum
print(f"Product of sums: {round(result, 6)}")
