import pandas as pd
from converter import dms_to_dd, ecef_distance, llh_to_ecef

sat_df = pd.read_csv('satellite_positions.csv')
debris_df = pd.read_csv('space_debris_positions.csv')
debris_df['Lat_dd'] = debris_df['Lat'].apply(dms_to_dd)
debris_df['Long_dd'] = debris_df['Long'].apply(dms_to_dd)
ecef = debris_df.apply(lambda row: llh_to_ecef(row['Lat_dd'], row['Long_dd'], row['Alt_km']), axis=1)
debris_df[['X_m', 'Y_m', 'Z_m']] = pd.DataFrame(ecef.tolist(), index=debris_df.index)
debris_df = debris_df[['DebrisID', 'X_m', 'Y_m', 'Z_m']]

nearby_satellites: list[tuple[str, float]] = []
for ix, sat in sat_df.iterrows():
    sat_coord = (sat['X_m'], sat['Y_m'], sat['Z_m'])
    # Check all debris for proximity
    for _, debris in debris_df.iterrows():
        debris_coord = (debris['X_m'], debris['Y_m'], debris['Z_m'])
        distance = ecef_distance(sat_coord, debris_coord)
        if distance <= 1000:
            nearby_satellites.append((sat['SatelliteID'], distance))

# Filter the original satellite dataframe
# Create a dictionary mapping satellite IDs to their distances
sat_distances = {sat_id: distance for sat_id, distance in nearby_satellites}

# Filter the original dataframe and add the distance column
result = sat_df[sat_df['SatelliteID'].isin([sat_id for sat_id, _ in nearby_satellites])].copy()
result['Distance_m'] = result['SatelliteID'].map(sat_distances)

print(result)
