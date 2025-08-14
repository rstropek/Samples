'use client';

import { useState, useEffect } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import styles from './page.module.css';

interface TableData {
  table: string;
  columns: string[];
  columnTypes: Record<string, 'numeric' | 'datetime' | 'text'>;
  rows: Record<string, any>[];
  pagination: {
    currentPage: number;
    totalPages: number;
    totalRows: number;
    pageSize: number;
  };
}

interface TableBrowserClientProps {
  params: Promise<{ table: string }>;
}


export default function TableBrowserClient({ params }: TableBrowserClientProps) {
  const router = useRouter();
  const searchParams = useSearchParams();
  const [data, setData] = useState<TableData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [table, setTable] = useState<string>('');

  const currentPage = parseInt(searchParams.get('page') || '1', 10);

  useEffect(() => {
    const initializeParams = async () => {
      const resolvedParams = await params;
      setTable(resolvedParams.table);
    };
    
    initializeParams();
  }, [params]);

  useEffect(() => {
    if (!table) return; // Wait for table to be set
    
    const fetchData = async () => {
      setLoading(true);
      setError(null);
      
      try {
        const response = await fetch(`/api/tables/${table}?page=${currentPage}`);
        
        if (!response.ok) {
          const errorData = await response.json();
          throw new Error(errorData.error || 'Failed to fetch data');
        }
        
        const tableData = await response.json();
        setData(tableData);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Unknown error');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [table, currentPage]);

  const handlePageChange = (page: number) => {
    const params = new URLSearchParams(searchParams.toString());
    params.set('page', page.toString());
    router.push(`/browse-client/${table}?${params.toString()}`);
  };

  if (loading || !table) {
    return (
      <div className={styles.container}>
        <h1>Loading...</h1>
      </div>
    );
  }

  if (error) {
    return (
      <div className={styles.container}>
        <h1>Error</h1>
        <p>{error}</p>
      </div>
    );
  }

  if (!data) {
    return (
      <div className={styles.container}>
        <h1>No data found</h1>
      </div>
    );
  }

  const { columns, columnTypes, rows, pagination } = data;

  return (
    <div className={styles.container}>
      <h1>Table: {table}</h1>
      
      {rows.length === 0 ? (
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
              {rows.map((row, rowIndex) => 
                columns.map((column) => {
                  const value = row[column];
                  const displayValue = value?.toString() || '';
                  
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
              Page {pagination.currentPage} of {pagination.totalPages} ({pagination.totalRows} total records)
            </div>
            
            <div className={styles.paginationControls}>
              {pagination.currentPage > 1 && (
                <button 
                  onClick={() => handlePageChange(pagination.currentPage - 1)}
                  className={styles.paginationLink}
                >
                  Previous
                </button>
              )}
              
              {Array.from({ length: pagination.totalPages }, (_, i) => i + 1).map((pageNum) => (
                <button
                  key={pageNum}
                  onClick={() => handlePageChange(pageNum)}
                  className={`${styles.paginationLink} ${pageNum === pagination.currentPage ? styles.active : ''}`}
                >
                  {pageNum}
                </button>
              ))}
              
              {pagination.currentPage < pagination.totalPages && (
                <button 
                  onClick={() => handlePageChange(pagination.currentPage + 1)}
                  className={styles.paginationLink}
                >
                  Next
                </button>
              )}
            </div>
          </div>
        </>
      )}
    </div>
  );
}
