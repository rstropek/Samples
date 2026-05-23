import math

def dms_to_dd(dms: str) -> float:
    # Convert degree/minutes/seconds to decimal degrees
    import re
    match = re.match(r"(\d+)°(\d+)'(\d+)([NSEW])", dms)
    if not match:
        raise ValueError(f"Invalid DMS format: {dms}")

    degrees, minutes, seconds, direction = match.groups()
    decimal = int(degrees) + int(minutes) / 60 + int(seconds) / 3600

    if direction in ['S', 'W']:
        decimal *= -1
    return round(decimal, 6)

def llh_to_ecef(lat_dd: float, lon_dd: float, alt_km: float) -> tuple[float, float, float]:
    # WGS84 constants
    a = 6378137.0  # meters
    f = 1 / 298.257223563
    e2 = f * (2 - f)

    # Convert inputs
    import math
    lat = math.radians(lat_dd)
    lon = math.radians(lon_dd)
    alt = alt_km * 1000  # convert km to meters

    # Compute prime vertical radius of curvature
    N = a / math.sqrt(1 - e2 * math.sin(lat)**2)

    # Compute ECEF
    X = (N + alt) * math.cos(lat) * math.cos(lon)
    Y = (N + alt) * math.cos(lat) * math.sin(lon)
    Z = ((1 - e2) * N + alt) * math.sin(lat)

    return round(X, 2), round(Y, 2), round(Z, 2)

def ecef_distance(coord1: tuple[float, float, float], coord2: tuple[float, float, float]) -> float:
    x1, y1, z1 = coord1
    x2, y2, z2 = coord2
    distance = math.sqrt((x2 - x1)**2 + (y2 - y1)**2 + (z2 - z1)**2)
    return distance
