import jwt from 'jsonwebtoken';
import dns from 'dns';

export function getRemainingTime(token: string): number | null {
    try {
        const decoded = jwt.decode(token);
        if (typeof decoded === 'object' && decoded !== null) {
            const currentTime = Math.floor(Date.now() / 1000); // Current time in seconds
            if (decoded.exp && typeof decoded.exp === 'number') {
                return decoded.exp - currentTime; // Remaining time in seconds
            }
        }

        return null;
    } catch (error) {
        return null;
    }
}

export function resolveIPAddress(dnsName: string): Promise<string> {
    return new Promise((resolve, reject) => {
        dns.lookup(dnsName, (err, address) => {
            if (err) {
                reject(err);
                return;
            }
            
            resolve(address);
        });
    });
}
