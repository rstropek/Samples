import { NextRequest, NextResponse } from 'next/server';
import { SQLiteCustomerDB } from '@/lib/dbUtils';

// Whitelist of allowed table names
const ALLOWED_TABLES = ['customers'];

// Helper function to determine if a value is numeric
function isNumeric(value: any): boolean {
  return typeof value === 'number' || (typeof value === 'string' && !isNaN(Number(value)) && value.trim() !== '');
}

// Helper function to determine if a value is a date/time
function isDateTime(value: any): boolean {
  if (!value) return false;
  const dateValue = new Date(value);
  return !isNaN(dateValue.getTime()) && 
         (typeof value === 'string' && 
          (value.includes('-') || value.includes('/') || value.includes('T') || value.toLowerCase().includes('date')));
}

// Helper function to format date/time as ISO 8601
function formatDateTime(value: any): string {
  const date = new Date(value);
  return date.toISOString();
}

// Helper function to determine column types from data
function analyzeColumnTypes(rows: Record<string, any>[], columns: string[]): Record<string, 'numeric' | 'datetime' | 'text'> {
  const columnTypes: Record<string, 'numeric' | 'datetime' | 'text'> = {};
  
  columns.forEach(column => {
    // Sample the first few non-null values to determine type
    const sampleValues = rows
      .map(row => row[column])
      .filter(value => value !== null && value !== undefined && value !== '')
      .slice(0, 5);
    
    if (sampleValues.length === 0) {
      columnTypes[column] = 'text';
      return;
    }
    
    // Check if all sample values are numeric
    if (sampleValues.every(isNumeric)) {
      columnTypes[column] = 'numeric';
    }
    // Check if all sample values are date/time
    else if (sampleValues.every(isDateTime)) {
      columnTypes[column] = 'datetime';
    }
    // Default to text
    else {
      columnTypes[column] = 'text';
    }
  });
  
  return columnTypes;
}

export async function GET(
  request: NextRequest,
  { params }: { params: Promise<{ table: string }> }
) {
  try {
    const { table } = await params;
    const searchParams = request.nextUrl.searchParams;
    const page = parseInt(searchParams.get('page') || '1', 10);
    
    // Check if table is in whitelist
    if (!ALLOWED_TABLES.includes(table)) {
      return NextResponse.json(
        { 
          error: `Table "${table}" is not allowed. Available tables: ${ALLOWED_TABLES.join(', ')}` 
        },
        { status: 400 }
      );
    }

    const currentPage = Math.max(1, page);
    const pageSize = 10;
    const offset = (currentPage - 1) * pageSize;

    const sqlite = new SQLiteCustomerDB('customer.db');
    await sqlite.connect();
    
    // Get all rows for the table
    const allRows = await sqlite.getAllRows<Record<string, any>>(table);
    
    // Calculate pagination
    const totalRows = allRows.length;
    const totalPages = Math.ceil(totalRows / pageSize);
    const paginatedRows = allRows.slice(offset, offset + pageSize);
    
    await sqlite.close();

    // Get column names from the first row
    const columns = allRows.length > 0 ? Object.keys(allRows[0]) : [];
    
    // Analyze column types
    const columnTypes = analyzeColumnTypes(allRows, columns);

    // Format the data rows with proper formatting for date/time
    const formattedRows = paginatedRows.map(row => {
      const formattedRow: Record<string, any> = {};
      columns.forEach(column => {
        const value = row[column];
        
        // Format date/time values as ISO 8601
        if (columnTypes[column] === 'datetime' && value) {
          try {
            formattedRow[column] = formatDateTime(value);
          } catch (error) {
            // If formatting fails, use original value
            formattedRow[column] = value;
          }
        } else {
          formattedRow[column] = value;
        }
      });
      return formattedRow;
    });

    return NextResponse.json({
      table,
      columns,
      columnTypes,
      rows: formattedRows,
      pagination: {
        currentPage,
        totalPages,
        totalRows,
        pageSize
      }
    });

  } catch (error) {
    console.error('Error fetching table data:', error);
    return NextResponse.json(
      { 
        error: `Failed to fetch data from table: ${error instanceof Error ? error.message : 'Unknown error'}` 
      },
      { status: 500 }
    );
  }
}
