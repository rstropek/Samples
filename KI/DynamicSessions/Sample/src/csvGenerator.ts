import { access, mkdir, writeFile } from "node:fs/promises";
import { join } from "node:path";

const TOTAL_ROWS = 10_000;
const FIRST_YEAR_ROW_COUNT = 5_000;
const MIN_REVENUE = 10_000;
const MAX_REVENUE = 12_000;
export const INPUT_FILE_NAME = "customer-revenue.csv";

export type CustomerRevenueCsvResult = {
  created: boolean;
  filePath: string;
};

export async function ensureCustomerRevenueCsv(
  dataDirectory: string,
): Promise<CustomerRevenueCsvResult> {
  await mkdir(dataDirectory, { recursive: true });

  const outputPath = join(dataDirectory, INPUT_FILE_NAME);
  if (await fileExists(outputPath)) {
    return {
      created: false,
      filePath: outputPath,
    };
  }

  const rows = ["Customer ID,Year,Revenue"];

  for (let index = 0; index < TOTAL_ROWS; index += 1) {
    const customerId = `C${String(index + 1).padStart(5, "0")}`;
    const year = index < FIRST_YEAR_ROW_COUNT ? 2025 : 2026;
    const revenue = randomRevenue();
    rows.push(`${customerId},${year},${revenue.toFixed(2)}`);
  }

  await writeFile(outputPath, `${rows.join("\n")}\n`, "utf-8");
  return {
    created: true,
    filePath: outputPath,
  };
}

function randomRevenue(): number {
  return MIN_REVENUE + Math.random() * (MAX_REVENUE - MIN_REVENUE);
}

async function fileExists(filePath: string): Promise<boolean> {
  try {
    await access(filePath);
    return true;
  } catch {
    return false;
  }
}
