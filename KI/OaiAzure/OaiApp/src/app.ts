import express, { Request, Response } from 'express';
import OpenAI from 'openai';
import { DefaultAzureCredential } from '@azure/identity';
import { getRemainingTime, resolveIPAddress } from './helper';
import dotenv from 'dotenv';

// Get settings from .env file. Note that in Azure, settings
// are stored in the environment variables of the app service.
dotenv.config();

// Configure express.js
const app = express();
app.set('views', __dirname + '/views')
app.set('view engine', 'ejs');
app.use(express.urlencoded({ extended: true }));
app.use(express.static('public'));

// Cached access token for Azure OpenAI
let token: string | undefined;

// Status endpoint for demonstration purposes
app.get('/status', async (req: Request, res: Response) => {
  let returnedToken = 'no cached token';
  if (token) {
    // NEVER, EVER DO THIS IN PRODUCTION. This is just for demo and learning purposes.
    returnedToken = `${token.substring(0, Math.min(token.length, 15))}...`;
  }

  const apiKey = process.env.OAI_AZURE_API_KEY;
  let returnedApiKey = 'no api key';
  if (apiKey) {
    // NEVER, EVER DO THIS IN PRODUCTION. This is just for demo and learning purposes.
    returnedApiKey = `${apiKey.substring(0, Math.min(apiKey.length, 5))}...`;
  }

  const status = {
    token: returnedToken,
    remainingTime: !token ? undefined : getRemainingTime(token) ?? 0,
    endpoint: process.env.OAI_AZURE_ENDPOINT,
    deployment: process.env.OAI_AZURE_DEPLOYMENT,
    apiKey: returnedApiKey,
    azureServiceIP: await resolveIPAddress(process.env.OAI_AZURE_ENDPOINT ?? ''),
  };
  res.json(status);
});

app.get('/', (req: Request, res: Response) => {
  res.render('form');
});

app.post('/submit', async (req: Request, res: Response) => {
  try {
    const inputData = req.body.inputField;
    const useApiKey = req.body.useApiKey;

    if (!useApiKey && (!token || (getRemainingTime(token) ?? 0) < 60)) {
      // Refresh token
      const tokenProvider = new DefaultAzureCredential();
      const accessToken = await tokenProvider.getToken('https://cognitiveservices.azure.com');
      token = accessToken?.token;
    }

    const client = new OpenAI({
      baseURL: `https://${process.env.OAI_AZURE_ENDPOINT}/openai/deployments/${process.env.OAI_AZURE_DEPLOYMENT}`,
      apiKey: useApiKey ? process.env.OAI_AZURE_API_KEY : '',
      defaultHeaders: {
        Authorization: `Bearer ${token}`,
        'api-key': useApiKey ? process.env.OAI_AZURE_API_KEY : '',
      },
      defaultQuery: {
        'api-version': '2024-02-15-preview',
      }
    });

    const response = await client.chat.completions.create({
      messages:
        [
          { role: "system", content: "You are a helpful assistant." },
          { role: "user", content: inputData },
        ],
      model: process.env.OAI_AZURE_DEPLOYMENT ?? ''
    }, undefined);

    res.render('result', { answer: response.choices[0].message.content });
  } catch (error) {
    res.render('error', { error: error });
  }
});

const PORT = process.env.PORT;
app.listen(PORT, () => console.log(`Server running on http://localhost:${PORT}`));
