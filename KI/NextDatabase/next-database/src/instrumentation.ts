import { SQLiteCustomerDB } from "@/lib/dbUtils";

export async function register() {
  const sqlite = new SQLiteCustomerDB("customer.db");
  await sqlite.connect();
  await sqlite.createCustomerTable();
  await sqlite.fillWithSampleData();
}