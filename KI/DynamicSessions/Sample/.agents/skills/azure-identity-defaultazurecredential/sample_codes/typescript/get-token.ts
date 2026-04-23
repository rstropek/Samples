import { DefaultAzureCredential } from "@azure/identity";

const tenantId = "...";
const scope = "https://management.azure.com/.default";

async function main(): Promise<void> {
  const credential = new DefaultAzureCredential({ tenantId });
  const token = await credential.getToken(scope, { tenantId });

  console.log("tenantId:", tenantId);
  console.log("scope:", scope);
  console.log("expiresOnTimestamp:", token?.expiresOnTimestamp);
  console.log("tokenPrefix:", token?.token.slice(0, 20));
}

main().catch((error) => {
  console.error(error);
  process.exit(1);
});
