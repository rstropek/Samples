# Invoice Processing Feature

## Overview

This feature allows users to upload invoice documents (PDF or JPG) and automatically extract structured data using OpenAI's vision capabilities. The system then generates a draft email in the selected language for submission to the accounting department.

## Features

- **File Upload**: Drag-and-drop or click to upload PDF/JPG files (max 3MB)
- **Multi-language Support**: Generate email drafts in English, German, Spanish, or French
- **AI-Powered Extraction**: Uses OpenAI GPT-4o-mini with vision to extract:
  - Invoice validation (determines if document is actually an invoice)
  - Supplier name
  - Purchase description
  - Total amount (including tax)
  - Tax amount
  - Invoice date
  - Currency code
- **Email Draft Generation**: Automatically creates formatted email for accounting team
- **Copy to Clipboard**: One-click copy functionality for generated email
- **Error Handling**: Comprehensive validation and user-friendly error messages
- **Loading States**: Clear visual feedback during processing

## Technical Stack

- **Framework**: Next.js 16 (App Router)
- **AI Integration**: OpenAI SDK (gpt-4o-mini model)
- **Validation**: Zod schema validation
- **UI**: React with Tailwind CSS
- **Server Actions**: Server-side processing for security and performance

## File Structure

```
app/invoice-upload/
├── page.tsx          # Main UI component with upload form and results display
├── actions.ts        # Server Action for file processing and OpenAI integration
├── types.ts          # TypeScript types and Zod schemas
└── README.md         # This file
```

## Usage

1. Navigate to `/invoice-upload` in your browser
2. Select the desired language for the email draft (English, German, Spanish, or French)
3. Upload an invoice file by:
   - Dragging and dropping a file onto the upload area, or
   - Clicking the upload area to select a file
4. Click "Process Invoice" to submit
5. View the extracted data and generated email draft
6. Use "Copy to Clipboard" to copy the email draft
7. Click "Process Another Invoice" to start over

## API Requirements

The feature requires an OpenAI API key configured in `.env.local`:

```env
OPENAI_API_KEY=your_api_key_here
```

## Validation Rules

- **File Types**: Only PDF and JPG/JPEG files are accepted
- **File Size**: Maximum 3MB per file
- **Content**: System validates that uploaded document is actually an invoice

## Error Handling

The system handles various error scenarios:
- Invalid file format or size
- Non-invoice documents
- OpenAI API errors (rate limits, timeouts)
- Missing or unclear data in invoice
- Network errors

All errors are displayed with user-friendly messages and appropriate visual feedback.

## Email Template Format

Generated emails follow this structure:

```
Dear Accounting Team,

Please process the attached invoice with the following details:

Supplier: [Supplier Name]
Description: [Purchase Description]
Invoice Date: [Invoice Date]
Total Amount: [Total Price]
Tax Amount: [Total Tax]

Best regards
```

The template is automatically translated based on the selected language.

## Security Considerations

- All file processing occurs server-side
- File validation on both client and server
- No sensitive invoice data is logged
- API keys are stored securely in environment variables
- Files are processed in-memory and not permanently stored

## Future Enhancements

Potential improvements for future iterations:
- Support for additional file formats (PNG, TIFF)
- Batch processing of multiple invoices
- Export functionality (CSV, JSON)
- Invoice history and tracking
- Custom email template editor
- Integration with accounting software APIs

