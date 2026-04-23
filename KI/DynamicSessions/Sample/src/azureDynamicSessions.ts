import { readFile, writeFile } from "node:fs/promises";
import { basename } from "node:path";
import { DefaultAzureCredential } from "@azure/identity";
import type { RequestInit } from "undici";
import { FormData, fetch } from "undici";

const TENANT_ID = "022e4faf-c745-475a-be06-06b1e1c9e39d";
const TOKEN_SCOPE = "https://dynamicsessions.io/.default";
const EXECUTIONS_API_VERSION = "2025-10-02-preview";
const FILES_API_VERSION = "2025-10-02-preview";
const SESSION_DELETE_API_VERSION = "2025-10-02-preview";

type JsonValue = null | boolean | number | string | JsonValue[] | { [key: string]: JsonValue };

class HttpError extends Error {
  constructor(
    message: string,
    readonly status: number,
    readonly bodyText: string,
    readonly body: JsonValue,
  ) {
    super(message);
  }
}

export async function getAccessToken(): Promise<string> {
  const credential = new DefaultAzureCredential({ tenantId: TENANT_ID });
  const token = await credential.getToken(TOKEN_SCOPE, { tenantId: TENANT_ID });

  if (!token) {
    throw new Error("Failed to acquire an Azure access token.");
  }

  return token.token;
}

export async function startDynamicSession(
  poolManagementEndpoint: string,
  sessionId: string,
  accessToken: string,
): Promise<void> {
  // Code interpreter sessions are created implicitly on the first request for a new identifier.
  await executePython(poolManagementEndpoint, sessionId, accessToken, "pass");
}

export async function executePython(
  poolManagementEndpoint: string,
  sessionId: string,
  accessToken: string,
  code: string,
): Promise<JsonValue> {
  const url = buildUrl(poolManagementEndpoint, "/executions", {
    "api-version": EXECUTIONS_API_VERSION,
    identifier: sessionId,
  });
  const headers = {
    Authorization: `Bearer ${accessToken}`,
    "Content-Type": "application/json",
  };

  try {
    return await sendJsonRequest(url, {
      method: "POST",
      headers,
      body: JSON.stringify({
        properties: {
          codeInputType: "inline",
          executionType: "synchronous",
          code,
        },
      }),
    });
  } catch (error: unknown) {
    if (!shouldRetryWithFlatExecutionBody(error)) {
      throw error;
    }
  }

  return sendJsonRequest(url, {
    method: "POST",
    headers,
    body: JSON.stringify({
      codeInputType: "inline",
      executionType: "synchronous",
      code,
    }),
  });
}

export async function stopDynamicSession(
  poolManagementEndpoint: string,
  sessionId: string,
  accessToken: string,
): Promise<void> {
  const response = await fetch(
    buildUrl(poolManagementEndpoint, "/session", {
      "api-version": SESSION_DELETE_API_VERSION,
      identifier: sessionId,
    }),
    {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    },
  );

  if (!response.ok) {
    const responseText = await response.text();
    throw new Error(
      `Failed to stop session ${sessionId}. Status ${response.status} ${response.statusText}: ${responseText}`,
    );
  }

  console.log(`Stopped session ${sessionId}.`);
}

export async function uploadFileToSession(
  poolManagementEndpoint: string,
  sessionId: string,
  accessToken: string,
  localFilePath: string,
): Promise<JsonValue> {
  const fileContents = await readFile(localFilePath);
  const formData = new FormData();
  formData.append("file", new Blob([fileContents], { type: "text/csv" }), basename(localFilePath));

  return sendJsonRequest(
    buildUrl(poolManagementEndpoint, "/files", {
      "api-version": FILES_API_VERSION,
      identifier: sessionId,
    }),
    {
      method: "POST",
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
      body: formData,
    },
  );
}

export async function downloadFileFromSession(
  poolManagementEndpoint: string,
  sessionId: string,
  accessToken: string,
  remoteFileName: string,
  localFilePath: string,
): Promise<void> {
  const response = await fetch(
    buildUrl(poolManagementEndpoint, `/files/${encodeURIComponent(remoteFileName)}/content`, {
      "api-version": FILES_API_VERSION,
      identifier: sessionId,
    }),
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    },
  );

  if (!response.ok) {
    const responseText = await response.text();
    throw new Error(
      `Failed to download ${remoteFileName} from session ${sessionId}. Status ${response.status} ${response.statusText}: ${responseText}`,
    );
  }

  const fileContents = Buffer.from(await response.arrayBuffer());
  await writeFile(localFilePath, fileContents);
}

function buildUrl(
  poolManagementEndpoint: string,
  path: string,
  queryParams: Record<string, string>,
): URL {
  const normalizedEndpoint = poolManagementEndpoint.endsWith("/")
    ? poolManagementEndpoint
    : `${poolManagementEndpoint}/`;

  const url = new URL(path.replace(/^\//, ""), normalizedEndpoint);
  for (const [key, value] of Object.entries(queryParams)) {
    url.searchParams.set(key, value);
  }

  return url;
}

async function sendJsonRequest(url: URL, init: RequestInit): Promise<JsonValue> {
  const response = await fetch(url, init);
  const responseText = await response.text();
  const responseBody = parseJsonValue(responseText);

  if (!response.ok) {
    throw new HttpError(
      `Request failed with status ${response.status} ${response.statusText}: ${responseText}`,
      response.status,
      responseText,
      responseBody,
    );
  }

  return responseBody;
}

function parseJsonValue(responseText: string): JsonValue {
  if (!responseText) {
    return null;
  }

  return JSON.parse(responseText) as JsonValue;
}

function shouldRetryWithFlatExecutionBody(error: unknown): boolean {
  if (!(error instanceof HttpError) || error.status !== 400) {
    return false;
  }

  const responseError = getObjectProperty(error.body, "error");
  return getStringProperty(responseError, "code") === "SessionPropertiesMissing";
}

function getObjectProperty(
  value: JsonValue,
  propertyName: string,
): { [key: string]: JsonValue } | null {
  if (typeof value !== "object" || value === null || Array.isArray(value)) {
    return null;
  }

  const propertyValue = value[propertyName];
  if (typeof propertyValue !== "object" || propertyValue === null || Array.isArray(propertyValue)) {
    return null;
  }

  return propertyValue;
}

function getStringProperty(
  value: { [key: string]: JsonValue } | null,
  propertyName: string,
): string | null {
  if (!value) {
    return null;
  }

  const propertyValue = value[propertyName];
  return typeof propertyValue === "string" ? propertyValue : null;
}
