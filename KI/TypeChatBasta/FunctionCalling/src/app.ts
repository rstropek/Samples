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

    const messages: OpenAI.ChatCompletionMessageParam[] = [
        {
            role: "system",
            content: `
            You are a helpful assistant answering questions about the
            BASTA IT conference. The conference is between Feb. 12th 2024
            and Feb. 16th 2024. The conference is in Frankfurt, Germany.
            `,
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
        const userMessage = await ask(rl, "Enter your message (Ctrl+c to exit): ");

        messages.push({
            role: "user",
            content: userMessage,
        });

        let repeat: boolean;
        do {
            repeat = false;
            const response = await client.chat.completions.create({
                model: process.env.AZURE_OPENAI_DEPLOYMENT ?? "",
                messages,
                tools: [
                    {
                        type: "function",
                        function: {
                            name: "findExperts",
                            description: "Finds an expert using their name",
                            parameters: {
                                type: "object",
                                properties: {
                                    surname: {
                                        type: "string",
                                        description: "The surname of the expert",
                                    },
                                    forename: {
                                        type: "string",
                                        description: "The forename of the expert",
                                    },
                                },
                                required: [],
                            },
                        },
                    },
                    {
                        type: "function",
                        function: {
                            name: "findSessions",
                            description: `
                                Finds a conference session based on expert name and/or part of the session title.
                                If expert name filter is used, always pass the surname and forename.
                                `,
                            parameters: {
                                type: "object",
                                properties: {
                                    surname: {
                                        type: "string",
                                        description: "The surname of the expert giving the talk",
                                    },
                                    forename: {
                                        type: "string",
                                        description: "The forename of the expert giving the talk",
                                    },
                                    title: {
                                        type: "string",
                                        description: "Part of the session title",
                                    },
                                },
                                required: [],
                            },
                        },
                    },
                    {
                        type: "function",
                        function: {
                            name: "addSessionToCalendar",
                            description: "Adds a session to the user's calendar",
                            parameters: {
                                type: "object",
                                properties: {
                                    sessionTitle: {
                                        type: "string",
                                        description: "Title of the session to add to the calendar",
                                    },
                                },
                                required: ["sessionTitle"],
                            },
                        },
                    },
                ],
            });

            messages.push(response.choices[0].message);
            if (response.choices[0].message.tool_calls) {
                console.log("\nGot tool call(s)");
                for (const call of response.choices[0].message.tool_calls) {
                    repeat = true;
                    console.log(`\tGot function call to ${call.function.name} (${call.function.arguments.substring(0, 40)})`);
                    switch (call.function.name) {
                        case "findExperts":
                            const expertFilter = JSON.parse(call.function.arguments);
                            const experts = conferenceProgram.findExperts(expertFilter);
                            messages.push({
                                role: "tool",
                                tool_call_id: call.id,
                                content: JSON.stringify(experts),
                            });
                            break;
                        case "findSessions":
                            const sessionFilter = JSON.parse(call.function.arguments);
                            const sessions = conferenceProgram.findSessions(sessionFilter);
                            messages.push({
                                role: "tool",
                                tool_call_id: call.id,
                                content: JSON.stringify(sessions),
                            });
                            break;
                        case "addSessionToCalendar":
                            const sessionTitle = JSON.parse(call.function.arguments) as { sessionTitle: string };
                            console.log(`\t\tSession "${sessionTitle.sessionTitle.substring(0, 40)}" added to calendar.`);
                            messages.push({
                                role: "tool",
                                tool_call_id: call.id,
                                content: `Session "${sessionTitle.sessionTitle}" added to calendar.`,
                            });
                            break;
                    }
                }
            } else {
                console.log();
                console.log(response.choices[0].message.content);
                console.log();
            }
        } while (repeat);
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
