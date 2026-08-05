# Create the project

Create the starting point for an internal clothes shop application. Eventually,
it will be used to process orders and support incidents coming from our sales
partners. For now, I only want the project setup and a tiny feature that proves
everything is working.

Use Java 25, Spring Boot 4.1, Maven with the Maven Wrapper, PostgreSQL, Flyway,
and Thymeleaf with server-side rendering. Flyway should own the database schema.
Use Spring Data JPA and Jakarta Bean Validation where appropriate. The project
should be called `ClothesShop`, with Maven coordinates
`com.example:clothesshop` and base package `com.example.clothesshop`.

Use the installed `spring init` command to generate the project in the current
directory. There is already a `compose.yaml` containing the PostgreSQL database;
keep it and configure the application to use it.

As a simple setup check, add a service that adds two integers and write a unit
test for it. Also add a small Thymeleaf page where I can enter two numbers and
see the result. It should be easy to find from the application's home page.

Run the tests and start the application to check that the project, database
migration, and test page all work. Summarize what you did and mention anything
that did not work.
