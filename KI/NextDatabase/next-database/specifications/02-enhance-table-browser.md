# Enhanced Table Browser

There is already a simple table browser in [page.tsx](../src/app/browse/[table]/page.tsx) and [page.module.css](../src/app/browse/[table]/page.module.css).

Add the folowing enhancements:

* If a column is numeric, make it right-aligned (including the heading).
* If a column is date/time, format it as ISO 8601 (e.g., 2023-03-15T12:34:56Z)
* Accept an optional parameter in the query string to change the page size for paging (e.g. `?pageSize=20`)
