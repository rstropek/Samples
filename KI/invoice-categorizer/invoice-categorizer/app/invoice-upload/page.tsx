'use client';

import { useState, useRef, FormEvent } from 'react';
import { processInvoice } from './actions';
import type { InvoiceData, EmailLanguage, ProcessingResult } from './types';

export default function InvoiceUploadPage() {
  const [file, setFile] = useState<File | null>(null);
  const [language, setLanguage] = useState<EmailLanguage>('en');
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState<ProcessingResult | null>(null);
  const [dragActive, setDragActive] = useState(false);
  const [copySuccess, setCopySuccess] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleDrag = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (e.type === 'dragenter' || e.type === 'dragover') {
      setDragActive(true);
    } else if (e.type === 'dragleave') {
      setDragActive(false);
    }
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setDragActive(false);

    if (e.dataTransfer.files && e.dataTransfer.files[0]) {
      const droppedFile = e.dataTransfer.files[0];
      validateAndSetFile(droppedFile);
    }
  };

  const validateAndSetFile = (selectedFile: File) => {
    const validTypes = ['application/pdf', 'image/jpeg', 'image/jpg'];
    const maxSize = 3 * 1024 * 1024; // 3MB

    if (!validTypes.includes(selectedFile.type)) {
      alert('Invalid file type. Only PDF and JPG files are accepted.');
      return;
    }

    if (selectedFile.size > maxSize) {
      alert('File size exceeds 3MB limit.');
      return;
    }

    setFile(selectedFile);
    setResult(null);
    setCopySuccess(false);
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      validateAndSetFile(e.target.files[0]);
    }
  };

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!file) {
      alert('Please select a file first.');
      return;
    }

    setLoading(true);
    setResult(null);
    setCopySuccess(false);

    const formData = new FormData();
    formData.append('file', file);
    formData.append('language', language);

    try {
      const processingResult = await processInvoice(formData);
      setResult(processingResult);
    } catch (error) {
      setResult({
        success: false,
        error: 'An unexpected error occurred.'
      });
    } finally {
      setLoading(false);
    }
  };

  const handleCopyToClipboard = async () => {
    if (result?.emailDraft) {
      try {
        await navigator.clipboard.writeText(result.emailDraft);
        setCopySuccess(true);
        setTimeout(() => setCopySuccess(false), 3000);
      } catch (error) {
        alert('Failed to copy to clipboard');
      }
    }
  };

  const resetForm = () => {
    setFile(null);
    setResult(null);
    setCopySuccess(false);
    if (fileInputRef.current) {
      fileInputRef.current.value = '';
    }
  };

  const formatCurrency = (value: number | null, currency?: string) => {
    if (value === null) return 'N/A';
    return `${value.toFixed(2)} ${currency || ''}`.trim();
  };

  return (
    <main className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-4xl mx-auto px-4">
        <div className="bg-white rounded-lg shadow-lg p-8">
          <h1 className="text-3xl font-bold text-gray-900 mb-2">Invoice Processing</h1>
          <p className="text-gray-600 mb-8">Upload an invoice to extract data and generate an email draft</p>

          <form onSubmit={handleSubmit} className="space-y-6">
            {/* Language Selector */}
            <div>
              <label htmlFor="language" className="block text-sm font-medium text-gray-700 mb-2">
                Email Language
              </label>
              <select
                id="language"
                value={language}
                onChange={(e) => setLanguage(e.target.value as EmailLanguage)}
                className="w-full px-4 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                disabled={loading}
              >
                <option value="en">English</option>
                <option value="de">German (Deutsch)</option>
                <option value="es">Spanish (Español)</option>
                <option value="fr">French (Français)</option>
              </select>
            </div>

            {/* File Upload Area */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Invoice File (PDF or JPG, max 3MB)
              </label>
              <div
                className={`relative border-2 border-dashed rounded-lg p-8 text-center transition-colors ${
                  dragActive
                    ? 'border-blue-500 bg-blue-50'
                    : 'border-gray-300 hover:border-gray-400'
                }`}
                onDragEnter={handleDrag}
                onDragLeave={handleDrag}
                onDragOver={handleDrag}
                onDrop={handleDrop}
              >
                <input
                  ref={fileInputRef}
                  type="file"
                  accept=".pdf,.jpg,.jpeg"
                  onChange={handleFileChange}
                  className="hidden"
                  id="file-upload"
                  disabled={loading}
                />
                <label
                  htmlFor="file-upload"
                  className="cursor-pointer"
                >
                  <div className="space-y-2">
                    <svg
                      className="mx-auto h-12 w-12 text-gray-400"
                      stroke="currentColor"
                      fill="none"
                      viewBox="0 0 48 48"
                      aria-hidden="true"
                    >
                      <path
                        d="M28 8H12a4 4 0 00-4 4v20m32-12v8m0 0v8a4 4 0 01-4 4H12a4 4 0 01-4-4v-4m32-4l-3.172-3.172a4 4 0 00-5.656 0L28 28M8 32l9.172-9.172a4 4 0 015.656 0L28 28m0 0l4 4m4-24h8m-4-4v8m-12 4h.02"
                        strokeWidth={2}
                        strokeLinecap="round"
                        strokeLinejoin="round"
                      />
                    </svg>
                    <div className="text-gray-600">
                      <span className="font-medium text-blue-600 hover:text-blue-500">
                        Click to upload
                      </span>{' '}
                      or drag and drop
                    </div>
                    <p className="text-xs text-gray-500">PDF or JPG up to 3MB</p>
                  </div>
                </label>
                {file && (
                  <div className="mt-4 text-sm text-gray-700">
                    <strong>Selected:</strong> {file.name} ({(file.size / 1024 / 1024).toFixed(2)} MB)
                  </div>
                )}
              </div>
            </div>

            {/* Submit Button */}
            <div className="flex gap-4">
              <button
                type="submit"
                disabled={!file || loading}
                className="flex-1 bg-blue-600 text-white py-3 px-6 rounded-md font-medium hover:bg-blue-700 disabled:bg-gray-300 disabled:cursor-not-allowed transition-colors"
              >
                {loading ? (
                  <span className="flex items-center justify-center">
                    <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    Processing...
                  </span>
                ) : (
                  'Process Invoice'
                )}
              </button>
              {file && !loading && (
                <button
                  type="button"
                  onClick={resetForm}
                  className="px-6 py-3 border border-gray-300 rounded-md font-medium text-gray-700 hover:bg-gray-50 transition-colors"
                >
                  Clear
                </button>
              )}
            </div>
          </form>

          {/* Results Display */}
          {result && (
            <div className="mt-8 space-y-6">
              {result.success && result.data ? (
                <>
                  {/* Invoice Data */}
                  <div className="border border-gray-200 rounded-lg overflow-hidden">
                    <div className="bg-gray-50 px-6 py-4 border-b border-gray-200">
                      <h2 className="text-xl font-semibold text-gray-900">Extracted Invoice Data</h2>
                    </div>
                    <div className="p-6">
                      {!result.data.isInvoice ? (
                        <div className="bg-yellow-50 border border-yellow-200 rounded-md p-4">
                          <p className="text-yellow-800 font-medium">
                            ⚠️ This document does not appear to be an invoice.
                          </p>
                        </div>
                      ) : (
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                          <div>
                            <div className="text-sm font-medium text-gray-500">Supplier</div>
                            <div className="mt-1 text-gray-900">{result.data.supplier || 'N/A'}</div>
                          </div>
                          <div>
                            <div className="text-sm font-medium text-gray-500">Invoice Date</div>
                            <div className="mt-1 text-gray-900">{result.data.invoiceDate || 'N/A'}</div>
                          </div>
                          <div className="md:col-span-2">
                            <div className="text-sm font-medium text-gray-500">Description</div>
                            <div className="mt-1 text-gray-900">{result.data.description || 'N/A'}</div>
                          </div>
                          <div>
                            <div className="text-sm font-medium text-gray-500">Total Amount</div>
                            <div className="mt-1 text-gray-900 font-semibold">
                              {formatCurrency(result.data.totalPrice, result.data.currency)}
                            </div>
                          </div>
                          <div>
                            <div className="text-sm font-medium text-gray-500">Tax Amount</div>
                            <div className="mt-1 text-gray-900">
                              {formatCurrency(result.data.totalTax, result.data.currency)}
                            </div>
                          </div>
                        </div>
                      )}
                    </div>
                  </div>

                  {/* Email Draft */}
                  <div className="border border-gray-200 rounded-lg overflow-hidden">
                    <div className="bg-gray-50 px-6 py-4 border-b border-gray-200 flex justify-between items-center">
                      <h2 className="text-xl font-semibold text-gray-900">Email Draft</h2>
                      <button
                        onClick={handleCopyToClipboard}
                        className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors text-sm font-medium"
                      >
                        {copySuccess ? (
                          <>
                            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                            </svg>
                            Copied!
                          </>
                        ) : (
                          <>
                            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
                            </svg>
                            Copy to Clipboard
                          </>
                        )}
                      </button>
                    </div>
                    <div className="p-6">
                      <pre className="whitespace-pre-wrap font-sans text-gray-900 bg-gray-50 p-4 rounded border border-gray-200">
                        {result.emailDraft}
                      </pre>
                    </div>
                  </div>

                  {/* Action Buttons */}
                  <div className="flex justify-center">
                    <button
                      onClick={resetForm}
                      className="px-6 py-3 bg-gray-600 text-white rounded-md font-medium hover:bg-gray-700 transition-colors"
                    >
                      Process Another Invoice
                    </button>
                  </div>
                </>
              ) : (
                <div className="bg-red-50 border border-red-200 rounded-lg p-6">
                  <div className="flex">
                    <svg className="h-6 w-6 text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    <div className="ml-3">
                      <h3 className="text-sm font-medium text-red-800">Processing Error</h3>
                      <div className="mt-2 text-sm text-red-700">
                        {result.error || 'An unexpected error occurred.'}
                      </div>
                    </div>
                  </div>
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </main>
  );
}

