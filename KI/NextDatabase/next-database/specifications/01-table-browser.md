# Table Browser

Add a table browser to [page.tsx](../src/app/browse/[table]/page.tsx) and [page.module.css](../src/app/browse/[table]/page.module.css). The table browser must be a server component.

The table browser must use the `getAllRows` function from [dbUtils.ts](../src/lib/dbUtils.ts). It receives the table name as a parameter.

The table browser component must have a whitelist of allowed table names.

It must display all rows and columns in a table using CSS grid.

It must implement paging (page size is 10 records).

Keep styling to a minimum.
