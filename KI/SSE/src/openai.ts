import OpenAI from 'openai';
import dotenv from 'dotenv';
import express, { Router } from "express";
import './openai-helpers.js';
import { Assistant } from 'openai/resources/beta/assistants.mjs';
import { TextDeltaBlock } from 'openai/resources/beta/threads/messages.mjs';

dotenv.config();

const openai = new OpenAI({ apiKey: process.env.OPENAI_API_KEY });

export async function createOrUpdateAssistant(): Promise<Assistant> {
  return await openai.beta.assistants.createOrUpdate({
    model: process.env.OPENAI_MODEL ?? 'gpt-4o',
    name: 'We Are Developers',
    description: 'Helps developers to argue why they must visit the We Are Developers conference.',
    instructions: `Support developers in arguing why they must visit the We Are Developers conference.
  They must be able to use your arguments to convince their managers to approve their attendance.
  Your suggestions should not be too serious. They should be fun and engaging. Feel free to 
  throw in some emojis.

  Do NOT answer any questions not related to why developers should visit the We Are Developers conference.`,
  });
}

export function route(assistant: Assistant): Router {
  const router = express.Router();

  router.get("/why-wad", async (_, response) => {
    response.writeHead(200, { "Content-Type": "text/event-stream" });

    const thread = await openai.beta.threads.create({
      messages: [
        { role: "user", content: "Give me five reasons that I can present to my manager why I have to go to We Are Developers conference." }
      ]
    });

    const stream = await openai.beta.threads.runs.create(thread.id, { assistant_id: assistant.id, stream: true });
    for await (const event of stream) {
      if (event.event === "thread.message.delta") {
        response.write(`data: ${JSON.stringify((event.data.delta.content![0] as TextDeltaBlock).text?.value)}\n\n`);
      }
    }

    response.write(`data\n\n`);
    response.end();
  });

  return router;
}
