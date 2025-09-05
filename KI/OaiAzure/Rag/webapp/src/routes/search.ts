import { SearchIndexClient } from "@azure/search-documents";
import { DefaultAzureCredential } from "@azure/identity";
import { Router } from "express";
import type { Request, Response } from 'express';

const endpoint = process.env.SEARCH_ENDPOINT || "https://srch-fw7iqasihm4s4.search.windows.net";

function getClient(): SearchIndexClient {
    const credentials = new DefaultAzureCredential();
    const indexClient = new SearchIndexClient(endpoint, credentials);
    return indexClient;
}

const router = Router();

router.get('/indexes', async (req: Request, res: Response) => {
    const client = getClient();

    const indexes: string[] = [];
    for await (const ix of client.listIndexes()) {
        indexes.push(ix.name);
    }

    res.status(200).json(indexes);
});

export default router;
