/**
 * Convert degree/minutes/seconds to decimal degrees
 * @param dms String in format "DD°MM'SS[NSEW]"
 * @returns Decimal degrees
 */
export function dmsToDd(dms: string): number {
    const match = dms.match(/(\d+)°(\d+)'(\d+)([NSEW])/);
    if (!match) {
        throw new Error(`Invalid DMS format: ${dms}`);
    }

    const [, degrees, minutes, seconds, direction] = match;
    const decimal = parseInt(degrees) + parseInt(minutes) / 60 + parseInt(seconds) / 3600;

    if (direction === 'S' || direction === 'W') {
        return Number((-decimal).toFixed(6));
    }
    return Number(decimal.toFixed(6));
}

/**
 * Convert latitude/longitude/height to ECEF coordinates
 * @param latDd Latitude in decimal degrees
 * @param lonDd Longitude in decimal degrees
 * @param altKm Altitude in kilometers
 * @returns Tuple of [X, Y, Z] coordinates in meters
 */
export function llhToEcef(latDd: number, lonDd: number, altKm: number): [number, number, number] {
    // WGS84 constants
    const a = 6378137.0; // meters
    const f = 1 / 298.257223563;
    const e2 = f * (2 - f);

    // Convert inputs
    const lat = latDd * Math.PI / 180;
    const lon = lonDd * Math.PI / 180;
    const alt = altKm * 1000; // convert km to meters

    // Compute prime vertical radius of curvature
    const N = a / Math.sqrt(1 - e2 * Math.sin(lat) ** 2);

    // Compute ECEF
    const X = (N + alt) * Math.cos(lat) * Math.cos(lon);
    const Y = (N + alt) * Math.cos(lat) * Math.sin(lon);
    const Z = ((1 - e2) * N + alt) * Math.sin(lat);

    return [
        Number(X.toFixed(2)),
        Number(Y.toFixed(2)),
        Number(Z.toFixed(2))
    ];
}

/**
 * Calculate distance between two ECEF coordinates
 * @param coord1 First ECEF coordinate [X1, Y1, Z1]
 * @param coord2 Second ECEF coordinate [X2, Y2, Z2]
 * @returns Distance in meters
 */
export function ecefDistance(coord1: [number, number, number], coord2: [number, number, number]): number {
    const [x1, y1, z1] = coord1;
    const [x2, y2, z2] = coord2;
    return Math.sqrt((x2 - x1) ** 2 + (y2 - y1) ** 2 + (z2 - z1) ** 2);
}
