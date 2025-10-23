# Enhanced Feature Specification: Invoice Processing with OpenAI

## Context

- **Framework**: Next.js (App Router)
- **Current State**: Basic app with navigation menu only
- **Dependencies**: OpenAI SDK installed, `OPENAI_API_KEY` configured in `.env.local`

## Feature Overview

Build a server-side invoice processing system that extracts structured data from uploaded documents (PDF/JPG) using OpenAI's Vision and Structured Outputs capabilities, then generates a draft email for the accounting department.

The user must be able to choose the language for the drafted email. Options are German, English, Spanish, and French.

## Technical Requirements

### 1. File Upload Route

- **Route**: `/app/invoice-upload/page.tsx` (or similar)
- **Accepted formats**: PDF and JPG/JPEG files
- **Max file size**: 3MB
- **Upload method**: Form with file input, submit via Server Action

### 2. Server-Side Processing Architecture

**Use Server Actions (preferred approach)**

**Processing Flow:**
1. Receive uploaded file via Server Action
2. Convert file to base64 or buffer for OpenAI API
4. Send to OpenAI Chat Completions API with vision capabilities
5. Use Structured Outputs to parse response
6. Return extracted data to client

### 3. OpenAI Integration Specifications

**API Details:**
- **Endpoint**: Chat Completions API with vision (`gpt-5` with minimal reasoning)
- **API Type**: Must use OpenAI's **Responses** API.
- **Method**: Structured Outputs using `response_format` parameter
- **Research Required**: Review OpenAI documentation for:
  - Vision API best practices for document analysis
  - Structured Outputs JSON schema definition
  - Token optimization for image inputs
- **Limit Tokens**: Use `max_completion_tokens` to limit the number of tokens in the response.

**Structured Output Schema:**

```typescript
interface InvoiceData {
  isInvoice: boolean; // First determine if document is an invoice
  supplier: string | null;
  description: string | null; // Summary of purchase
  totalPrice: number | null; // Including tax
  totalTax: number | null;
  invoiceDate: string | null; // ISO 8601 format (YYYY-MM-DD)
  currency?: string; // Optional: detect currency
}
```

**Prompt Engineering:**
- System prompt: Define role as invoice analysis specialist
- User prompt: Clear instructions to identify and extract specified fields
- Handle edge cases: Non-invoice documents, partially visible information, multiple languages

### 4. UI/UX Flow

**Upload Page:**
- File input with drag-and-drop support
- Language selector for the drafted email
- Clear validation messages for file type/size
- Loading state during processing
- Error handling for failed uploads/API errors

**Results Display:**
- Show extracted invoice data in a readable format (table or card layout)
- Display drafted email in a `<textarea>` or pre-formatted text block
- "Copy to Clipboard" button with success feedback
- Option to retry or upload another invoice

**Email Template:**
```
Subject: Invoice Submission - [Supplier Name]

Dear Accounting Team,

Please process the attached invoice with the following details:

Supplier: [Supplier Name]
Description: [Purchase Description]
Invoice Date: [Invoice Date]
Total Amount: [Total Price]
Tax Amount: [Total Tax]

Best regards
```

### 5. Error Handling
- Invalid file format/size
- Non-invoice document detection
- OpenAI API errors (rate limits, timeouts)
- Missing or unclear data in invoice
- Provide user-friendly error messages for each case

### 6. Implementation Checklist
- [ ] Research OpenAI Structured Outputs documentation and examples
- [ ] Research OpenAI Vision API best practices for documents
- [ ] Create Server Action for file processing
- [ ] Implement file validation and conversion logic
- [ ] Define Zod schema for structured output validation
- [ ] Build OpenAI API integration with proper error handling
- [ ] Create upload UI component
- [ ] Implement results display and email generation
- [ ] Add clipboard copy functionality
- [ ] Test with various invoice formats and edge cases
- [ ] Add loading states and error boundaries

### 8. Security Considerations
- Validate file types on both client and server
- Sanitize extracted data before displaying
- Implement rate limiting for uploads (optional)
- Don't log sensitive invoice data
- Consider temporary file storage cleanup

## Acceptance Criteria
- [ ] Users can upload PDF or JPG files
- [ ] System correctly identifies invoices vs non-invoices
- [ ] All specified fields are extracted with high accuracy
- [ ] Email draft is properly formatted and contains extracted data
- [ ] Copy to clipboard works reliably
- [ ] All OpenAI interactions occur server-side
- [ ] Error states are handled gracefully
- [ ] Loading states provide clear feedback

## Research Tasks Before Implementation
1. Review OpenAI Structured Outputs documentation for latest schema syntax
2. Review Next.js Server Actions best practices for file uploads
