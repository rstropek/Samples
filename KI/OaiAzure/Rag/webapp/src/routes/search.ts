import type  { ExhaustiveKnnAlgorithmConfiguration, SearchIndex, VectorSearchProfile } from "@azure/search-documents";
import { Router } from "express";
import type { Request, Response } from 'express';
import { getOpenAIClient, getSearchClient } from "./clients.js";

const router = Router();

router.get('/indexes', async (req: Request, res: Response) => {
    const client = getSearchClient();

    const indexes: string[] = [];
    for await (const ix of client.listIndexes()) {
        indexes.push(ix.name);
    }

    res.status(200).json(indexes);
});

router.post('/indexes/create', async (req: Request, res: Response) => {
    if (!req.body || !req.body.name) {
        res.status(400).json({ error: "Missing 'name' in request body" });
        return;
    }

    const { name } = req.body;
    
    const client = getSearchClient();
    if (await client.getIndex(name)) {
        res.status(400).json({ error: `Index '${name}' already exists` });
        return;
    }

    const vectorSearchAlgorithm: ExhaustiveKnnAlgorithmConfiguration = {
        name: "eknn",
        kind: "exhaustiveKnn",
        parameters: {
            metric: "dotProduct"
        }
    }

    const vectorSearchProfile: VectorSearchProfile = {
        name: "vector-profile-1",
        algorithmConfigurationName: vectorSearchAlgorithm.name
    }

    const index: SearchIndex = {
        name,
        fields: [
            { 
                name: "documentId", 
                type: "Edm.String", 
                key: true,
                searchable: true,
                filterable: true
            },
            { 
                name: "cityName", 
                type: "Edm.String",
                searchable: true, 
                filterable: false, 
                sortable: true
            },
            { 
                name: "cityDescription", 
                type: "Edm.String", 
                searchable: true, 
                filterable: false, 
                sortable: false, 
                facetable: false
            },
            {
                name: "contentVector",
                type: "Collection(Edm.Single)",
                searchable: true,
                stored: false,
                vectorSearchDimensions: 3072,
                vectorSearchProfileName: vectorSearchProfile.name
            }
        ],
        vectorSearch: {
            algorithms: [vectorSearchAlgorithm],
            profiles: [ vectorSearchProfile ]
        },
    };

    await client.createIndex(index);
    res.status(201).json({ message: `Index '${name}' created successfully` });
});

router.delete('/indexes/remove', async (req: Request, res: Response) => {
    if (!req.body || !req.body.name) {
        res.status(400).json({ error: "Missing 'name' in request body" });
        return;
    }

    const { name } = req.body;
    
    const client = getSearchClient();
    if (!await client.getIndex(name)) {
        res.status(400).json({ error: `Index '${name}' does not exist` });
        return;
    }

    await client.deleteIndex(name);
    res.status(204).send();
});

export default router;
