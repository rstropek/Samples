"use server";

import OpenAI from "openai";
import { InvoiceDataSchema, type InvoiceData, type Language, type ProcessingResult } from "./types";

const openai = new OpenAI({
  apiKey: process.env.OPENAI_API_KEY,
});

// Email templates for different languages
const emailTemplates = {
  en: {
    subject: "Invoice Submission - {supplier}",
    body: `Dear Accounting Team,

Please process the attached invoice with the following details:

Supplier: {supplier}
Description: {description}
Invoice Date: {invoiceDate}
Total Amount: {totalPrice} {currency}
Tax Amount: {totalTax} {currency}

Best regards`,
  },
  de: {
    subject: "Rechnungseinreichung - {supplier}",
    body: `Sehr geehrtes Buchhaltungsteam,

bitte bearbeiten Sie die beigefügte Rechnung mit folgenden Details:

Lieferant: {supplier}
Beschreibung: {description}
Rechnungsdatum: {invoiceDate}
Gesamtbetrag: {totalPrice} {currency}
Steuerbetrag: {totalTax} {currency}

Mit freundlichen Grüßen`,
  },
  es: {
    subject: "Envío de Factura - {supplier}",
    body: `Estimado Equipo de Contabilidad,

Por favor procesen la factura adjunta con los siguientes detalles:

Proveedor: {supplier}
Descripción: {description}
Fecha de Factura: {invoiceDate}
Monto Total: {totalPrice} {currency}
Monto de Impuesto: {totalTax} {currency}

Saludos cordiales`,
  },
  fr: {
    subject: "Soumission de Facture - {supplier}",
    body: `Cher Équipe Comptable,

Veuillez traiter la facture ci-jointe avec les détails suivants:

Fournisseur: {supplier}
Description: {description}
Date de Facture: {invoiceDate}
Montant Total: {totalPrice} {currency}
Montant de Taxe: {totalTax} {currency}

Cordialement`,
  },
};

function generateEmailDraft(
  data: InvoiceData,
  language: Language
): string {
  if (!data.isInvoice) {
    return "This document is not an invoice. No email draft can be generated.";
  }

  const template = emailTemplates[language];
  const currency = data.currency || "";
  
  const replacements: Record<string, string> = {
    "{supplier}": data.supplier && data.supplier !== "Unknown" ? data.supplier : "Unknown",
    "{description}": data.description && data.description !== "N/A" ? data.description : "N/A",
    "{invoiceDate}": data.invoiceDate || "N/A",
    "{totalPrice}": data.totalPrice && data.totalPrice > 0 ? data.totalPrice.toFixed(2) : "N/A",
    "{totalTax}": data.totalTax && data.totalTax > 0 ? data.totalTax.toFixed(2) : "N/A",
    "{currency}": currency,
  };

  let subject = template.subject;
  let body = template.body;

  for (const [key, value] of Object.entries(replacements)) {
    subject = subject.replace(key, value);
    body = body.replace(new RegExp(key.replace(/[{}]/g, "\\$&"), "g"), value);
  }

  return `Subject: ${subject}\n\n${body}`;
}

export async function processInvoice(
  formData: FormData
): Promise<ProcessingResult> {
  try {
    // Extract file and language from form data
    const file = formData.get("file") as File;
    const language = (formData.get("language") as Language) || "en";

    if (!file) {
      return { success: false, error: "No file provided" };
    }

    // Validate file size (3MB max)
    const maxSize = 3 * 1024 * 1024;
    if (file.size > maxSize) {
      return { success: false, error: "File size exceeds 3MB limit" };
    }

    // Validate file type
    const validTypes = ["application/pdf", "image/jpeg", "image/jpg", "image/png"];
    if (!validTypes.includes(file.type)) {
      return {
        success: false,
        error: "Invalid file type. Only PDF and JPG/JPEG files are accepted",
      };
    }

    // Convert file to base64
    const buffer = await file.arrayBuffer();
    const base64 = Buffer.from(buffer).toString("base64");
    
    // Determine media type
    const mediaType = file.type === "application/pdf" 
      ? "image/jpeg" // For PDFs, we'll rely on OpenAI to handle it
      : file.type;

    // Define JSON schema for structured output
    const jsonSchema = {
      type: "object" as const,
      properties: {
        isInvoice: {
          type: "boolean" as const,
          description: "Whether the document is an invoice",
        },
        supplier: {
          type: "string" as const,
          description: "Name of the supplier or vendor, or 'Unknown' if not available",
        },
        description: {
          type: "string" as const,
          description: "Brief summary of the purchase, or 'N/A' if not available",
        },
        totalPrice: {
          type: "number" as const,
          description: "Total price including tax, or 0 if not available",
        },
        totalTax: {
          type: "number" as const,
          description: "Total tax amount, or 0 if not available",
        },
        invoiceDate: {
          type: "string" as const,
          description: "Invoice date in ISO 8601 format (YYYY-MM-DD), or empty string if not available",
        },
        currency: {
          type: "string" as const,
          description: "Currency code (e.g., USD, EUR, GBP), or empty string if not detected",
        },
      },
      required: ["isInvoice", "supplier", "description", "totalPrice", "totalTax", "invoiceDate", "currency"],
      additionalProperties: false,
    };

    // Call OpenAI API with vision and structured outputs
    const completion = await openai.chat.completions.create({
      model: "gpt-4o-2024-08-06",
      messages: [
        {
          role: "system",
          content: `You are an expert invoice analysis specialist. Your task is to:
1. First determine if the document is an invoice (set isInvoice to true or false)
2. If it is an invoice, extract all relevant information accurately
3. Handle invoices in any language
4. For missing or unclear fields, use these defaults:
   - supplier: "Unknown"
   - description: "N/A"
   - totalPrice: 0
   - totalTax: 0
   - invoiceDate: "" (empty string)
   - currency: "" (empty string)
5. Parse dates into ISO 8601 format (YYYY-MM-DD)
6. Parse numerical values as numbers (not strings)
7. Detect and include the currency code if present (e.g., USD, EUR, GBP)`,
        },
        {
          role: "user",
          content: [
            {
              type: "text",
              text: "Analyze this document and extract invoice information. If it's not an invoice, set isInvoice to false.",
            },
            {
              type: "image_url",
              image_url: {
                url: `data:${mediaType};base64,${base64}`,
              },
            },
          ],
        },
      ],
      response_format: {
        type: "json_schema",
        json_schema: {
          name: "invoice_data",
          schema: jsonSchema,
          strict: true,
        },
      },
      max_completion_tokens: 1000,
    });

    const message = completion.choices[0].message;
    
    // Parse and validate the response with Zod
    let invoiceData: InvoiceData;
    try {
      const content = message.content;
      if (!content) {
        throw new Error("No content in response");
      }
      const parsedContent = JSON.parse(content);
      invoiceData = InvoiceDataSchema.parse(parsedContent);
    } catch (parseError) {
      return {
        success: false,
        error: "Failed to parse invoice data from OpenAI response",
      };
    }

    // Generate email draft
    const emailDraft = generateEmailDraft(invoiceData, language);

    return {
      success: true,
      data: invoiceData,
      emailDraft,
    };
  } catch (error) {
    console.error("Error processing invoice:", error);
    
    if (error instanceof Error) {
      return {
        success: false,
        error: `Failed to process invoice: ${error.message}`,
      };
    }
    
    return {
      success: false,
      error: "An unexpected error occurred while processing the invoice",
    };
  }
}

