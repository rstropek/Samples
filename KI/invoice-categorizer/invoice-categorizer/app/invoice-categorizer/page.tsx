"use client";

import { useState, useRef } from "react";
import { processInvoice } from "./actions";
import type { InvoiceData, Language, ProcessingResult } from "./types";

export default function InvoiceCategorizerPage() {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [language, setLanguage] = useState<Language>("en");
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState<ProcessingResult | null>(null);
  const [dragActive, setDragActive] = useState(false);
  const [copySuccess, setCopySuccess] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleDrag = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (e.type === "dragenter" || e.type === "dragover") {
      setDragActive(true);
    } else if (e.type === "dragleave") {
      setDragActive(false);
    }
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setDragActive(false);

    if (e.dataTransfer.files && e.dataTransfer.files[0]) {
      const file = e.dataTransfer.files[0];
      validateAndSetFile(file);
    }
  };

  const validateAndSetFile = (file: File) => {
    const validTypes = ["application/pdf", "image/jpeg", "image/jpg", "image/png"];
    const maxSize = 3 * 1024 * 1024; // 3MB

    if (!validTypes.includes(file.type)) {
      alert("Invalid file type. Please upload a PDF or JPG/JPEG file.");
      return;
    }

    if (file.size > maxSize) {
      alert("File size exceeds 3MB limit.");
      return;
    }

    setSelectedFile(file);
    setResult(null);
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      validateAndSetFile(e.target.files[0]);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!selectedFile) {
      alert("Please select a file");
      return;
    }

    setLoading(true);
    setResult(null);

    try {
      const formData = new FormData();
      formData.append("file", selectedFile);
      formData.append("language", language);

      const result = await processInvoice(formData);
      setResult(result);
    } catch (error) {
      setResult({
        success: false,
        error: "An unexpected error occurred",
      });
    } finally {
      setLoading(false);
    }
  };

  const handleCopyToClipboard = async () => {
    if (result && result.success) {
      try {
        await navigator.clipboard.writeText(result.emailDraft);
        setCopySuccess(true);
        setTimeout(() => setCopySuccess(false), 2000);
      } catch (err) {
        alert("Failed to copy to clipboard");
      }
    }
  };

  const handleReset = () => {
    setSelectedFile(null);
    setResult(null);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  const languageNames: Record<Language, string> = {
    en: "English",
    de: "German (Deutsch)",
    es: "Spanish (Español)",
    fr: "French (Français)",
  };

  return (
    <div className="min-h-[calc(100vh-4rem)] bg-gray-50 dark:bg-gray-900 py-8">
      <div className="max-w-4xl mx-auto px-4">
        <h1 className="text-3xl font-bold mb-2 text-gray-900 dark:text-white">
          Invoice Processor
        </h1>
        <p className="text-gray-600 dark:text-gray-400 mb-8">
          Upload an invoice (PDF or JPG) to extract information and generate an email draft
        </p>

        {/* Upload Form */}
        {!result && (
          <form onSubmit={handleSubmit} className="space-y-6">
            {/* File Upload Area */}
            <div
              className={`border-2 border-dashed rounded-lg p-8 text-center transition-colors ${
                dragActive
                  ? "border-blue-500 bg-blue-50 dark:bg-blue-950"
                  : "border-gray-300 dark:border-gray-700"
              }`}
              onDragEnter={handleDrag}
              onDragLeave={handleDrag}
              onDragOver={handleDrag}
              onDrop={handleDrop}
            >
              <input
                ref={fileInputRef}
                type="file"
                accept=".pdf,.jpg,.jpeg,.png"
                onChange={handleFileChange}
                className="hidden"
                id="file-upload"
              />
              <label
                htmlFor="file-upload"
                className="cursor-pointer flex flex-col items-center"
              >
                <svg
                  className="w-16 h-16 text-gray-400 mb-4"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12"
                  />
                </svg>
                <span className="text-lg text-gray-700 dark:text-gray-300 mb-2">
                  {selectedFile ? selectedFile.name : "Click to upload or drag and drop"}
                </span>
                <span className="text-sm text-gray-500 dark:text-gray-400">
                  PDF or JPG/JPEG (max 3MB)
                </span>
              </label>
            </div>

            {/* Language Selector */}
            <div>
              <label
                htmlFor="language"
                className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2"
              >
                Email Language
              </label>
              <select
                id="language"
                value={language}
                onChange={(e) => setLanguage(e.target.value as Language)}
                className="w-full px-4 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              >
                {Object.entries(languageNames).map(([code, name]) => (
                  <option key={code} value={code}>
                    {name}
                  </option>
                ))}
              </select>
            </div>

            {/* Submit Button */}
            <button
              type="submit"
              disabled={!selectedFile || loading}
              className="w-full bg-blue-600 hover:bg-blue-700 disabled:bg-gray-400 text-white font-semibold py-3 px-6 rounded-lg transition-colors disabled:cursor-not-allowed"
            >
              {loading ? "Processing..." : "Process Invoice"}
            </button>
          </form>
        )}

        {/* Loading State */}
        {loading && (
          <div className="flex flex-col items-center justify-center py-12">
            <div className="animate-spin rounded-full h-16 w-16 border-b-2 border-blue-600 mb-4"></div>
            <p className="text-gray-600 dark:text-gray-400">
              Analyzing invoice with AI...
            </p>
          </div>
        )}

        {/* Results Display */}
        {result && !loading && (
          <div className="space-y-6">
            {result.success ? (
              <>
                {/* Invoice Data */}
                <div className="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
                  <h2 className="text-2xl font-bold mb-4 text-gray-900 dark:text-white">
                    Extracted Invoice Data
                  </h2>
                  
                  {!result.data.isInvoice ? (
                    <div className="bg-yellow-50 dark:bg-yellow-900/20 border border-yellow-200 dark:border-yellow-800 rounded-lg p-4">
                      <p className="text-yellow-800 dark:text-yellow-200">
                        <strong>Warning:</strong> This document does not appear to be an invoice.
                      </p>
                    </div>
                  ) : (
                    <div className="space-y-3">
                      <DataRow 
                        label="Supplier" 
                        value={result.data.supplier !== "Unknown" ? result.data.supplier : null} 
                      />
                      <DataRow 
                        label="Description" 
                        value={result.data.description !== "N/A" ? result.data.description : null} 
                      />
                      <DataRow 
                        label="Invoice Date" 
                        value={result.data.invoiceDate || null} 
                      />
                      <DataRow
                        label="Total Price"
                        value={
                          result.data.totalPrice > 0
                            ? `${result.data.totalPrice.toFixed(2)} ${result.data.currency || ""}`
                            : null
                        }
                      />
                      <DataRow
                        label="Total Tax"
                        value={
                          result.data.totalTax > 0
                            ? `${result.data.totalTax.toFixed(2)} ${result.data.currency || ""}`
                            : null
                        }
                      />
                      {result.data.currency && (
                        <DataRow label="Currency" value={result.data.currency} />
                      )}
                    </div>
                  )}
                </div>

                {/* Email Draft */}
                <div className="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
                  <div className="flex items-center justify-between mb-4">
                    <h2 className="text-2xl font-bold text-gray-900 dark:text-white">
                      Email Draft
                    </h2>
                    <button
                      onClick={handleCopyToClipboard}
                      className="flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg transition-colors"
                    >
                      {copySuccess ? (
                        <>
                          <svg
                            className="w-5 h-5"
                            fill="none"
                            stroke="currentColor"
                            viewBox="0 0 24 24"
                          >
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              strokeWidth={2}
                              d="M5 13l4 4L19 7"
                            />
                          </svg>
                          Copied!
                        </>
                      ) : (
                        <>
                          <svg
                            className="w-5 h-5"
                            fill="none"
                            stroke="currentColor"
                            viewBox="0 0 24 24"
                          >
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              strokeWidth={2}
                              d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z"
                            />
                          </svg>
                          Copy to Clipboard
                        </>
                      )}
                    </button>
                  </div>
                  <textarea
                    readOnly
                    value={result.emailDraft}
                    className="w-full h-64 p-4 border border-gray-300 dark:border-gray-700 rounded-lg bg-gray-50 dark:bg-gray-900 text-gray-900 dark:text-white font-mono text-sm resize-none"
                  />
                </div>

                {/* Reset Button */}
                <button
                  onClick={handleReset}
                  className="w-full bg-gray-600 hover:bg-gray-700 text-white font-semibold py-3 px-6 rounded-lg transition-colors"
                >
                  Process Another Invoice
                </button>
              </>
            ) : (
              <>
                {/* Error Display */}
                <div className="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg p-6">
                  <h2 className="text-2xl font-bold mb-2 text-red-800 dark:text-red-200">
                    Error
                  </h2>
                  <p className="text-red-700 dark:text-red-300">{result.error}</p>
                </div>
                <button
                  onClick={handleReset}
                  className="w-full bg-gray-600 hover:bg-gray-700 text-white font-semibold py-3 px-6 rounded-lg transition-colors"
                >
                  Try Again
                </button>
              </>
            )}
          </div>
        )}
      </div>
    </div>
  );
}

function DataRow({ label, value }: { label: string; value: string | null }) {
  return (
    <div className="flex border-b border-gray-200 dark:border-gray-700 pb-3">
      <dt className="font-semibold text-gray-700 dark:text-gray-300 w-40">
        {label}:
      </dt>
      <dd className="text-gray-900 dark:text-white flex-1">
        {value || <span className="text-gray-400">Not available</span>}
      </dd>
    </div>
  );
}

