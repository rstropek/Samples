import csv
import random
import pandas as pd

from converter import dms_to_dd, llh_to_ecef

def random_dms_lat() -> str:
    deg = random.randint(0, 90)
    minutes = random.randint(0, 59)
    seconds = random.randint(0, 59)
    direction = random.choice(['N', 'S'])
    return f"{deg}°{minutes}'{seconds}{direction}"

def random_dms_lon() -> str:
    deg = random.randint(0, 180)
    minutes = random.randint(0, 59)
    seconds = random.randint(0, 59)
    direction = random.choice(['E', 'W'])
    return f"{deg}°{minutes}'{seconds}{direction}"

space_debris: list[tuple[str, str, str, float]] = []
with open('space_debris_positions.csv', 'w', newline='') as f:
    writer = csv.writer(f)
    writer.writerow(['DebrisID', 'Lat', 'Long', 'Alt_km'])
    for i in range(1, 1001):
        debris_id = f"D{i:04d}"
        lat = random_dms_lat()
        lon = random_dms_lon()
        alt = round(random.uniform(100, 2000), 2)  # altitude in km
        space_debris.append((debris_id, lat, lon, alt))
        writer.writerow([debris_id, lat, lon, alt])

def generate_satellite_positions(n: int) -> list[tuple[float, float, float]]:
    satellites: list[tuple[float, float, float]] = []
    for i in range(1, n + 1):
        lat = dms_to_dd(random_dms_lat())
        lon = dms_to_dd(random_dms_lon())
        alt = round(random.uniform(100, 2000), 2)
        x, y, z = llh_to_ecef(lat, lon, alt)
        satellites.append((x, y, z))
    return satellites

def write_satellite_positions(filepath: str, data: list[tuple[str, float, float, float]]):
    with open(filepath, 'w', newline='') as f:
        writer = csv.writer(f)
        writer.writerow(['SatelliteID', 'X_m', 'Y_m', 'Z_m'])
        writer.writerows(data)

# Turn space_debris list into dataframe
debris_df = pd.DataFrame(space_debris, columns=['DebrisID', 'Lat', 'Long', 'Alt_km'])
debris_df['DebrisID'] = debris_df['DebrisID'].str.lstrip('D').astype(int)
debris_df['Lat_dd'] = debris_df['Lat'].apply(dms_to_dd)
debris_df['Long_dd'] = debris_df['Long'].apply(dms_to_dd)
ecef = debris_df.apply(lambda row: llh_to_ecef(row['Lat_dd'], row['Long_dd'], row['Alt_km']), axis=1)
debris_df[['X_m', 'Y_m', 'Z_m']] = pd.DataFrame(ecef.tolist(), index=debris_df.index)
debris_df = debris_df[['DebrisID', 'X_m', 'Y_m', 'Z_m']]

satellites = generate_satellite_positions(997)
random_debris = debris_df.sample(n=3)

# Add offsets to debris coordinates with exact Euclidean distances
offsets = [250, 500, 750]
for (index, row), offset in zip(random_debris.iterrows(), offsets):
    # Create a unit vector (1/√3, 1/√3, 1/√3) for equal displacement in all dimensions
    unit_factor = 1 / (3 ** 0.5)
    # Scale by the desired distance
    dx = offset * unit_factor
    dy = offset * unit_factor
    dz = offset * unit_factor
    satellites.append((row['X_m'] + dx, row['Y_m'] + dy, row['Z_m'] + dz))
random.shuffle(satellites)
satellites_with_id = [(f"S{i:04d}", x, y, z) for i, (x, y, z) in enumerate(satellites, start=1)]
write_satellite_positions('satellite_positions.csv', satellites_with_id)
