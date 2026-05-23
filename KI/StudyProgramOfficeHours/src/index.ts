import { McpServer, ResourceTemplate } from '@modelcontextprotocol/sdk/server/mcp.js';
import { StreamableHTTPServerTransport } from '@modelcontextprotocol/sdk/server/streamableHttp.js';
import express from 'express';
import * as z from 'zod/v4';
import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

// Create an MCP server
const server = new McpServer({
    name: 'studyprogram-officehours',
    version: '1.0.0'
});

// Load office hours data
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const dataPath = path.join(__dirname, 'OfficeHours.json');
let officeHoursData: any[] = [];

try {
    officeHoursData = JSON.parse(fs.readFileSync(dataPath, 'utf-8'));
} catch (error) {
    console.error('Error loading OfficeHours.json:', error);
}

// Add tool to get all program names
server.registerTool(
    'get_program_names',
    {
        title: 'Get Program Names',
        description: 'Retrieve a list of all distinct study program names',
        inputSchema: z.object({}),
        outputSchema: z.object({
            programs: z.array(z.string())
        })
    },
    async () => {
        const programs = [...new Set(officeHoursData.map(item => item.programName))];
        return {
            content: [{ type: 'text', text: JSON.stringify(programs) }],
            structuredContent: { programs }
        };
    }
);

// Add tool to get office hours for a specific program
server.registerTool(
    'get_office_hours',
    {
        title: 'Get Office Hours',
        description: 'Get office hour information for a specific study program',
        inputSchema: z.object({
            programName: z.string()
        }),
        outputSchema: z.object({
            officeHours: z.any() // Using any for flexibility, or could define specific schema
        })
    },
    async ({ programName }) => {
        const programInfo = officeHoursData.find(item => item.programName === programName);

        if (!programInfo) {
            return {
                content: [{ type: 'text', text: `Program "${programName}" not found.` }],
                isError: true
            };
        }

        return {
            content: [{ type: 'text', text: JSON.stringify(programInfo) }],
            structuredContent: { officeHours: programInfo }
        };
    }
);

// Set up Express and HTTP transport
const app = express();
app.use(express.json());

app.post('/mcp', async (req, res) => {
    // Create a new transport for each request to prevent request ID collisions
    const transport = new StreamableHTTPServerTransport({
        sessionIdGenerator: undefined,
        enableJsonResponse: true
    });

    res.on('close', () => {
        transport.close();
    });

    await server.connect(transport);
    await transport.handleRequest(req, res, req.body);
});

const port = parseInt(process.env.PORT || '3000');
app.listen(port, '0.0.0.0', () => {
    console.log(`Demo MCP Server running on http://0.0.0.0:${port}/mcp`);
}).on('error', error => {
    console.error('Server error:', error);
    process.exit(1);
});