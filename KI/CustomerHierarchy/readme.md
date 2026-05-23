# Customer Hierarchy

## Introduction

In this application, we maintain bank accounts with their holders of the rights of disposal. Holders can be natural and legal persons. Holders can have the right to read bank statements, the right to transfer money (includes right to read), and the right to manage the account (e.g. close it, manage holders and their rights; includes right to transfer money and read). The application also stores connections between holders (e.g. spouses, children, representative of a legal person, accountant, custodian, etc.).

The application supports various queries, such as:

* Who are the holders of a bank account?
* Which holders are directly or indirectly connected to a bank account?

## Non-Functional Requirements

* The data is stored in a SQLite database.

## Database Structure

The database contains the following tables:

* `Account` (id, number)
* `Person` (id, type, name)
* `Holder` (links `Account` and `Person`; id, account_id, person_id, permission level)
* `Connection` (links `Person` and `Person`; id, person1_id, person2_id, type)
