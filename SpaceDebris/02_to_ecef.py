import csv
from converter import dms_to_dd, llh_to_ecef

total_sum: float = 0

with open('space_debris_positions.csv', 'r') as file:
    reader = csv.DictReader(file)
    for row in reader:
        lat_decimal = dms_to_dd(row['Lat'])
        long_decimal = dms_to_dd(row['Long'])
        x, y, z = llh_to_ecef(lat_decimal, long_decimal, float(row['Alt_km']))
        total_sum += (x + y + z) / 1000

print(f"Total sum: {total_sum}")
