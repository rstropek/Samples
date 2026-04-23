import { randomUUID } from "node:crypto";
import {
  executePython,
  getAccessToken,
  startDynamicSession,
  stopDynamicSession,
} from "./azureDynamicSessions.js";

async function main(): Promise<void> {
  const poolManagementEndpoint = requireEnv("POOL_MANAGEMENT_ENDPOINT");
  const sessionId = process.env.DYNAMIC_SESSION_ID ?? `sample-${randomUUID()}`;

  console.log(`Using session ID: ${sessionId}`);

  const accessToken = await getAccessToken();

  console.log("Starting dynamic session...");
  await startDynamicSession(poolManagementEndpoint, sessionId, accessToken);

  console.log("Running Python code...");
  const executionResult = await executePython(
    poolManagementEndpoint,
    sessionId,
    accessToken,
    "print('hello world')",
  );
  console.dir(executionResult, { depth: null });

  await stopDynamicSession(poolManagementEndpoint, sessionId, accessToken);
}

function requireEnv(name: string): string {
  const value = process.env[name];
  if (!value) {
    throw new Error(`Missing required environment variable: ${name}`);
  }

  return value;
}

main().catch((error: unknown) => {
  console.error(error);
  process.exit(1);
});
