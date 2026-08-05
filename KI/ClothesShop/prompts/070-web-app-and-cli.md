# Turn the application into a web app *and* a CLI

The application ships as one artifact with two faces. Started the way it is
started today, it is the web app and nothing changes. Started with
command-line arguments, it does not become a server at all: it runs one
command, writes the result, and exits. No port, no browser, no interactive
prompts.

Propose the concrete command-line syntax to me before you implement it. That
includes the command and option names, how multiple grouping columns are
passed, and the default output format. I care about the behaviour described
below, not about which parsing library you pick.

## The caller is an agent, not a person

The CLI exists so that AI agents can answer questions about our sales figures
without going through the UI. Everything follows from that:

- The result goes to standard output and is parseable without guessing.
  Choose one default format and stay with it. A prettier format for humans is
  optional; if you add one it must not be the default.
- Logs, warnings, and the Spring banner are not results. They must never end
  up mixed into the output stream that carries the result.
- Success and failure are distinguishable from the exit code alone.
- On failure there is a message that says what was wrong with the request, and
  no half-written result before it.
- There is a way to ask the CLI what it can do and which options it takes.

## The command: revenue statistics

One command for now: revenue over the order data.

All filters are optional and combine with each other:

- order year, in **UTC**
- partner, by primary key
- product, by primary key

Grouping columns, one or several, in the order the caller lists them:

- partner
- product
- month, as `YYYY-MM` in **UTC**
- color
- size

The grouping order decides the column order and the sort order of the rows.
Without any grouping the answer is a single total row.

Every row reports, for its group: revenue, the quantity sold, and how many
orders contributed to it.

## What "revenue" means here

- Revenue is the sum of unit price x quantity over the order lines, with the
  line's own price snapshot. Rebates are deliberately ignored, so this never
  needs to be split, allocated, or reconciled with the header totals. Keep it
  that simple; the interesting part of this task is the CLI, not the
  arithmetic.
- Money stays exact to the cent, with the numeric types the application
  already uses. No binary floating point in the aggregation either.
- Cancelled orders are not revenue. If the caller wants them anyway, that has
  to be an explicit opt-in.
- Color and size come from the product data. Products without a color or size
  are not silently dropped and do not produce an empty column value; they get
  a stable, documented label.
- Partner and product groups carry both the primary key and a readable name,
  so an agent can use the one and show the other.
- Year and month are UTC. The order number already uses the Berlin year for a
  different purpose, so a naive implementation looks right on our machines and
  is wrong at the end of December. Write the tests so they would actually
  catch this.

An unknown partner or product ID is a bad request and says so. A filter that
legitimately matches nothing is a successful, empty result. These two are not
the same answer.

## Constraints

- One artifact, one build, one configuration. No second Maven module and no
  separate database setup; the CLI talks to the same PostgreSQL as the web
  app.
- The aggregation belongs in the domain code next to orders, not inside the
  argument parsing. Do the grouping and filtering in the database, not by
  loading all orders into memory.
- The web app keeps working exactly as before, including `./mvnw
  spring-boot:run` and the Playwright suite.
- Document the CLI in `AGENTS.md` the way the other entry points are
  documented.

## The accompanying skill

Create a skill for this CLI using the `skill-creator` skill, stored in the
project and not globally. It is what makes the CLI usable by an agent that has
never seen this repository: when to reach for it, how to invoke it from the
project root, the available filters and grouping columns, the shape of the
output, the exit codes, and worked examples for realistic questions such as
"revenue per partner in 2025", "monthly revenue of one product broken down by
size", or "which colors sold best last year". Its description has to trigger
on questions about revenue, sales figures, and order statistics.

## Tests

Java tests for the aggregation itself: each filter, several grouping
combinations, the exclusion of cancelled orders, and the UTC year and month
boundaries. Tests for the argument handling, including
the error cases above. A test that proves CLI mode does not bring up the web
server. The existing tests stay green.

When you are done, run `./mvnw verify` and the Playwright suite, and show me
the CLI answering two of the example questions against the sample data.

Plan this before you write code, and ask me about whatever the description
leaves open.
