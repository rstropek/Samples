import express from 'express';
import type { Request, Response } from 'express';
import cors from 'cors';
import healthCheckRoutes from './routes/healthcheck.js';
import searchRoutes from './routes/search.js';
import openaiRoutes from './routes/openai.js';

const app = express();
const port = process.env.PORT || 3000;

// Enable CORS
app.use(cors());

// Enable JSON body parsing
app.use(express.json());

// Endpoints
app.use('/', healthCheckRoutes);
app.use('/search', searchRoutes);
app.use('/openai', openaiRoutes);

// Default route
app.get('/', (req: Request, res: Response) => {
    res.json({ message: 'RAG Web App API is running' });
});

// Start server
app.listen(port, () => {
    console.log(`Server is running on port ${port}`);
});
