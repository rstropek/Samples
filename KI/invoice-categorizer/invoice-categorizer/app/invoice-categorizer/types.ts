import { z } from "zod";

// Zod schema for invoice data validation
// Note: Using optional() with default values for better OpenAI structured outputs compatibility
export const InvoiceDataSchema = z.object({
  isInvoice: z.boolean().describe("Whether the document is an invoice"),
  supplier: z.string().describe("Name of the supplier or vendor, or 'Unknown' if not available"),
  description: z.string().describe("Brief summary of the purchase, or 'N/A' if not available"),
  totalPrice: z.number().describe("Total price including tax, or 0 if not available"),
  totalTax: z.number().describe("Total tax amount, or 0 if not available"),
  invoiceDate: z.string().describe("Invoice date in ISO 8601 format (YYYY-MM-DD), or empty string if not available"),
  currency: z.string().describe("Currency code (e.g., USD, EUR, GBP), or empty string if not detected"),
});

export type InvoiceData = z.infer<typeof InvoiceDataSchema>;

// Supported languages for email drafts
export const languages = ['en', 'de', 'es', 'fr'] as const;
export type Language = typeof languages[number];

// Result type for the processing action
export type ProcessingResult = {
  success: true;
  data: InvoiceData;
  emailDraft: string;
} | {
  success: false;
  error: string;
};

