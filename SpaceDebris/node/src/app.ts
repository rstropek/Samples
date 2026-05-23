import { readFileSync } from 'fs';
import Papa from 'papaparse';
import { dmsToDd, llhToEcef, ecefDistance } from './convert';

// === Challenge 1 ===================================================================================
interface SpaceDebrisRow {
    Lat: string;
    Long: string;
}

let latSum = 0;
let longSum = 0;

try {
    // Read the CSV file
    const fileContent = readFileSync('space_debris_positions.csv', 'utf-8');
    
    // Parse CSV using Papa Parse
    const { data } = Papa.parse<SpaceDebrisRow>(fileContent, {
        header: true,
        skipEmptyLines: true
    });

    // Process each row
    for (const row of data) {
        const latDecimal = dmsToDd(row.Lat);
        const longDecimal = dmsToDd(row.Long);
        
        latSum += latDecimal;
        longSum += longDecimal;
    }

    const result = latSum * longSum;
    console.log(`Product of sums: ${result.toFixed(6)}`);
} catch (error) {
    console.error('Error processing CSV file:', error);
    process.exit(1);
}

// === Challenge 2 ===================================================================================
interface SpaceDebrisRowWithAlt extends SpaceDebrisRow {
    Alt_km: string;
}

let totalSum = 0;

try {
    // Read the CSV file again
    const fileContent = readFileSync('space_debris_positions.csv', 'utf-8');
    
    // Parse CSV using Papa Parse
    const { data } = Papa.parse<SpaceDebrisRowWithAlt>(fileContent, {
        header: true,
        skipEmptyLines: true
    });

    // Process each row
    for (const row of data) {
        const latDecimal = dmsToDd(row.Lat);
        const longDecimal = dmsToDd(row.Long);
        const [x, y, z] = llhToEcef(latDecimal, longDecimal, parseFloat(row.Alt_km));
        totalSum += (x + y + z) / 1000;
    }

    console.log(`Total sum: ${Number(totalSum.toFixed(5))}`);
} catch (error) {
    console.error('Error processing CSV file:', error);
    process.exit(1);
}

// === Challenge 3 ===================================================================================
interface SatelliteRow {
    SatelliteID: string;
    X_m: number;
    Y_m: number;
    Z_m: number;
}

interface DebrisRowWithECEF extends SpaceDebrisRowWithAlt {
    DebrisID: string;
    Lat_dd?: number;
    Long_dd?: number;
    X_m?: number;
    Y_m?: number;
    Z_m?: number;
}

try {
    // Read both CSV files
    const satelliteContent = readFileSync('satellite_positions.csv', 'utf-8');
    const debrisContent = readFileSync('space_debris_positions.csv', 'utf-8');
    
    // Parse CSVs
    const { data: satelliteData } = Papa.parse<SatelliteRow>(satelliteContent, {
        header: true,
        skipEmptyLines: true,
        transform: (value, field) => {
            if (field === 'X_m' || field === 'Y_m' || field === 'Z_m') {
                return parseFloat(value);
            }
            return value;
        }
    });

    const { data: debrisData } = Papa.parse<DebrisRowWithECEF>(debrisContent, {
        header: true,
        skipEmptyLines: true
    });

    // Convert debris coordinates to ECEF
    const debrisWithECEF = debrisData.map(debris => {
        const latDd = dmsToDd(debris.Lat);
        const longDd = dmsToDd(debris.Long);
        const [x, y, z] = llhToEcef(latDd, longDd, parseFloat(debris.Alt_km));
        return {
            DebrisID: debris.DebrisID,
            X_m: x,
            Y_m: y,
            Z_m: z
        };
    });

    // Find nearby satellites
    const nearbySatellites: Array<{ satelliteId: string; distance: number }> = [];
    
    for (const sat of satelliteData) {
        const satCoord: [number, number, number] = [sat.X_m, sat.Y_m, sat.Z_m];
        
        for (const debris of debrisWithECEF) {
            const debrisCoord: [number, number, number] = [debris.X_m, debris.Y_m, debris.Z_m];
            const distance = ecefDistance(satCoord, debrisCoord);
            
            if (distance <= 1000) {
                nearbySatellites.push({
                    satelliteId: sat.SatelliteID,
                    distance: distance
                });
            }
        }
    }

    // Create result with distances
    const result = satelliteData
        .filter(sat => nearbySatellites.some(ns => ns.satelliteId === sat.SatelliteID))
        .map(sat => ({
            ...sat,
            Distance_m: nearbySatellites.find(ns => ns.satelliteId === sat.SatelliteID)?.distance
        }));

    // Print results
    console.log(JSON.stringify(result, null, 2));
} catch (error) {
    console.error('Error processing CSV files:', error);
    process.exit(1);
}
