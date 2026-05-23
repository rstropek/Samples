import { SQLiteCustomerDB } from '@/lib/dbUtils';
import styles from './page.module.css';

interface TableBrowserProps {
  params: Promise<{ table: string }>;
  searchParams: Promise<{ page?: string }>;
}

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

export default async function TableBrowser({ params, searchParams }: TableBrowserProps) {
  const { table } = await params;
  const { page = '1' } = await searchParams;
  
  // Check if table is in whitelist
  if (!ALLOWED_TABLES.includes(table)) {
    return (
      <div className={styles.container}>
        <h1>Error</h1>
        <p>Table "{table}" is not allowed. Available tables: {ALLOWED_TABLES.join(', ')}</p>
      </div>
    );
  }

  const currentPage = Math.max(1, parseInt(page, 10) || 1);
  const pageSize = 10;
  const offset = (currentPage - 1) * pageSize;

  try {
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
    const columns = paginatedRows.length > 0 ? Object.keys(paginatedRows[0]) : [];
    
    // Analyze column types
    const columnTypes = analyzeColumnTypes(allRows, columns);

    return (
      <div className={styles.container}>
        <h1>Table: {table}</h1>
        
        {paginatedRows.length === 0 ? (
          <p>No data found in table "{table}".</p>
        ) : (
          <>
            <div className={styles.tableContainer}>
              <div className={styles.table} style={{ gridTemplateColumns: `repeat(${columns.length}, 1fr)` }}>
                {/* Header row */}
                {columns.map((column) => (
                  <div 
                    key={column} 
                    className={`${styles.headerCell} ${columnTypes[column] === 'numeric' ? styles.numericCell : ''}`}
                  >
                    {column}
                  </div>
                ))}
                
                {/* Data rows */}
                {paginatedRows.map((row, rowIndex) => 
                  columns.map((column) => {
                    const value = row[column];
                    let displayValue = value?.toString() || '';
                    
                    // Format date/time values as ISO 8601
                    if (columnTypes[column] === 'datetime' && value) {
                      try {
                        displayValue = formatDateTime(value);
                      } catch (error) {
                        // If formatting fails, use original value
                        displayValue = value.toString();
                      }
                    }
                    
                    return (
                      <div 
                        key={`${rowIndex}-${column}`} 
                        className={`${styles.dataCell} ${columnTypes[column] === 'numeric' ? styles.numericCell : ''}`}
                      >
                        {displayValue}
                      </div>
                    );
                  })
                )}
              </div>
            </div>

            {/* Pagination */}
            <div className={styles.pagination}>
              <div className={styles.paginationInfo}>
                Page {currentPage} of {totalPages} ({totalRows} total records)
              </div>
              
              <div className={styles.paginationControls}>
                {currentPage > 1 && (
                  <a href={`/browse/${table}?page=${currentPage - 1}`} className={styles.paginationLink}>
                    Previous
                  </a>
                )}
                
                {Array.from({ length: totalPages }, (_, i) => i + 1).map((pageNum) => (
                  <a
                    key={pageNum}
                    href={`/browse/${table}?page=${pageNum}`}
                    className={`${styles.paginationLink} ${pageNum === currentPage ? styles.active : ''}`}
                  >
                    {pageNum}
                  </a>
                ))}
                
                {currentPage < totalPages && (
                  <a href={`/browse/${table}?page=${currentPage + 1}`} className={styles.paginationLink}>
                    Next
                  </a>
                )}
              </div>
            </div>
          </>
        )}
      </div>
    );
  } catch (error) {
    console.error('Error fetching table data:', error);
    return (
      <div className={styles.container}>
        <h1>Error</h1>
        <p>Failed to fetch data from table "{table}": {error instanceof Error ? error.message : 'Unknown error'}</p>
      </div>
    );
  }
}
