import { Router } from 'express';
import type { Request, Response } from 'express';
import { getOpenAIClient } from './clients.js';

const model = process.env.AZURE_DEPLOYMENT || "gpt-4.1";

const router = Router();

router.get('/query', async (req: Request, res: Response) => {
    try {
        const client = getOpenAIClient();
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
