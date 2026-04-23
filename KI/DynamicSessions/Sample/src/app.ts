import { runAgentMode } from "./agentMode.js";
import { runSimpleMode } from "./simpleMode.js";

const SUPPORTED_MODES = new Set(["agent", "simple"]);

async function main(): Promise<void> {
  const mode = (process.argv[2] ?? "simple").toLowerCase();
  if (!SUPPORTED_MODES.has(mode)) {
    throw new Error(`Unsupported mode "${mode}". Use "simple" or "agent".`);
  }

  if (mode === "agent") {
    await runAgentMode();
    return;
  }

  await runSimpleMode();
}

main().catch((error: unknown) => {
  console.error(error);
  process.exit(1);
});
