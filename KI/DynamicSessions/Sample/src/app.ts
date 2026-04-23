import { randomUUID } from "node:crypto";
import { mkdir } from "node:fs/promises";
import { join } from "node:path";
import {
  downloadFileFromSession,
  executePython,
  getAccessToken,
  startDynamicSession,
  stopDynamicSession,
  uploadFileToSession,
} from "./azureDynamicSessions.js";
import { ensureCustomerRevenueCsv } from "./csvGenerator.js";

const DATA_DIRECTORY = "data";
const SUMMARY_FILE_NAME = "customer-revenue-summary.csv";

async function main(): Promise<void> {
  const poolManagementEndpoint = requireEnv("POOL_MANAGEMENT_ENDPOINT");
  const sessionId = process.env.DYNAMIC_SESSION_ID ?? `sample-${randomUUID()}`;
  const dataDirectory = join(process.cwd(), DATA_DIRECTORY);
  const summaryFilePath = join(dataDirectory, SUMMARY_FILE_NAME);

  console.log(`Using session ID: ${sessionId}`);
  await mkdir(dataDirectory, { recursive: true });

  const csvResult = await ensureCustomerRevenueCsv(dataDirectory);
  console.log(
    csvResult.created
      ? `Generated input CSV at ${csvResult.filePath}.`
      : `Reusing input CSV at ${csvResult.filePath}.`,
  );

  const accessToken = await getAccessToken();

  console.log("Starting dynamic session...");
  await startDynamicSession(poolManagementEndpoint, sessionId, accessToken);

  console.log("Uploading CSV to session...");
  await uploadFileToSession(poolManagementEndpoint, sessionId, accessToken, csvResult.filePath);

  console.log("Running Python code...");
  const executionResult = await executePython(
    poolManagementEndpoint,
    sessionId,
    accessToken,
    [
      "from pathlib import Path",
      "",
      "import pandas as pd",
      "",
      "input_path = Path('/mnt/data/customer-revenue.csv')",
      "output_path = Path('/mnt/data/customer-revenue-summary.csv')",
      "",
      "dataframe = pd.read_csv(input_path)",
      "summary = (",
      "    dataframe.groupby('Year', as_index=False)['Revenue']",
      "    .mean()",
      "    .rename(columns={'Revenue': 'Average Revenue per Customer'})",
      ")",
      "summary['Average Revenue per Customer'] = summary['Average Revenue per Customer'].map(",
      "    lambda value: f'{value:.2f}'",
      ")",
      "summary.to_csv(output_path, index=False)",
      "",
      "print(f'Wrote summary to {output_path}')",
    ].join("\n"),
  );
  console.dir(executionResult, { depth: null });

  console.log("Downloading summary file...");
  await downloadFileFromSession(
    poolManagementEndpoint,
    sessionId,
    accessToken,
    SUMMARY_FILE_NAME,
    summaryFilePath,
  );
  console.log(`Saved summary to ${summaryFilePath}`);

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
