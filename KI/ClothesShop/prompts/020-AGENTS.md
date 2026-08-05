# ClothesShop

## Introduction

Internal application for processing orders and support incidents from sales
partners.

- Flyway exclusively owns the database schema. Hibernate may validate it but
  must never create or update it. Add new migrations; never modify an applied
  migration.
- Render the UI server-side with Thymeleaf.

## Important Commands

* Run: `./mvnw spring-boot:run`
* Test: `./mvnw test`

## Documentation

To get up-to-date documentation, read and follow the `find-docs` skill (context7 CLI).

## Explorative Testing

If you want to explore the application, read and follow the `playwright-cli` skill.
