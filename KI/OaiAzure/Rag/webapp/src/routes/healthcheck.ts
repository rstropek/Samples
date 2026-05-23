import { Router } from 'express';
import type { Request, Response } from 'express';
import { promises as dns } from 'dns';

const router = Router();

// Basic ping endpoint
router.get('/ping', async (req: Request, res: Response) => {
    try {
        // Extract hostnames from endpoints
        const searchEndpoint = process.env.SEARCH_ENDPOINT || "https://srch-fw7iqasihm4s4.search.windows.net";
        const openaiEndpoint = process.env.AZURE_ENDPOINT || "https://fw7iqasihm4s4.openai.azure.com";
        
        // Extract hostnames from URLs
        const searchHostname = new URL(searchEndpoint).hostname;
        const openaiHostname = new URL(openaiEndpoint).hostname;
        
        // Perform DNS resolution
        const [searchIps, openaiIps] = await Promise.all([
            dns.lookup(searchHostname, { all: true }),
            dns.lookup(openaiHostname, { all: true })
        ]);

        res.status(200).json({
            status: 'OK',
            message: 'Server is healthy',
            timestamp: new Date().toISOString(),
            uptime: process.uptime(),
            dns: {
                search: {
                    hostname: searchHostname,
                    ips: searchIps.map(ip => ip.address)
                },
                openai: {
                    hostname: openaiHostname,
                    ips: openaiIps.map(ip => ip.address)
                }
            }
        });
    } catch (error) {
        res.status(500).json({
            status: 'ERROR',
            message: 'DNS resolution failed',
            timestamp: new Date().toISOString(),
            error: error instanceof Error ? error.message : 'Unknown error'
        });
    }
});

export default router;
