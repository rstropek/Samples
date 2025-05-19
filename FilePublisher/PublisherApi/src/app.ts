import express from 'express';
import cors from 'cors';
import compression from 'compression';
import { promises as fs } from 'fs';
import path from 'path';
import crypto from 'crypto';
import { fileURLToPath } from 'url';
import mime from 'mime';

const app = express();
const port = parseInt(process.env.PORT || '3000');

// Get directory path equivalent to __dirname in ES modules
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// Cache for sessions data
interface SessionsCache {
    data: Record<string, string>;
    timestamp: number;
}

let sessionsCache: SessionsCache | null = null;
const CACHE_TTL = 5 * 60 * 1000; // 5 minutes in milliseconds

// Helper function to read sessions.json with caching
async function getSessionsData(): Promise<Record<string, string>> {
    const currentTime = Date.now();
    
    // Use cached data if it exists and is less than 5 minutes old
    if (sessionsCache && (currentTime - sessionsCache.timestamp < CACHE_TTL)) {
        return sessionsCache.data;
    }
    
    // Read and parse the sessions file
    const sessionsData = JSON.parse(await fs.readFile("sessions.json", "utf-8"));
    
    // Update the cache
    sessionsCache = {
        data: sessionsData,
        timestamp: currentTime
    };
    
    return sessionsData;
}

// Helper function to get session path
async function getSessionPath(sessionCode: string): Promise<string | null> {
    const sessions = await getSessionsData();
    return sessions[sessionCode] || null;
}

// Middleware
app.use(cors({
    exposedHeaders: ['ETag']
}));
app.use(compression());
app.use(express.json());

// Request logging middleware
app.use((req: express.Request, res: express.Response, next: express.NextFunction) => {
    const start = Date.now();
    
    // Log when the request completes
    res.on('finish', () => {
        const duration = Date.now() - start;
        console.log(`${new Date().toISOString()} | ${req.method} ${req.originalUrl} | ${res.statusCode} | ${duration}ms`);
    });
    
    next();
});

// Routes
app.get('/ping', (req: express.Request, res: express.Response) => {
    res.json({ message: 'pong' });
});

app.get('/sessions/:code', async (req: express.Request, res: express.Response) => {
    const sessionCode = req.params.code;

    const sessionPath = await getSessionPath(sessionCode);
    if (!sessionPath) {
        res.status(404).json({ error: 'Session not found' });
        return;
    }

    try {
        // Check if the session folder exists
        await fs.access(sessionPath);
        
        // Get all files recursively
        const getAllFiles = async (dirPath: string, baseDir: string): Promise<string[]> => {
            const files: string[] = [];
            const entries = await fs.readdir(dirPath, { withFileTypes: true });
            
            for (const entry of entries) {
                // Skip node_modules folder and files/folders starting with a dot
                const skipDirs = ['node_modules', 'dist', 'bin', 'obj', 'target'];
                if (skipDirs.includes(entry.name) || entry.name.startsWith('.')) {
                    continue;
                }
                
                const fullPath = path.join(dirPath, entry.name);
                const relativePath = path.relative(baseDir, fullPath);
                
                if (entry.isDirectory()) {
                    files.push(...await getAllFiles(fullPath, baseDir));
                } else {
                    files.push(relativePath);
                }
            }
            
            return files;
        };

        const files = await getAllFiles(sessionPath, sessionPath);
        res.json({ files });
    } catch (error) {
        res.status(404).json({ error: 'Session not found' });
    }
});

app.get('/sessions/:code/file', async (req: express.Request, res: express.Response) => {
    const sessionCode = req.params.code;
    const fileName = req.query.file as string;

    if (!fileName) {
        res.status(400).json({ error: 'File parameter is required' });
        return;
    }
    
    const sessionPath = await getSessionPath(sessionCode);
    if (!sessionPath) {
        res.status(404).json({ error: 'Session not found' });
        return;
    }

    const filePath = path.join(sessionPath, fileName);

    try {
        // Ensure the file path doesn't escape the session directory
        const normalizedFilePath = path.normalize(filePath);
        if (!normalizedFilePath.startsWith(sessionPath)) {
            res.status(403).json({ error: 'Invalid file path' });
            return;
        }

        // Read file contents
        const content = await fs.readFile(filePath, 'utf-8');
        
        // Generate ETag using SHA-256 hash
        const etag = `${crypto.createHash('sha256').update(content).digest('hex')}`;
        
        // Check If-None-Match header
        const ifNoneMatch = req.header('If-None-Match');
        if (ifNoneMatch === etag) {
            res.status(304).end(); // Not Modified
            return;
        }

        // Set ETag header and return content
        res.set('ETag', etag);
        res.json({ content });
    } catch (error) {
        res.status(404).json({ error: 'File not found' });
    }
});

// Serve static files from the public directory with correct MIME types
const subpath = process.env.SUBPATH || '';
app.use(`/${subpath}`, express.static(path.join(__dirname, '../public'), {
    setHeaders: (res, filePath) => {
        const mimeType = mime.getType(filePath);
        console.log(`Serving file: ${filePath} with MIME type: ${mimeType}`);
        if (mimeType) {
            res.setHeader('Content-Type', mimeType);
        }
    }
}));

// Fallback route for SPA (Single Page Applications)
app.get('*', (req, res) => {
    // Only handle HTML requests, not API requests
    if (req.accepts('html')) {
        res.sendFile(path.join(__dirname, '../public/index.html'));
    } else {
        res.status(404).json({ error: 'Not found' });
    }
});

// Start server
app.listen(port, () => {
    console.log(`Server is running on port ${port}`);
});
