# Add partner management

Replace the dummy addition feature with CRUD for sales partners: small shops and
boutiques that sell our clothes.

A partner has a generated surrogate ID and these fields:

- name: required, max. 120 characters
- contact person: required, max. 120 characters
- email: required, valid email, max. 254 characters
- phone: required, stored as E.164 (`+` and at most 15 digits)
- active: required, defaults to true

For the phone input, use a country dropdown limited to AT (+43), DE (+49), and
CH (+41), plus one national-number field. Use libphonenumber to validate and
normalize it. Accept familiar national formatting, including spaces and the
domestic `0`; store only E.164. Editing must show the right country and a
nationally formatted number again.

Only inactive partners can be deleted.

Add a Thymeleaf list page with a case-insensitive text filter over name, contact,
and email, plus an all/active/inactive filter. Sort by name. Add create, edit, and
delete, including form validation and confirmation before deletion.

Give the application a horizontal top-level menu that stays at the top while the
page scrolls. Single-level navigation is enough. Add Partners and "Fill DB with
sample data" to it. The latter must ask for confirmation, clear all application
data without touching the schema or Flyway history, and insert ten partners using
Datafaker.

Keep the CSS simple and clean, follow the `witt-weiden-design`skill. 
This is the first of several CRUD areas, so share the page layout, 
navigation, styling, and useful view fragments. Do not build a
generic CRUD framework or abstract base classes yet.

Remove the addition feature and its tests. Add focused Java tests for phone
conversion and validation, filtering, MVC behavior, and sample-data generation.
Use the existing development PostgreSQL for persistence tests and keep those
tests transactional so their changes roll back. Replace the Playwright setup
check with a repeatable partner workflow using the same database. Do not add
separate database infrastructure for tests. Run the Java and Playwright tests
when finished.
