import { z } from 'zod';

// Zod schema for invoice data validation
export const invoiceDataSchema = z.object({
  isInvoice: z.boolean().describe('Whether the document is an invoice'),
  supplier: z.string().nullable().describe('The name of the supplier/vendor'),
  description: z.string().nullable().describe('A summary of what was purchased'),
  totalPrice: z.number().nullable().describe('The total price including tax'),
  totalTax: z.number().nullable().describe('The total tax amount'),
  invoiceDate: z.string().nullable().describe('The invoice date in ISO 8601 format (YYYY-MM-DD)'),
  currency: z.string().optional().describe('The currency code (e.g., EUR, USD)')
});

// TypeScript type derived from schema
export type InvoiceData = z.infer<typeof invoiceDataSchema>;

// Language options for email drafts
export type EmailLanguage = 'en' | 'de' | 'es' | 'fr';

// Result type for the processing action
export interface ProcessingResult {
  success: boolean;
  data?: InvoiceData;
  emailDraft?: string;
  error?: string;
}

