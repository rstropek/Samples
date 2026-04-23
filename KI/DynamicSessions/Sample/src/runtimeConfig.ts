import { randomUUID } from "node:crypto";
import { join } from "node:path";

export const DATA_DIRECTORY_NAME = "data";

export type RuntimeConfig = {
  dataDirectory: string;
  poolManagementEndpoint: string;
  sessionId: string;
  tenantId: string | undefined;
};

export function getRuntimeConfig(mode: string): RuntimeConfig {
  return {
    dataDirectory: join(process.cwd(), DATA_DIRECTORY_NAME),
    poolManagementEndpoint: requireEnv("POOL_MANAGEMENT_ENDPOINT"),
    sessionId: process.env.DYNAMIC_SESSION_ID ?? `${mode}-${randomUUID()}`,
    tenantId: process.env.AZURE_TENANT_ID,
  };
}

export function requireEnv(name: string): string {
  const value = process.env[name];
  if (!value) {
    throw new Error(`Missing required environment variable: ${name}`);
  }

  return value;
}
