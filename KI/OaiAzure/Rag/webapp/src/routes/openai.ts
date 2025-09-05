import { DefaultAzureCredential, getBearerTokenProvider } from '@azure/identity';
import { Router } from 'express';
import OpenAI, { AzureOpenAI } from 'openai';
import type { Request, Response } from 'express';

const credential = new DefaultAzureCredential();
const scope = 'https://cognitiveservices.azure.com/.default';
const azureADTokenProvider = getBearerTokenProvider(credential, scope);

const endpoint = process.env.AZURE_ENDPOINT || "https://fw7iqasihm4s4.openai.azure.com/";
const model = process.env.AZURE_DEPLOYMENT || "gpt-4.1";

function getClient(): OpenAI {
    const client = new AzureOpenAI({
        endpoint: endpoint,
        azureADTokenProvider: azureADTokenProvider,
        apiVersion: '2025-04-01-preview',
        deployment: model,
    });
    return client;
}

const router = Router();

router.get('/query', async (req: Request, res: Response) => {
    try {
        const client = getClient();
        const response = await client.responses.create({
            instructions: 'You are a helpful assistant',
            input: 'Are dolphins fish?',
            model
        });
        res.status(200).json(response.output_text);
    } catch (error) {
        console.error('Error in /query:', error);
        res.status(500).json({ error: 'An error occurred while processing your request.' });
    }
});

export default router;
