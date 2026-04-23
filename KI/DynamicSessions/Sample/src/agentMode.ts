import { mkdir } from "node:fs/promises";
import { basename, join } from "node:path";
import { stdin as input, stdout as output } from "node:process";
import { createInterface } from "node:readline/promises";
import { Agent, MemorySession, run, setTracingDisabled, tool } from "@openai/agents";
import { z } from "zod";
import {
  downloadFileFromSession,
  executePython,
  extractExecutionOutput,
  getAccessToken,
  listFilesInSession,
  type SessionFile,
  startDynamicSession,
  stopDynamicSession,
  uploadFileToSession,
} from "./azureDynamicSessions.js";
import { ensureCustomerRevenueCsv, INPUT_FILE_NAME } from "./csvGenerator.js";
import { getRuntimeConfig, requireEnv } from "./runtimeConfig.js";

const EXIT_COMMANDS = new Set(["exit", "quit", "/exit"]);
const COLORS = {
  blue: "\u001B[34m",
  cyan: "\u001B[36m",
  dim: "\u001B[2m",
  gray: "\u001B[37m",
  green: "\u001B[32m",
  magenta: "\u001B[35m",
  red: "\u001B[31m",
  reset: "\u001B[0m",
  yellow: "\u001B[33m",
};

export async function runAgentMode(): Promise<void> {
  requireEnv("OPENAI_API_KEY");
  setTracingDisabled(true);

  const { dataDirectory, poolManagementEndpoint, sessionId } = getRuntimeConfig("agent");
  await mkdir(dataDirectory, { recursive: true });

  const csvResult = await ensureCustomerRevenueCsv(dataDirectory);
  const uploadedCsvFileName = basename(csvResult.filePath);
  const downloadedFileNames = new Set([uploadedCsvFileName]);
  const accessToken = await getAccessToken();

  logSection("Agent Mode");
  logInfo(`Using session ID: ${sessionId}`);
  logInfo(
    csvResult.created
      ? `Generated input CSV at ${csvResult.filePath}.`
      : `Reusing input CSV at ${csvResult.filePath}.`,
  );
  logDebug(`Dynamic session endpoint: ${poolManagementEndpoint}`);
  logDebug(`Session CSV path inside sandbox: /mnt/data/${uploadedCsvFileName}`);

  logSection("Session Startup");
  logInfo("Starting dynamic session...");
  await startDynamicSession(poolManagementEndpoint, sessionId, accessToken);
  logSuccess("Dynamic session is ready.");

  try {
    logInfo("Uploading CSV to dynamic session...");
    await uploadFileToSession(poolManagementEndpoint, sessionId, accessToken, csvResult.filePath);
    logSuccess(`Uploaded ${uploadedCsvFileName} to /mnt/data/${uploadedCsvFileName}.`);

    const revenueAgent = new Agent({
      name: "Revenue Analyst",
      instructions: [
        "You are a data analyst working inside an Azure Container Apps dynamic session.",
        "The uploaded CSV file is available at /mnt/data/customer-revenue.csv.",
        "Use the execute_python_in_dynamic_session tool whenever you need to inspect or process data.",
        "For any question that depends on the CSV contents, use the tool instead of guessing.",
        "The tool only returns stdout and stderr, so your script must print any values you need.",
        "If you generate files such as CSV outputs, charts, or images, you MUST write them into /mnt/data.",
        "Use pandas for tabular work unless there is a strong reason not to.",
        "Keep responses concise and answer based on actual tool results.",
      ].join(" "),
      model: "gpt-5.4",
      tools: [
        tool({
          name: "execute_python_in_dynamic_session",
          description:
            "Execute a Python script inside the current Azure dynamic session and return stdout and stderr.",
          parameters: z.object({
            script: z
              .string()
              .describe("A complete Python script to execute inside the dynamic session."),
          }),
          async execute({ script }) {
            logSection("Tool Call");
            logInfo("Agent requested Python execution in the dynamic session.");
            logDebugBlock("Generated Python Script", script);

            try {
              logDebug("POST /executions -> Azure Container Apps dynamic sessions");
              const executionResponse = await executePython(
                poolManagementEndpoint,
                sessionId,
                accessToken,
                script,
              );
              const executionOutput = extractExecutionOutput(executionResponse);
              logDebugBlock("Dynamic Session stdout", executionOutput.stdout || "(empty)");
              logDebugBlock("Dynamic Session stderr", executionOutput.stderr || "(empty)");
              if (executionOutput.executionResult) {
                logDebugBlock("Dynamic Session executionResult", executionOutput.executionResult);
              }

              return {
                stderr: executionOutput.stderr,
                stdout: executionOutput.stdout || executionOutput.executionResult,
              };
            } catch (error: unknown) {
              logError("Dynamic session execution failed.");
              logDebugBlock(
                "Execution error",
                error instanceof Error ? error.message : String(error),
              );
              return {
                stderr: error instanceof Error ? error.message : String(error),
                stdout: "",
              };
            }
          },
        }),
      ],
    });
    const conversationSession = new MemorySession();
    const terminal = createInterface({ input, output });

    logSection("Interactive REPL");
    logInfo("Agent mode ready. Ask a question about the uploaded CSV.");
    logInfo("Type 'exit' to quit.");

    try {
      while (true) {
        let userInput: string;
        try {
          userInput = (await terminal.question(colorize("> ", COLORS.cyan))).trim();
        } catch {
          break;
        }

        if (!userInput) {
          continue;
        }

        if (EXIT_COMMANDS.has(userInput.toLowerCase())) {
          break;
        }

        logSection("User Turn");
        logUser(userInput);
        logInfo("Running agent...");
        const result = await run(revenueAgent, userInput, {
          session: conversationSession,
        });
        logAssistant(formatAgentResponse(result.finalOutput));

        logSection("Session Sync");
        logInfo("Listing files in the dynamic session...");
        const sessionFiles = await listFilesInSession(
          poolManagementEndpoint,
          sessionId,
          accessToken,
        );
        await downloadNewSessionFiles(
          sessionFiles,
          downloadedFileNames,
          poolManagementEndpoint,
          sessionId,
          accessToken,
          dataDirectory,
        );
      }
    } finally {
      terminal.close();
    }
  } finally {
    logSection("Session Shutdown");
    await stopDynamicSession(poolManagementEndpoint, sessionId, accessToken);
  }
}

async function downloadNewSessionFiles(
  sessionFiles: SessionFile[],
  downloadedFileNames: Set<string>,
  poolManagementEndpoint: string,
  sessionId: string,
  accessToken: string,
  dataDirectory: string,
): Promise<void> {
  const fileNames = sessionFiles.map((file) => file.name);
  logDebug(`Session files: ${fileNames.length > 0 ? fileNames.join(", ") : "(none)"}`);

  let downloadedFileCount = 0;

  for (const fileName of fileNames) {
    if (fileName === INPUT_FILE_NAME || downloadedFileNames.has(fileName)) {
      continue;
    }

    const localFilePath = join(dataDirectory, fileName);
    await downloadFileFromSession(
      poolManagementEndpoint,
      sessionId,
      accessToken,
      fileName,
      localFilePath,
    );
    downloadedFileNames.add(fileName);
    downloadedFileCount += 1;
    logSuccess(`Downloaded generated file to ${localFilePath}`);
  }

  if (downloadedFileCount === 0) {
    logInfo("No new generated files to download.");
  }
}

function formatAgentResponse(finalOutput: unknown): string {
  if (typeof finalOutput === "string") {
    return finalOutput;
  }

  if (finalOutput === undefined || finalOutput === null) {
    return "";
  }

  return JSON.stringify(finalOutput, null, 2);
}

function logSection(title: string): void {
  console.log(colorize(`\n=== ${title} ===`, COLORS.blue));
}

function logInfo(message: string): void {
  console.log(colorize(message, COLORS.cyan));
}

function logSuccess(message: string): void {
  console.log(colorize(message, COLORS.green));
}

function logError(message: string): void {
  console.log(colorize(message, COLORS.red));
}

function logUser(message: string): void {
  console.log(`${colorize("User:", COLORS.yellow)} ${message}`);
}

function logAssistant(message: string): void {
  console.log(`${colorize("Agent:", COLORS.magenta)} ${message}`);
}

function logDebug(message: string): void {
  console.log(colorize(`[debug] ${message}`, COLORS.gray, COLORS.dim));
}

function logDebugBlock(title: string, content: string): void {
  logDebug(`${title}:`);
  for (const line of content.split("\n")) {
    console.log(colorize(`  ${line}`, COLORS.gray, COLORS.dim));
  }
}

function colorize(text: string, ...styles: string[]): string {
  if (!output.isTTY) {
    return text;
  }

  return `${styles.join("")}${text}${COLORS.reset}`;
}
