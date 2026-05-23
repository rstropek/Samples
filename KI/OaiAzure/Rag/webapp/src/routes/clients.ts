import { DefaultAzureCredential, getBearerTokenProvider } from "@azure/identity";
import { SearchIndexClient } from "@azure/search-documents";
import OpenAI, { AzureOpenAI } from 'openai';

const searchEndpoint = process.env.SEARCH_ENDPOINT || "https://srch-fw7iqasihm4s4.search.windows.net";

export function getSearchClient(): SearchIndexClient {
    const credentials = new DefaultAzureCredential();
    const indexClient = new SearchIndexClient(searchEndpoint, credentials);
    return indexClient;
}

const credential = new DefaultAzureCredential();
const scope = 'https://cognitiveservices.azure.com/.default';
const azureADTokenProvider = getBearerTokenProvider(credential, scope);

const openaiEndpoint = process.env.AZURE_ENDPOINT || "https://fw7iqasihm4s4.openai.azure.com/";
const model = process.env.AZURE_DEPLOYMENT || "gpt-4.1";

export function getOpenAIClient(): OpenAI {
    const client = new AzureOpenAI({
        endpoint: openaiEndpoint,
        azureADTokenProvider: azureADTokenProvider,
        apiVersion: '2025-04-01-preview',
        deployment: model,
    });
    return client;
}
