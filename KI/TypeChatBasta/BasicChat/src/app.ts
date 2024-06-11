import path from "path";
import readline from "readline";
import OpenAI from "openai";
import dotenv from "dotenv";
import { ConferenceProgram } from "./sessions";

async function run() {
    // Load environment variables from .env file
    dotenv.config({ path: path.join(__dirname, "../../.env") });

    // Read the conference program from the JSON file
    const conferenceProgram = ConferenceProgram.readFromFile();

    // Create the OpenAI client.
    // Note that we are using Azure OpenAI here. In production environments,
    // do NOT use API keys. Use managed identities instead!
    const client = new OpenAI({
        baseURL: process.env.AZURE_OPENAI_BASE_URL,
        apiKey: process.env.AZURE_OPENAI_API_KEY,
        defaultHeaders: {
            "api-key": process.env.AZURE_OPENAI_API_KEY,
        },
        defaultQuery: {
            "api-version": "2023-12-01-preview",
        },
    });

    // In this example, we want to demonstrate simple chat capabilities
    // related to sessions of an IT conference. Therefore, we add a system
    // message that includes the conference program.
    //
    // Big disadvantage: Large prompt even though the conference program
    // was limited to session names.
    const messages: OpenAI.ChatCompletionMessageParam[] = [
        {
            role: "system",
            content:
                `
            You are a helpful assistant answering questions about the
            BASTA IT conference. The conference is between Feb. 12th 2024
            and Feb. 16th 2024. The conference is in Frankfurt, Germany.
            Here are the sessions of the conference:
            ` +
                conferenceProgram
                    .getSessionTitles()
                    .map((s) => `* ${s}`)
                    .join("\n"),
        },
        {
            role: "assistant",
            content: `How can I help you?`,
        },
    ];

    console.log(messages[messages.length - 1].content);
    console.log();

    const rl = readline.createInterface({
        input: process.stdin,
        output: process.stdout,
    });
    while (true) {
        // Ask the user for a question
        const userMessage = await ask(rl, "Enter your message (Ctrl+c to exit): ");

        // Add the user message to the conversation history
        messages.push({ role: "user", content: userMessage });

        // Send the conversation to OpenAI to generate an answer
        const response = await client.chat.completions.create({
            model: process.env.AZURE_OPENAI_DEPLOYMENT ?? "",
            messages,
        });

        // Add the response to the conversation history
        messages.push(response.choices[0].message);

        // Print the response
        console.log();
        console.log(response.choices[0].message.content);
        console.log();
    }
}

function ask(readline: readline.Interface, prompt: string): Promise<string> {
    return new Promise((resolve) => {
        readline.question(prompt, (line) => {
            resolve(line);
        });
    });
}

run();
