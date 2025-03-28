from typing import Optional

import pandas as pd

from converter import dms_to_dd, llh_to_ecef, ecef_distance

class KDNode:
    def __init__(self, point: tuple[float, float, float], axis: int,
                 left: Optional['KDNode'] = None, right: Optional['KDNode'] = None):
        self.point = point
        self.axis = axis
        self.left = left
        self.right = right

def build_kd_tree(points: list[tuple[float, float, float]], depth: int = 0) -> Optional[KDNode]:
    if not points:
        return None

    k = 3
    axis = depth % k
    points.sort(key=lambda p: p[axis])
    median = len(points) // 2

    return KDNode(
        point=points[median],
        axis=axis,
        left=build_kd_tree(points[:median], depth + 1),
        right=build_kd_tree(points[median + 1:], depth + 1)
    )

def kd_radius_search(node: Optional[KDNode],
                     target: tuple[float, float, float],
                     radius: float,
                     results: list[tuple[float, float, float]]) -> None:
    if node is None:
        return

    # Compute squared distance for speed
    dist_sq = sum((tc - nc) ** 2 for tc, nc in zip(target, node.point))
    if dist_sq <= radius ** 2:
        results.append(node.point)

    axis = node.axis
    diff = target[axis] - node.point[axis]

    # Decide which subtree to search first
    nearer, further = (node.left, node.right) if diff < 0 else (node.right, node.left)
    kd_radius_search(nearer, target, radius, results)

    # Only search the further subtree if hypersphere crosses splitting plane
    if diff ** 2 <= radius ** 2:
        kd_radius_search(further, target, radius, results)

def find_satellites_with_debris_within(sat_df, kd_tree, radius=1000):
    nearby_sat_ids = []
    total_distances = {}
    
    for _, sat in sat_df.iterrows():
        sat_point = (sat['X_m'], sat['Y_m'], sat['Z_m'])
        hits = []
        kd_radius_search(kd_tree, sat_point, radius, hits)
        
        if hits:
            sat_id = sat['SatelliteID']
            nearby_sat_ids.append(sat_id)
            
            # Calculate total distance to all debris
            total_distance = sum(ecef_distance(sat_point, debris_point) for debris_point in hits)
            total_distances[sat_id] = total_distance
    
    # Get the filtered dataframe
    result_df = sat_df[sat_df['SatelliteID'].isin(nearby_sat_ids)].copy()
    
    # Add the total distance column
    result_df['TotalDebrisDistance'] = result_df['SatelliteID'].map(total_distances)
    
    return result_df


sat_df = pd.read_csv('satellite_positions.csv')
debris_df = pd.read_csv('space_debris_positions.csv')
debris_df['Lat_dd'] = debris_df['Lat'].apply(dms_to_dd)
debris_df['Long_dd'] = debris_df['Long'].apply(dms_to_dd)
ecef = debris_df.apply(lambda row: llh_to_ecef(row['Lat_dd'], row['Long_dd'], row['Alt_km']), axis=1)
debris_df[['X_m', 'Y_m', 'Z_m']] = pd.DataFrame(ecef.tolist(), index=debris_df.index)
debris_df = debris_df[['DebrisID', 'X_m', 'Y_m', 'Z_m']]

debris_points = list(zip(debris_df['X_m'], debris_df['Y_m'], debris_df['Z_m']))
kd_tree = build_kd_tree(debris_points)

result = find_satellites_with_debris_within(sat_df, kd_tree)
print(result)
