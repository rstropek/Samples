import Database from 'better-sqlite3';
import fs from 'fs';
import path from 'path';
export function importData() {
    // Replace 'path_to_database.db' with the path to your SQLite database file
    const db = new Database('database.db');
    // Read the JSON data from the file
    const data = JSON.parse(fs.readFileSync(path.join('data', 'data.json'), 'utf8'));
    // Function to insert data into the database
    const insertData = () => {
        // Prepare SQL statements
        const insertAccount = db.prepare('INSERT INTO Account (id, number) VALUES (?, ?)');
        const insertPerson = db.prepare('INSERT INTO Person (id, type, name) VALUES (?, ?, ?)');
        const insertHolder = db.prepare('INSERT INTO Holder (id, account_id, person_id, permission_level) VALUES (?, ?, ?, ?)');
        const insertConnection = db.prepare('INSERT INTO Connection (id, person1_id, person2_id, type) VALUES (?, ?, ?, ?)');
        // Use transactions for batch inserts
        const insertAccounts = db.transaction((accounts) => {
            for (const account of accounts) {
                insertAccount.run(account.id, account.number);
            }
        });
        const insertPersons = db.transaction((persons) => {
            for (const person of persons) {
                insertPerson.run(person.id, person.type, person.name);
            }
        });
        const insertHolders = db.transaction((holders) => {
            for (const holder of holders) {
                insertHolder.run(holder.id, holder.account_id, holder.person_id, holder.permission_level);
            }
        });
        const insertConnections = db.transaction((connections) => {
            for (const connection of connections) {
                insertConnection.run(connection.id, connection.person1_id, connection.person2_id, connection.type);
            }
        });
        // Execute the transactions
        insertAccounts(data.Account);
        insertPersons(data.Person);
        insertHolders(data.Holder);
        insertConnections(data.Connection);
    };
    // Call the function to insert data
    insertData();
    console.log('Data imported successfully.');
}
//# sourceMappingURL=import.js.map