-- Analyze performance of last queries
-- Based on http://msdn.microsoft.com/en-us/library/ff394114.aspx
SELECT top 1000 last_execution_time, execution_count, total_worker_time, last_worker_time, total_rows, statement_text
FROM 
    (SELECT QS.*, SUBSTRING(ST.text, (QS.statement_start_offset/2) + 1, 
		((CASE statement_end_offset 
			WHEN -1 THEN DATALENGTH(st.text) 
			ELSE QS.statement_end_offset 
		  END - QS.statement_start_offset)/2) + 1) AS statement_text
     FROM sys.dm_exec_query_stats AS QS
     CROSS APPLY sys.dm_exec_sql_text(QS.sql_handle) as ST) as query_stats
order by last_execution_time desc
