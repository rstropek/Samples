-- Create the Account table
CREATE TABLE Account (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    number TEXT NOT NULL
);

-- Create the Person table
CREATE TABLE Person (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    type TEXT NOT NULL, -- Either 'natural' or 'legal'
    name TEXT NOT NULL
);

-- Create the Holder table linking Account and Person
CREATE TABLE Holder (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    account_id INTEGER NOT NULL,
    person_id INTEGER NOT NULL,
    permission_level TEXT NOT NULL, -- e.g., 'read', 'transfer', 'manage'
    FOREIGN KEY (account_id) REFERENCES Account(id),
    FOREIGN KEY (person_id) REFERENCES Person(id)
);

-- Create the Connection table linking two Persons
CREATE TABLE Connection (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    person1_id INTEGER NOT NULL,
    person2_id INTEGER NOT NULL,
    type TEXT NOT NULL, -- e.g., 'spouse', 'child', 'representative'
    FOREIGN KEY (person1_id) REFERENCES Person(id),
    FOREIGN KEY (person2_id) REFERENCES Person(id)
);
