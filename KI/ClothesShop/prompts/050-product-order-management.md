# Add product and order management

Extend ClothesShop with product management and order management. Structure
everything like the existing partner module: controller, service, repository,
form, and Thymeleaf templates in a feature folder. Follow `AGENTS.md`, and use
the `witt-weiden-design` skill for every user-facing page.

## Product management

Full CRUD for products with these fields:

- description: required, max. 500 characters
- color: optional, max. 20 characters
- size: optional, max. 10 characters
- tags: optional, comma-separated, max. 250 characters
- unit of measure: required, one of single piece, pack of 5, pack of 10
- unit price: required, non-negative

Store the unit of measure so that adding or reordering the values later cannot
change the meaning of rows that already exist, and show friendly labels in the
UI. Keep tags simple: normalize whitespace, drop empty
entries, store the result in the one column. No tag entity, no generic tagging
system.

Add list, create, edit, and delete pages. The list is sorted by description and
has a case-insensitive text filter over description, color, size, and tags.
Truncate long descriptions in the table. Deletion needs an explicit
confirmation page, like partners.

Products have no lifecycle state yet: no "active", no "discontinued". Do not
add one.

## Order management

Order header:

- order number: assigned by the server, see below
- partner: required, reference to an existing partner
- order date: date and time, set by the server on creation, not editable
- cancellation date: date and time, empty while the order is active
- total price: sum of quantity x unit price over all lines
- rebate percentage: optional, 0 to 100, NULL means no rebate
- final total price: total price x (1 - rebate percentage / 100)

Order line, at least one per order:

- product: required, reference to an existing product
- quantity: required, integer, at least 1
- unit price: required, non-negative, defaults from the product but editable

Amounts are exact to the cent. No binary floating point anywhere in the
calculation: a total must never come out as 10.229999. After applying the
rebate, round to cents, halves upward. Single currency (EUR), no currency
column, and no order will ever come near a million euros.

The unit price on a line is a price snapshot. Later changes to the product
price must not change existing orders. Persist both calculated totals on the
header for the same reason: a stored order must never change retroactively.

Recompute the totals from the posted quantities and unit prices on every save,
and ignore any total that arrives with the request. A posted total is user
input, not a result.

### Timestamps

Store every point in time unambiguously and display it in Berlin local time,
through one shared formatter. Our machines already run in that zone, so a test
that compares against the local default would pass even if the conversion were
wrong. Make sure the tests would actually catch it.

### Order numbers

Every order gets a unique, human-readable number in the format
`ORD-<year>-<5 digits>`, for example `ORD-2026-00001`. The counter starts at 1
and restarts for each year. The year is the year of the order date **in
`Europe/Berlin`**, so an order created at 23:30 UTC on 31 December already
belongs to the next year's sequence.

The number is assigned by the server when the order is created, is never
editable, and is never taken from the request. Keep the counter in the
database, and make sure that two orders created at the same moment can never
end up with the same number. Show the order number in the list, on
the detail page, and in the flash message after a successful create, and let
the list filter search for it.

### The order form

This application contains no JavaScript, and this feature does not introduce
any. Build the line editor as server-side round trips:

- The form shows the current lines plus one empty row.
- "Add line" and "Remove" are normal round trips to the server. Adding a row
  must not turn the untouched fields red while the user is still filling the
  form.
- Rows that are left completely empty are dropped on save. At least one filled
  line is required.
- The product is picked from a simple dropdown. Ten products do not need a
  search box or an autocomplete.
- Totals are not previewed while typing. They are calculated on save and shown
  on the order detail page.

### Pages and rules

- List, sorted by order date with the newest first, with a partner dropdown, an
  all/active/cancelled filter, and a text search over the order number.
- A read-only detail page for a single order: header data, its lines, and the
  totals. Partners have no detail page yet, so this is a new kind of screen.
- Create.
- Cancel, which sets the cancellation date to the current time. It is offered
  on the detail page and needs a confirmation page, like a deletion. An order
  that is already cancelled cannot be cancelled again.

Orders cannot be edited, deleted, or re-activated.

## Deletion rules across features

A product that is referenced by an order line cannot be deleted. The same holds
for a partner that is referenced by an order header: a partner with orders can
be deactivated, and deactivating them does not remove their order history, so
the reference has to block the deletion on its own. Cancelled orders still
count as references.

Enforce this in the database as well as in the application, so that a missing
check in the code cannot corrupt the data. The confirmation page of a blocked
record explains why and offers no delete button, and the request that would
delete it refuses as well and comes back with an error. The application can
only show green success messages today; errors have to be just as visible.

## Database

One migration for the new tables: product, order header, order line, and the
order number counter. Index the references between them and whatever the list
pages sort or filter on. The existing migration shows the level of care I
expect.

## Sample data

Extend the sample data generator with 10 products and 10 order headers of 2 to
5 lines each, on top of the existing partners. Cancel two of the orders and
give roughly half of them a rebate, so both display paths are visible on screen
right after filling the database. The generator already does this deliberately
by making every fifth partner inactive. Clearing the database has to keep
working now that there are references between the tables, and the order number
counter has to be reset with the rest of the data.

## User experience

Add Products and Orders to the top navigation and to the "Start here" panel on
the home page. Handle empty lists and invalid submissions gracefully, redirect
after a successful create, delete, or cancel, and show a clear flash message.
Accessible labels, visible keyboard focus, and operable tables and forms are
part of the work, not an extra.

Apply the Witt design without turning an internal tool into a marketing site.

## Keep the features separate

Do not build:

- a generic database-management system
- a metadata-driven CRUD framework
- a generic dynamic filtering or query framework

Partners, products, and orders keep their own controllers, services, forms,
repositories, templates, and business rules, because they will evolve
differently. The three list pages will look similar and their filters are
deliberately not identical. Leave the filtering and searching logic
feature-specific anyway.

The shared page layout, the form fragments, and the CSS already exist and are
enough sharing for now. If you find real duplication beyond that, the only
extraction I want in this step is a shared confirmation-page fragment for
deleting and cancelling. Leave the partner module otherwise untouched, in
particular the partner search.

## Tests

Java tests in the style of the existing ones: against the development
PostgreSQL, rolling their changes back, no separate test infrastructure. Cover
at least:

- product validation and CRUD
- order validation, including "at least one line" and the quantity and rebate
  bounds
- total, rebate, and rounding calculations
- a line's unit price surviving a later change of the product price
- both deletion guards, including the case where the only reference is a
  cancelled order
- order number format, per-year restart, and the year being taken from the
  Berlin date, including an order placed late on 31 December UTC
- the UTC-to-Berlin display conversion
- sample data generation

Two Playwright specs in `e2e/tests`:

1. Create an order with two lines for an existing partner, check the order
   number, the totals, and the rebate on the detail page, then cancel it.
2. Try to delete a product that an order references, check that the deletion is
   refused with an explanation, and that the product is still in the list.

The partner specs create and delete their own data so that they can be run
repeatedly. Orders cannot be deleted, so the order spec cannot do the same. Use
the order number as its handle instead of a generated marker; orders piling up
in the development database over time is acceptable.

Run `./mvnw verify` and `npm test` in `e2e` when you are finished.
