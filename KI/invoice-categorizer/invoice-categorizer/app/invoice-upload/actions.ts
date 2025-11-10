'use server';

import OpenAI from 'openai';
import { invoiceDataSchema, type InvoiceData, type EmailLanguage, type ProcessingResult } from './types';

const openai = new OpenAI({
  apiKey: process.env.OPENAI_API_KEY,
});

// Email templates for different languages
const emailTemplates = {
  en: {
    subject: 'Invoice Submission - {supplier}',
    body: `Dear Accounting Team,

Please process the attached invoice with the following details:

Supplier: {supplier}
Description: {description}
Invoice Date: {invoiceDate}
Total Amount: {totalPrice}
Tax Amount: {totalTax}

Best regards`
  },
  de: {
    subject: 'Rechnungseinreichung - {supplier}',
    body: `Sehr geehrtes Buchhaltungsteam,

bitte verarbeiten Sie die angehängte Rechnung mit den folgenden Details:

Lieferant: {supplier}
Beschreibung: {description}
Rechnungsdatum: {invoiceDate}
Gesamtbetrag: {totalPrice}
Steuerbetrag: {totalTax}

Mit freundlichen Grüßen`
  },
  es: {
    subject: 'Envío de Factura - {supplier}',
    body: `Estimado equipo de contabilidad,

Por favor procesen la factura adjunta con los siguientes detalles:

Proveedor: {supplier}
Descripción: {description}
Fecha de factura: {invoiceDate}
Importe total: {totalPrice}
Importe de impuestos: {totalTax}

Saludos cordiales`
  },
  fr: {
    subject: 'Soumission de facture - {supplier}',
    body: `Cher équipe comptable,

Veuillez traiter la facture ci-jointe avec les détails suivants:

Fournisseur: {supplier}
Description: {description}
Date de facture: {invoiceDate}
Montant total: {totalPrice}
Montant de la taxe: {totalTax}

Cordialement`
  }
};

function generateEmailDraft(data: InvoiceData, language: EmailLanguage): string {
  if (!data.isInvoice) {
    return language === 'en' 
      ? 'This document does not appear to be an invoice.'
      : language === 'de'
      ? 'Dieses Dokument scheint keine Rechnung zu sein.'
      : language === 'es'
      ? 'Este documento no parece ser una factura.'
      : 'Ce document ne semble pas être une facture.';
  }

  const template = emailTemplates[language];
  const currencySymbol = data.currency || '';
  
  return template.body
    .replace('{supplier}', data.supplier || 'N/A')
    .replace('{description}', data.description || 'N/A')
    .replace('{invoiceDate}', data.invoiceDate || 'N/A')
    .replace('{totalPrice}', data.totalPrice !== null ? `${data.totalPrice} ${currencySymbol}` : 'N/A')
    .replace('{totalTax}', data.totalTax !== null ? `${data.totalTax} ${currencySymbol}` : 'N/A');
}

export async function processInvoice(
  formData: FormData
): Promise<ProcessingResult> {
  try {
    const file = formData.get('file') as File;
    const language = (formData.get('language') as EmailLanguage) || 'en';

    if (!file) {
      return {
        success: false,
        error: 'No file provided'
      };
    }

    // Validate file type
    const validTypes = ['application/pdf', 'image/jpeg', 'image/jpg'];
    if (!validTypes.includes(file.type)) {
      return {
        success: false,
        error: 'Invalid file type. Only PDF and JPG files are accepted.'
      };
    }

    // Validate file size (3MB max)
    const maxSize = 3 * 1024 * 1024; // 3MB in bytes
    if (file.size > maxSize) {
      return {
        success: false,
        error: 'File size exceeds 3MB limit.'
      };
    }

    // Convert file to base64
    const bytes = await file.arrayBuffer();
    const buffer = Buffer.from(bytes);
    const base64 = buffer.toString('base64');
    
    // Determine the media type
    const mediaType = file.type === 'application/pdf' ? 'application/pdf' : 'image/jpeg';

    // Call OpenAI API with structured outputs using JSON schema
    const completion = await openai.chat.completions.create({
      model: 'gpt-4o-mini',
      messages: [
        {
          role: 'system',
          content: `You are an expert invoice analysis specialist. Your task is to analyze document images and extract structured invoice data.

First, determine if the document is actually an invoice. If it is not an invoice, set isInvoice to false and return null for all other fields.

If it is an invoice, extract the following information:
- Supplier name
- Brief description of what was purchased
- Total price including tax
- Total tax amount
- Invoice date (in YYYY-MM-DD format)
- Currency code if visible

Handle edge cases:
- Multiple languages on invoices
- Partially visible information
- Different invoice formats
- Non-invoice documents

Be precise and only extract information that is clearly visible in the document.

Respond with a JSON object matching this schema:
{
  "isInvoice": boolean,
  "supplier": string | null,
  "description": string | null,
  "totalPrice": number | null,
  "totalTax": number | null,
  "invoiceDate": string | null,
  "currency": string (optional)
}`
        },
        {
          role: 'user',
          content: [
            {
              type: 'text',
              text: 'Please analyze this document and extract invoice information if it is an invoice.'
            },
            {
              type: 'image_url',
              image_url: {
                url: `data:${mediaType};base64,${base64}`
              }
            }
          ]
        }
      ],
      response_format: { type: 'json_object' },
      max_completion_tokens: 500,
    });

    // Extract and parse the response
    const messageContent = completion.choices[0]?.message?.content;
    
    if (!messageContent) {
      return {
        success: false,
        error: 'No response content from the API.'
      };
    }

    // Parse the JSON response and validate with Zod
    let invoiceData: InvoiceData;
    try {
      const parsedData = JSON.parse(messageContent);
      invoiceData = invoiceDataSchema.parse(parsedData);
    } catch (parseError) {
      console.error('Parse error:', parseError);
      return {
        success: false,
        error: 'Failed to parse invoice data from the API response.'
      };
    }

    // Generate email draft
    const emailDraft = generateEmailDraft(invoiceData, language);

    return {
      success: true,
      data: invoiceData,
      emailDraft
    };
  } catch (error) {
    console.error('Error processing invoice:', error);
    
    if (error instanceof Error) {
      return {
        success: false,
        error: `Processing failed: ${error.message}`
      };
    }
    
    return {
      success: false,
      error: 'An unexpected error occurred while processing the invoice.'
    };
  }
}

